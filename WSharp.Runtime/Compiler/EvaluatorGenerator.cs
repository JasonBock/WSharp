using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;

namespace WSharp.Runtime.Compiler
{
	public static class EvaluatorGenerator
	{
		private static BigInteger Evaluate(ExpressionSyntax node, IExecutionEngineActions actions)
		{
			if(node is UpdateLineCountExpressionSyntax line)
			{
				var lineToUpdate = (BigInteger)EvaluatorGenerator.Evaluate(line.Left, actions);
				var count = (BigInteger)EvaluatorGenerator.Evaluate(line.Right, actions);
				actions.UpdateCount(lineToUpdate, count);
				return BigInteger.Zero;
			}
			else if(node is NumberExpressionSyntax number)
			{
				return (BigInteger)number.NumberToken.Value!;
			}
			else if(node is BinaryExpressionSyntax binary)
			{
				var left = EvaluatorGenerator.Evaluate(binary.Left, actions);
				var right = EvaluatorGenerator.Evaluate(binary.Right, actions);

				return binary.OperatorToken.Kind switch
				{
					SyntaxKind.PlusToken => left + right,
					SyntaxKind.MinusToken => left - right,
					SyntaxKind.StarToken => left * right,
					SyntaxKind.SlashToken => left / right,
					_ => throw new EvaluationException($"Unexpected binary operator {binary.OperatorToken.Kind}")
				};
			}
			else if(node is ParenthesizedExpressionSyntax parenthesized)
			{
				return EvaluatorGenerator.Evaluate(parenthesized.Expression, actions);
			}
			else
			{
				throw new EvaluationException($"Unexpected operator {node.Kind}");
			}
		}

		public static ImmutableList<Line> Generate(List<SyntaxTree> trees)
		{
			var builder = ImmutableList.CreateBuilder<Line>();

			foreach (var tree in trees)
			{
				var lineNumberNode = (NumberExpressionSyntax)tree.Root.GetChildren().First(_ => _.Kind == SyntaxKind.NumberExpression);
				var lineNumber = (BigInteger)lineNumberNode.NumberToken.Value!;

				builder.Add(new Line(lineNumber, BigInteger.One, actions =>
				{
					foreach (var expression in tree.Root.GetChildren().Where(_ => _ != lineNumberNode).Cast<ExpressionSyntax>())
					{
						EvaluatorGenerator.Evaluate(expression, actions);
					}
				}));
			}

			return builder.ToImmutable();
		}
	}
}