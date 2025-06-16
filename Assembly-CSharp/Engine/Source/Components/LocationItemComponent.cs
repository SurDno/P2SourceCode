using System;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (ILocationItemComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LocationItemComponent : EngineComponent, ILocationItemComponent, IComponent
  {
    [Inspected]
    private LocationComponent currentLocation;
    private bool isHibernation = true;

    [Inspected(Mutable = true)]
    public bool IsHibernation
    {
      get => isHibernation;
      set
      {
        if (isHibernation == value)
          return;
        isHibernation = value;
        Action<ILocationItemComponent> hibernationChanged = OnHibernationChanged;
        if (hibernationChanged == null)
          return;
        hibernationChanged(this);
      }
    }

    public ILocationComponent Location
    {
      get => currentLocation;
      set
      {
        if (currentLocation == value)
          return;
        if (currentLocation != null)
        {
          currentLocation.OnHibernationChanged -= ComputeHibernation;
          currentLocation = null;
        }
        currentLocation = (LocationComponent) value;
        if (currentLocation != null)
          currentLocation.OnHibernationChanged += ComputeHibernation;
        Action<ILocationItemComponent, ILocationComponent> onChangeLocation = OnChangeLocation;
        if (onChangeLocation != null)
          onChangeLocation(this, currentLocation);
        ComputeHibernation(currentLocation);
      }
    }

    [Inspected]
    public ILocationComponent LogicLocation
    {
      get
      {
        return currentLocation != null ? currentLocation.LogicLocation : null;
      }
    }

    [Inspected]
    public bool IsIndoor
    {
      get
      {
        ILocationComponent logicLocation = LogicLocation;
        return logicLocation != null && ((LocationComponent) logicLocation).IsIndoor;
      }
    }

    public event Action<ILocationItemComponent> OnHibernationChanged;

    public event Action<ILocationItemComponent, ILocationComponent> OnChangeLocation;

    private void ComputeHibernation(ILocationComponent sender)
    {
      IsHibernation = currentLocation == null || currentLocation.IsHibernation;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      IEntity sceneEntity = ((IEntityHierarchy) Owner).SceneEntity;
      if (sceneEntity != null)
      {
        LocationComponent component = sceneEntity.GetComponent<LocationComponent>();
        if (component != null)
          Location = component;
      }
      ComputeHibernation(Location);
    }

    public override void OnRemoved()
    {
      Location = null;
      base.OnRemoved();
    }
  }
}
