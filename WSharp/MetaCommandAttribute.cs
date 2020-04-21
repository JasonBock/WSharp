using System;

namespace WSharp
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class MetaCommandAttribute
		: Attribute
	{
		public MetaCommandAttribute(string name, string description) => 
			(this.Name, this.Description) = (name, description);

		public string Description { get; }
		public string Name { get; }
	}
}