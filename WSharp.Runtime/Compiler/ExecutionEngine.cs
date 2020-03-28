using Spackle;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;

namespace WSharp.Runtime
{
	public sealed class ExecutionEngine
		: IExecutionEngineActions
	{
		private readonly Dictionary<BigInteger, Line> lines;
		private readonly Random random;
		private bool shouldStatementBeDeferred;
		private bool shouldStatementBeKept;
		private readonly TextWriter writer;

		public ExecutionEngine(ImmutableList<Line> lines, Random random, TextWriter writer)
		{
			this.random = random ?? throw new ArgumentNullException(nameof(random));
			this.writer = writer ?? throw new ArgumentNullException(nameof(writer));

			if (lines == null)
			{
				throw new ArgumentNullException(nameof(lines));
			}
			else if (lines.Count == 0)
			{
				throw new ArgumentException("Must pass in at least one line.", nameof(lines));
			}
			else
			{
				var messages = new List<string>();

				for (var i = 0; i < lines.Count; i++)
				{
					if (lines[i] == null)
					{
						messages.Add($"The line at index {i} is null.");
					}
				}

				if (messages.Count > 0)
				{
					throw new ExecutionEngineLinesException(messages.ToImmutableList());
				}
			}

			this.lines = new Dictionary<BigInteger, Line>();

			foreach (var line in lines)
			{
				this.lines.Add(line.Identifier, line);
			}
		}

		public bool Again(bool shouldKeep)
		{
			this.shouldStatementBeKept |= shouldKeep;
			return shouldKeep;
		}

		public BigInteger GetCurrentLineCount()
		{
			var lineCount = BigInteger.Zero;

			foreach (var line in this.lines.Values)
			{
				lineCount += line.Count;
			}

			return lineCount;
		}

		public bool Defer(bool shouldDefer)
		{
			this.shouldStatementBeDeferred |= shouldDefer;
			return shouldDefer;
		}

		public bool DoesLineExist(BigInteger identifier) =>
			this.lines[identifier].Count > 0;

		public void Execute()
		{
			this.shouldStatementBeDeferred = false;
			this.shouldStatementBeKept = false;
			var currentLineCount = this.GetCurrentLineCount();

			while (currentLineCount > 0)
			{
				var buffer = currentLineCount.ToByteArray();
				this.random.NextBytes(buffer);

				var generated = BigInteger.Abs(new BigInteger(buffer) % currentLineCount);
				var currentLowerBound = BigInteger.Zero;

				foreach (var line in this.lines.Values.Where(_ => _.Count > BigInteger.Zero))
				{
					var range = new Range<BigInteger>(currentLowerBound, line.Count + currentLowerBound - 1);
					if (range.Contains(generated))
					{
						line.Code(this);

						if (!this.shouldStatementBeKept && !this.shouldStatementBeDeferred)
						{
							var newLine = line.UpdateCount(-1);
							this.lines[newLine.Identifier] = newLine;
						}

						this.shouldStatementBeDeferred = false;
						this.shouldStatementBeKept = false;
						break;
					}
					else
					{
						currentLowerBound += line.Count;
					}
				}

				currentLineCount = this.GetCurrentLineCount();
			}
		}

		public BigInteger N(BigInteger identifier) => this.lines[identifier].Count;

		public void Print(string message) => this.writer.WriteLine(message);

		public string U(long number) => number.ToString();

		public void UpdateCount(BigInteger identifier, BigInteger delta) =>
			this.lines[identifier] = this.lines[identifier].UpdateCount(delta);
	}
}