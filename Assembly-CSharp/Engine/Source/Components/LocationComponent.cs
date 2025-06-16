using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (ILocationComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LocationComponent : EngineComponent, ILocationComponent, IComponent
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
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
    public bool IsIndoor => locationType == LocationType.Indoor;

    [Inspected]
    public LocationType LocationType => locationType;

    [Inspected(Mutable = true)]
    public bool IsHibernation
    {
      get => isHibernation;
      set
      {
        if (isHibernation == value)
          return;
        isHibernation = value;
        Action<ILocationComponent> hibernationChanged = OnHibernationChanged;
        if (hibernationChanged == null)
          return;
        hibernationChanged(this);
      }
    }

    [Inspected]
    public IEntity Player
    {
      get => player;
      set
      {
        player = value;
        Action onPlayerChanged = OnPlayerChanged;
        if (onPlayerChanged == null)
          return;
        onPlayerChanged();
      }
    }

    public IEnumerable<ILocationComponent> Childs => childs;

    public ILocationComponent Parent => parent;

    public ILocationComponent CurrentLocation
    {
      get => currentLocation;
      set
      {
        if (currentLocation != null)
        {
          currentLocation.RemoveChild(this);
          currentLocation = null;
        }
        currentLocation = (LocationComponent) value;
        if (currentLocation != null)
          currentLocation.AddChild(this);
        logicLocation = GetLogicLocation();
      }
    }

    public ILocationComponent LogicLocation => logicLocation;

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
      return null;
label_3:
      return logicLocation;
    }

    public event Action<ILocationComponent> OnHibernationChanged;

    public event Action OnPlayerChanged;

    public void AddChild(ILocationComponent child)
    {
      childs.Add(child);
      ((LocationComponent) child).parent = this;
    }

    public void RemoveChild(ILocationComponent child)
    {
      childs.Remove(child);
      ((LocationComponent) child).parent = null;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      IEntity sceneEntity = ((IEntityHierarchy) Owner).SceneEntity;
      if (sceneEntity == null)
        return;
      LocationComponent component = sceneEntity.GetComponent<LocationComponent>();
      if (component != null)
        CurrentLocation = component;
    }
  }
}
