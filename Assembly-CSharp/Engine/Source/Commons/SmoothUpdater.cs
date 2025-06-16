using Cofe.Utility;
using Engine.Common;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Engine.Source.Commons
{
  public class SmoothUpdater : IUpdater, IUpdateItem<IUpdatable>
  {
    [Inspected]
    private List<IUpdatable> updatable = new List<IUpdatable>();
    [Inspected]
    private ReduceUpdateProxy<IUpdatable> updater;

    public SmoothUpdater(float delay)
    {
      this.updater = new ReduceUpdateProxy<IUpdatable>(this.updatable, (IUpdateItem<IUpdatable>) this, delay);
    }

    public void AddUpdatable(IUpdatable up) => this.updatable.Add(up);

    public void RemoveUpdatable(IUpdatable up)
    {
      int updatableIndex = this.GetUpdatableIndex(up);
      if (updatableIndex == -1)
        throw new Exception();
      this.updatable[updatableIndex] = (IUpdatable) null;
    }

    private int GetUpdatableIndex(IUpdatable up)
    {
      if (up == null)
        throw new Exception();
      for (int index = 0; index < this.updatable.Count; ++index)
      {
        if (this.updatable[index] == up)
          return index;
      }
      return -1;
    }

    public void ComputeUpdate() => this.updater.Update();

    public void ComputeUpdateItem(IUpdatable item)
    {
      if (Profiler.enabled)
        Profiler.BeginSample(TypeUtility.GetTypeName(item.GetType()));
      item.ComputeUpdate();
      if (!Profiler.enabled)
        return;
      Profiler.EndSample();
    }
  }
}
