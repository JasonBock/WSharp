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
		private SecureRandom random = new SecureRandom();
		private readonly TextReader reader;
		private readonly TextWriter writer;

		public ExecutionEngine(ImmutableArray<Line> lines, SecureRandom random, TextWriter writer, TextReader reader)
		{
			this.random = random ?? throw new ArgumentNullException(nameof(random));
			this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
			this.reader = reader ?? throw new ArgumentNullException(nameof(reader));

			if (lines == null)
			{
				throw new ArgumentNullException(nameof(lines));
			}
			else if (lines.Length == 0)
			{
				throw new ArgumentException("Must pass in at least one line.", nameof(lines));
			}
			else
			{
				var messages = new List<string>();

				for (var i = 0; i < lines.Length; i++)
				{
					if (lines[i] == null)
					{
						messages.Add($"The line at index {i} is null.");
					}
				}

				if (messages.Count > 0)
				{
					throw new ExecutionEngineLinesException(messages.ToImmutableArray());
				}
			}

			this.lines = new Dictionary<BigInteger, Line>();

			foreach (var line in lines)
			{
				this.lines.Add(line.Identifier, line);
			}
		}

		public void Again(bool shouldKeep) => this.ShouldStatementBeKept |= shouldKeep;

		public BigInteger GetCurrentLineCount()
		{
			var lineCount = BigInteger.Zero;

			foreach (var line in this.lines.Values)
			{
				lineCount += line.Count;
			}

			return lineCount;
		}

		public void Defer(bool shouldDefer) => this.ShouldStatementBeDeferred |= shouldDefer;

		public bool DoesLineExist(BigInteger identifier) =>
			this.lines[identifier].Count > 0;

		public void Execute()
		{
			this.ShouldStatementBeDeferred = false;
			this.ShouldStatementBeKept = false;
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

						if (!this.ShouldStatementBeKept && !this.ShouldStatementBeDeferred)
						{
							var newLine = line.UpdateCount(-1);
							this.lines[newLine.Identifier] = newLine;
						}

						this.ShouldStatementBeDeferred = false;
						this.ShouldStatementBeKept = false;
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

		public BigInteger Random(BigInteger maximum) => this.random.GetBigInteger((uint)maximum);

		public string Read() => this.reader.ReadLine() ?? string.Empty;

		public string U(BigInteger number) => char.ConvertFromUtf32((int)number);

		public void UpdateCount(BigInteger identifier, BigInteger delta) =>
			this.lines[identifier] = this.lines[identifier].UpdateCount(delta);

		public bool ShouldStatementBeDeferred { get; private set; }
		public bool ShouldStatementBeKept { get; private set; }
	}
}