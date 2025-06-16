using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;

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
      get => this.isHibernation;
      set
      {
        if (this.isHibernation == value)
          return;
        this.isHibernation = value;
        Action<ILocationItemComponent> hibernationChanged = this.OnHibernationChanged;
        if (hibernationChanged == null)
          return;
        hibernationChanged((ILocationItemComponent) this);
      }
    }

    public ILocationComponent Location
    {
      get => (ILocationComponent) this.currentLocation;
      set
      {
        if (this.currentLocation == value)
          return;
        if (this.currentLocation != null)
        {
          this.currentLocation.OnHibernationChanged -= new Action<ILocationComponent>(this.ComputeHibernation);
          this.currentLocation = (LocationComponent) null;
        }
        this.currentLocation = (LocationComponent) value;
        if (this.currentLocation != null)
          this.currentLocation.OnHibernationChanged += new Action<ILocationComponent>(this.ComputeHibernation);
        Action<ILocationItemComponent, ILocationComponent> onChangeLocation = this.OnChangeLocation;
        if (onChangeLocation != null)
          onChangeLocation((ILocationItemComponent) this, (ILocationComponent) this.currentLocation);
        this.ComputeHibernation((ILocationComponent) this.currentLocation);
      }
    }

    [Inspected]
    public ILocationComponent LogicLocation
    {
      get
      {
        return this.currentLocation != null ? this.currentLocation.LogicLocation : (ILocationComponent) null;
      }
    }

    [Inspected]
    public bool IsIndoor
    {
      get
      {
        ILocationComponent logicLocation = this.LogicLocation;
        return logicLocation != null && ((LocationComponent) logicLocation).IsIndoor;
      }
    }

    public event Action<ILocationItemComponent> OnHibernationChanged;

    public event Action<ILocationItemComponent, ILocationComponent> OnChangeLocation;

    private void ComputeHibernation(ILocationComponent sender)
    {
      this.IsHibernation = this.currentLocation == null || this.currentLocation.IsHibernation;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      IEntity sceneEntity = ((IEntityHierarchy) this.Owner).SceneEntity;
      if (sceneEntity != null)
      {
        LocationComponent component = sceneEntity.GetComponent<LocationComponent>();
        if (component != null)
          this.Location = (ILocationComponent) component;
      }
      this.ComputeHibernation(this.Location);
    }

    public override void OnRemoved()
    {
      this.Location = (ILocationComponent) null;
      base.OnRemoved();
    }
  }
}
