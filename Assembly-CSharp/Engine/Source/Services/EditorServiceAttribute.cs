using Cofe.Meta;
using Cofe.Proxies;
using Engine.Common.Services;
using Engine.Common.Types;
using System;
using System.Collections.Generic;

namespace Engine.Source.Services
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class EditorServiceAttribute : TypeAttribute
  {
    private Type[] types;
    private static List<Pair<Type[], Type>> services = new List<Pair<Type[], Type>>();

    public EditorServiceAttribute(params Type[] types) => this.types = types;

    public override void ComputeType(Type type)
    {
      EditorServiceAttribute.services.Add(new Pair<Type[], Type>(this.types, type));
    }

    public static void CreateServices()
    {
      foreach (Pair<Type[], Type> service1 in EditorServiceAttribute.services)
      {
        object service2 = ProxyFactory.Create(service1.Item2);
        ServiceLocator.AddService(service1.Item1, service2);
      }
    }
  }
}
