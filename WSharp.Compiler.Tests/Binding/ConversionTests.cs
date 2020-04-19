using NUnit.Framework;
using WSharp.Compiler.Binding;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Tests.Compiler.Binding
{
	public static class ConversionTests
	{
		[Test]
		public static void VerifyExplicitConversion() =>
			Assert.Multiple(() =>
			{
				Assert.That(Conversion.Explicit.Exists, Is.True, nameof(Conversion.Exists));
				Assert.That(Conversion.Explicit.IsExplicit, Is.True, nameof(Conversion.IsExplicit));
				Assert.That(Conversion.Explicit.IsIdentity, Is.False, nameof(Conversion.IsIdentity));
				Assert.That(Conversion.Explicit.IsImplicit, Is.False, nameof(Conversion.IsImplicit));
			});

		[Test]
		public static void VerifyIdentityConversion() =>
			Assert.Multiple(() =>
			{
				Assert.That(Conversion.Identity.Exists, Is.True, nameof(Conversion.Exists));
				Assert.That(Conversion.Identity.IsExplicit, Is.False, nameof(Conversion.IsExplicit));
				Assert.That(Conversion.Identity.IsIdentity, Is.True, nameof(Conversion.IsIdentity));
				Assert.That(Conversion.Identity.IsImplicit, Is.True, nameof(Conversion.IsImplicit));
			});

		[Test]
		public static void VerifyImplicitConversion() =>
			Assert.Multiple(() =>
			{
				Assert.That(Conversion.Implicit.Exists, Is.True, nameof(Conversion.Exists));
				Assert.That(Conversion.Implicit.IsExplicit, Is.False, nameof(Conversion.IsExplicit));
				Assert.That(Conversion.Implicit.IsIdentity, Is.False, nameof(Conversion.IsIdentity));
				Assert.That(Conversion.Implicit.IsImplicit, Is.True, nameof(Conversion.IsImplicit));
			});

		[Test]
		public static void VerifyNoneConversion() =>
			Assert.Multiple(() =>
			{
				Assert.That(Conversion.None.Exists, Is.False, nameof(Conversion.Exists));
				Assert.That(Conversion.None.IsExplicit, Is.False, nameof(Conversion.IsExplicit));
				Assert.That(Conversion.None.IsIdentity, Is.False, nameof(Conversion.IsIdentity));
				Assert.That(Conversion.None.IsImplicit, Is.False, nameof(Conversion.IsImplicit));
			});

		[Test]
		public static void ClassifyIdentiy() => 
			Assert.That(Conversion.Classify(TypeSymbol.Boolean, TypeSymbol.Boolean), Is.EqualTo(Conversion.Identity));

		[Test]
		public static void ClassifyExplicitWhenFromIsInteger() =>
			Assert.That(Conversion.Classify(TypeSymbol.Integer, TypeSymbol.String), Is.EqualTo(Conversion.Explicit));

		[Test]
		public static void ClassifyExplicitWhenFromIsBoolean() =>
			Assert.That(Conversion.Classify(TypeSymbol.Boolean, TypeSymbol.String), Is.EqualTo(Conversion.Explicit));

		[Test]
		public static void ClassifyExplicitWhenFromIsStringAndToIsInteger() =>
			Assert.That(Conversion.Classify(TypeSymbol.String, TypeSymbol.Integer), Is.EqualTo(Conversion.Explicit));

		[Test]
		public static void ClassifyExplicitWhenFromIsStringAndToIsBoolean() =>
			Assert.That(Conversion.Classify(TypeSymbol.String, TypeSymbol.Boolean), Is.EqualTo(Conversion.Explicit));

		[Test]
		public static void ClassifyNone() =>
			Assert.That(Conversion.Classify(TypeSymbol.Integer, TypeSymbol.Boolean), Is.EqualTo(Conversion.None));
	}
}