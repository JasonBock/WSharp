using Mono.Cecil;
using Mono.Cecil.Cil;
using Spackle;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using WSharp.Compiler.Binding;
using WSharp.Compiler.Symbols;
using WSharp.Runtime;

namespace WSharp.Compiler.Emit
{
	internal sealed class Emitter
	{
		private readonly HashSet<AssemblyDefinition> assemblies = new HashSet<AssemblyDefinition>();
		private readonly AssemblyDefinition assemblyDefinition;
		private readonly Dictionary<TypeSymbol, TypeReference> knownTypes;
		private readonly MethodReference? consoleWriteLineReference;
		private readonly DiagnosticBag diagnostics = new DiagnosticBag();

		public EmitResult? Result { get; private set; }

		private Emitter(string moduleName, FileInfo[] references)
		{
			this.assemblies = new HashSet<AssemblyDefinition>
			{
				AssemblyDefinition.ReadAssembly(typeof(object).Assembly.Location),
				AssemblyDefinition.ReadAssembly(typeof(Console).Assembly.Location),
				AssemblyDefinition.ReadAssembly(typeof(ImmutableArray).Assembly.Location),
				AssemblyDefinition.ReadAssembly(typeof(IExecutionEngineActions).Assembly.Location),
				AssemblyDefinition.ReadAssembly(typeof(SecureRandom).Assembly.Location),
				AssemblyDefinition.ReadAssembly(typeof(BigInteger).Assembly.Location)
			};

			if (references is { })
			{
				foreach (var reference in references)
				{
					try
					{
						this.assemblies.Add(AssemblyDefinition.ReadAssembly(reference.FullName));
					}
					catch (BadImageFormatException)
					{
						this.diagnostics.ReportInvalidReference(reference);
					}
				}
			}

			var builtInTypes = new List<(TypeSymbol type, string metadataName)>
			{
				(TypeSymbol.Any, "System.Object"),
				(TypeSymbol.Boolean, "System.Boolean"),
				(TypeSymbol.Integer, "System.Numerics.BigInteger"),
				(TypeSymbol.String, "System.String"),
				(TypeSymbol.Void, "System.Void"),
			};

			this.knownTypes = new Dictionary<TypeSymbol, TypeReference>();

			var assemblyName = new AssemblyNameDefinition(moduleName, new Version(1, 0));
			this.assemblyDefinition = AssemblyDefinition.CreateAssembly(assemblyName, moduleName,
				ModuleKind.Console);

			foreach (var (typeSymbol, metadataName) in builtInTypes)
			{
				var typeReference = this.ResolveType(typeSymbol.Name, metadataName);

				if (typeReference is { })
				{
					this.knownTypes.Add(typeSymbol, typeReference);
				}
			}

			this.consoleWriteLineReference = this.ResolveMethod(typeof(Console).FullName!, nameof(Console.Out.WriteLine), new[] { typeof(string).FullName! });
		}

		private MethodReference? ResolveMethod(string typeName, string methodName, string[] parameterTypeNames)
		{
			var foundTypes = this.assemblies.SelectMany(a => a.Modules)
				.SelectMany(m => m.Types)
				.Where(t => t.FullName == typeName)
				.ToArray();

			if (foundTypes.Length == 1)
			{
				var foundType = foundTypes[0];
				var methods = foundType.Methods.Where(m => m.Name == methodName);

				foreach (var method in methods)
				{
					if (method.Parameters.Count != parameterTypeNames.Length)
					{
						continue;
					}

					var allParametersMatch = true;

					for (var i = 0; i < parameterTypeNames.Length; i++)
					{
						if (method.Parameters[i].ParameterType.FullName != parameterTypeNames[i])
						{
							allParametersMatch = false;
							break;
						}
					}

					if (!allParametersMatch)
					{
						continue;
					}

					return this.assemblyDefinition.MainModule.ImportReference(method);
				}

				this.diagnostics.ReportRequiredMethodNotFound(typeName, methodName, parameterTypeNames);
				return null;
			}
			else if (foundTypes.Length == 0)
			{
				this.diagnostics.ReportRequiredTypeNotFound(string.Empty, typeName);
			}
			else
			{
				this.diagnostics.ReportRequiredTypeAmbiguous(string.Empty, typeName, foundTypes);
			}

			return null;
		}

