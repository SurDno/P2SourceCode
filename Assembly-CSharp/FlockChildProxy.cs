using Engine.Common;
using Engine.Source.Commons;
using System;

public class FlockChildProxy : IUpdatable, IDisposable
{
  private FlockChild child;

  public FlockChildProxy(FlockChild child)
  {
    this.child = child;
    InstanceByRequest<UpdateService>.Instance.FlockCastUpdater.AddUpdatable((IUpdatable) this);
  }

  public void ComputeUpdate() => this.child.ProxyUpdate();

  public void Dispose()
  {
    InstanceByRequest<UpdateService>.Instance.FlockCastUpdater.RemoveUpdatable((IUpdatable) this);
  }
}
