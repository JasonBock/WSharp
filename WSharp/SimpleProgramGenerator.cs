using System.Collections.Immutable;
using WSharp.Runtime;

namespace WSharp
{
	internal static class SimpleProgramGenerator
	{
		/*
1 print("hello");
2 1#2;
3 1#-1;
		*/
		internal static ImmutableArray<Line> Generate()
		{
			var builder = ImmutableArray.CreateBuilder<Line>();

			// TODO: Finish :)

			return builder.ToImmutable();
		}
	}
}