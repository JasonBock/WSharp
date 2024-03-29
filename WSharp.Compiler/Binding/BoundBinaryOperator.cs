﻿using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

public sealed class BoundBinaryOperator
{
	private static readonly BoundBinaryOperator[] operators =
	{
		new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.Integer),
		new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, TypeSymbol.String),
		new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, TypeSymbol.Integer),
		new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, TypeSymbol.Integer),
		new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, TypeSymbol.Integer),
		new BoundBinaryOperator(SyntaxKind.PercentToken, BoundBinaryOperatorKind.Modulo, TypeSymbol.Integer),
		new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, TypeSymbol.Integer),
		new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, TypeSymbol.Integer),
		new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.BitwiseXor, TypeSymbol.Integer),
		new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.Integer, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, TypeSymbol.String, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Integer, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, TypeSymbol.String, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.LessToken, BoundBinaryOperatorKind.Less, TypeSymbol.Integer, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken, BoundBinaryOperatorKind.LessOrEqualsTo, TypeSymbol.Integer, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.GreaterToken, BoundBinaryOperatorKind.Greater, TypeSymbol.Integer, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken, BoundBinaryOperatorKind.GreaterOrEqualsTo, TypeSymbol.Integer, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.BitwiseAnd, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.BitwiseOr, TypeSymbol.Boolean),
		new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, TypeSymbol.Boolean),
	};

	private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind operatorKind, TypeSymbol type)
		: this(syntaxKind, operatorKind, type, type, type) { }

	private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind operatorKind, TypeSymbol operandType, TypeSymbol resultType)
		: this(syntaxKind, operatorKind, operandType, operandType, resultType) { }

	private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind operatorKind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType) =>
		(this.SyntaxKind, this.OperatorKind, this.LeftType, this.RightType, this.ResultType) = (syntaxKind, operatorKind, leftType, rightType, resultType);

	public static BoundBinaryOperator? Bind(SyntaxKind syntaxKind, TypeSymbol leftType, TypeSymbol rightType)
	{
		foreach (var @operator in BoundBinaryOperator.operators)
		{
			if (@operator.SyntaxKind == syntaxKind && @operator.LeftType == leftType && @operator.RightType == rightType)
			{
				return @operator;
			}
		}

		return null;
	}

	public TypeSymbol LeftType { get; }
	public BoundBinaryOperatorKind OperatorKind { get; }
	public TypeSymbol ResultType { get; }
	public TypeSymbol RightType { get; }
	public SyntaxKind SyntaxKind { get; }
}