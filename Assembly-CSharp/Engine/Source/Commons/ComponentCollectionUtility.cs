// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.ComponentCollectionUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons.Cloneable;
using System.Collections.Generic;

#nullable disable
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
        T obj = CloneableObjectUtility.Clone<T>(source1);
        target.Add(obj);
      }
    }
  }
}
