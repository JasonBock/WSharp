using System.Numerics;

namespace WSharp.Runtime
{
	public interface IExecutionEngineActions
	{
		bool Again(bool shouldKeep);
		bool Defer(bool shouldDefer);
		bool DoesLineExist(BigInteger identifier);
		BigInteger N(BigInteger identifier);
		void Print(string message);
		BigInteger Random(BigInteger maximum);
		string Read();
		string U(BigInteger number);
		void UpdateCount(BigInteger identifier, BigInteger delta);
	}
}