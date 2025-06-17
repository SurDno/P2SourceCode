using System;
using System.Collections.Generic;
using Cofe.Meta;
using Cofe.Proxies;
using Engine.Common.Services;
using Engine.Common.Types;

namespace Engine.Source.Services
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class GameServiceAttribute(params Type[] types) : TypeAttribute 
  {
    private static List<Pair<Type[], Type>> services = [];
    private static List<object> instances = [];

    public override void ComputeType(Type type)
    {
      services.Add(new Pair<Type[], Type>(types, type));
    }

    public static List<object> GetServices() => instances;

    public static void CreateServices()
    {
      foreach (Pair<Type[], Type> service1 in services)
      {
        object service2 = ProxyFactory.Create(service1.Item2);
        instances.Add(service2);
        ServiceLocator.AddService(service1.Item1, service2);
      }
    }

    public static void DestroyServices()
    {
      foreach (object instance in instances)
        ServiceLocator.RemoveService(instance);
      instances.Clear();
    }
  }
}
