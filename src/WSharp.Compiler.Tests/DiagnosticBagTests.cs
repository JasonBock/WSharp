using NUnit.Framework;
using System.Numerics;
using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests;

public static class DiagnosticBagTests
{
	[Test]
	public static void Create()
	{
		var bag = new DiagnosticBag();
		Assert.That(bag.Count, Is.EqualTo(0), nameof(bag.Count));
	}

	[Test]
	public static void Enumerate()
	{
		var bag = new DiagnosticBag();
		bag.ReportRequiredTypeNotFound("a", "b");

		Assert.That(bag.Count, Is.EqualTo(1));
	}

	[Test]
	public static void AddRange()
	{
		var rangeBag = new DiagnosticBag();
		rangeBag.ReportRequiredTypeNotFound("a", "b");

		var bag = new DiagnosticBag();
		bag.AddRange(rangeBag);
		Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
	}

	[Test]
	public static void ReportInvalidReference()
	{
		var bag = new DiagnosticBag();
		var file = new FileInfo(typeof(DiagnosticBagTests).Assembly.Location);
		bag.ReportInvalidReference(file);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(default(TextLocation)), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message, Is.EqualTo($"The reference is not a valid .NET assembly: '{file.FullName}'."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportInvalidReferenceWhenReferenceIsNull() =>
		Assert.That(() => new DiagnosticBag().ReportInvalidReference(null!), Throws.TypeOf<ArgumentNullException>());

	[Test]
	public static void ReportRequiredTypeNotFound()
	{
		var bag = new DiagnosticBag();
		bag.ReportRequiredTypeNotFound("a", "b");

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(default(TextLocation)), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message, Is.EqualTo("The required type 'a' ('b') cannot be resolved."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportRequiredTypeNotFoundWhenWheneverNameIsNull()
	{
		var bag = new DiagnosticBag();
		bag.ReportRequiredTypeNotFound(null, "b");

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(default(TextLocation)), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message, Is.EqualTo("The required type 'b' cannot be resolved."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportRequiredMethodNotFound()
	{
		var bag = new DiagnosticBag();
		bag.ReportRequiredMethodNotFound("a", "b", new[] { "c", "d" });

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(default(TextLocation)), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("The required method 'a.b(c, d)' cannot be resolved among the given references."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportInvalidNumber()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportInvalidNumber(location, "a");

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("The number 'a' isn't valid."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUnterminatedString()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportUnterminatedString(location);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Unterminated string literal."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUnterminatedMultiLineComment()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportUnterminatedMultiLineComment(location);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Unterminated multi-line comment."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportBadCharacter()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportBadCharacter(location, 'a');

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Bad character input: 'a'."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUnexpectedToken()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportUnexpectedToken(location, SyntaxKind.AmpersandAmpersandToken, SyntaxKind.AmpersandToken);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Unexpected token <AmpersandAmpersandToken>, expected <AmpersandToken>."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportExpressionMustHaveValue()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportExpressionMustHaveValue(location);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Expression must have a value."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportMissingSemicolon()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportMissingSemicolon(location);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("; expected."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUndefinedUnaryOperator()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportUndefinedUnaryOperator(location, "s", TypeSymbol.Integer);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Unary operator 's' is not defined for type 'integer'."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUndefinedBinaryOperator()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportUndefinedBinaryOperator(location, "||", TypeSymbol.Integer, TypeSymbol.String);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Binary operator '||' is not defined for types 'integer' and 'string'."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportMissingLineStatements()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportMissingLineStatements(location);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("No line statements exist."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUndefinedLineCountOperator()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportUndefinedLineCountOperator(location, "#", TypeSymbol.Integer, TypeSymbol.String);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Update line count operator '#' is not defined for types 'integer' and 'string'."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportWrongArgumentCount()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportWrongArgumentCount(location, "a", 1, 2);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Function 'a' requires 1 argument(s) but was given 2."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUndefinedFunction()
	{
		var (tokens, _) = SyntaxTree.ParseTokens("a");
		var token = tokens[0];
		var bag = new DiagnosticBag();
		bag.ReportUndefinedFunction(token);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(token.Location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Function 'a' doesn't exist."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUndefinedFunctionWhenIdentifierIsNull() =>
		Assert.That(() => new DiagnosticBag().ReportUndefinedFunction(null!), Throws.TypeOf<ArgumentNullException>());

	[Test]
	public static void ReportInvalidLineNumberReference()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportInvalidLineNumberReference(location, BigInteger.One);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("The line number 1 does not exist."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportWrongArgumentType()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportWrongArgumentType(location, "a", TypeSymbol.Boolean, TypeSymbol.Integer);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Parameter 'a' requires a value of type 'boolean' but was given a value of type 'integer'."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportCannotConvert()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportCannotConvert(location, TypeSymbol.String, TypeSymbol.Integer);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Cannot convert type 'string' to 'integer'."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUnexpectedArgumentSyntax()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportUnexpectedArgumentSyntax(location);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Unexpected argument syntax."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportUnexpectedLineStatementToken()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportUnexpectedLineStatementToken(location);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Unexpected line statement token."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportCannotConvertImplicitly()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportCannotConvertImplicitly(location, TypeSymbol.String, TypeSymbol.Integer);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("Cannot convert type 'string' to 'integer'. An explicit conversion exists (are you missing a cast?)"), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportNoStatementsAfterDefer()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportNoStatementsAfterDefer(location);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("No statements exist after a call to defer()."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void ReportDuplicateLineNumber()
	{
		var bag = new DiagnosticBag();
		var location = new TextLocation();
		bag.ReportDuplicateLineNumber(location, BigInteger.One);

		Assert.Multiple(() =>
		{
			Assert.That(bag.Count, Is.EqualTo(1), nameof(bag.Count));
			var diagnostic = bag.ToArray()[0];
			Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message,
					 Is.EqualTo("The line number 1 was already used."), nameof(diagnostic.Message));
		});
	}
}