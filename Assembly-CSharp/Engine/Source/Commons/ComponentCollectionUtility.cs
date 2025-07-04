﻿using System.Collections.Generic;
using Engine.Common.Commons.Cloneable;

namespace Engine.Source.Commons
{
  public static class ComponentCollectionUtility
  {
    public static void CopyListTo<T>(List<T> target, List<T> source) where T : class
    {
      target.Clear();
      target.Capacity = source.Count;
      foreach (T source1 in source)
      {
        T obj = CloneableObjectUtility.Clone(source1);
        target.Add(obj);
      }
    }
  }
}
