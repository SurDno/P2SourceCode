using System;
using Engine.Common;

namespace Engine.Source.Debugs
{
  public class UpdatableProxy(Action action) : IUpdatable 
  {
    public void ComputeUpdate()
    {
      Action action1 = action;
      if (action1 == null)
        return;
      action1();
    }
  }
}
