using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine.Profiling;

namespace Engine.Impl.Services.Factories
{
  [EditorService(typeof (Factory), typeof (IFactory))]
  [RuntimeService(typeof (Factory), typeof (IFactory))]
  public class Factory : IFactory
  {
    [Inspected]
    private static Dictionary<Type, Type> implements = new();

    public static void RegisterType(Type factory, Type face) => implements[face] = factory;

    public T Create<T>() where T : class => Create<T>(Guid.NewGuid());

    public T Create<T>(Guid id) where T : class => (T) Create(typeof (T), id);

    public object Create(Type type, Guid id)
    {
      if (implements.TryGetValue(type, out Type type1))
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
      return Instantiate(type, Guid.NewGuid(), template);
    }

    public object Instantiate(Type type, Guid id, object template)
    {
      if (template == null)
        throw new Exception();
      if (Profiler.enabled)
        Profiler.BeginSample("Instantiate : " + TypeUtility.GetTypeName(type));
      object target = Create(type, id);
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
      return template != null ? (T) Instantiate(template.GetType(), template) : throw new Exception();
    }

    public T Instantiate<T>(T template, Guid id) where T : class
    {
      return template != null ? (T) Instantiate(template.GetType(), id, template) : throw new Exception();
    }
  }
}
