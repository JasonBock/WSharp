using System;
using System.Numerics;

namespace WSharp.Runtime
{
	public sealed class Line
	{
		public Line(BigInteger identifier, BigInteger count, Action<IExecutionEngineActions> code)
		{
			if(identifier < BigInteger.One)
			{
				throw new ArgumentException($"The identifier, {identifier}, must be greater than zero.", nameof(identifier));
			}

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

		public Action<IExecutionEngineActions> Code { get; }
		public BigInteger Count { get; }
		public BigInteger Identifier { get; }
	}
}