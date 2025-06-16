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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Commons
{
  public class ComponentCollection : EngineObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.CustomListComponent)]
    [StateLoadProxy(MemberEnum.CustomListComponent)]
    [CopyableProxy(MemberEnum.None)]
    protected List<IComponent> components = new List<IComponent>();

    [Inspected]
    protected bool NeedSaveComponents
    {
      get
      {
        if (this.IsTemplate)
          return false;
        foreach (IComponent component in this.components)
        {
          if (component is INeedSave needSave && needSave.NeedSave)
            return true;
        }
        return false;
      }
    }

    public IEnumerable<IComponent> Components => (IEnumerable<IComponent>) this.components;

    public T Add<T>() where T : class, IComponent => (T) this.Add(typeof (T));

    public IComponent Add(System.Type type)
    {
      if (!type.IsClass || !TypeUtility.IsAssignableFrom(typeof (IComponent), type))
        throw new Exception(type.ToString());
      IComponent component1 = this.components.FirstOrDefault<IComponent>((Func<IComponent, bool>) (o => o.GetType() == type));
      if (component1 != null)
        return component1;
      IComponent component2 = (IComponent) ServiceLocator.GetService<Factory>().Create(type, Guid.NewGuid());
      if (component2 == null)
        throw new Exception(type.ToString());
      this.components.Add(component2);
      ((IEngineComponent) component2).Owner = (IEntity) this;
      this.ComputeRequired(type);
      return component2;
    }

    public T GetComponent<T>() where T : class, IComponent
    {
      System.Type type = typeof (T);
      foreach (IComponent component1 in this.components)
      {
        if (component1 is T component2)
          return component2;
      }
      return default (T);
    }

    public IComponent GetComponent(System.Type type)
    {
      if (type == (System.Type) null)
        throw new Exception("Type is null");
      foreach (IComponent component in this.components)
      {
        if (TypeUtility.IsAssignableFrom(type, component.GetType()))
          return component;
      }
      return (IComponent) null;
    }

    public void Remove(System.Type type)
    {
      if (type == (System.Type) null)
        throw new Exception();
      for (int index = 0; index < this.components.Count; ++index)
      {
        IComponent component = this.components[index];
        if (component.GetType() == type)
        {
          ((IEngineComponent) component).Owner = (IEntity) null;
          this.components.RemoveAt(index);
          return;
        }
      }
      throw new Exception(type.ToString());
    }

    protected void DisposeComponents()
    {
      foreach (IEngineComponent component in this.components)
        component.Owner = (IEntity) null;
      this.components.Clear();
    }

    protected void OnAddedComponents()
    {
      foreach (IEngineComponent component in this.components)
        component.PrepareAdded();
      foreach (IInjectable component in this.components)
        component.OnAdded();
    }

    protected void OnRemovedComponents()
    {
      foreach (IInjectable component in this.components)
        component.OnRemoved();
      foreach (IEngineComponent component in this.components)
        component.PostRemoved();
    }

    private void ComputeRequired(System.Type type)
    {
      foreach (RequiredAttribute customAttribute in type.GetCustomAttributes(typeof (RequiredAttribute), false))
      {
        if (!customAttribute.Type.IsClass)
          Debug.LogError((object) ("Need component type : " + (object) type + " , required : " + (object) customAttribute.Type));
        else
          this.Add(customAttribute.Type);
      }
    }

    protected void ConstructCompleteComponents()
    {
      int index = 0;
      while (index < this.components.Count)
      {
        IComponent component = this.components[index];
        if (component != null)
        {
          ((IEngineComponent) component).Owner = (IEntity) this;
          ++index;
        }
        else
        {
          this.components.RemoveAt(index);
          Debug.LogError((object) ("Component not found : " + this.GetInfo()));
        }
      }
    }

    public void Clear()
    {
      while (this.components.Count != 0)
        this.Remove(this.components[this.components.Count - 1].GetType());
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded()
    {
      foreach (object component in this.components)
        MetaService.Compute(component, OnLoadedAttribute.Id);
    }
  }
}
