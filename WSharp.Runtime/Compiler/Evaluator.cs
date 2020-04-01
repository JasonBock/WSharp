using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using WSharp.Runtime.Compiler.Binding;

namespace WSharp.Runtime.Compiler
{
	public sealed class Evaluator
	{
		private object lastValue = new object();

		public Evaluator(List<BoundStatement> root) => this.Root = root;

		public ImmutableArray<Line> Evaluate()
		{
			var builder = ImmutableArray.CreateBuilder<Line>();

			foreach (var statement in this.Root)
			{
				this.EvaluateStatement(statement);

				builder.Add((Line)this.lastValue);
			}

			return builder.ToImmutable();
		}

		private void EvaluateStatement(BoundStatement node, IExecutionEngineActions? actions = null)
		{
			switch (node)
			{
				case BoundLineStatement line:
					this.EvaluateLineStatement(actions, line);
					break;
				case BoundExpressionStatement expression:
					this.lastValue = this.EvaluateExpression(expression.Expression, actions);
					break;
				default:
					throw new EvaluationException($"Unexpected operator {node.Kind}");
			};
		}

		private object EvaluateExpression(BoundExpression node, IExecutionEngineActions? actions = null) =>
			node switch
			{
				BoundUpdateLineCountExpression line => this.EvaluateUpdateLineCountExpression(actions, line),
				BoundLiteralExpression literal => this.EvaluateLiteralExpression(literal),
				BoundUnaryExpression unary => this.EvaluateUnaryExpression(actions, unary),
				BoundBinaryExpression binary => this.EvaluateBinaryExpression(actions, binary),
				_ => throw new EvaluationException($"Unexpected operator {node.Kind}")
			};

		private void EvaluateLineStatement(IExecutionEngineActions? actions, BoundLineStatement line)
		{
			this.EvaluateStatement(line.Number);

			var lineNumber = (BigInteger)this.lastValue;

			this.lastValue = new Line(lineNumber, BigInteger.One, actions =>
			{
				foreach (var statement in line.Statements)
				{
					this.EvaluateStatement(statement, actions);
				}
			});
		}

		private object EvaluateUpdateLineCountExpression(IExecutionEngineActions? actions, BoundUpdateLineCountExpression line)
		{
			var lineToUpdate = (BigInteger)this.EvaluateExpression(line.Left, actions);
			var count = (BigInteger)this.EvaluateExpression(line.Right, actions);
			actions!.UpdateCount(lineToUpdate, count);
			return BigInteger.Zero;
		}

		private object EvaluateBinaryExpression(IExecutionEngineActions? actions, BoundBinaryExpression binary)
		{
			var left = this.EvaluateExpression(binary.Left, actions);
			var right = this.EvaluateExpression(binary.Right, actions);

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
			var operand = this.EvaluateExpression(unary.Operand, actions);

			return unary.Operator.OperatorKind switch
			{
				BoundUnaryOperatorKind.Identity => (BigInteger)operand,
				BoundUnaryOperatorKind.Negation => -(BigInteger)operand,
				BoundUnaryOperatorKind.LogicalNegation => !(bool)operand,
				_ => throw new EvaluationException($"Unexpected unary operator {unary.Operator}")
			};
		}

		private object EvaluateLiteralExpression(BoundLiteralExpression literal) => literal.Value;

		public List<BoundStatement> Root { get; }
	}
}