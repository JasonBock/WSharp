using Spackle;
using System;
using WSharp.Runtime;

namespace WSharp
{
	class Program
	{
		static void Main()
		{
			var lines = FibonacciSequenceGenerator.Generate();
			var engine = new ExecutionEngine(lines, new SecureRandom(), Console.Out);
			engine.Execute();
		}
	}
}
