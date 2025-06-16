using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Meta;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Impl.Services.Simulations;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons
{
  public class ComponentCollection : EngineObject
  {
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy(MemberEnum.CustomListComponent)]
    [StateLoadProxy(MemberEnum.CustomListComponent)]
    [CopyableProxy()]
    protected List<IComponent> components = new List<IComponent>();

    [Inspected]
    protected bool NeedSaveComponents
    {
      get
      {
        if (IsTemplate)
          return false;
        foreach (IComponent component in components)
        {
          if (component is INeedSave needSave && needSave.NeedSave)
            return true;
        }
        return false;
      }
    }

    public IEnumerable<IComponent> Components => components;

    public T Add<T>() where T : class, IComponent => (T) Add(typeof (T));

    public IComponent Add(Type type)
    {
      if (!type.IsClass || !TypeUtility.IsAssignableFrom(typeof (IComponent), type))
        throw new Exception(type.ToString());
      IComponent component1 = components.FirstOrDefault(o => o.GetType() == type);
      if (component1 != null)
        return component1;
      IComponent component2 = (IComponent) ServiceLocator.GetService<Factory>().Create(type, Guid.NewGuid());
      if (component2 == null)
        throw new Exception(type.ToString());
      components.Add(component2);
      ((IEngineComponent) component2).Owner = (IEntity) this;
      ComputeRequired(type);
      return component2;
    }

    public T GetComponent<T>() where T : class, IComponent
    {
      Type type = typeof (T);
      foreach (IComponent component1 in components)
      {
        if (component1 is T component2)
          return component2;
      }
      return default (T);
    }

    public IComponent GetComponent(Type type)
    {
      if (type == null)
        throw new Exception("Type is null");
      foreach (IComponent component in components)
      {
        if (TypeUtility.IsAssignableFrom(type, component.GetType()))
          return component;
      }
      return null;
    }

    public void Remove(Type type)
    {
      if (type == null)
        throw new Exception();
      for (int index = 0; index < components.Count; ++index)
      {
        IComponent component = components[index];
        if (component.GetType() == type)
        {
          ((IEngineComponent) component).Owner = null;
          components.RemoveAt(index);
          return;
        }
      }
      throw new Exception(type.ToString());
    }

    protected void DisposeComponents()
    {
      foreach (IEngineComponent component in components)
        component.Owner = null;
      components.Clear();
    }

    protected void OnAddedComponents()
    {
      foreach (IEngineComponent component in components)
        component.PrepareAdded();
      foreach (IInjectable component in components)
        component.OnAdded();
    }

    protected void OnRemovedComponents()
    {
      foreach (IInjectable component in components)
        component.OnRemoved();
      foreach (IEngineComponent component in components)
        component.PostRemoved();
    }

    private void ComputeRequired(Type type)
    {
      foreach (RequiredAttribute customAttribute in type.GetCustomAttributes(typeof (RequiredAttribute), false))
      {
        if (!customAttribute.Type.IsClass)
          Debug.LogError("Need component type : " + type + " , required : " + customAttribute.Type);
        else
          Add(customAttribute.Type);
      }
    }

    protected void ConstructCompleteComponents()
    {
      int index = 0;
      while (index < components.Count)
      {
        IComponent component = components[index];
        if (component != null)
        {
          ((IEngineComponent) component).Owner = (IEntity) this;
          ++index;
        }
        else
        {
          components.RemoveAt(index);
          Debug.LogError("Component not found : " + this.GetInfo());
        }
      }
    }

    public void Clear()
    {
      while (components.Count != 0)
        Remove(components[components.Count - 1].GetType());
    }

    [OnLoaded]
    private void OnLoaded()
    {
      foreach (object component in components)
        MetaService.Compute(component, OnLoadedAttribute.Id);
    }
  }
}
