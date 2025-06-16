// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Entity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Impl.Services.HierarchyServices;
using Engine.Impl.Services.Simulations;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons
{
  [Factory(typeof (IEntity))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class Entity : 
    ComponentCollection,
    IEntity,
    IObject,
    IDisposable,
    IInjectable,
    IFactoryProduct,
    INeedSave,
    IEntityHierarchy,
    IEntityView,
    IEntityViewSetter
  {
    private IEntityEventsListener[] listeners;
    private List<IEntity> childs;
    private IEntity parent;
    private HierarchyItem hierarchyItem;
    [Inspected]
    private EntityView view;
    private static List<MonoBehaviour> tmp = new List<MonoBehaviour>();

    public Entity() => this.isEnabled = true;

    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected bool isEnabled
    {
      get => this.GetParameter(EngineObject.Parameters.IsEnabled);
      set => this.SetParameter(EngineObject.Parameters.IsEnabled, value);
    }

    [Inspected]
    public bool DontSave
    {
      get => this.GetParameter(EngineObject.Parameters.DontSave);
      set => this.SetParameter(EngineObject.Parameters.DontSave, value);
    }

    [Inspected]
    public bool IsPlayer
    {
      get => this.GetParameter(EngineObject.Parameters.IsPlayer);
      set => this.SetParameter(EngineObject.Parameters.IsPlayer, value);
    }

    public bool NeedSave
    {
      get
      {
        return !this.DontSave && (this.NeedSaveComponents || !(this.Template is Entity template) || template.Name != this.Name || template.IsEnabled != this.IsEnabled);
      }
    }

    [Inspected]
    [StateSaveProxy(MemberEnum.None)]
    public string HierarchyPath => this.GetHierarchyPath();

    [Inspected]
    public string Context
    {
      get => (string) null;
      set
      {
      }
    }

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        if (this.isEnabled == value)
          return;
        this.isEnabled = value;
        this.UpdateEnabled();
      }
    }

    [Inspected]
    public bool IsEnabledInHierarchy
    {
      get => this.GetParameter(EngineObject.Parameters.IsEnabledInHierarchy);
      private set => this.SetParameter(EngineObject.Parameters.IsEnabledInHierarchy, value);
    }

    [Inspected]
    public bool IsAdded
    {
      get => this.GetParameter(EngineObject.Parameters.IsAdded);
      private set => this.SetParameter(EngineObject.Parameters.IsAdded, value);
    }

    public override void Dispose()
    {
      if (this.IsDisposed)
      {
        Debug.LogError((object) ("Object already disposed : " + this.GetInfo()));
      }
      else
      {
        this.EntityEventInvoke(EntityEvents.DisposeEvent);
        if (this.IsAdded)
          ServiceLocator.GetService<Simulation>().Remove((IEntity) this);
        this.DisposeComponents();
        this.parent = (IEntity) null;
        if (this.childs != null)
          this.childs.Clear();
        base.Dispose();
      }
    }

    public void OnAdded()
    {
      this.IsAdded = !this.IsAdded ? true : throw new Exception(this.GetInfo());
      this.OnAddedHierarchy();
      this.OnAddedComponents();
      this.UpdateEnabled();
    }

    public void OnRemoved()
    {
      if (!this.IsAdded)
        throw new Exception(this.GetInfo());
      this.OnRemovedComponents();
      this.OnRemovedHierarchy();
      this.IsAdded = false;
    }

    private void UpdateEnabled()
    {
      this.IsEnabledInHierarchy = (this.Parent != null ? (this.Parent.IsEnabledInHierarchy ? 1 : 0) : 1) != 0 && this.IsEnabled;
      this.EntityEventInvoke(EntityEvents.EnableChangedEvent);
      this.UpdateEnabledView();
      if (this.childs != null)
      {
        foreach (Entity child in this.childs)
          child.UpdateEnabled();
      }
      foreach (IEngineComponent component in this.Components)
        component.OnChangeEnabled();
    }

    public void ConstructComplete() => this.ConstructCompleteComponents();

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded() => this.UpdateEnabled();

    protected void EntityEventInvoke(EntityEvents kind)
    {
      if (this.listeners == null)
        return;
      foreach (IEntityEventsListener listener in this.listeners)
      {
        try
        {
          listener.OnEntityEvent((IEntity) this, kind);
        }
        catch (Exception ex)
        {
          Debug.LogException(ex);
        }
      }
    }

    public void AddListener(IEntityEventsListener listener)
    {
      if (this.listeners == null)
      {
        this.listeners = new IEntityEventsListener[1]
        {
          listener
        };
      }
      else
      {
        if (Array.IndexOf<IEntityEventsListener>(this.listeners, listener) != -1)
          return;
        Array.Resize<IEntityEventsListener>(ref this.listeners, this.listeners.Length + 1);
        this.listeners[this.listeners.Length - 1] = listener;
      }
    }

    public void RemoveListener(IEntityEventsListener listener)
    {
      int index = this.listeners != null ? Array.IndexOf<IEntityEventsListener>(this.listeners, listener) : -1;
      if (index == -1)
        return;
      if (this.listeners.Length == 1)
      {
        this.listeners = (IEntityEventsListener[]) null;
      }
      else
      {
        this.listeners[index] = this.listeners[this.listeners.Length - 1];
        Array.Resize<IEntityEventsListener>(ref this.listeners, this.listeners.Length - 1);
      }
    }

    [Inspected]
    public IEnumerable<IEntity> Childs => (IEnumerable<IEntity>) this.childs;

    [Inspected]
    public IEntity Parent => this.parent;

    [Inspected]
    public HierarchyItem HierarchyItem
    {
      get => this.hierarchyItem;
      set => this.hierarchyItem = value;
    }

    [Inspected]
    public IEntity SceneEntity => Entity.GetContainer((IEntity) this);

    public void Add(IEntity child)
    {
      if (child == null || child.Parent != null)
        throw new Exception(this.GetInfo());
      ((Entity) child).parent = (IEntity) this;
      if (this.childs == null)
        this.childs = new List<IEntity>();
      this.childs.Add(child);
    }

    public void Remove(IEntity child)
    {
      if (child == null || child.Parent != this)
        throw new Exception(this.GetInfo());
      ((Entity) child).parent = (IEntity) null;
      this.childs.Remove(child);
    }

    public void OnAddedHierarchy()
    {
      this.hierarchyItem = (HierarchyItem) null;
      IEntity sceneEntity = this.SceneEntity;
      if (sceneEntity == null)
        return;
      HierarchyContainer container = ((Entity) sceneEntity).HierarchyItem.Container;
      if (container != null)
      {
        IObject template = this.Template;
        if (template != null)
          this.hierarchyItem = container.GetItemByTemplateId(template.Id);
        else
          Debug.LogError((object) ("Template not found , scene : " + sceneEntity.GetInfo() + " , entity : " + this.GetInfo()));
      }
      else
        Debug.LogError((object) ("Container not found : " + sceneEntity.GetInfo()));
    }

    public void OnRemovedHierarchy() => this.hierarchyItem = (HierarchyItem) null;

    private static IEntity GetContainer(IEntity entity)
    {
      IEntity parent = entity.Parent;
      if (parent == null)
        return (IEntity) null;
      HierarchyItem hierarchyItem = ((Entity) parent).HierarchyItem;
      if (hierarchyItem == null)
        return (IEntity) null;
      return hierarchyItem.Container != null ? parent : Entity.GetContainer(parent);
    }

    private EntityView View
    {
      get
      {
        if (this.view == null)
          this.view = new EntityView();
        return this.view;
      }
    }

    public event Action OnGameObjectChangedEvent
    {
      add => this.View.OnGameObjectChangedEvent += value;
      remove => this.View.OnGameObjectChangedEvent -= value;
    }

    [Inspected]
    public bool IsAttached
    {
      get => this.GetParameter(EngineObject.Parameters.IsAttached);
      private set => this.SetParameter(EngineObject.Parameters.IsAttached, value);
    }

    public GameObject GameObject
    {
      get => this.View.GameObject;
      set
      {
        this.IsAttached = false;
        if ((UnityEngine.Object) this.View.GameObject != (UnityEngine.Object) null)
        {
          Entity.tmp.Clear();
          this.View.GameObject.GetComponents<MonoBehaviour>(Entity.tmp);
          foreach (MonoBehaviour monoBehaviour in Entity.tmp)
          {
            if (monoBehaviour is IEntityAttachable entityAttachable)
              entityAttachable.Detach();
          }
          this.View.GameObject = (GameObject) null;
        }
        this.View.GameObject = value;
        if ((UnityEngine.Object) this.View.GameObject != (UnityEngine.Object) null)
        {
          Entity.tmp.Clear();
          this.View.GameObject.GetComponents<MonoBehaviour>(Entity.tmp);
          foreach (MonoBehaviour monoBehaviour in Entity.tmp)
          {
            if (monoBehaviour is IEntityAttachable entityAttachable)
              entityAttachable.Attach((IEntity) this);
          }
          this.View.GameObject.SetActive(this.IsEnabled);
          this.IsAttached = true;
        }
        this.View.InvokeEvent();
      }
    }

    public Vector3 Position
    {
      get => this.View.Position;
      set => this.View.Position = value;
    }

    public Quaternion Rotation
    {
      get => this.View.Rotation;
      set => this.View.Rotation = value;
    }

    private void UpdateEnabledView()
    {
      if (!this.IsAttached)
        return;
      this.View.GameObject.SetActive(this.IsEnabled);
    }
  }
}
