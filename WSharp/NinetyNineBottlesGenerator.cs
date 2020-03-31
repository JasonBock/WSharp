using System.Collections.Immutable;
using System.Numerics;
using WSharp.Runtime;

namespace WSharp
{
	internal static class NinetyNineBottlesGenerator
	{
		/*
1 defer (4 || N(1)<N(2) || N(2)<N(3)) print(N(1)+" bottles of beer on the wall, "+N(1)+" bottles of beer,");
2 defer (4 || N(1)==N(2)) print("Take one down and pass it around,");
3 defer (4 || N(2)==N(3)) print(N(1)+" bottles of beer on the wall.");
4 1#98,2#98,3#98; 
 		*/
		internal static ImmutableArray<Line> Generate()
		{
			var builder = ImmutableArray.CreateBuilder<Line>();

			// 1 defer (4 || N(1)<N(2) || N(2)<N(3)) print(N(1)+" bottles of beer on the wall, "+N(1)+" bottles of beer,");
			builder.Add(new Line(1, new BigInteger(1), actions =>
			{
				if(!actions.Defer(actions.DoesLineExist(4) || actions.N(1) < actions.N(2) ||
					actions.N(2) < actions.N(3)))
				{
					actions.Print(actions.N(1) + " bottles of beer on the wall, " + actions.N(1) + " bottles of beer,");
				}
			}));
			// 2 defer (4 || N(1)==N(2)) print("Take one down and pass it around,");
			builder.Add(new Line(2, new BigInteger(1), actions =>
			{
				if (!actions.Defer(actions.DoesLineExist(4) || actions.N(1) == actions.N(2)))
				{
					actions.Print("Take one down and pass it around,");
				}
			}));
			// 3 defer (4 || N(2)==N(3)) print(N(1)+" bottles of beer on the wall.");
			builder.Add(new Line(3, new BigInteger(1), actions =>
			{
				if (!actions.Defer(actions.DoesLineExist(4) || actions.N(2) == actions.N(3)))
				{
					actions.Print(actions.N(1) + " bottles of beer on the wall.");
				}
			}));
			// 4 1#98,2#98,3#98;
			builder.Add(new Line(4, new BigInteger(1), actions =>
			{
				actions.UpdateCount(1, 98);
				actions.UpdateCount(2, 98);
				actions.UpdateCount(3, 98);
			}));

			return builder.ToImmutable();
		}
	}
}