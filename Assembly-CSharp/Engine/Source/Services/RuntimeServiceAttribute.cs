// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.RuntimeServiceAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Proxies;
using Engine.Common.Services;
using Engine.Common.Types;
using System;
using System.Collections.Generic;

#nullable disable
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
