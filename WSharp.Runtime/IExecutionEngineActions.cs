using System.Numerics;

namespace WSharp.Runtime
{
	public interface IExecutionEngineActions
	{
		bool Again(bool shouldKeep);
		bool Defer(bool shouldDefer);
		bool DoesLineExist(ulong identifier);
		BigInteger N(ulong identifier);
		void Print(string message);
		string U(long number);
		void UpdateCount(ulong identifier, BigInteger delta);
	}
}
