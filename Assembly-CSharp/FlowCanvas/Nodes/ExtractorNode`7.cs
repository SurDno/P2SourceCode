using ParadoxNotion;

namespace FlowCanvas.Nodes;

public abstract class ExtractorNode<TInstance, T1, T2, T3, T4, T5, T6> : ExtractorNode {
	private T1 a;
	private T2 b;
	private T3 c;
	private T4 d;
	private T5 e;
	private T6 f;

	public abstract void Invoke(
		TInstance instance,
		out T1 a,
		out T2 b,
		out T3 c,
		out T4 d,
		out T5 e,
		out T6 f);

	protected sealed override void OnRegisterPorts(FlowNode node) {
		var i = node.AddValueInput<TInstance>(typeof(TInstance).FriendlyName());
		node.AddValueOutput(parameters[1].Name.SplitCamelCase(), () => {
			Invoke(i.value, out a, out b, out c, out d, out e, out f);
			return a;
		});
		node.AddValueOutput(parameters[2].Name.SplitCamelCase(), () => {
			Invoke(i.value, out a, out b, out c, out d, out e, out f);
			return b;
		});
		node.AddValueOutput(parameters[3].Name.SplitCamelCase(), () => {
			Invoke(i.value, out a, out b, out c, out d, out e, out f);
			return c;
		});
		node.AddValueOutput(parameters[4].Name.SplitCamelCase(), () => {
			Invoke(i.value, out a, out b, out c, out d, out e, out f);
			return d;
		});
		node.AddValueOutput(parameters[5].Name.SplitCamelCase(), () => {
			Invoke(i.value, out a, out b, out c, out d, out e, out f);
			return e;
		});
		node.AddValueOutput(parameters[6].Name.SplitCamelCase(), () => {
			Invoke(i.value, out a, out b, out c, out d, out e, out f);
			return f;
		});
	}
}