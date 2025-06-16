using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using System;

namespace Engine.Source.Components
{
  [Required(typeof (LocationItemComponent))]
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlayerLocationComponent : EngineComponent
  {
    private LocationComponent location;
    [FromThis]
    private LocationItemComponent locationItem;

    public override void OnAdded()
    {
      base.OnAdded();
      this.locationItem.OnChangeLocation += new Action<ILocationItemComponent, ILocationComponent>(this.LocationItemOnChangeLocation);
      this.ComputeLocation();
    }

    public override void OnRemoved()
    {
      this.locationItem.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.LocationItemOnChangeLocation);
      base.OnRemoved();
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      this.ComputeLocation();
    }

    private void ComputeLocation()
    {
      ILocationComponent location1 = !((Entity) this.Owner).IsAdded || !this.Owner.IsEnabledInHierarchy ? (ILocationComponent) null : this.locationItem.Location;
      if (this.location == location1)
        return;
      LocationComponent location2 = this.location;
      this.location = (LocationComponent) location1;
      if (this.location != null)
        this.SetNewLocation(this.location);
      if (location2 == null)
        return;
      this.ClearOldLocation(location2, this.location);
    }

    private void LocationItemOnChangeLocation(
      ILocationItemComponent sender,
      ILocationComponent newLocation)
    {
      this.ComputeLocation();
    }

    private void SetNewLocation(LocationComponent location)
    {
      location.Player = this.Owner;
      if (location.Parent == null || ((LocationComponent) location.Parent).Player == this.Owner)
        return;
      this.SetNewLocation((LocationComponent) location.Parent);
    }

    private void ClearOldLocation(LocationComponent prevLocation, LocationComponent newLocation)
    {
      if (!this.Containts(prevLocation, newLocation))
        prevLocation.Player = (IEntity) null;
      if (prevLocation.Parent == null)
        return;
      this.ClearOldLocation((LocationComponent) prevLocation.Parent, newLocation);
    }

    private bool Containts(LocationComponent prevLocation, LocationComponent newLocation)
    {
      if (newLocation == null)
        return false;
      if (prevLocation == newLocation)
        return true;
      return newLocation.Parent != null && this.Containts(prevLocation, (LocationComponent) newLocation.Parent);
    }
  }
}
