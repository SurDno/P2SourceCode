// Decompiled with JetBrains decompiler
// Type: UnityHeapCrawler.TypeStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

#nullable disable
namespace UnityHeapCrawler
{
  public class TypeStats : IComparable<TypeStats>
  {
    [NotNull]
    public static readonly Dictionary<System.Type, TypeStats> Data = new Dictionary<System.Type, TypeStats>();
    [NotNull]
    public readonly System.Type Type;
    public long SelfSize;
    public long TotalSize;
    public long NativeSize;
    public int Count;

    public static void Init() => TypeStats.Data.Clear();

    public static void RegisterItem([NotNull] CrawlItem item)
    {
      TypeStats typeStats = TypeStats.DemandTypeStats(item.Object.GetType());
      ++typeStats.Count;
      typeStats.SelfSize += (long) item.SelfSize;
      typeStats.TotalSize += (long) item.TotalSize;
      UnityEngine.Object o = item.Object as UnityEngine.Object;
      if (!(o != (UnityEngine.Object) null))
        return;
      typeStats.NativeSize += Profiler.GetRuntimeMemorySizeLong(o);
    }

    public static void RegisterInstance([NotNull] CrawlItem parent, [NotNull] string name, [NotNull] object instance)
    {
      TypeStats.DemandTypeStats(instance.GetType());
    }

    private TypeStats([NotNull] System.Type type) => this.Type = type;

    public int CompareTo([CanBeNull] TypeStats other)
    {
      if (this == other)
        return 0;
      return other == null ? 1 : other.SelfSize.CompareTo(this.SelfSize);
    }

    [NotNull]
    private static TypeStats DemandTypeStats([NotNull] System.Type type)
    {
      TypeStats typeStats;
      if (!TypeStats.Data.TryGetValue(type, out typeStats))
      {
        typeStats = new TypeStats(type);
        TypeStats.Data[type] = typeStats;
      }
      return typeStats;
    }
  }
}
