using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;

namespace Engine.Source.Components
{
  [Factory(typeof (ILocationComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LocationComponent : EngineComponent, ILocationComponent, IComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected LocationType locationType;
    [Inspected]
    private HashSet<ILocationComponent> childs = new HashSet<ILocationComponent>();
    [Inspected]
    private LocationComponent currentLocation;
    [Inspected]
    private LocationComponent logicLocation;
    [Inspected]
    private ILocationComponent parent;
    private IEntity player;
    private bool isHibernation = true;

    [Inspected]
    public bool IsIndoor => this.locationType == LocationType.Indoor;

    [Inspected]
    public LocationType LocationType => this.locationType;

    [Inspected(Mutable = true)]
    public bool IsHibernation
    {
      get => this.isHibernation;
      set
      {
        if (this.isHibernation == value)
          return;
        this.isHibernation = value;
        Action<ILocationComponent> hibernationChanged = this.OnHibernationChanged;
        if (hibernationChanged == null)
          return;
        hibernationChanged((ILocationComponent) this);
      }
    }

    [Inspected]
    public IEntity Player
    {
      get => this.player;
      set
      {
        this.player = value;
        Action onPlayerChanged = this.OnPlayerChanged;
        if (onPlayerChanged == null)
          return;
        onPlayerChanged();
      }
    }

    public IEnumerable<ILocationComponent> Childs => (IEnumerable<ILocationComponent>) this.childs;

    public ILocationComponent Parent => this.parent;

    public ILocationComponent CurrentLocation
    {
      get => (ILocationComponent) this.currentLocation;
      set
      {
        if (this.currentLocation != null)
        {
          this.currentLocation.RemoveChild((ILocationComponent) this);
          this.currentLocation = (LocationComponent) null;
        }
        this.currentLocation = (LocationComponent) value;
        if (this.currentLocation != null)
          this.currentLocation.AddChild((ILocationComponent) this);
        this.logicLocation = this.GetLogicLocation();
      }
    }

    public ILocationComponent LogicLocation => (ILocationComponent) this.logicLocation;

    private LocationComponent GetLogicLocation()
    {
      LocationComponent logicLocation = this;
      while (true)
      {
        if (logicLocation != null)
        {
          if (logicLocation.LocationType == 0)
            logicLocation = (LocationComponent) logicLocation.Parent;
          else
            goto label_3;
        }
        else
          break;
      }
      return (LocationComponent) null;
label_3:
      return logicLocation;
    }

    public event Action<ILocationComponent> OnHibernationChanged;

    public event Action OnPlayerChanged;

    public void AddChild(ILocationComponent child)
    {
      this.childs.Add(child);
      ((LocationComponent) child).parent = (ILocationComponent) this;
    }

    public void RemoveChild(ILocationComponent child)
    {
      this.childs.Remove(child);
      ((LocationComponent) child).parent = (ILocationComponent) null;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      IEntity sceneEntity = ((IEntityHierarchy) this.Owner).SceneEntity;
      if (sceneEntity == null)
        return;
      LocationComponent component = sceneEntity.GetComponent<LocationComponent>();
      if (component != null)
        this.CurrentLocation = (ILocationComponent) component;
    }
  }
}
