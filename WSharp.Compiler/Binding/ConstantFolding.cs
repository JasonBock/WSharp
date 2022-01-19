using System.Numerics;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding;

internal static class ConstantFolding
{
	internal static BoundConstant? ComputeConstant(
		BoundUnaryOperator @operator, BoundExpression operand)
	{
		if (operand.ConstantValue is { })
		{
			return @operator.OperatorKind switch
			{
				BoundUnaryOperatorKind.Identity => new BoundConstant((BigInteger)operand.ConstantValue.Value),
				BoundUnaryOperatorKind.Negation => new BoundConstant(-(BigInteger)operand.ConstantValue.Value),
				BoundUnaryOperatorKind.LogicalNegation => new BoundConstant(!(bool)operand.ConstantValue.Value),
				BoundUnaryOperatorKind.OnesComplement => new BoundConstant(~(BigInteger)operand.ConstantValue.Value),
				_ => throw new BindingException($"Unexpected unary operator {@operator.OperatorKind}")
			};
		}

		return null;
	}

	internal static BoundConstant? ComputeConstant(
		BoundExpression left, BoundBinaryOperator @operator, BoundExpression right)
	{
		var leftConstant = left.ConstantValue;
		var rightConstant = right.ConstantValue;

		if (@operator.OperatorKind == BoundBinaryOperatorKind.LogicalAnd)
		{
			if ((leftConstant is { } && !(bool)leftConstant.Value) ||
				(rightConstant is { } && !(bool)rightConstant.Value))
			{
				return new BoundConstant(false);
			}
		}

		if (@operator.OperatorKind == BoundBinaryOperatorKind.LogicalOr)
		{
			if ((leftConstant is { } && (bool)leftConstant.Value) ||
				(rightConstant is { } && (bool)rightConstant.Value))
			{
				return new BoundConstant(true);
			}
		}

		if (leftConstant is null || rightConstant is null)
		{
			return null;
		}

		var leftValue = leftConstant.Value;
		var rightValue = rightConstant.Value;

		return new BoundConstant(
			@operator.OperatorKind switch
			{
				BoundBinaryOperatorKind.Addition => left.Type == TypeSymbol.Integer ?
					(object)((BigInteger)leftValue + (BigInteger)rightValue) :
					(string)leftValue + (string)rightValue,
				BoundBinaryOperatorKind.Subtraction => (BigInteger)leftValue - (BigInteger)rightValue,
				BoundBinaryOperatorKind.Multiplication => (BigInteger)leftValue * (BigInteger)rightValue,
				BoundBinaryOperatorKind.Division => (BigInteger)leftValue / (BigInteger)rightValue,
				BoundBinaryOperatorKind.Modulo => (BigInteger)leftValue % (BigInteger)rightValue,
				BoundBinaryOperatorKind.BitwiseAnd => left.Type == TypeSymbol.Integer ?
					(object)((BigInteger)leftValue & (BigInteger)rightValue) :
					(bool)leftValue & (bool)rightValue,
				BoundBinaryOperatorKind.BitwiseOr => left.Type == TypeSymbol.Integer ?
					(object)((BigInteger)leftValue | (BigInteger)rightValue) :
					(bool)leftValue | (bool)rightValue,
				BoundBinaryOperatorKind.BitwiseXor => left.Type == TypeSymbol.Integer ?
					(object)((BigInteger)leftValue ^ (BigInteger)rightValue) :
					(bool)leftValue ^ (bool)rightValue,
				BoundBinaryOperatorKind.LogicalAnd => (bool)leftValue && (bool)rightValue,
				BoundBinaryOperatorKind.LogicalOr => (bool)leftValue || (bool)rightValue,
				BoundBinaryOperatorKind.Equals => object.Equals(left, right),
				BoundBinaryOperatorKind.NotEquals => !object.Equals(left, right),
				BoundBinaryOperatorKind.Less => (BigInteger)leftValue < (BigInteger)rightValue,
				BoundBinaryOperatorKind.LessOrEqualsTo => (BigInteger)leftValue <= (BigInteger)rightValue,
				BoundBinaryOperatorKind.Greater => (BigInteger)leftValue > (BigInteger)rightValue,
				BoundBinaryOperatorKind.GreaterOrEqualsTo => (BigInteger)leftValue >= (BigInteger)rightValue,
				_ => throw new BindingException($"Unexpected binary operator {@operator.OperatorKind}")
			});
	}
}