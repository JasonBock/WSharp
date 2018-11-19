namespace WSharp.Runtime
{
	public interface IExecutionEngineActions
	{
		bool Defer(bool shouldDefer);
		bool DoesLineExist(ulong identifier);
		ulong N(ulong identifier);
		string U(long number);
	}
}
