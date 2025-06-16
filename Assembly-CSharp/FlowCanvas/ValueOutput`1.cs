using System;

namespace FlowCanvas;

public class ValueOutput<T> : ValueOutput {
	public ValueOutput() { }

	public ValueOutput(FlowNode parent, string name, string ID, ValueHandler<T> getter)
		: base(parent, name, ID) {
		this.getter = getter;
	}

	public ValueOutput(FlowNode parent, string name, string ID, ValueHandler<object> getter)
		: base(parent, name, ID) {
		this.getter = getter as ValueHandler<T>;
		if (this.getter != null)
			return;
		this.getter = () => (T)getter();
	}

	public ValueHandler<T> getter { get; set; }

	public override object GetValue() {
		return getter();
	}

	public override Type type => typeof(T);
}