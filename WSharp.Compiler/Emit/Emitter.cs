using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Spackle;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using WSharp.Compiler.Binding;
using WSharp.Compiler.Extensions;
using WSharp.Compiler.Symbols;
using WSharp.Runtime;

namespace WSharp.Compiler.Emit
{
	internal sealed class Emitter
	{
		private readonly HashSet<AssemblyDefinition> assemblies = new HashSet<AssemblyDefinition>();
		private readonly AssemblyDefinition assemblyDefinition;
		private readonly Dictionary<TypeSymbol, TypeReference> knownTypes;
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
				var programTypeDefinition = new TypeDefinition(string.Empty, "Program",
					TypeAttributes.Abstract | TypeAttributes.Sealed,
					this.knownTypes[TypeSymbol.Any]);
				this.assemblyDefinition.MainModule.Types.Add(programTypeDefinition);

				var lines = this.EmitLineMethods(statements, programTypeDefinition);
				var mainMethod = this.EmitMainMethod(programTypeDefinition, lines);

				this.assemblyDefinition.EntryPoint = mainMethod;
				this.assemblyDefinition.Write(outputPath.FullName);
			}

			this.Result = new EmitResult(this.diagnostics.ToImmutableArray());
		}

		private MethodDefinition EmitMainMethod(TypeDefinition programTypeDefinition, List<(BigInteger, MethodDefinition)> lines)
		{
			var mainMethod = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Private,
				this.knownTypes[TypeSymbol.Void]);
			programTypeDefinition.Methods.Add(mainMethod);

			var iExecutionEngineActionsReference = this.ResolveType(string.Empty, typeof(IExecutionEngineActions).FullName!);
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

			foreach (var (lineNumber, lineMethod) in lines)
			{
				mainIlProcessor.Emit(OpCodes.Dup);
				mainIlProcessor.EmitBigInteger(lineNumber);
				mainIlProcessor.Emit(OpCodes.Call,
					this.assemblyDefinition.MainModule.ImportReference(
						typeof(BigInteger).GetProperties().Single(_ => _.Name == nameof(BigInteger.One)).GetGetMethod()));

				var lineActionCtor = this.assemblyDefinition.MainModule.ImportReference(
					typeof(Action<>).GetConstructors().Single(_ => _.GetParameters().Length == 2));
				var genericLineActionCtor = new GenericInstanceMethod(lineActionCtor);
				genericLineActionCtor.GenericArguments.Add(iExecutionEngineActionsReference);

				mainIlProcessor.Emit(OpCodes.Ldnull);
				mainIlProcessor.Emit(OpCodes.Ldftn,
					this.assemblyDefinition.MainModule.ImportReference(lineMethod));
				mainIlProcessor.Emit(OpCodes.Newobj, genericLineActionCtor);
				mainIlProcessor.Emit(OpCodes.Newobj,
					this.assemblyDefinition.MainModule.ImportReference(
						typeof(Line).GetConstructors().Single(_ => _.GetParameters().Length == 3)));

				mainIlProcessor.Emit(OpCodes.Callvirt,
					this.assemblyDefinition.MainModule.ImportReference(
						typeof(ImmutableArray<Line>.Builder).GetMethod(nameof(ImmutableArray<Line>.Builder.Add))));
			}

			mainIlProcessor.Emit(OpCodes.Call,
				this.assemblyDefinition.MainModule.ImportReference(
					typeof(ImmutableArray<Line>.Builder).GetMethod(nameof(ImmutableArray<Line>.Builder.ToImmutable))));
			mainIlProcessor.Emit(OpCodes.Newobj,
				this.assemblyDefinition.MainModule.ImportReference(
					typeof(SecureRandom).GetConstructors().Single(_ => _.GetParameters().Length == 0)));
			mainIlProcessor.Emit(OpCodes.Call,
				this.assemblyDefinition.MainModule.ImportReference(
					typeof(Console).GetProperties().Single(_ => _.Name == nameof(Console.In)).GetGetMethod()));
			mainIlProcessor.Emit(OpCodes.Call,
				this.assemblyDefinition.MainModule.ImportReference(
					typeof(Console).GetProperties().Single(_ => _.Name == nameof(Console.Out)).GetGetMethod()));
			mainIlProcessor.Emit(OpCodes.Newobj,
				this.assemblyDefinition.MainModule.ImportReference(
					typeof(ExecutionEngine).GetConstructors().Single(_ => _.GetParameters().Length == 4)));
			mainIlProcessor.Emit(OpCodes.Call,
				this.assemblyDefinition.MainModule.ImportReference(
					typeof(ExecutionEngine).GetMethod(nameof(ExecutionEngine.Execute))));
			mainIlProcessor.Emit(OpCodes.Ret);

			mainMethod.Body.OptimizeMacros();
			return mainMethod;
		}

		private List<(BigInteger, MethodDefinition)> EmitLineMethods(BoundLineStatements statements, TypeDefinition programTypeDefinition)
		{
			var lines = new List<(BigInteger, MethodDefinition)>();
			var engineActionsReference = this.ResolveType(string.Empty, typeof(IExecutionEngineActions).FullName!);

			foreach (var lineStatement in statements.LineStatements)
			{
				var lineNumber = (BigInteger)((lineStatement.Number as BoundExpressionStatement)!.Expression as BoundLiteralExpression)!.Value;

				var lineMethod = new MethodDefinition($"Line{lineNumber}", MethodAttributes.Static | MethodAttributes.Private,
					this.knownTypes[TypeSymbol.Void]);
				var lineMethodParameter = new ParameterDefinition("actions", ParameterAttributes.None, engineActionsReference);
				lineMethod.Parameters.Add(lineMethodParameter);

				lines.Add((lineNumber, lineMethod));

				programTypeDefinition.Methods.Add(lineMethod);
				this.EmitLineMethod(lineStatement, lineMethod.Body.GetILProcessor());
				lineMethod.Body.OptimizeMacros();
			}

			return lines;
		}

		private void EmitLineMethod(BoundLineStatement lineStatement, ILProcessor ilProcessor)
		{
			// TODO: Generate the body for each of the statements in the method.
			foreach (var statement in lineStatement.Statements)
			{
				this.EmitStatement(statement, ilProcessor);
			}

			ilProcessor.Emit(OpCodes.Ret);
		}

		private void EmitStatement(BoundStatement statement, ILProcessor ilProcessor)
		{
			switch (statement)
			{
				case BoundExpressionStatement expression:
					this.EmitExpressionStatement(expression, ilProcessor);
					break;
				default:
					throw new EmitException($"Unexpected kind: {statement.Kind}.");
			}
		}

		private void EmitExpressionStatement(BoundExpressionStatement statement, ILProcessor ilProcessor)
		{
			this.EmitExpression(statement.Expression, ilProcessor);

			if (statement.Expression.Type != TypeSymbol.Void)
			{
				ilProcessor.Emit(OpCodes.Pop);
			}
		}

		private void EmitExpression(BoundExpression expression, ILProcessor ilProcessor)
		{
			switch (expression)
			{
				case BoundCallExpression call:
					this.EmitCallExpression(call, ilProcessor);
					break;
				case BoundUpdateLineCountExpression line:
					this.EmitUpdateLineCountExpression(line, ilProcessor);
					break;
				case BoundConversionExpression conversion:
					this.EmitConversionExpression(conversion, ilProcessor);
					break;
				case BoundLiteralExpression literal:
					this.EmitLiteralExpression(literal, ilProcessor);
					break;
				case BoundBinaryExpression binary:
					this.EmitBinaryExpression(binary, ilProcessor);
					break;
				case BoundUnaryExpression unary:
					this.EmitUnaryExpression(unary, ilProcessor);
					break;
				case BoundUnaryUpdateLineCountExpression unaryLine:
					this.EmitUnaryUpdateLineCountExpression(unaryLine, ilProcessor);
					break;
				default:
					throw new EmitException($"Unexpected kind: {expression.Kind}.");
			}
		}

		private void EmitUpdateLineCountExpression(BoundUpdateLineCountExpression line, ILProcessor ilProcessor)
		{

		}

		private void EmitCallExpression(BoundCallExpression call, ILProcessor ilProcessor)
		{
			var deferGuardLabel = Instruction.Create(OpCodes.Nop);

			ilProcessor.Emit(OpCodes.Ldarg_0);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(IExecutionEngineActions).GetProperty(nameof(IExecutionEngineActions.ShouldStatementBeDeferred))!.GetGetMethod()));
			ilProcessor.Emit(OpCodes.Brtrue, deferGuardLabel);

			ilProcessor.Emit(OpCodes.Ldarg_0);

			foreach (var argument in call.Arguments)
			{
				this.EmitExpression(argument, ilProcessor);
			}

			if(call.Function == BuiltinFunctions.Print)
			{
				ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.Print))));
			}
			else if (call.Function == BuiltinFunctions.Read)
			{
				ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.Read))));
			}
			else if (call.Function == BuiltinFunctions.Random)
			{
				ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.Random))));
			}

			ilProcessor.Append(deferGuardLabel);
		}

		private void EmitConversionExpression(BoundConversionExpression conversion, ILProcessor ilProcessor)
		{
			this.EmitExpression(conversion.Expression, ilProcessor);

			if (conversion.Type != TypeSymbol.Any)
			{
				if (conversion.Type == TypeSymbol.Boolean)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(Convert).GetMethod(nameof(Convert.ToBoolean), new[] { typeof(object) })));
				}
				else if (conversion.Type == TypeSymbol.Integer)
				{
					if(conversion.Expression.Type == TypeSymbol.Boolean)
					{
						var endLabel = Instruction.Create(OpCodes.Nop);
						var trueLabel = Instruction.Create(OpCodes.Nop);

						ilProcessor.Emit(OpCodes.Brtrue, trueLabel);
						ilProcessor.Emit(OpCodes.Call,
							ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger).GetProperty(nameof(BigInteger.Zero))!.GetGetMethod()));
						ilProcessor.Emit(OpCodes.Br, endLabel);
						ilProcessor.Append(trueLabel);
						ilProcessor.Emit(OpCodes.Call,
							ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger).GetProperty(nameof(BigInteger.One))!.GetGetMethod()));
						ilProcessor.Append(endLabel);
					}
					else if(conversion.Expression.Type == TypeSymbol.String)
					{
						ilProcessor.Emit(OpCodes.Call,
							ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger).GetMethod(nameof(BigInteger.Parse), new[] { typeof(string) })));
					}
					else
					{
						throw new EmitException($"Unexpected conversion type {conversion.Type}");
					}
				}
				else if (conversion.Type == TypeSymbol.String)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(Convert).GetMethod(nameof(Convert.ToString), new[] { typeof(object) })));
				}
				else
				{
					throw new EvaluationException($"Unexpected type {conversion.Type}");
				}
			}
		}

		private void EmitLiteralExpression(BoundLiteralExpression literal, ILProcessor ilProcessor)
		{
			if(literal.Type == TypeSymbol.Boolean)
			{
				ilProcessor.Emit((bool)literal.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
			}
			else if (literal.Type == TypeSymbol.Integer)
			{
				ilProcessor.EmitBigInteger((BigInteger)literal.Value);
			}
			else if (literal.Type == TypeSymbol.String)
			{
				ilProcessor.Emit(OpCodes.Ldstr, (string)literal.Value);
			}
			else
			{
				throw new EmitException($"Unexpected literal type: {literal.Kind}.");
			}
		}

		private void EmitBinaryExpression(BoundBinaryExpression binary, ILProcessor ilProcessor)
		{

		}

		private void EmitUnaryExpression(BoundUnaryExpression unary, ILProcessor ilProcessor)
		{

		}

		private void EmitUnaryUpdateLineCountExpression(BoundUnaryUpdateLineCountExpression unaryLine, ILProcessor ilProcessor)
		{

		}

		public static EmitResult Emit(BoundLineStatements statement, string moduleName, FileInfo[] references, FileInfo outputPath)
		{
			var emitter = new Emitter(moduleName, references);
			emitter.Emit(statement, outputPath);
			return emitter.Result!;
		}
	}
}