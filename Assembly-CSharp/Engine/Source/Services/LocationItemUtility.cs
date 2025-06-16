using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Source.Services;

public static class LocationItemUtility {
	public static bool CheckLocation(ILocationItemComponent target, ILocationItemComponent owner) {
		var logicLocation1 = target.LogicLocation;
		if (logicLocation1 == null)
			return false;
		var logicLocation2 = owner.LogicLocation;
		return logicLocation2 != null &&
		       ((LocationComponent)logicLocation2).LocationType == ((LocationComponent)logicLocation1).LocationType &&
		       (((LocationComponent)logicLocation2).LocationType != LocationType.Indoor ||
		        logicLocation2 == logicLocation1);
	}

	public static LocationType GetLocationType(GameObject go) {
		for (var entity = GetFirstEngineObject(go.transform); entity != null; entity = entity.Parent) {
			var component = entity.GetComponent<LocationComponent>();
			if (component != null) {
				var logicLocation = component.LogicLocation;
				if (logicLocation != null)
					return ((LocationComponent)logicLocation).LocationType;
			}
		}

		return LocationType.None;
	}

	public static IEntity GetFirstEngineObject(Transform trans) {
		do {
			var componentNonAlloc = trans.GetComponentNonAlloc<EngineGameObject>();
			if (componentNonAlloc != null)
				return componentNonAlloc.Owner;
			trans = trans.parent;
		} while (trans != null);

		return null;
	}

	public static T FindParentComponent<T>(IEntity entity) where T : class, IComponent {
		for (; entity != null; entity = entity.Parent) {
			var component = entity.GetComponent<T>();
			if (component != null)
				return component;
		}

		return default;
	}

	public static ILocationComponent GetLocation(IEntity entity) {
		var parentComponent = FindParentComponent<LocationItemComponent>(entity);
		return parentComponent != null ? parentComponent.Location : FindParentComponent<LocationComponent>(entity);
	}
}