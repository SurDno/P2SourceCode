using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common;
using Inspectors;

namespace Engine.Source.Commons
{
  public class Updater : IUpdater
  {
    [Inspected]
    private List<IUpdatable> updatable = new List<IUpdatable>();

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

    public void ComputeUpdate()
    {
      int index = 0;
      while (index < this.updatable.Count)
      {
        IUpdatable updatable = this.updatable[index];
        if (updatable == null)
        {
          this.updatable[index] = this.updatable[this.updatable.Count - 1];
          this.updatable.RemoveAt(this.updatable.Count - 1);
        }
        else
        {
          if (Profiler.enabled)
            Profiler.BeginSample(TypeUtility.GetTypeName(updatable.GetType()));
          try
          {
            updatable.ComputeUpdate();
          }
          catch (Exception ex)
          {
            Debug.LogException(ex);
          }
          if (Profiler.enabled)
            Profiler.EndSample();
          ++index;
        }
      }
    }
  }
}
