namespace WSharp.Runtime
{
	public interface IExecutionEngineActions
	{
		bool Defer(ulong identifier);
		ulong N(ulong identifier);
		string U(long number);
	}
}
