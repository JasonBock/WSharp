using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using WSharp.Runtime.Compiler.Binding;

namespace WSharp.Runtime.Compiler
{
	public static class BoundEvaluatorGenerator
	{
		private static BigInteger Evaluate(BoundExpression node, IExecutionEngineActions? actions = null)
		{
			if (node is BoundUpdateLineCountExpression line)
			{
				var lineToUpdate = BoundEvaluatorGenerator.Evaluate(line.Left, actions);
				var count = BoundEvaluatorGenerator.Evaluate(line.Right, actions);
				actions!.UpdateCount(lineToUpdate, count);
				return BigInteger.Zero;
			}
			else if (node is BoundLiteralExpression literal)
			{
				return (BigInteger)literal.Value;
			}
			else if (node is BoundUnaryExpression unary)
			{
				var operand = BoundEvaluatorGenerator.Evaluate(unary.Operand, actions);

				if (unary.OperatorKind == BoundUnaryOperatorKind.Identity)
				{
					return operand;
				}
				else if (unary.OperatorKind == BoundUnaryOperatorKind.Negation)
				{
					return -operand;
				}
				else
				{
					throw new EvaluationException($"Unexpected unary operator {unary.OperatorKind}");
				}
			}
			else if (node is BoundBinaryExpression binary)
			{
				var left = BoundEvaluatorGenerator.Evaluate(binary.Left, actions);
				var right = BoundEvaluatorGenerator.Evaluate(binary.Right, actions);

				return binary.OperatorKind switch
				{
					BoundBinaryOperatorKind.Addition => left + right,
					BoundBinaryOperatorKind.Subtraction => left - right,
					BoundBinaryOperatorKind.Multiplication => left * right,
					BoundBinaryOperatorKind.Division => left / right,
					_ => throw new EvaluationException($"Unexpected binary operator {binary.OperatorKind}")
				};
			}
			else
			{
				throw new EvaluationException($"Unexpected operator {node.Kind}");
			}
		}

		public static ImmutableList<Line> Generate(List<BoundExpression> expressions)
		{
			var builder = ImmutableList.CreateBuilder<Line>();

			foreach (var expression in expressions)
			{
				if(expression is BoundLineExpression line)
				{
					var lineNumber = BoundEvaluatorGenerator.Evaluate(line.Number);

					builder.Add(new Line(lineNumber, BigInteger.One, actions =>
					{
						foreach(var lineExpression in line.Expressions)
						{
							BoundEvaluatorGenerator.Evaluate(expression, actions);
						}
					}));
				}
				else
				{
					throw new EvaluationException($"Unexpected expression {expression.Kind}");
				}
			}

			return builder.ToImmutable();
		}
	}
}