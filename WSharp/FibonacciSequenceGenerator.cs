using System.Collections.Immutable;
using System.Numerics;
using WSharp.Runtime;

namespace WSharp
{
	internal static class FibonacciSequenceGenerator
	{
		internal static ImmutableList<Line> Generate()
		{
			var builder = ImmutableList.CreateBuilder<Line>();

			builder.Add(new Line(1, BigInteger.One, actions =>
			{
				actions.Again(actions.DoesLineExist(1));
				if (!actions.Defer(actions.DoesLineExist(3) || actions.N(1) <= actions.N(2) ||
					actions.N(7) > new BigInteger(99)))
				{
					actions.UpdateCount(2, actions.N(1));
					actions.UpdateCount(3, BigInteger.One);
					actions.UpdateCount(7, BigInteger.One);
				}
			}));
			builder.Add(new Line(2, BigInteger.One, actions =>
			{
				actions.Again(actions.DoesLineExist(2));
				if (!actions.Defer(actions.DoesLineExist(3) || actions.N(2) <= actions.N(1) ||
					actions.N(7) > new BigInteger(99)))
				{
					actions.UpdateCount(1, actions.N(2));
					actions.UpdateCount(3, BigInteger.One);
					actions.UpdateCount(7, BigInteger.One);
				}
			}));
			builder.Add(new Line(3, BigInteger.One, actions =>
			{
				if (!actions.Defer(actions.DoesLineExist(5)))
				{
					actions.Print(actions.N(1) + " " + actions.N(2));
				}
			}));
			builder.Add(new Line(4, BigInteger.One, actions =>
			{
				if (!actions.Defer(actions.DoesLineExist(5)))
				{
					actions.Print("1");
				}
			}));
			builder.Add(new Line(5, BigInteger.One, actions =>
			{
				actions.UpdateCount(4, BigInteger.One);
				actions.UpdateCount(3, BigInteger.MinusOne);
				actions.UpdateCount(7, BigInteger.One);
			}));
			builder.Add(new Line(6, BigInteger.One, actions =>
			{
				if (!actions.Defer(actions.DoesLineExist(4)))
				{
					actions.UpdateCount(3, BigInteger.One);
				}
			}));
			builder.Add(new Line(7, BigInteger.One, actions =>
			{
				actions.UpdateCount(7, BigInteger.One);
			}));
			builder.Add(new Line(8, BigInteger.One, actions =>
			{
				if (!actions.Defer(actions.N(7) < new BigInteger(100)))
				{
					actions.UpdateCount(1, BigInteger.MinusOne * actions.N(1));
					actions.UpdateCount(2, BigInteger.MinusOne * actions.N(2));
					actions.UpdateCount(7, BigInteger.MinusOne * new BigInteger(100));
				}
			}));
			builder.Add(new Line(9, BigInteger.One, actions =>
			{
				if (!actions.Defer(actions.DoesLineExist(3) || actions.DoesLineExist(6)))
				{
					actions.UpdateCount(1, BigInteger.One);
					actions.UpdateCount(3, BigInteger.One);
				}
			}));

			return builder.ToImmutable();
		}
	}
}
