using System.Reflection;

namespace WSharp
{
	internal sealed class MetaCommand
	{
		public MetaCommand(string name, string description, MethodInfo method) => 
			(this.Name, this.Description, this.Method) = (name, description, method);

		public string Description { get; }
		public string Name { get; }
		public MethodInfo Method { get; }
	}
}