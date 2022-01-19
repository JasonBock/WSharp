using System.Collections.Immutable;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax;

public sealed class SyntaxTree
{
	private delegate (CompilationUnitSyntax root, ImmutableArray<Diagnostic> diagnostics) ParseHandler(SyntaxTree tree);

	// TODO: Why not make this a Lazy<>?
	private Dictionary<SyntaxNode, SyntaxNode?>? parents;

	private SyntaxTree(SourceText text, ParseHandler handler)
	{
		this.Text = text;
		(this.Root, this.Diagnostics) = handler(this);
	}

	private static Dictionary<SyntaxNode, SyntaxNode?> CreateParentsDictionary(CompilationUnitSyntax root)
	{
		static void CreateParentsDictionary(Dictionary<SyntaxNode, SyntaxNode?> result, SyntaxNode node)
		{
			foreach (var child in node.GetChildren())
			{
				result.Add(child, node);
				CreateParentsDictionary(result, child);
			}
		}

		var result = new Dictionary<SyntaxNode, SyntaxNode?>
		{
			{ root, null }
		};
		CreateParentsDictionary(result, root);

		return result;
	}

	public SyntaxNode? GetParent(SyntaxNode node)
	{
		if (this.parents is null)
		{
			var parents = SyntaxTree.CreateParentsDictionary(this.Root);
			Interlocked.CompareExchange(ref this.parents, parents, null);
		}

		return this.parents[node];
	}

	public static async Task<SyntaxTree> LoadAsync(FileInfo file)
	{
		if (file is null)
		{
			throw new ArgumentNullException(nameof(file));
		}

		return SyntaxTree.Parse(SourceText.From(
			await File.ReadAllTextAsync(file.FullName).ConfigureAwait(false), file));
	}

	public static SyntaxTree Parse(string text) =>
		SyntaxTree.Parse(SourceText.From(text));

	public static SyntaxTree Parse(SourceText text) => new(text, SyntaxTree.Parse);

	private static (CompilationUnitSyntax root, ImmutableArray<Diagnostic> diagnostics) Parse(SyntaxTree tree)
	{
		var parser = new Parser(tree);
		return (parser.ParseCompilationUnit(), parser.Diagnostics.ToImmutableArray());
	}

	public static (ImmutableArray<SyntaxToken> tokens, ImmutableArray<Diagnostic> diagnostics) ParseTokens(
		string text, bool includeEndOfFile = false) =>
			SyntaxTree.ParseTokens(SourceText.From(text), includeEndOfFile);

	public static (ImmutableArray<SyntaxToken> tokens, ImmutableArray<Diagnostic> diagnostics) ParseTokens(
		SourceText text, bool includeEndOfFile = false)
	{
		var tokens = new List<SyntaxToken>();

		(CompilationUnitSyntax root, ImmutableArray<Diagnostic> diagnostics) ParseTokens(SyntaxTree tree)
		{
			var lexer = new Lexer(tree);

			while (true)
			{
				var token = lexer.Lex();

				if (token.Kind != SyntaxKind.EndOfFileToken || includeEndOfFile)
				{
					tokens.Add(token);
				}

				if (token.Kind == SyntaxKind.EndOfFileToken)
				{
					return (new CompilationUnitSyntax(tree, new LineStatementsSyntax(tree, new List<LineStatementSyntax>()), token),
						lexer.Diagnostics.ToImmutableArray());
				}
			}
		}

		var tree = new SyntaxTree(text, ParseTokens);
		return (tokens.ToImmutableArray(), tree.Diagnostics);
	}

	public ImmutableArray<Diagnostic> Diagnostics { get; }
	public CompilationUnitSyntax Root { get; }
	public SourceText Text { get; }
}