using System;

namespace Engine.Common.Generator;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GenerateProxyAttribute : Attribute {
	public Type Type { get; set; }

	public TypeEnum Detail { get; set; }

	public GenerateProxyAttribute(TypeEnum detail) {
		Detail = detail;
	}
}