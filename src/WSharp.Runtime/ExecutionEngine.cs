using Spackle;
using Spackle.Extensions;
using System.Collections.Immutable;
using System.Numerics;
using System.Security.Cryptography;

namespace WSharp.Runtime;

public sealed class ExecutionEngine
	: IExecutionEngineActions
{
	private readonly Dictionary<BigInteger, Line> lines;
	private readonly RandomNumberGenerator random;
	private readonly TextReader reader;
	private readonly TextWriter writer;

	public ExecutionEngine(ImmutableArray<Line> lines, RandomNumberGenerator random, TextReader reader, TextWriter writer)
	{
		this.random = random ?? throw new ArgumentNullException(nameof(random));
		this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
		this.writer = writer ?? throw new ArgumentNullException(nameof(writer));

		if (lines.Length == 0)
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
				throw new ExecutionEngineLinesException([.. messages]);
			}
		}

		this.lines = [];

		foreach (var line in lines)
		{
			this.lines.Add(line.Identifier, line);
		}
	}

	public void Again(bool shouldKeep) => this.ShouldStatementBeKept |= shouldKeep;

	public BigInteger CurrentLineCount
	{
		get
		{
			var lineCount = BigInteger.Zero;

			foreach (var line in this.lines.Values)
			{
				lineCount += line.Count;
			}

			return lineCount;
		}
	}

	public void Defer(bool shouldDefer) => this.ShouldStatementBeDeferred |= shouldDefer;

	public bool E(BigInteger identifier) =>
		this.lines[identifier].Count > 0;

	public void Execute()
	{
		this.ShouldStatementBeDeferred = false;
		this.ShouldStatementBeKept = false;
		var currentLineCount = this.CurrentLineCount;

		while (currentLineCount > 0)
		{
			var buffer = currentLineCount.ToByteArray();
			this.random.GetBytes(buffer);

			var generated = BigInteger.Abs(new BigInteger(buffer) % currentLineCount);
			var currentLowerBound = BigInteger.Zero;
			var foundLine = BigInteger.Zero;

			foreach (var line in this.lines.Values.Where(_ => _.Count > BigInteger.Zero))
			{
				var range = new Range<BigInteger>(currentLowerBound, line.Count + currentLowerBound);
				if (range.Contains(generated))
				{
					foundLine = line.Identifier;
					line.Code(this);
					break;
				}
				else
				{
					currentLowerBound += line.Count;
				}
			}

			var executedLine = this.lines[foundLine];

			if (!this.ShouldStatementBeKept && !this.ShouldStatementBeDeferred)
			{
				this.lines[executedLine.Identifier] = executedLine.UpdateCount(-1);
			}

			this.ShouldStatementBeDeferred = false;
			this.ShouldStatementBeKept = false;

			currentLineCount = this.CurrentLineCount;
		}
	}

	public BigInteger N(BigInteger identifier) => this.lines[identifier].Count;

	public void Print(object value) => this.writer.WriteLine(value);

	public BigInteger Random(BigInteger maximum) => this.random.GetBigInteger((uint)maximum);

	public string Read() => this.reader.ReadLine() ?? string.Empty;

	public string U(BigInteger number) => char.ConvertFromUtf32((int)number);

	public void UpdateCount(BigInteger identifier, BigInteger delta) =>
		this.lines[identifier] = this.lines[identifier].UpdateCount(delta);

	public bool ShouldStatementBeDeferred { get; private set; }
	public bool ShouldStatementBeKept { get; private set; }
}