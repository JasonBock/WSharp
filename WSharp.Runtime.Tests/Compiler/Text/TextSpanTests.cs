using NUnit.Framework;
using WSharp.Runtime.Compiler.Text;

namespace WSharp.Runtime.Tests.Compiler.Text
{
	public static class TextSpanTests
	{
		[Test]
		public static void Create()
		{
			var span = new TextSpan();

			Assert.Multiple(() =>
			{
				Assert.That(span.Start, Is.EqualTo(0), nameof(span.Start));
				Assert.That(span.Length, Is.EqualTo(0), nameof(span.Length));
				Assert.That(span.End, Is.EqualTo(0), nameof(span.End));
			});
		}

		[Test]
		public static void CreateWithValues()
		{
			var span = new TextSpan(1, 2);

			Assert.Multiple(() =>
			{
				Assert.That(span.Start, Is.EqualTo(1), nameof(span.Start));
				Assert.That(span.Length, Is.EqualTo(2), nameof(span.Length));
				Assert.That(span.End, Is.EqualTo(3), nameof(span.End));
			});
		}

		[Test]
		public static void CreateFromBounds()
		{
			var span = TextSpan.FromBounds(1, 4);

			Assert.Multiple(() =>
			{
				Assert.That(span.Start, Is.EqualTo(1), nameof(span.Start));
				Assert.That(span.Length, Is.EqualTo(3), nameof(span.Length));
				Assert.That(span.End, Is.EqualTo(4), nameof(span.End));
			});
		}
	}
}