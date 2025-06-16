using Cofe.Meta;
using Cofe.Proxies;
using Engine.Common.Services;
using Engine.Common.Types;
using System;
using System.Collections.Generic;

namespace Engine.Source.Services
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class RuntimeServiceAttribute : TypeAttribute
  {
    private Type[] types;
    private static List<Pair<Type[], Type>> services = new List<Pair<Type[], Type>>();
    private static List<object> instances = new List<object>();

    public RuntimeServiceAttribute(params Type[] types) => this.types = types;

    public override void ComputeType(Type type)
    {
      RuntimeServiceAttribute.services.Add(new Pair<Type[], Type>(this.types, type));
    }

    public static void CreateServices()
    {
      RuntimeServiceAttribute.instances.Capacity = RuntimeServiceAttribute.services.Count;
      foreach (Pair<Type[], Type> service1 in RuntimeServiceAttribute.services)
      {
        object service2 = ProxyFactory.Create(service1.Item2);
        RuntimeServiceAttribute.instances.Add(service2);
        ServiceLocator.AddService(service1.Item1, service2);
      }
    }

    public static List<object> GetServices() => RuntimeServiceAttribute.instances;
  }
}
