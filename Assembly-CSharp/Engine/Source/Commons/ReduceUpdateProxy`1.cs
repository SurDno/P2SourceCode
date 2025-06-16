using System;
using System.Collections.Generic;
using Inspectors;

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
      accumulator = 0.0f;
      lastIndex = 0;
      count = 0;
    }

    public void Update()
    {
      int count1 = list.Count;
      if (count1 == 0)
        return;
      if (delay != 0.0)
      {
        accumulator += Time.deltaTime;
        count = (int) (accumulator * (double) count1 / delay);
        if (count == 0)
          return;
        accumulator -= delay * count / count1;
        count = Mathf.Min(count, count1);
      }
      else
      {
        lastIndex = 0;
        count = count1;
      }
      for (int index = 0; index < count; ++index)
      {
        int count2 = list.Count;
        if (count2 == 0)
          break;
        if (lastIndex >= count2)
          lastIndex = 0;
        T obj = list[lastIndex];
        if (obj == null)
        {
          list[lastIndex] = list[list.Count - 1];
          list.RemoveAt(list.Count - 1);
        }
        else
        {
          try
          {
            updater.ComputeUpdateItem(obj);
          }
          catch (Exception ex)
          {
            Debug.LogException(ex);
          }
          ++lastIndex;
        }
      }
    }
  }
}
