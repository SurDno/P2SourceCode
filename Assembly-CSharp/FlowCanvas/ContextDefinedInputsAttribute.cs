using System;

namespace FlowCanvas;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ContextDefinedInputsAttribute : Attribute {
	public Type[] types;

	public ContextDefinedInputsAttribute(params Type[] types) {
		this.types = types;
	}
}