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
		string U(long number);
		void UpdateCount(BigInteger identifier, BigInteger delta);
	}
}