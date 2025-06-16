using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common;
using Inspectors;

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
      updater = new ReduceUpdateProxy<IUpdatable>(updatable, this, delay);
    }

    public void AddUpdatable(IUpdatable up) => updatable.Add(up);

    public void RemoveUpdatable(IUpdatable up)
    {
      int updatableIndex = GetUpdatableIndex(up);
      if (updatableIndex == -1)
        throw new Exception();
      updatable[updatableIndex] = null;
    }

    private int GetUpdatableIndex(IUpdatable up)
    {
      if (up == null)
        throw new Exception();
      for (int index = 0; index < updatable.Count; ++index)
      {
        if (updatable[index] == up)
          return index;
      }
      return -1;
    }

    public void ComputeUpdate() => updater.Update();

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
