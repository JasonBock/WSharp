using System.Numerics;

namespace WSharp.Runtime
{
	// TODO: Need to have a Read() method here. From the docs:
	// A basic read statement reads from STDIN. read() reads a variable amount of data - if the data form a number, 
	// it reads to the end of the number and returns that number. If the first character it reads is non-numeric, 
	// it returns the Unicode numeric value of that character.
	public interface IExecutionEngineActions
	{
		bool Again(bool shouldKeep);
		bool Defer(bool shouldDefer);
		bool DoesLineExist(BigInteger identifier);
		BigInteger N(BigInteger identifier);
		void Print(string message);
		string Read();
		string U(long number);
		void UpdateCount(BigInteger identifier, BigInteger delta);
	}
}