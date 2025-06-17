using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace Engine.Source.Commons
{
  public class DelayUpdater(float delay) : IUpdater 
  {
    [Inspected]
    private List<IUpdatable> updatable = [];
    [Inspected(Mutable = true)]
    private float delay = delay;
    [Inspected]
    private float accumulator = delay * Random.value;

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
      accumulator += Time.deltaTime;
      if (accumulator < (double) delay || ServiceCache.OptimizationService.FrameHasSpike)
        return;
      accumulator = 0.0f;
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
