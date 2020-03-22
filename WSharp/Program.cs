using Spackle;
using System;
using WSharp.Runtime;

namespace WSharp
{
	public static class Program
	{
		public static void Main()
		{
			var lines = FibonacciSequenceGenerator.Generate();
			var engine = new ExecutionEngine(lines, new SecureRandom(), Console.Out);
			engine.Execute();
		}
	}
}