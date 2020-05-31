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
using System.Text;
using WSharp.Compiler.Binding;
using WSharp.Compiler.Extensions;
using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;
using WSharp.Runtime;

namespace WSharp.Compiler.Emit
{
	internal sealed class Emitter
	{
		private readonly HashSet<AssemblyDefinition> assemblies = new HashSet<AssemblyDefinition>();
		private readonly AssemblyDefinition assemblyDefinition;
		private readonly Dictionary<TypeSymbol, TypeReference> knownTypes;
		private readonly DiagnosticBag diagnostics = new DiagnosticBag();
		private TypeSymbol? currentStackType;
		private Instruction? deferGuardLabel;

		private Emitter(string moduleName, FileInfo[] references)
		{
			this.assemblies = new HashSet<AssemblyDefinition>();

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
				var typeReference = this.ResolveType(metadataName);

				if (typeReference is { })
				{
					this.knownTypes.Add(typeSymbol, typeReference);
				}
			}
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

		public static EmitResult Emit(BoundLineStatements statement, string moduleName, FileInfo[] references, FileInfo outputPath)
		{
			var emitter = new Emitter(moduleName, references);
			emitter.Emit(statement, outputPath);
			return emitter.Result!;
		}

		private void EmitExpressionStatement(BoundExpressionStatement statement, ILProcessor ilProcessor) =>
			this.EmitExpression(statement.Expression, ilProcessor);

		private void EmitBinaryExpression(BoundBinaryExpression binary, ILProcessor ilProcessor)
		{
			if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Addition)
			{
				if (binary.Left.Type == TypeSymbol.String && binary.Right.Type == TypeSymbol.String)
				{
					this.EmitStringConcatExpression(ilProcessor, binary);
					this.currentStackType = TypeSymbol.String;
					return;
				}
			}

			this.EmitExpression(binary.Left, ilProcessor);
			this.EmitExpression(binary.Right, ilProcessor);

