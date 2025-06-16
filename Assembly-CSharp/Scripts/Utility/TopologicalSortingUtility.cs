// Decompiled with JetBrains decompiler
// Type: Scripts.Utility.TopologicalSortingUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Source.Commons;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Scripts.Utility
{
  public static class TopologicalSortingUtility
  {
    public static IEnumerable<T> TopologicalSort<T>(
      this IEnumerable<T> source,
      Func<T, IEnumerable<T>> dependencies,
      bool throwOnCycle)
    {
      List<T> sorted = new List<T>();
      HashSet<T> visited = new HashSet<T>();
      foreach (T obj in source)
        TopologicalSortingUtility.Visit<T>(obj, visited, sorted, dependencies, throwOnCycle);
      return (IEnumerable<T>) sorted;
    }

    private static void Visit<T>(
      T item,
      HashSet<T> visited,
      List<T> sorted,
      Func<T, IEnumerable<T>> dependencies,
      bool throwOnCycle)
    {
      if (!visited.Contains(item))
      {
        visited.Add(item);
        foreach (T obj in dependencies(item))
          TopologicalSortingUtility.Visit<T>(obj, visited, sorted, dependencies, throwOnCycle);
        sorted.Add(item);
      }
      else if (throwOnCycle && !sorted.Contains(item))
        throw new Exception("Cyclic dependency found");
    }

    public static IEnumerable<T> GetDependencies<T, T2>(
      T item,
      List<T> services,
      Dictionary<T, List<Type>> cache)
      where T2 : BaseDependAttribute
    {
      List<Type> types;
      if (!cache.TryGetValue(item, out types))
      {
        T2[] depends = (T2[]) item.GetType().GetCustomAttributes(typeof (T2), true);
        types = ((IEnumerable<T2>) depends).Select<T2, Type>((Func<T2, Type>) (o => o.Type)).ToList<Type>();
        types.Sort((Comparison<Type>) ((a, b) => a.Name.CompareTo(b.Name)));
        cache.Add(item, types);
        depends = (T2[]) null;
      }
      foreach (T service1 in services)
      {
        T service = service1;
        foreach (Type type1 in types)
        {
          Type type = type1;
          if (TypeUtility.IsAssignableFrom(type, service.GetType()))
          {
            yield return service;
            break;
          }
          type = (Type) null;
        }
        service = default (T);
      }
    }
  }
}
