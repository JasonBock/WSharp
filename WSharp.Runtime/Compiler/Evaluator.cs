using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using WSharp.Runtime.Compiler.Binding;

namespace WSharp.Runtime.Compiler
{
	public sealed class Evaluator
	{
		private object Evaluate(BoundExpression node, IExecutionEngineActions? actions = null) =>
			node switch
			{
				BoundUpdateLineCountExpression line => this.EvaluateUpdateLineCountExpression(actions, line),
				BoundLiteralExpression literal => this.EvaluateLiteralExpression(literal),
				BoundUnaryExpression unary => this.EvaluateUnaryExpression(actions, unary),
				BoundBinaryExpression binary => this.EvaluateBinaryExpression(actions, binary),
				_ => throw new EvaluationException($"Unexpected operator {node.Kind}")
			};

		private object EvaluateUpdateLineCountExpression(IExecutionEngineActions? actions, BoundUpdateLineCountExpression line)
		{
			var lineToUpdate = (BigInteger)this.Evaluate(line.Left, actions);
			var count = (BigInteger)this.Evaluate(line.Right, actions);
			actions!.UpdateCount(lineToUpdate, count);
			return BigInteger.Zero;
		}

		private object EvaluateBinaryExpression(IExecutionEngineActions? actions, BoundBinaryExpression binary)
		{
			var left = this.Evaluate(binary.Left, actions);
			var right = this.Evaluate(binary.Right, actions);

			return binary.Operator.OperatorKind switch
			{
				BoundBinaryOperatorKind.Addition => (BigInteger)left + (BigInteger)right,
				BoundBinaryOperatorKind.Subtraction => (BigInteger)left - (BigInteger)right,
				BoundBinaryOperatorKind.Multiplication => (BigInteger)left * (BigInteger)right,
				BoundBinaryOperatorKind.Division => (BigInteger)left / (BigInteger)right,
				BoundBinaryOperatorKind.LogicalAnd => (bool)left && (bool)right,
				BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
				BoundBinaryOperatorKind.Equals => object.Equals(left, right),
				BoundBinaryOperatorKind.NotEquals => !object.Equals(left, right),
				_ => throw new EvaluationException($"Unexpected binary operator {binary.Operator}")
			};
		}

		private object EvaluateUnaryExpression(IExecutionEngineActions? actions, BoundUnaryExpression unary)
		{
			var operand = this.Evaluate(unary.Operand, actions);

			return unary.Operator.OperatorKind switch
			{
				BoundUnaryOperatorKind.Identity => (BigInteger)operand,
				BoundUnaryOperatorKind.Negation => -(BigInteger)operand,
				BoundUnaryOperatorKind.LogicalNegation => !(bool)operand,
				_ => throw new EvaluationException($"Unexpected unary operator {unary.Operator}")
			};
		}

		private object EvaluateLiteralExpression(BoundLiteralExpression literal) => literal.Value;

		public ImmutableArray<Line> Evaluate(List<BoundExpression> expressions)
		{
			var builder = ImmutableArray.CreateBuilder<Line>();

			foreach (var expression in expressions)
			{
				if (expression is BoundLineExpression line)
				{
					var lineNumber = (BigInteger)this.Evaluate(line.Number);

					builder.Add(new Line(lineNumber, BigInteger.One, actions =>
					{
						foreach (var lineExpression in line.Expressions)
						{
							this.Evaluate(expression, actions);
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