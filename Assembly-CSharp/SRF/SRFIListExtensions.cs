// Decompiled with JetBrains decompiler
// Type: SRF.SRFIListExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
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
      return list.Count == 0 ? default (T) : list.Random<T>();
    }

    public static T PopLast<T>(this IList<T> list)
    {
      T obj = list.Count != 0 ? list[list.Count - 1] : throw new InvalidOperationException();
      list.RemoveAt(list.Count - 1);
      return obj;
    }
  }
}
