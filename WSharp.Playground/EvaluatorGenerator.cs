using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using WSharp.Runtime;

namespace WSharp.Playground
{
	public static class EvaluatorGenerator
	{
		internal static ImmutableList<Line> Generate(List<SyntaxTree> trees)
		{
			var builder = ImmutableList.CreateBuilder<Line>();

			foreach(var tree in trees)
			{
				var lineNumberNode = (NumberExpressionSyntax)tree.Root.GetChildren().First(_ => _.Kind == SyntaxKind.NumberExpression);
				var lineNumber = (BigInteger)lineNumberNode.NumberToken.Value!;
				
				builder.Add(new Line(lineNumber, BigInteger.One, actions =>
				{
					foreach (var updateLineCountExpression in tree.Root.GetChildren().Where(
						_ => _.Kind == SyntaxKind.UpdateLineCountExpression).Cast<UpdateLineCountExpressionSyntax>())
					{
						var lineToUpdate = (BigInteger)((NumberExpressionSyntax)updateLineCountExpression.GetChildren().First(
							_ => _.Kind == SyntaxKind.NumberExpression)).NumberToken.Value!;
						var count = (BigInteger)((NumberExpressionSyntax)updateLineCountExpression.GetChildren().Last(
							_ => _.Kind == SyntaxKind.NumberExpression)).NumberToken.Value!;
						actions.UpdateCount(lineToUpdate, count);
					}
				}));
			}

			return builder.ToImmutable();
		}
	}
}