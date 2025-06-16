using System;
using Engine.Common;
using Engine.Source.Commons;

public class FlockChildProxy : IUpdatable, IDisposable {
	private FlockChild child;

	public FlockChildProxy(FlockChild child) {
		this.child = child;
		InstanceByRequest<UpdateService>.Instance.FlockCastUpdater.AddUpdatable(this);
	}

	public void ComputeUpdate() {
		child.ProxyUpdate();
	}

	public void Dispose() {
		InstanceByRequest<UpdateService>.Instance.FlockCastUpdater.RemoveUpdatable(this);
	}
}