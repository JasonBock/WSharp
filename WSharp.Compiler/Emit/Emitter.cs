using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using WSharp.Compiler.Binding;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Emit
{
	internal static class Emitter
	{
		public static EmitResult Emit(BoundStatement statement, string moduleName, FileInfo[] references, FileInfo outputPath)
		{
			var diagnostics = new DiagnosticBag();

			var assemblies = new HashSet<AssemblyDefinition>
			{
				AssemblyDefinition.ReadAssembly(typeof(object).Assembly.Location),
				AssemblyDefinition.ReadAssembly(typeof(BigInteger).Assembly.Location)
			};

			foreach (var reference in references)
			{
				try
				{
					assemblies.Add(AssemblyDefinition.ReadAssembly(reference.FullName));
				}
				catch(BadImageFormatException)
				{
					diagnostics.ReportInvalidReference(reference);
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

			var knownTypes = new Dictionary<TypeSymbol, TypeReference>();

			var assemblyName = new AssemblyNameDefinition(moduleName, new Version(1, 0));
			var assemblyDefinition = AssemblyDefinition.CreateAssembly(assemblyName, moduleName,
				ModuleKind.Console);

			foreach (var (typeSymbol, metadataName) in builtInTypes)
			{
				var typeReference = ResolveType(typeSymbol.Name, metadataName);

				if(typeReference is { })
				{
					knownTypes.Add(typeSymbol, typeReference);
				}
			}

			TypeReference? ResolveType(string wheneverName, string metadataName)
			{
				var foundTypes = assemblies.SelectMany(a => a.Modules)
													.SelectMany(m => m.Types)
													.Where(t => t.FullName == metadataName)
													.ToArray();
				if (foundTypes.Length == 1)
				{
					var typeReference = assemblyDefinition.MainModule.ImportReference(foundTypes[0]);
					return typeReference;
				}
				else if (foundTypes.Length == 0)
				{
					diagnostics.ReportRequiredTypeNotFound(wheneverName, metadataName);
				}
				else
				{
					diagnostics.ReportRequiredTypeAmbiguous(wheneverName, metadataName, foundTypes);
				}

				return null;
			}

			MethodReference? ResolveMethod(string typeName, string methodName, string[] parameterTypeNames)
			{
				var foundTypes = assemblies.SelectMany(a => a.Modules)
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
							continue;

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
							continue;

						return assemblyDefinition.MainModule.ImportReference(method);
					}

					diagnostics.ReportRequiredMethodNotFound(typeName, methodName, parameterTypeNames);
					return null;
				}
				else if (foundTypes.Length == 0)
				{
					diagnostics.ReportRequiredTypeNotFound(string.Empty, typeName);
				}
				else
				{
					diagnostics.ReportRequiredTypeAmbiguous(string.Empty, typeName, foundTypes);
				}

				return null;
			}

			var consoleWriteLineReference = ResolveMethod(typeof(Console).FullName!, nameof(Console.Out.WriteLine), new[] { typeof(string).FullName! });

			if (diagnostics.Count == 0)
			{
				var typeDefinition = new TypeDefinition(string.Empty, "Program",
					TypeAttributes.Abstract | TypeAttributes.Sealed,
					knownTypes[TypeSymbol.Any]);

				assemblyDefinition.MainModule.Types.Add(typeDefinition);

				var mainMethod = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Private, 
					knownTypes[TypeSymbol.Void]);

				typeDefinition.Methods.Add(mainMethod);

				var ilProcessor = mainMethod.Body.GetILProcessor();
				ilProcessor.Emit(OpCodes.Ldstr, "Hello world from Whenever!");
				ilProcessor.Emit(OpCodes.Call, consoleWriteLineReference);
				ilProcessor.Emit(OpCodes.Ret);

				assemblyDefinition.EntryPoint = mainMethod;
				assemblyDefinition.Write(outputPath.FullName);
			}

			return new EmitResult(diagnostics.ToImmutableArray());
		}
	}
}