using System;
using System.Numerics;

namespace WSharp.Runtime
{
	public sealed class Line
	{
		public Line(ulong identifier, BigInteger count, Action<IExecutionEngineActions> code)
		{
			if(count < BigInteger.Zero)
			{
				throw new ArgumentException("Cannot use a negative count for a line.", nameof(count));
			}

			this.Count = count;
			this.Identifier = identifier;
			this.Code = code ?? throw new ArgumentNullException(nameof(code));
		}

		public Line UpdateCount(BigInteger delta)
		{
			var newCount = this.Count + delta;

			if (newCount < BigInteger.Zero)
			{
				newCount = BigInteger.Zero;
			}

			return new Line(this.Identifier, newCount, this.Code);
		}

		public ulong Identifier { get; }
		public BigInteger Count { get; }
		public Action<IExecutionEngineActions> Code { get; }
	}
}
