using Spackle;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WSharp.Runtime
{
	public sealed class ExecutionEngine
		: IExecutionEngineActions
	{
		private readonly ImmutableDictionary<ulong, Line> lines;
		private readonly Random random;
		private bool shouldStatementBeDeferred;

		public ExecutionEngine(ImmutableList<Line> lines, Random random)
		{
			this.random = random ?? throw new ArgumentNullException(nameof(random));

			if (lines == null)
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

			var indexedLines = ImmutableDictionary.CreateBuilder<ulong, Line>();

			foreach(var line in lines)
			{
				indexedLines.Add(line.Identifier, line);
			}

			this.lines = indexedLines.ToImmutable();
		}

		public ulong GetCurrentLineCount()
		{
			var lineCount = 0ul;

			foreach(var line in this.lines.Values)
			{
				lineCount = lineCount + line.Count;
			}

			return lineCount;
		}

		public bool Defer(bool shouldDefer) => 
			throw new NotImplementedException();

		public bool DoesLineExist(ulong identifier)
		{
			if(this.lines.TryGetValue(identifier, out var line))
			{
				return line.Count > 0;
			}
			else
			{
				return false;
			}
		}

		public bool Execute()
		{
			this.shouldStatementBeDeferred = false;
			var currentLineCount = this.GetCurrentLineCount();

			var buffer = new byte[8];
			this.random.NextBytes(buffer);

			var generated = BitConverter.ToUInt64(buffer, 0) % currentLineCount;
			// Turn wasDeferCalled
			// Randomly pick a line if there are lines to execute.
			// Execute the line's Code.
			// If wasDeferCalled == true
			//   keep the line count the same
			// Else
			//   decrement the line's count by 1.
		}

		public ulong N(ulong identifier) => 
			throw new NotImplementedException();

		public string U(long number) => number.ToString();
	}
}
