// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.ReduceUpdateProxy`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons
{
  public struct ReduceUpdateProxy<T>
  {
    [Inspected]
    private List<T> list;
    [Inspected]
    private float delay;
    [Inspected]
    private float accumulator;
    [Inspected]
    private int lastIndex;
    [Inspected]
    private int count;
    [Inspected]
    private IUpdateItem<T> updater;

    public ReduceUpdateProxy(List<T> list, IUpdateItem<T> updater, float delay)
    {
      this.list = list;
      this.delay = delay;
      this.updater = updater;
      this.accumulator = 0.0f;
      this.lastIndex = 0;
      this.count = 0;
    }

    public void Update()
    {
      int count1 = this.list.Count;
      if (count1 == 0)
        return;
      if ((double) this.delay != 0.0)
      {
        this.accumulator += Time.deltaTime;
        this.count = (int) ((double) this.accumulator * (double) count1 / (double) this.delay);
        if (this.count == 0)
          return;
        this.accumulator -= this.delay * (float) this.count / (float) count1;
        this.count = Mathf.Min(this.count, count1);
      }
      else
      {
        this.lastIndex = 0;
        this.count = count1;
      }
      for (int index = 0; index < this.count; ++index)
      {
        int count2 = this.list.Count;
        if (count2 == 0)
          break;
        if (this.lastIndex >= count2)
          this.lastIndex = 0;
        T obj = this.list[this.lastIndex];
        if ((object) obj == null)
        {
          this.list[this.lastIndex] = this.list[this.list.Count - 1];
          this.list.RemoveAt(this.list.Count - 1);
        }
        else
        {
          try
          {
            this.updater.ComputeUpdateItem(obj);
          }
          catch (Exception ex)
          {
            Debug.LogException(ex);
          }
          ++this.lastIndex;
        }
      }
    }
  }
}
