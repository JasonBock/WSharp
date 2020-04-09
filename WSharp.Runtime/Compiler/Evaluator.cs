using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using WSharp.Runtime.Compiler.Binding;
using WSharp.Runtime.Compiler.Symbols;

namespace WSharp.Runtime.Compiler
{
	public sealed class Evaluator
	{
		private object lastValue = new object();

		public Evaluator(BoundStatement root) => this.Root = root;

		public ImmutableArray<Line> Evaluate()
		{
			this.EvaluateStatement(this.Root);
			return ((List<Line>)this.lastValue).ToImmutableArray();
		}

		private void EvaluateStatement(BoundStatement node, IExecutionEngineActions? actions = null)
		{
			switch (node)
			{
				case BoundLineStatements lineStatements:
					this.EvaluateLineStatements(lineStatements);
					break;
				case BoundLineStatement line:
					this.EvaluateLineStatement(line);
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
				BoundCallExpression call => this.EvaluateCallExpression(actions, call),
				BoundUnaryUpdateLineCountExpression unaryLine => this.EvaluateUnaryUpdateLineCountExpression(actions, unaryLine),
				BoundUpdateLineCountExpression line => this.EvaluateUpdateLineCountExpression(actions, line),
				BoundLiteralExpression literal => this.EvaluateLiteralExpression(literal),
				BoundUnaryExpression unary => this.EvaluateUnaryExpression(actions, unary),
				BoundBinaryExpression binary => this.EvaluateBinaryExpression(actions, binary),
				_ => throw new EvaluationException($"Unexpected operator {node.Kind}")
			};

		private object EvaluateCallExpression(IExecutionEngineActions? actions, BoundCallExpression call)
		{
			if(call.Function == BuiltinFunctions.Read)
			{
				return actions!.Read();
			}
			else if(call.Function == BuiltinFunctions.Print)
			{
				actions!.Print((string)this.EvaluateExpression(call.Arguments[0]));
				return new object();
			}
			else
			{
				throw new EvaluationException($"Unexpected function {call.Function.Name}");
			}
		}

		private void EvaluateLineStatements(BoundLineStatements lineStatements)
		{
			var lines = new List<Line>();

			foreach(var line in lineStatements.LineStatements)
			{
				this.EvaluateLineStatement(line);
				lines.Add((Line)this.lastValue);
			}

			this.lastValue = lines;
		}

		private void EvaluateLineStatement(BoundLineStatement line)
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

		private object EvaluateUnaryUpdateLineCountExpression(IExecutionEngineActions? actions, BoundUnaryUpdateLineCountExpression unaryLine)
		{
			var lineToUpdate = BigInteger.Abs((BigInteger)this.EvaluateExpression(unaryLine.LineNumber, actions));
			var count = lineToUpdate > BigInteger.Zero ? BigInteger.One : 
				lineToUpdate < BigInteger.Zero ? -BigInteger.One : BigInteger.Zero;
			actions!.UpdateCount(lineToUpdate, count);
			return BigInteger.Zero;
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
				BoundBinaryOperatorKind.Addition => binary.Type == TypeSymbol.Number ? (object)((BigInteger)left + (BigInteger)right) : (string)left + (string)right,
				BoundBinaryOperatorKind.Subtraction => (BigInteger)left - (BigInteger)right,
				BoundBinaryOperatorKind.Multiplication => (BigInteger)left * (BigInteger)right,
				BoundBinaryOperatorKind.Division => (BigInteger)left / (BigInteger)right,
				BoundBinaryOperatorKind.BitwiseAnd => binary.Type == TypeSymbol.Number ? (object)((BigInteger)left & (BigInteger)right) : (bool)left & (bool)right,
				BoundBinaryOperatorKind.BitwiseOr => binary.Type == TypeSymbol.Number ? (object)((BigInteger)left | (BigInteger)right) : (bool)left | (bool)right,
				BoundBinaryOperatorKind.BitwiseXor => binary.Type == TypeSymbol.Number ? (object)((BigInteger)left ^ (BigInteger)right) : (bool)left ^ (bool)right,
				BoundBinaryOperatorKind.LogicalAnd => (bool)left && (bool)right,
				BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
				BoundBinaryOperatorKind.Equals => object.Equals(left, right),
				BoundBinaryOperatorKind.NotEquals => !object.Equals(left, right),
				BoundBinaryOperatorKind.Less => (BigInteger)left < (BigInteger)right,
				BoundBinaryOperatorKind.LessOrEqualsTo => (BigInteger)left <= (BigInteger)right,
				BoundBinaryOperatorKind.Greater => (BigInteger)left > (BigInteger)right,
				BoundBinaryOperatorKind.GreaterOrEqualsTo => (BigInteger)left >= (BigInteger)right,
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
				BoundUnaryOperatorKind.OnesComplement => ~(BigInteger)operand,
				_ => throw new EvaluationException($"Unexpected unary operator {unary.Operator}")
			};
		}

		private object EvaluateLiteralExpression(BoundLiteralExpression literal) => literal.Value;

		public BoundStatement Root { get; }
	}
}