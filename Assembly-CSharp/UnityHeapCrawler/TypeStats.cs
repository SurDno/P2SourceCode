using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace UnityHeapCrawler
{
  public class TypeStats : IComparable<TypeStats>
  {
    [NotNull]
    public static readonly Dictionary<Type, TypeStats> Data = new Dictionary<Type, TypeStats>();
    [NotNull]
    public readonly Type Type;
    public long SelfSize;
    public long TotalSize;
    public long NativeSize;
    public int Count;

    public static void Init() => Data.Clear();

    public static void RegisterItem([NotNull] CrawlItem item)
    {
      TypeStats typeStats = DemandTypeStats(item.Object.GetType());
      ++typeStats.Count;
      typeStats.SelfSize += item.SelfSize;
      typeStats.TotalSize += item.TotalSize;
      Object o = item.Object as Object;
      if (!(o != null))
        return;
      typeStats.NativeSize += Profiler.GetRuntimeMemorySizeLong(o);
    }

    public static void RegisterInstance([NotNull] CrawlItem parent, [NotNull] string name, [NotNull] object instance)
    {
      DemandTypeStats(instance.GetType());
    }

    private TypeStats([NotNull] Type type) => Type = type;

    public int CompareTo([CanBeNull] TypeStats other)
    {
      if (this == other)
        return 0;
      return other == null ? 1 : other.SelfSize.CompareTo(SelfSize);
    }

    [NotNull]
    private static TypeStats DemandTypeStats([NotNull] Type type)
    {
      TypeStats typeStats;
      if (!Data.TryGetValue(type, out typeStats))
      {
        typeStats = new TypeStats(type);
        Data[type] = typeStats;
      }
      return typeStats;
    }
  }
}
