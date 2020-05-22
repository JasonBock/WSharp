using System.Numerics;

namespace WSharp.Runtime
{
	public interface IExecutionEngineActions
	{
		void Again(bool shouldKeep);
		void Defer(bool shouldDefer);
		bool E(BigInteger identifier);
		BigInteger N(BigInteger identifier);
		void Print(object value);
		BigInteger Random(BigInteger maximum);
		string Read();
		string U(BigInteger number);
		void UpdateCount(BigInteger identifier, BigInteger delta);

		bool ShouldStatementBeDeferred { get; }
		bool ShouldStatementBeKept { get; }
	}
}