			if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Addition)
			{
				if (binary.Type == TypeSymbol.Integer)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
							.GetMethod(nameof(BigInteger.Add), new[] { typeof(BigInteger), typeof(BigInteger) })));
					this.currentStackType = TypeSymbol.Integer;
				}
				else
				{
					throw new EmitException($"Unexpected type '{binary.Type.Name}' for binary operator {binary.Operator.OperatorKind}.");
				}
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Subtraction)
			{
				ilProcessor.Emit(OpCodes.Call,
					ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
						.GetMethod(nameof(BigInteger.Subtract), new[] { typeof(BigInteger), typeof(BigInteger) })));
				this.currentStackType = TypeSymbol.Integer;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Multiplication)
			{
				ilProcessor.Emit(OpCodes.Call,
					ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
						.GetMethod(nameof(BigInteger.Multiply), new[] { typeof(BigInteger), typeof(BigInteger) })));
				this.currentStackType = TypeSymbol.Integer;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Division)
			{
				ilProcessor.Emit(OpCodes.Call,
					ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
						.GetMethod(nameof(BigInteger.Divide), new[] { typeof(BigInteger), typeof(BigInteger) })));
				this.currentStackType = TypeSymbol.Integer;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Modulo)
			{
				ilProcessor.Emit(OpCodes.Call,
					ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
						.GetMethod(nameof(BigInteger.Remainder), new[] { typeof(BigInteger), typeof(BigInteger) })));
				this.currentStackType = TypeSymbol.Integer;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.BitwiseAnd)
			{
				if (binary.Type == TypeSymbol.Integer)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
							.GetMethod("op_BitwiseAnd", new[] { typeof(BigInteger), typeof(BigInteger) })));
					this.currentStackType = TypeSymbol.Integer;
				}
				else if (binary.Type == TypeSymbol.Boolean)
				{
					ilProcessor.Emit(OpCodes.And);
					this.currentStackType = TypeSymbol.Boolean;
				}
				else
				{
					throw new EmitException($"Unexpected type '{binary.Type.Name}' for binary operator {binary.Operator.OperatorKind}.");
				}
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.BitwiseOr)
			{
				if (binary.Type == TypeSymbol.Integer)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
							.GetMethod("op_BitwiseOr", new[] { typeof(BigInteger), typeof(BigInteger) })));
					this.currentStackType = TypeSymbol.Integer;
				}
				else if (binary.Type == TypeSymbol.Boolean)
				{
					ilProcessor.Emit(OpCodes.Or);
					this.currentStackType = TypeSymbol.Boolean;
				}
				else
				{
					throw new EmitException($"Unexpected type '{binary.Type.Name}' for binary operator {binary.Operator.OperatorKind}.");
				}
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.BitwiseXor)
			{
				if (binary.Type == TypeSymbol.Integer)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
							.GetMethod("op_ExclusiveOr", new[] { typeof(BigInteger), typeof(BigInteger) })));
					this.currentStackType = TypeSymbol.Integer;
				}
				else if (binary.Type == TypeSymbol.Boolean)
				{
					ilProcessor.Emit(OpCodes.Xor);
					this.currentStackType = TypeSymbol.Boolean;
				}
				else
				{
					throw new EmitException($"Unexpected type '{binary.Type.Name}' for binary operator {binary.Operator.OperatorKind}.");
				}
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.LogicalAnd)
			{
				ilProcessor.Emit(OpCodes.And);
				this.currentStackType = TypeSymbol.Boolean;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.LogicalOr)
			{
				ilProcessor.Emit(OpCodes.Or);
				this.currentStackType = TypeSymbol.Boolean;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Equals)
			{
				if (binary.Left.Type == TypeSymbol.Integer)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
							.GetMethod("op_Equality", new[] { typeof(BigInteger), typeof(BigInteger) })));
				}
				else if (binary.Left.Type == TypeSymbol.String)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(string)
							.GetMethod(nameof(string.Equals), new[] { typeof(string) })));
				}
				else if (binary.Left.Type == TypeSymbol.Boolean)
				{
					ilProcessor.Emit(OpCodes.Ceq);
				}
				else
				{
					throw new EmitException($"Unexpected type '{binary.Type.Name}' for binary operator {binary.Operator.OperatorKind}.");
				}

				this.currentStackType = TypeSymbol.Boolean;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.NotEquals)
			{
				if (binary.Left.Type == TypeSymbol.Integer)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
							.GetMethod("op_Equality", new[] { typeof(BigInteger), typeof(BigInteger) })));
				}
				else if (binary.Left.Type == TypeSymbol.String)
				{
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(string)
							.GetMethod(nameof(string.Equals), new[] { typeof(string) })));
				}
				else if (binary.Left.Type == TypeSymbol.Boolean)
				{
					ilProcessor.Emit(OpCodes.Ceq);
				}
				else
				{
					throw new EmitException($"Unexpected type '{binary.Type.Name}' for binary operator {binary.Operator.OperatorKind}.");
				}

				ilProcessor.Emit(OpCodes.Ldc_I4_0);
				ilProcessor.Emit(OpCodes.Ceq);

				this.currentStackType = TypeSymbol.Boolean;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Less)
			{
				ilProcessor.Emit(OpCodes.Call,
					ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
						.GetMethod("op_LessThan", new[] { typeof(BigInteger), typeof(BigInteger) })));
				this.currentStackType = TypeSymbol.Integer;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.LessOrEqualsTo)
			{
				ilProcessor.Emit(OpCodes.Call,
					ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
						.GetMethod("op_LessThanOrEqual", new[] { typeof(BigInteger), typeof(BigInteger) })));
				this.currentStackType = TypeSymbol.Integer;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.Greater)
			{
				ilProcessor.Emit(OpCodes.Call,
					ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
						.GetMethod("op_GreaterThan", new[] { typeof(BigInteger), typeof(BigInteger) })));
				this.currentStackType = TypeSymbol.Integer;
			}
			else if (binary.Operator.OperatorKind == BoundBinaryOperatorKind.GreaterOrEqualsTo)
			{
				ilProcessor.Emit(OpCodes.Call,
					ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)
						.GetMethod("op_GreaterThanOrEqual", new[] { typeof(BigInteger), typeof(BigInteger) })));
				this.currentStackType = TypeSymbol.Integer;
			}
			else
			{
				throw new EmitException($"Unexpected binary expression kind: {binary.Operator.OperatorKind}.");
			}
		}

		private void EmitCallExpression(BoundCallExpression call, ILProcessor ilProcessor)
		{
			ilProcessor.Emit(OpCodes.Ldarg_0);

			foreach (var argument in call.Arguments)
			{
				this.EmitExpression(argument, ilProcessor);
			}

			if (call.Function == BuiltinFunctions.Again)
			{
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.Again))));
			}
			else if (call.Function == BuiltinFunctions.Defer)
			{
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.Defer))));

				this.deferGuardLabel = Instruction.Create(OpCodes.Nop);

				ilProcessor.Emit(OpCodes.Ldarg_0);
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetProperty(nameof(IExecutionEngineActions.ShouldStatementBeDeferred))!.GetGetMethod()));
				ilProcessor.Emit(OpCodes.Brtrue, this.deferGuardLabel);
			}
			else if (call.Function == BuiltinFunctions.E)
			{
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.E))));
			}
			else if (call.Function == BuiltinFunctions.N)
			{
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.N))));
			}
			else if (call.Function == BuiltinFunctions.Print)
			{
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.Print))));
			}
			else if (call.Function == BuiltinFunctions.Random)
			{
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.Random))));
			}
			else if (call.Function == BuiltinFunctions.Read)
			{
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.Read))));
			}
			else if (call.Function == BuiltinFunctions.U)
			{
				ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
					typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.U))));
			}
		}

		private void EmitConversionExpression(BoundConversionExpression conversion, ILProcessor ilProcessor)
		{
			this.EmitExpression(conversion.Expression, ilProcessor);

			if (conversion.Type != TypeSymbol.Any)
			{
				if (conversion.Type == TypeSymbol.Boolean)
				{
					ilProcessor.EmitBox(this.currentStackType);
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(Convert).GetMethod(nameof(Convert.ToBoolean), new[] { typeof(object) })));
				}
				else if (conversion.Type == TypeSymbol.Integer)
				{
					if (conversion.Expression.Type == TypeSymbol.Boolean)
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
					else if (conversion.Expression.Type == TypeSymbol.String)
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
					ilProcessor.EmitBox(this.currentStackType);
					ilProcessor.Emit(OpCodes.Call,
						ilProcessor.Body.Method.Module.ImportReference(typeof(Convert).GetMethod(nameof(Convert.ToString), new[] { typeof(object) })));
				}
				else
				{
					throw new EvaluationException($"Unexpected type {conversion.Type}");
				}
			}
		}

		private void EmitConstantExpression(BoundExpression node, ILProcessor ilProcessor)
		{
			if (node.Type == TypeSymbol.Boolean)
			{
				ilProcessor.Emit((bool)node.ConstantValue!.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
				this.currentStackType = TypeSymbol.Boolean;
			}
			else if (node.Type == TypeSymbol.Integer)
			{
				ilProcessor.EmitBigInteger((BigInteger)node.ConstantValue!.Value);
				this.currentStackType = TypeSymbol.Integer;
			}
			else if (node.Type == TypeSymbol.String)
			{
				ilProcessor.Emit(OpCodes.Ldstr, (string)node.ConstantValue!.Value);
				this.currentStackType = TypeSymbol.String;
			}
			else
			{
				throw new EmitException($"Unexpected literal type: {node.Kind}.");
			}
		}

		private void EmitExpression(BoundExpression expression, ILProcessor ilProcessor)
		{
			if (expression.ConstantValue is { })
			{
				this.EmitConstantExpression(expression, ilProcessor);
			}
			else
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
		}

		private void EmitLineMethod(BoundLineStatement lineStatement, ILProcessor ilProcessor)
		{
			foreach (var statement in lineStatement.Statements)
			{
				this.EmitStatement(statement, ilProcessor);
			}

			if (this.deferGuardLabel is { })
			{
				ilProcessor.Append(this.deferGuardLabel);
			}

			ilProcessor.Emit(OpCodes.Ret);
		}

		private List<(BigInteger, MethodDefinition)> EmitLineMethods(BoundLineStatements statements, TypeDefinition programTypeDefinition)
		{
			var lines = new List<(BigInteger, MethodDefinition)>();

			foreach (var lineStatement in statements.LineStatements)
			{
				var lineNumber = (BigInteger)((lineStatement.Number as BoundExpressionStatement)!.Expression as BoundLiteralExpression)!.Value;

				var lineMethod = new MethodDefinition($"Line{lineNumber}", MethodAttributes.Static | MethodAttributes.Private,
					this.knownTypes[TypeSymbol.Void]);
				var lineMethodParameter = new ParameterDefinition("actions", ParameterAttributes.None,
					programTypeDefinition.Module.ImportReference(typeof(IExecutionEngineActions)));
				lineMethod.Parameters.Add(lineMethodParameter);

				lines.Add((lineNumber, lineMethod));
				programTypeDefinition.Methods.Add(lineMethod);
				this.currentStackType = null;
				this.EmitLineMethod(lineStatement, lineMethod.Body.GetILProcessor());
				this.currentStackType = null;

				lineMethod.Body.OptimizeMacros();
			}

			return lines;
		}

		private MethodDefinition EmitMainMethod(TypeDefinition programTypeDefinition, List<(BigInteger, MethodDefinition)> lines)
		{
			var mainMethod = new MethodDefinition("Main", MethodAttributes.Static | MethodAttributes.Private,
				this.knownTypes[TypeSymbol.Void]);
			programTypeDefinition.Methods.Add(mainMethod);

			var mainIlProcessor = mainMethod.Body.GetILProcessor();
			mainIlProcessor.Emit(OpCodes.Call, mainMethod.Module.ImportReference(
				typeof(ImmutableArray).GetMethod(nameof(ImmutableArray.CreateBuilder), Array.Empty<Type>())!
					.MakeGenericMethod(new[] { typeof(Line) })));

			foreach (var (lineNumber, lineMethod) in lines)
			{
				mainIlProcessor.Emit(OpCodes.Dup);
				mainIlProcessor.EmitBigInteger(lineNumber);
				mainIlProcessor.Emit(OpCodes.Call,
					this.assemblyDefinition.MainModule.ImportReference(
						typeof(BigInteger).GetProperties().Single(_ => _.Name == nameof(BigInteger.One)).GetGetMethod()));

				var lineActionCtor = this.assemblyDefinition.MainModule.ImportReference(
					typeof(Action<IExecutionEngineActions>).GetConstructors().Single(_ => _.GetParameters().Length == 2));

				mainIlProcessor.Emit(OpCodes.Ldnull);
				mainIlProcessor.Emit(OpCodes.Ldftn,
					this.assemblyDefinition.MainModule.ImportReference(lineMethod));
				mainIlProcessor.Emit(OpCodes.Newobj, lineActionCtor);
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

		private void EmitStringConcatExpression(ILProcessor ilProcessor, BoundBinaryExpression node)
		{
			// [a, "foo", "bar", b, ""] --> [a, "foobar", b]
			static IEnumerable<BoundExpression> FoldConstants(SyntaxNode syntax, IEnumerable<BoundExpression> nodes)
			{
				StringBuilder? builder = null;

				foreach (var node in nodes)
				{
					if (node.ConstantValue != null)
					{
						var value = (string)node.ConstantValue.Value;

						if (string.IsNullOrEmpty(value))
						{
							continue;
						}

						builder ??= new StringBuilder();
						builder.Append(value);
					}
					else
					{
						if (builder?.Length > 0)
						{
							yield return new BoundLiteralExpression(syntax, builder.ToString());
							builder.Clear();
						}

						yield return node;
					}
				}

				if (builder?.Length > 0)
				{
					yield return new BoundLiteralExpression(syntax, builder.ToString());
				}
			}

			// (a + b) + (c + d) --> [a, b, c, d]
			static IEnumerable<BoundExpression> Flatten(BoundExpression node)
			{
				if (node is BoundBinaryExpression binaryExpression &&
					 binaryExpression.Operator.OperatorKind == BoundBinaryOperatorKind.Addition &&
					 binaryExpression.Left.Type == TypeSymbol.String &&
					 binaryExpression.Right.Type == TypeSymbol.String)
				{
					foreach (var result in Flatten(binaryExpression.Left))
					{
						yield return result;
					}

					foreach (var result in Flatten(binaryExpression.Right))
					{
						yield return result;
					}
				}
				else
				{
					if (node.Type != TypeSymbol.String)
					{
						throw new EmitException($"Unexpected node type in string concatenation: {node.Type}.");
					}

					yield return node;
				}
			}

			var nodes = FoldConstants(node.Syntax, Flatten(node)).ToList();

			switch (nodes.Count)
			{
				case 0:
					ilProcessor.Emit(OpCodes.Ldstr, string.Empty);
					break;
				case 1:
					this.EmitExpression(nodes[0], ilProcessor);
					break;
				case 2:
					this.EmitExpression(nodes[0], ilProcessor);
					this.EmitExpression(nodes[1], ilProcessor);
					ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
						typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) })));
					break;
				case 3:
					this.EmitExpression(nodes[0], ilProcessor);
					this.EmitExpression(nodes[1], ilProcessor);
					this.EmitExpression(nodes[2], ilProcessor);
					ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
						typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string), typeof(string) })));
					break;
				case 4:
					this.EmitExpression(nodes[0], ilProcessor);
					this.EmitExpression(nodes[1], ilProcessor);
					this.EmitExpression(nodes[2], ilProcessor);
					this.EmitExpression(nodes[3], ilProcessor);
					ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
						typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string), typeof(string), typeof(string) })));
					break;
				default:
					ilProcessor.Emit(OpCodes.Ldc_I4, nodes.Count);
					ilProcessor.Emit(OpCodes.Newarr, this.knownTypes[TypeSymbol.String]);

					for (var i = 0; i < nodes.Count; i++)
					{
						ilProcessor.Emit(OpCodes.Dup);
						ilProcessor.Emit(OpCodes.Ldc_I4, i);
						this.EmitExpression(nodes[i], ilProcessor);
						ilProcessor.Emit(OpCodes.Stelem_Ref);
					}

					ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
						typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string[]) })));
					break;
			}
		}

		private void EmitUnaryExpression(BoundUnaryExpression unary, ILProcessor ilProcessor)
		{
			this.EmitExpression(unary.Operand, ilProcessor);

			if (unary.Operator.OperatorKind != BoundUnaryOperatorKind.Identity)
			{
				if (unary.Operator.OperatorKind == BoundUnaryOperatorKind.Negation)
				{
					ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
						typeof(BigInteger).GetMethod("op_UnaryNegation")));
				}
				else if (unary.Operator.OperatorKind == BoundUnaryOperatorKind.LogicalNegation)
				{
					ilProcessor.Emit(OpCodes.Ldc_I4_0);
					ilProcessor.Emit(OpCodes.Ceq);
				}
				else if (unary.Operator.OperatorKind == BoundUnaryOperatorKind.OnesComplement)
				{
					ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
						typeof(BigInteger).GetMethod("op_OnesComplement")));
				}
				else
				{
					throw new EmitException($"Unexpected unary type: {unary.Operator.OperatorKind}.");
				}
			}
		}

		private void EmitUnaryUpdateLineCountExpression(BoundUnaryUpdateLineCountExpression unaryLine, ILProcessor ilProcessor)
		{
			var lineNumber = new VariableDefinition(
				ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)));
			var count = new VariableDefinition(
				ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)));

			ilProcessor.Body.Variables.Add(lineNumber);
			ilProcessor.Body.Variables.Add(count);

			this.EmitExpression(unaryLine.LineNumber, ilProcessor);
			ilProcessor.Emit(OpCodes.Stloc, lineNumber);

			ilProcessor.Emit(OpCodes.Ldloc, lineNumber);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetProperty(nameof(BigInteger.Zero))!.GetGetMethod()));
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetMethod("op_GreaterThan", new[] { typeof(BigInteger), typeof(BigInteger) })!));

			var greaterThanZero = Instruction.Create(OpCodes.Nop);
			ilProcessor.Emit(OpCodes.Brtrue, greaterThanZero);

			ilProcessor.Emit(OpCodes.Ldloc, lineNumber);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetProperty(nameof(BigInteger.Zero))!.GetGetMethod()));
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetMethod("op_LessThan", new[] { typeof(BigInteger), typeof(BigInteger) })!));

			var lessThanZero = Instruction.Create(OpCodes.Nop);
			ilProcessor.Emit(OpCodes.Brtrue, lessThanZero);

			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetProperty(nameof(BigInteger.Zero))!.GetGetMethod()));
			var gotoCall = Instruction.Create(OpCodes.Nop);
			ilProcessor.Emit(OpCodes.Br, gotoCall);

			ilProcessor.Append(lessThanZero);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetProperty(nameof(BigInteger.One))!.GetGetMethod()));
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetMethod("op_UnaryNegation", new[] { typeof(BigInteger) })!));
			ilProcessor.Emit(OpCodes.Br, gotoCall);

			ilProcessor.Append(greaterThanZero);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetProperty(nameof(BigInteger.One))!.GetGetMethod()));

			ilProcessor.Append(gotoCall);
			ilProcessor.Emit(OpCodes.Stloc, count);
			ilProcessor.Emit(OpCodes.Ldarg_0);
			ilProcessor.Emit(OpCodes.Ldloc, lineNumber);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetMethod(nameof(BigInteger.Abs))));
			ilProcessor.Emit(OpCodes.Ldloc, count);
			ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
				typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.UpdateCount))));
		}

		private void EmitUpdateLineCountExpression(BoundUpdateLineCountExpression line, ILProcessor ilProcessor)
		{
			var lineNumber = new VariableDefinition(
				ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)));
			var lineToUpdate = new VariableDefinition(
				ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)));
			var count = new VariableDefinition(
				ilProcessor.Body.Method.Module.ImportReference(typeof(BigInteger)));
			ilProcessor.Body.Variables.Add(lineNumber);
			ilProcessor.Body.Variables.Add(lineToUpdate);
			ilProcessor.Body.Variables.Add(count);

			this.EmitExpression(line.Left, ilProcessor);
			ilProcessor.Emit(OpCodes.Stloc, lineNumber);
			this.EmitExpression(line.Right, ilProcessor);
			ilProcessor.Emit(OpCodes.Stloc, count);

			ilProcessor.Emit(OpCodes.Ldloc, lineNumber);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetMethod(nameof(BigInteger.Abs))));
			ilProcessor.Emit(OpCodes.Stloc, lineToUpdate);

			ilProcessor.Emit(OpCodes.Ldarg_0);
			ilProcessor.Emit(OpCodes.Ldloc, lineToUpdate);
			ilProcessor.Emit(OpCodes.Ldloc, lineNumber);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetProperty(nameof(BigInteger.Zero))!.GetGetMethod()));
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetMethod("op_GreaterThanOrEqual", new[] { typeof(BigInteger), typeof(BigInteger) })!));

			var noNegation = Instruction.Create(OpCodes.Nop);
			ilProcessor.Emit(OpCodes.Brtrue, noNegation);

			ilProcessor.Emit(OpCodes.Ldloc, count);
			ilProcessor.Emit(OpCodes.Call, ilProcessor.Body.Method.Module.ImportReference(
				typeof(BigInteger).GetMethod("op_UnaryNegation", new[] { typeof(BigInteger) })!));
			var negation = Instruction.Create(OpCodes.Nop);
			ilProcessor.Emit(OpCodes.Br, negation);

			ilProcessor.Append(noNegation);
			ilProcessor.Emit(OpCodes.Ldloc, count);
			ilProcessor.Append(negation);

			ilProcessor.Emit(OpCodes.Callvirt, ilProcessor.Body.Method.Module.ImportReference(
				typeof(IExecutionEngineActions).GetMethod(nameof(IExecutionEngineActions.UpdateCount))));
		}

		private TypeReference? ResolveType(string metadataName) =>
			this.assemblyDefinition.MainModule.ImportReference(
				this.assemblies.SelectMany(a => a.Modules)
					.SelectMany(m => m.Types)
					.First(t => t.FullName == metadataName));

		public EmitResult? Result { get; private set; }
	}
}