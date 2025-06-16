using Engine.Common;
using System;

namespace Engine.Source.Debugs
{
  public class UpdatableProxy : IUpdatable
  {
    private Action action;

    public UpdatableProxy(Action action) => this.action = action;

    public void ComputeUpdate()
    {
      Action action = this.action;
      if (action == null)
        return;
      action();
    }
  }
}
