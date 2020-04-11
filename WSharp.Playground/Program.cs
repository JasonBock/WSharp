using System.Threading.Tasks;

namespace WSharp.Playground
{
	public static class Program
	{
		public static async Task Main() =>
			await (new Repl()).RunAsync();
	}
}