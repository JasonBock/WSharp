﻿using System.Collections.Immutable;
using System.Numerics;
using WSharp.Runtime;

namespace WSharp
{
	internal static class Rot13Generator
	{
		// Copied from: https://raw.githubusercontent.com/megahallon/whenever/master/rot13.we
		// Note that the original code used "put()", which is actually "print()"
		/*
1 2#read()-N(2);
2 2;
3 defer(1 || N(2) > 90 || N(2) < 65) 5#65-N(5);
4 defer(1 || N(2) > 122 || N(2) < 97) 5#97-N(5);
5 5;
6 defer(3 && 4) print(U(((N(2) - N(5) + 13) % 26) + N(5)));
7 defer(1 || N(2) > 64 || N(2) == 0) print(U(N(2)));
8 again(8) defer(6 && 7) 1,-6#N(6)-1,-7#N(7)-1,-3#N(3)-1,-4#N(4)-1;
9 defer(1 || N(2) != 0) -3,-4,-5#N(5),-6,-7,-8;
10 defer(1 || 9) print("\n");
		*/
		internal static ImmutableArray<Line> Generate()
		{
			var builder = ImmutableArray.CreateBuilder<Line>();

			// TODO: Finish :)

			return builder.ToImmutable();
		}
	}
}