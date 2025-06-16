using System;
using System.Collections.Generic;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Impl.Services.HierarchyServices;
using Engine.Impl.Services.Simulations;
using Inspectors;

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

    public Entity() => isEnabled = true;

    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy()]
    protected bool isEnabled
    {
      get => GetParameter(Parameters.IsEnabled);
      set => SetParameter(Parameters.IsEnabled, value);
    }

    [Inspected]
    public bool DontSave
    {
      get => GetParameter(Parameters.DontSave);
      set => SetParameter(Parameters.DontSave, value);
    }

    [Inspected]
    public bool IsPlayer
    {
      get => GetParameter(Parameters.IsPlayer);
      set => SetParameter(Parameters.IsPlayer, value);
    }

    public bool NeedSave
    {
      get
      {
        return !DontSave && (NeedSaveComponents || !(Template is Entity template) || template.Name != Name || template.IsEnabled != IsEnabled);
      }
    }

    [Inspected]
    [StateSaveProxy]
    public string HierarchyPath => this.GetHierarchyPath();

    [Inspected]
    public string Context
    {
      get => null;
      set
      {
      }
    }

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => isEnabled;
      set
      {
        if (isEnabled == value)
          return;
        isEnabled = value;
        UpdateEnabled();
      }
    }

    [Inspected]
    public bool IsEnabledInHierarchy
    {
      get => GetParameter(Parameters.IsEnabledInHierarchy);
      private set => SetParameter(Parameters.IsEnabledInHierarchy, value);
    }

    [Inspected]
    public bool IsAdded
    {
      get => GetParameter(Parameters.IsAdded);
      private set => SetParameter(Parameters.IsAdded, value);
    }

    public override void Dispose()
    {
      if (IsDisposed)
      {
        Debug.LogError((object) ("Object already disposed : " + this.GetInfo()));
      }
      else
      {
        EntityEventInvoke(EntityEvents.DisposeEvent);
        if (IsAdded)
          ServiceLocator.GetService<Simulation>().Remove(this);
        DisposeComponents();
        parent = null;
        if (childs != null)
          childs.Clear();
        base.Dispose();
      }
    }

    public void OnAdded()
    {
      IsAdded = !IsAdded ? true : throw new Exception(this.GetInfo());
      OnAddedHierarchy();
      OnAddedComponents();
      UpdateEnabled();
    }

    public void OnRemoved()
    {
      if (!IsAdded)
        throw new Exception(this.GetInfo());
      OnRemovedComponents();
      OnRemovedHierarchy();
      IsAdded = false;
    }

    private void UpdateEnabled()
    {
      IsEnabledInHierarchy = (Parent != null ? (Parent.IsEnabledInHierarchy ? 1 : 0) : 1) != 0 && IsEnabled;
      EntityEventInvoke(EntityEvents.EnableChangedEvent);
      UpdateEnabledView();
      if (childs != null)
      {
        foreach (Entity child in childs)
          child.UpdateEnabled();
      }
      foreach (IEngineComponent component in Components)
        component.OnChangeEnabled();
    }

    public void ConstructComplete() => ConstructCompleteComponents();

    [OnLoaded]
    private void OnLoaded() => UpdateEnabled();

    protected void EntityEventInvoke(EntityEvents kind)
    {
      if (listeners == null)
        return;
      foreach (IEntityEventsListener listener in listeners)
      {
        try
        {
          listener.OnEntityEvent(this, kind);
        }
        catch (Exception ex)
        {
          Debug.LogException(ex);
        }
      }
    }

    public void AddListener(IEntityEventsListener listener)
    {
      if (listeners == null)
      {
        listeners = new IEntityEventsListener[1]
        {
          listener
        };
      }
      else
      {
        if (Array.IndexOf(listeners, listener) != -1)
          return;
        Array.Resize(ref listeners, listeners.Length + 1);
        listeners[listeners.Length - 1] = listener;
      }
    }

    public void RemoveListener(IEntityEventsListener listener)
    {
      int index = listeners != null ? Array.IndexOf(listeners, listener) : -1;
      if (index == -1)
        return;
      if (listeners.Length == 1)
      {
        listeners = null;
      }
      else
      {
        listeners[index] = listeners[listeners.Length - 1];
        Array.Resize(ref listeners, listeners.Length - 1);
      }
    }

    [Inspected]
    public IEnumerable<IEntity> Childs => childs;

    [Inspected]
    public IEntity Parent => parent;

    [Inspected]
    public HierarchyItem HierarchyItem
    {
      get => hierarchyItem;
      set => hierarchyItem = value;
    }

    [Inspected]
    public IEntity SceneEntity => GetContainer(this);

    public void Add(IEntity child)
    {
      if (child == null || child.Parent != null)
        throw new Exception(this.GetInfo());
      ((Entity) child).parent = this;
      if (childs == null)
        childs = new List<IEntity>();
      childs.Add(child);
    }

    public void Remove(IEntity child)
    {
      if (child == null || child.Parent != this)
        throw new Exception(this.GetInfo());
      ((Entity) child).parent = null;
      childs.Remove(child);
    }

    public void OnAddedHierarchy()
    {
      hierarchyItem = null;
      IEntity sceneEntity = SceneEntity;
      if (sceneEntity == null)
        return;
      HierarchyContainer container = ((Entity) sceneEntity).HierarchyItem.Container;
      if (container != null)
      {
        IObject template = Template;
        if (template != null)
          hierarchyItem = container.GetItemByTemplateId(template.Id);
        else
          Debug.LogError((object) ("Template not found , scene : " + sceneEntity.GetInfo() + " , entity : " + this.GetInfo()));
      }
      else
        Debug.LogError((object) ("Container not found : " + sceneEntity.GetInfo()));
    }

    public void OnRemovedHierarchy() => hierarchyItem = null;

    private static IEntity GetContainer(IEntity entity)
    {
      IEntity parent = entity.Parent;
      if (parent == null)
        return null;
      HierarchyItem hierarchyItem = ((Entity) parent).HierarchyItem;
      if (hierarchyItem == null)
        return null;
      return hierarchyItem.Container != null ? parent : GetContainer(parent);
    }

    private EntityView View
    {
      get
      {
        if (view == null)
          view = new EntityView();
        return view;
      }
    }

    public event Action OnGameObjectChangedEvent
    {
      add => View.OnGameObjectChangedEvent += value;
      remove => View.OnGameObjectChangedEvent -= value;
    }

    [Inspected]
    public bool IsAttached
    {
      get => GetParameter(Parameters.IsAttached);
      private set => SetParameter(Parameters.IsAttached, value);
    }

    public GameObject GameObject
    {
      get => View.GameObject;
      set
      {
        IsAttached = false;
        if ((UnityEngine.Object) View.GameObject != (UnityEngine.Object) null)
        {
          tmp.Clear();
          View.GameObject.GetComponents<MonoBehaviour>(tmp);
          foreach (MonoBehaviour monoBehaviour in tmp)
          {
            if (monoBehaviour is IEntityAttachable entityAttachable)
              entityAttachable.Detach();
          }
          View.GameObject = (GameObject) null;
        }
        View.GameObject = value;
        if ((UnityEngine.Object) View.GameObject != (UnityEngine.Object) null)
        {
          tmp.Clear();
          View.GameObject.GetComponents<MonoBehaviour>(tmp);
          foreach (MonoBehaviour monoBehaviour in tmp)
          {
            if (monoBehaviour is IEntityAttachable entityAttachable)
              entityAttachable.Attach(this);
          }
          View.GameObject.SetActive(IsEnabled);
          IsAttached = true;
        }
        View.InvokeEvent();
      }
    }

    public Vector3 Position
    {
      get => View.Position;
      set => View.Position = value;
    }

    public Quaternion Rotation
    {
      get => View.Rotation;
      set => View.Rotation = value;
    }

    private void UpdateEnabledView()
    {
      if (!IsAttached)
        return;
      View.GameObject.SetActive(IsEnabled);
    }
  }
}
