using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;

namespace Engine.Source.Components;

[Required(typeof(LocationItemComponent))]
[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class PlayerLocationComponent : EngineComponent {
	private LocationComponent location;
	[FromThis] private LocationItemComponent locationItem;

	public override void OnAdded() {
		base.OnAdded();
		locationItem.OnChangeLocation += LocationItemOnChangeLocation;
		ComputeLocation();
	}

	public override void OnRemoved() {
		locationItem.OnChangeLocation -= LocationItemOnChangeLocation;
		base.OnRemoved();
	}

	public override void OnChangeEnabled() {
		base.OnChangeEnabled();
		ComputeLocation();
	}

	private void ComputeLocation() {
		var location1 = !((Entity)Owner).IsAdded || !Owner.IsEnabledInHierarchy ? null : locationItem.Location;
		if (location == location1)
			return;
		var location2 = location;
		location = (LocationComponent)location1;
		if (location != null)
			SetNewLocation(location);
		if (location2 == null)
			return;
		ClearOldLocation(location2, location);
	}

	private void LocationItemOnChangeLocation(
		ILocationItemComponent sender,
		ILocationComponent newLocation) {
		ComputeLocation();
	}

	private void SetNewLocation(LocationComponent location) {
		location.Player = Owner;
		if (location.Parent == null || ((LocationComponent)location.Parent).Player == Owner)
			return;
		SetNewLocation((LocationComponent)location.Parent);
	}

	private void ClearOldLocation(LocationComponent prevLocation, LocationComponent newLocation) {
		if (!Containts(prevLocation, newLocation))
			prevLocation.Player = null;
		if (prevLocation.Parent == null)
			return;
		ClearOldLocation((LocationComponent)prevLocation.Parent, newLocation);
	}

	private bool Containts(LocationComponent prevLocation, LocationComponent newLocation) {
		if (newLocation == null)
			return false;
		if (prevLocation == newLocation)
			return true;
		return newLocation.Parent != null && Containts(prevLocation, (LocationComponent)newLocation.Parent);
	}
}