using System;

namespace FlowCanvas;

public class FlowInput : Port {
	public FlowInput(FlowNode parent, string name, string ID, FlowHandler pointer)
		: base(parent, name, ID) {
		this.pointer = pointer;
	}

	public FlowHandler pointer { get; private set; }

	public override Type type => typeof(void);
}