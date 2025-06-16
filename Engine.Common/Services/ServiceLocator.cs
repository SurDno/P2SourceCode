// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.ServiceLocator
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Cofe.Loggers;
using Cofe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Engine.Common.Services
{
  public static class ServiceLocator
  {
    private static Dictionary<Type, object> serviceMap = new Dictionary<Type, object>();
    private static List<object> services = new List<object>();
    private static object lockObject = new object();

    public static object GetService(Type type)
    {
      object service;
      lock (ServiceLocator.lockObject)
        ServiceLocator.serviceMap.TryGetValue(type, out service);
      return service;
    }

    public static T GetService<T>() where T : class
    {
      object service;
      lock (ServiceLocator.lockObject)
        ServiceLocator.serviceMap.TryGetValue(typeof (T), out service);
      return service as T;
    }

    public static void AddService(Type[] types, object service)
    {
      lock (ServiceLocator.lockObject)
      {
        ServiceLocator.services.Add(service);
        ServiceLocator.services.Sort((Comparison<object>) ((a, b) => a.GetType().Name.CompareTo(b.GetType().Name)));
        Type type1 = service.GetType();
        foreach (Type type2 in types)
        {
          object obj;
          if (ServiceLocator.serviceMap.TryGetValue(type2, out obj))
            Logger.AddWarning("Exist service , face : " + (object) type2 + " , type : " + (object) obj.GetType());
          if (!TypeUtility.IsAssignableFrom(type2, type1))
            Logger.AddError("Wrong service type, original : " + (object) type1 + " , target : " + (object) type2);
          else
            ServiceLocator.serviceMap[type2] = service;
        }
      }
    }

    public static void RemoveService(object instance)
    {
      lock (ServiceLocator.lockObject)
      {
label_2:
        foreach (KeyValuePair<Type, object> service in ServiceLocator.serviceMap)
        {
          if (service.Value == instance)
          {
            ServiceLocator.serviceMap.Remove(service.Key);
            goto label_2;
          }
        }
        if (!ServiceLocator.services.Remove(instance))
          return;
        ServiceLocator.services.Sort((Comparison<object>) ((a, b) => a.GetType().Name.CompareTo(b.GetType().Name)));
      }
    }

    public static void Clear()
    {
      lock (ServiceLocator.lockObject)
      {
        ServiceLocator.serviceMap.Clear();
        ServiceLocator.services.Clear();
      }
    }

    public static IEnumerable<object> GetServices()
    {
      lock (ServiceLocator.lockObject)
        return (IEnumerable<object>) ServiceLocator.services.ToList<object>();
    }
  }
}
