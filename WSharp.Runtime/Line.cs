using System;

namespace WSharp.Runtime
{
	public sealed class Line
	{
		public Line(ulong identifier, ulong count, Action<IExecutionEngineActions> code) => 
			(this.Identifier, this.Count, this.Code) = (identifier, count, code ?? throw new ArgumentNullException(nameof(code)));

		public Line UpdateCount(ulong delta) =>
			new Line(this.Identifier, delta > this.Count ? 0 : this.Count - delta, this.Code);

		public ulong Identifier { get; }
		public ulong Count { get; }
		public Action<IExecutionEngineActions> Code { get; }
	}
}
