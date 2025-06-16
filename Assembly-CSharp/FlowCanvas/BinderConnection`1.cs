namespace FlowCanvas;

public class BinderConnection<T> : BinderConnection {
	public override void Bind() {
		if (!isActive)
			return;
		DoNormalBinding(sourcePort, targetPort);
	}

	public override void UnBind() {
		if (!(targetPort is ValueInput))
			return;
		(targetPort as ValueInput).UnBind();
	}

	private void DoNormalBinding(Port source, Port target) {
		(target as ValueInput<T>).BindTo((ValueOutput)source);
	}
}