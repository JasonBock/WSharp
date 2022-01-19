namespace WSharp.Compiler.Tests;

public static class CompilationTests
{
	// TODO: All of this needs to be done with syntax nodes from the tree 
	// and not bound nodes, because all of them are internal.

	//private static void GetStatements(string code, Action<List<BoundStatement>> statements)
	//{
	//	var tree = SyntaxTree.Parse(code);
	//	var compiler = new Compilation(tree);

	//	Assert.Multiple(() =>
	//	{
	//		Assert.That(compiler.Diagnostics.Count, Is.EqualTo(0), nameof(compiler.Diagnostics));
	//		Assert.That(compiler.Tree, Is.SameAs(tree), nameof(compiler.Tree));

	//		var lineStatements = (BoundLineStatements)compiler.Statement;
	//		Assert.That(lineStatements.LineStatements.Count, Is.EqualTo(1), nameof(lineStatements.LineStatements));
	//		Assert.That(lineStatements.Kind, Is.EqualTo(BoundNodeKind.LineStatements), nameof(lineStatements.Kind));

	//		var statement = lineStatements.GetChildren().ToArray()[0];
	//		Assert.That(statement.Kind, Is.EqualTo(BoundNodeKind.LineStatement), nameof(statement.Kind));

	//		statements(((BoundLineStatement)statement).Statements);
	//	});
	//}

	//[Test]
	//public static void CompileWithPrintCall() =>
	//	CompilationTests.GetStatements("1 print(\"hi\")", statements =>
	//		{
	//			Assert.That(statements.Count, Is.EqualTo(1), nameof(statements.Count));

	//			var call = (BoundCallExpression)((BoundExpressionStatement)statements[0]).Expression;
	//			Assert.That(call.Kind, Is.EqualTo(BoundNodeKind.CallExpression), nameof(call.Kind));
	//			Assert.That(call.Type, Is.EqualTo(TypeSymbol.Void), nameof(call.Type));
	//			Assert.That(call.Arguments.Length, Is.EqualTo(1), nameof(call.Arguments));
	//			Assert.That(call.ConstantValue, Is.Null, nameof(call.ConstantValue));
	//			Assert.That(call.Function.Name, Is.EqualTo("print"), nameof(call.Function.Name));
	//			Assert.That(call.Function.ReturnType, Is.EqualTo(TypeSymbol.Void), nameof(call.Function.ReturnType));
	//			Assert.That(call.Function.Parameters.Length, Is.EqualTo(1), nameof(call.Function.Parameters));

	//			var callParameter = call.Function.Parameters[0];
	//			Assert.That(callParameter.Kind, Is.EqualTo(SymbolKind.Parameter), nameof(callParameter.Kind));
	//			Assert.That(callParameter.Name, Is.EqualTo("value"), nameof(callParameter.Name));
	//			Assert.That(callParameter.Type, Is.EqualTo(TypeSymbol.Any), nameof(callParameter.Type));

	//			var callArgument = (BoundConversionExpression)call.Arguments[0];
	//			Assert.That(callArgument.ConstantValue, Is.Null, nameof(callArgument.ConstantValue));
	//			Assert.That(callArgument.Kind, Is.EqualTo(BoundNodeKind.ConversionExpression), nameof(callArgument.Kind));
	//			Assert.That(callArgument.Type, Is.EqualTo(TypeSymbol.Any), nameof(callArgument.Type));

	//			var callArgumentLiteral = (BoundLiteralExpression)callArgument.Expression;
	//			Assert.That(callArgumentLiteral.ConstantValue.Value, Is.EqualTo("hi"), nameof(callArgumentLiteral.ConstantValue.Value));
	//			Assert.That(callArgumentLiteral.Kind, Is.EqualTo(BoundNodeKind.LiteralExpression), nameof(callArgument.Kind));
	//			Assert.That(callArgumentLiteral.Type, Is.EqualTo(TypeSymbol.String), nameof(callArgument.Type));
	//		});

	//[Test]
	//public static void CompileWithUpdateLineCount() =>
	//	CompilationTests.GetStatements("1 1#2;", statements =>
	//	{
	//		Assert.That(statements.Count, Is.EqualTo(1), nameof(statements.Count));

	//		var update = (BoundUpdateLineCountExpression)((BoundExpressionStatement)statements[0]).Expression;
	//		Assert.That(update.Kind, Is.EqualTo(BoundNodeKind.UpdateLineCountExpression), nameof(update.Kind));
	//		Assert.That(update.ConstantValue, Is.Null, nameof(update.ConstantValue));
	//		Assert.That(update.OperatorKind, Is.EqualTo(BoundUpdateLineCountOperatorKind.Update), nameof(update.ConstantValue));
	//		Assert.That(update.Type, Is.EqualTo(TypeSymbol.Integer), nameof(update.Type));

	//		var left = (BoundLiteralExpression)update.Left;
	//		Assert.That(left.Kind, Is.EqualTo(BoundNodeKind.LiteralExpression), nameof(left.Kind));
	//		Assert.That(left.ConstantValue.Value, Is.EqualTo(BigInteger.One), nameof(left.ConstantValue));
	//		Assert.That(left.Type, Is.EqualTo(TypeSymbol.Integer), nameof(left.Type));
	//		Assert.That(left.Value, Is.EqualTo(BigInteger.One), nameof(left.Value));

	//		var right = (BoundLiteralExpression)update.Right;
	//		Assert.That(right.Kind, Is.EqualTo(BoundNodeKind.LiteralExpression), nameof(right.Kind));
	//		Assert.That(right.ConstantValue.Value, Is.EqualTo(new BigInteger(2)), nameof(right.ConstantValue));
	//		Assert.That(right.Type, Is.EqualTo(TypeSymbol.Integer), nameof(right.Type));
	//		Assert.That(right.Value, Is.EqualTo(new BigInteger(2)), nameof(right.Value));
	//	});

	//[Test]
	//public static void CompileWithUnaryUpdateLineCount() =>
	//	CompilationTests.GetStatements("1 1;", statements =>
	//	{
	//		Assert.That(statements.Count, Is.EqualTo(1), nameof(statements.Count));

	//		var update = (BoundUnaryUpdateLineCountExpression)((BoundExpressionStatement)statements[0]).Expression;
	//		Assert.That(update.Kind, Is.EqualTo(BoundNodeKind.UnaryUpdateLineCountExpression), nameof(update.Kind));
	//		Assert.That(update.ConstantValue, Is.Null, nameof(update.ConstantValue));
	//		Assert.That(update.Type, Is.EqualTo(TypeSymbol.Integer), nameof(update.Type));

	//		var left = (BoundLiteralExpression)update.LineNumber;
	//		Assert.That(left.Kind, Is.EqualTo(BoundNodeKind.LiteralExpression), nameof(left.Kind));
	//		Assert.That(left.ConstantValue.Value, Is.EqualTo(BigInteger.One), nameof(left.ConstantValue));
	//		Assert.That(left.Type, Is.EqualTo(TypeSymbol.Integer), nameof(left.Type));
	//		Assert.That(left.Value, Is.EqualTo(BigInteger.One), nameof(left.Value));
	//	});
}