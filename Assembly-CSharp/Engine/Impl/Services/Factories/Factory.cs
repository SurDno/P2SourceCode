using Cofe.Proxies;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Engine.Impl.Services.Factories
{
  [EditorService(new Type[] {typeof (Factory), typeof (IFactory)})]
  [RuntimeService(new Type[] {typeof (Factory), typeof (IFactory)})]
  public class Factory : IFactory
  {
    [Inspected]
    private static Dictionary<Type, Type> implements = new Dictionary<Type, Type>();

    public static void RegisterType(Type factory, Type face) => Factory.implements[face] = factory;

    public T Create<T>() where T : class => this.Create<T>(Guid.NewGuid());

    public T Create<T>(Guid id) where T : class => (T) this.Create(typeof (T), id);

    public object Create(Type type, Guid id)
    {
      Type type1;
      if (Factory.implements.TryGetValue(type, out type1))
        type = type1;
      type = ProxyFactory.GetProxyType(type);
      object instance = Activator.CreateInstance(type);
      if (instance is IIdSetter idSetter)
        idSetter.Id = id;
      if (instance is IFactoryProduct factoryProduct)
        factoryProduct.ConstructComplete();
      return instance;
    }

    public object Instantiate(Type type, object template)
    {
      return this.Instantiate(type, Guid.NewGuid(), template);
    }

    public object Instantiate(Type type, Guid id, object template)
    {
      if (template == null)
        throw new Exception();
      if (Profiler.enabled)
        Profiler.BeginSample("Instantiate : " + TypeUtility.GetTypeName(type));
      object target = this.Create(type, id);
      if (template is ICopyable copyable)
        copyable.CopyTo(target);
      if (target is ITemplateSetter templateSetter && template is IObject @object)
        templateSetter.Template = @object;
      if (target is IFactoryProduct factoryProduct)
        factoryProduct.ConstructComplete();
      if (Profiler.enabled)
        Profiler.EndSample();
      return target;
    }

    public T Instantiate<T>(T template) where T : class
    {
      return (object) template != null ? (T) this.Instantiate(template.GetType(), (object) template) : throw new Exception();
    }

    public T Instantiate<T>(T template, Guid id) where T : class
    {
      return (object) template != null ? (T) this.Instantiate(template.GetType(), id, (object) template) : throw new Exception();
    }
  }
}