		private TypeReference? ResolveType(string wheneverName, string metadataName)
		{
			var foundTypes = this.assemblies.SelectMany(a => a.Modules)
				.SelectMany(m => m.Types)
				.Where(t => t.FullName == metadataName)
				.ToArray();

			if (foundTypes.Length == 1)
			{
				var typeReference = this.assemblyDefinition.MainModule.ImportReference(foundTypes[0]);
				return typeReference;
			}
			else if (foundTypes.Length == 0)
			{
				this.diagnostics.ReportRequiredTypeNotFound(wheneverName, metadataName);
			}
			else
			{
				this.diagnostics.ReportRequiredTypeAmbiguous(wheneverName, metadataName, foundTypes);
			}

			return null;
		}

		public void Emit(BoundLineStatements statements, FileInfo outputPath)
		{
			if (this.diagnostics.Count == 0)
			{
				var typeDefinition = new TypeDefinition(string.Empty, "Program",
					TypeAttributes.Abstract | TypeAttributes.Sealed,
					this.knownTypes[TypeSymbol.Any]);

				this.assemblyDefinition.MainModule.Types.Add(typeDefinition);

				var methodLineMap = new List<(BigInteger, MethodDefinition)>();
				var engineActionsReference = this.ResolveType(string.Empty, typeof(IExecutionEngineActions).FullName!);
				
				foreach (var lineStatement in statements.LineStatements)
				{
					var lineNumber = (BigInteger)((lineStatement.Number as BoundExpressionStatement)!.Expression as BoundLiteralExpression)!.Value;

					var lineMethod = new MethodDefinition($"Line{lineNumber}", MethodAttributes.Static | MethodAttributes.Private,
						this.knownTypes[TypeSymbol.Void]);
					var lineMethodParameter = new ParameterDefinition("actions", ParameterAttributes.None, engineActionsReference);
					lineMethod.Parameters.Add(lineMethodParameter);

					methodLineMap.Add((lineNumber, lineMethod));

					// TODO: Generate the body for each of the statements in the method.
					typeDefinition.Methods.Add(lineMethod);

					var lineIlProcessor = lineMethod.Body.GetILProcessor();
					lineIlProcessor.Emit(OpCodes.Ret);
				}

				var mainMethod = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Private,
					this.knownTypes[TypeSymbol.Void]);

				typeDefinition.Methods.Add(mainMethod);

				var executionEngineReference = this.ResolveType(string.Empty, typeof(ExecutionEngine).FullName!);
				var secureRandomReference = this.ResolveType(string.Empty, typeof(SecureRandom).FullName!);
				var consoleReference = this.ResolveType(string.Empty, typeof(Console).FullName!);
				var lineReference = this.ResolveType(string.Empty, typeof(Line).FullName!);
				var linesReference = this.ResolveType(string.Empty, typeof(ImmutableArray).FullName!);
				var linesBuilderMethodReference = this.ResolveMethod(typeof(ImmutableArray).FullName!, nameof(ImmutableArray.CreateBuilder), Array.Empty<string>());
				var linesBuilderMethodReferenceGeneric = new GenericInstanceMethod(linesBuilderMethodReference);
				linesBuilderMethodReferenceGeneric.GenericArguments.Add(lineReference);

				var mainIlProcessor = mainMethod.Body.GetILProcessor();

				mainIlProcessor.Emit(OpCodes.Call, linesBuilderMethodReferenceGeneric);
				// TODO: Create a new ExecutionEngine, and then call Execute(). Done!
				// This will require create Line objects for each emitted method
				//ilProcessor.Emit(OpCodes.Ldstr, "Hello world from Whenever!");
				//ilProcessor.Emit(OpCodes.Call, this.consoleWriteLineReference);
				mainIlProcessor.Emit(OpCodes.Ret);

				this.assemblyDefinition.EntryPoint = mainMethod;
				this.assemblyDefinition.Write(outputPath.FullName);
			}

			this.Result = new EmitResult(this.diagnostics.ToImmutableArray());
		}

		public static EmitResult Emit(BoundLineStatements statement, string moduleName, FileInfo[] references, FileInfo outputPath)
		{
			var emitter = new Emitter(moduleName, references);
			emitter.Emit(statement, outputPath);
			return emitter.Result!;
		}
	}
}