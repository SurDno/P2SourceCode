using System;

namespace ParadoxNotion.FlowCanvas.Module;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ContextDefinedOutputsAttribute : Attribute {
	public Type[] types;

	public ContextDefinedOutputsAttribute(params Type[] types) {
		this.types = types;
	}
}