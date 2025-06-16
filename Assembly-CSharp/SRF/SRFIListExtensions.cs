using System;
using System.Collections.Generic;

namespace SRF
{
  public static class SRFIListExtensions
  {
    public static T Random<T>(this IList<T> list)
    {
      if (list.Count == 0)
        throw new IndexOutOfRangeException("List needs at least one entry to call Random()");
      return list.Count == 1 ? list[0] : list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T RandomOrDefault<T>(this IList<T> list)
    {
      return list.Count == 0 ? default (T) : list.Random();
    }

    public static T PopLast<T>(this IList<T> list)
    {
      T obj = list.Count != 0 ? list[list.Count - 1] : throw new InvalidOperationException();
      list.RemoveAt(list.Count - 1);
      return obj;
    }
  }
}
