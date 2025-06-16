using Cofe.Utility;
using Engine.Common;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Engine.Source.Commons
{
  public class Updater : IUpdater
  {
    [Inspected]
    private List<IUpdatable> updatable = new List<IUpdatable>();

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
