﻿using Spackle;
using System.Collections.Immutable;
using System.Numerics;
using WSharp.Runtime;

namespace WSharp
{
   internal static class FibonacciSequenceGenerator
	{
		/*
1 again (1) defer (3 || N(1)<=N(2) || N(7)>99) 2#N(1),3,7;
2 again (2) defer (3 || N(2)<=N(1) || N(7)>99) 1#N(2),3,7;
3 defer (5) print(N(1)+N(2));
4 defer (5) print("1");
5 4,-3,7;
6 defer (4) 3;
7 7;
8 defer (N(7)<100) -1#N(1),-2#N(2),-7#100,-3;
9 defer (3 || 6) 1,3; 
		*/

		internal static void Run()
		{
			using var random = new SecureRandom();
			new ExecutionEngine(FibonacciSequenceGenerator.Generate(), random, Console.In, Console.Out).Execute();
		}

		private static ImmutableArray<Line> Generate()
		{
			var builder = ImmutableArray.CreateBuilder<Line>();

			// 1 again (1) defer (3 || N(1)<=N(2) || N(7)>99) 2#N(1),3,7;
			builder.Add(new Line(1, BigInteger.One, actions =>
			{
				actions.Again(actions.E(1));
				actions.Defer(actions.E(3) || actions.N(1) <= actions.N(2) || actions.N(7) > new BigInteger(99));
				if (!actions.ShouldStatementBeDeferred)
				{
					actions.UpdateCount(2, actions.N(1));
					actions.UpdateCount(3, BigInteger.One);
					actions.UpdateCount(7, BigInteger.One);
				}
			}));
			// 2 again (2) defer (3 || N(2)<=N(1) || N(7)>99) 1#N(2),3,7;
			builder.Add(new Line(2, BigInteger.One, actions =>
			{
				actions.Again(actions.E(2));
				actions.Defer(actions.E(3) || actions.N(2) <= actions.N(1) || actions.N(7) > new BigInteger(99));
				if (!actions.ShouldStatementBeDeferred)
				{
					actions.UpdateCount(1, actions.N(2));
					actions.UpdateCount(3, BigInteger.One);
					actions.UpdateCount(7, BigInteger.One);
				}
			}));
			// 3 defer (5) print(N(1)+N(2));
			builder.Add(new Line(3, BigInteger.One, actions =>
			{
				actions.Defer(actions.E(5));
				if (!actions.ShouldStatementBeDeferred)
				{
					actions.Print(actions.N(1) + " " + actions.N(2));
				}
			}));
			// 4 defer (5) print("1");
			builder.Add(new Line(4, BigInteger.One, actions =>
			{
				actions.Defer(actions.E(5));
				if (!actions.ShouldStatementBeDeferred)
				{
					actions.Print("1");
				}
			}));
			// 5 4,-3,7;
			builder.Add(new Line(5, BigInteger.One, actions =>
			{
				actions.UpdateCount(4, BigInteger.One);
				actions.UpdateCount(3, BigInteger.MinusOne);
				actions.UpdateCount(7, BigInteger.One);
			}));
			// 6 defer (4) 3;
			builder.Add(new Line(6, BigInteger.One, actions =>
			{
				actions.Defer(actions.E(4));
				if (!actions.ShouldStatementBeDeferred)
				{
					actions.UpdateCount(3, BigInteger.One);
				}
			}));
			// 7 7;
			builder.Add(new Line(7, BigInteger.One, actions =>
			{
				actions.UpdateCount(7, BigInteger.One);
			}));
			// 8 defer (N(7)<100) -1#N(1),-2#N(2),-7#100,-3;
			builder.Add(new Line(8, BigInteger.One, actions =>
			{
				actions.Defer(actions.N(7) < new BigInteger(100));
				if (!actions.ShouldStatementBeDeferred)
				{
					actions.UpdateCount(1, BigInteger.MinusOne * actions.N(1));
					actions.UpdateCount(2, BigInteger.MinusOne * actions.N(2));
					actions.UpdateCount(7, BigInteger.MinusOne * new BigInteger(100));
				}
			}));
			// 9 defer (3 || 6) 1,3; 
			builder.Add(new Line(9, BigInteger.One, actions =>
			{
				actions.Defer(actions.E(3) || actions.E(6));
				if (!actions.ShouldStatementBeDeferred)
				{
					actions.UpdateCount(1, BigInteger.One);
					actions.UpdateCount(3, BigInteger.One);
				}
			}));

			return builder.ToImmutable();
		}
	}
}