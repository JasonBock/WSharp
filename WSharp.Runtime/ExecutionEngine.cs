using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WSharp.Runtime
{
	public sealed class ExecutionEngine
		: IExecutionEngineActions
	{
		private readonly ImmutableDictionary<ulong, Line> lines =
			ImmutableDictionary<ulong, Line>.Empty;

		public ExecutionEngine(ImmutableList<Line> lines)
		{
			if(lines == null)
			{
				throw new ArgumentNullException(nameof(lines));
			}
			else if(lines.Count == 0)
			{
				throw new ArgumentException("Must pass in at least one line.", nameof(lines));
			}
			else
			{
				var messages = new List<string>();

				for(var i = 0; i < lines.Count; i++)
				{
					if(lines[i] == null)
					{
						messages.Add($"The line at index {i} is null.");
					}
				}

				if(messages.Count > 0)
				{
					throw new ExecutionEngineLinesException(messages.ToImmutableList());
				}
			}

			foreach(var line in lines)
			{
				this.lines = this.lines.Add(line.Identifier, line);
			}
		}

		public bool Defer(ulong identifier) => 
			throw new NotImplementedException();

		public ulong N(ulong identifier) => 
			throw new NotImplementedException();

		public string U(long number) => number.ToString();
	}
}
