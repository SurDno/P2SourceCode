// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.SmoothUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

#nullable disable
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
