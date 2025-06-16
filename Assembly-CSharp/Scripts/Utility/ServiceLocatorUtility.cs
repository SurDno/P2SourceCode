using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common.Services;
using Engine.Source.Commons;

namespace Scripts.Utility
{
  public static class ServiceLocatorUtility
  {
    public static List<T> GetServices<T, T2>()
      where T : class
      where T2 : BaseDependAttribute
    {
      return SortServices<T, T2>(ServiceLocator.GetServices().Select(o => o as T).Where(o => o != null).ToList());
    }

    public static List<T> SortServices<T, T2>(List<T> services)
      where T : class
      where T2 : BaseDependAttribute
    {
      services.Sort((a, b) => a.GetType().Name.CompareTo(b.GetType().Name));
      Dictionary<T, List<Type>> cache = new Dictionary<T, List<Type>>();
      services = services.TopologicalSort(item => TopologicalSortingUtility.GetDependencies<T, T2>(item, services, cache), true).ToList();
      string message = "ServiceLocatorUtility , attribute : " + typeof (T2).Name;
      foreach (T service in services)
        message = message + "\n   --- service : " + service.GetType().Name;
      Debug.Log((object) message);
      return services;
    }
  }
}
