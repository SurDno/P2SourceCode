using Engine.Common.Services;
using Engine.Source.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Utility
{
  public static class ServiceLocatorUtility
  {
    public static List<T> GetServices<T, T2>()
      where T : class
      where T2 : BaseDependAttribute
    {
      return ServiceLocatorUtility.SortServices<T, T2>(ServiceLocator.GetServices().Select<object, T>((Func<object, T>) (o => o as T)).Where<T>((Func<T, bool>) (o => (object) o != null)).ToList<T>());
    }

    public static List<T> SortServices<T, T2>(List<T> services)
      where T : class
      where T2 : BaseDependAttribute
    {
      services.Sort((Comparison<T>) ((a, b) => a.GetType().Name.CompareTo(b.GetType().Name)));
      Dictionary<T, List<System.Type>> cache = new Dictionary<T, List<System.Type>>();
      services = services.TopologicalSort<T>((Func<T, IEnumerable<T>>) (item => TopologicalSortingUtility.GetDependencies<T, T2>(item, services, cache)), true).ToList<T>();
      string message = "ServiceLocatorUtility , attribute : " + typeof (T2).Name;
      foreach (T service in services)
        message = message + "\n   --- service : " + service.GetType().Name;
      Debug.Log((object) message);
      return services;
    }
  }
}
