using System;
using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.Detectablies;
using SoundPropagation;
using UnityEngine;

namespace Engine.Source.Services;

public static class DetectorUtility {
	public static bool CheckDistance(
		Vector3 detectorPosition,
		Vector3 detectablePosition,
		float distance) {
		return detectorPosition.x + (double)distance >= detectablePosition.x &&
		       detectorPosition.x - (double)distance <= detectablePosition.x &&
		       detectorPosition.z + (double)distance >= detectablePosition.z &&
		       detectorPosition.z - (double)distance <= detectablePosition.z;
	}

	public static bool CheckRadiusDistance(
		Vector3 detectorPosition,
		Vector3 detectablePosition,
		float distance) {
		return detectorPosition.x + (double)distance >= detectablePosition.x &&
		       detectorPosition.x - (double)distance <= detectablePosition.x &&
		       detectorPosition.z + (double)distance >= detectablePosition.z &&
		       detectorPosition.z - (double)distance <= detectablePosition.z &&
		       (detectablePosition - detectorPosition).sqrMagnitude <= distance * (double)distance;
	}

	public static bool CanHear(
		GameObject go,
		Vector3 goPosition,
		GameObject candidat,
		Vector3 candidatPosition,
		float hearDistance,
		float noiseDistance) {
		var roughMaxCost = hearDistance + noiseDistance;
		var vector3_1 = goPosition;
		var component1 = go.GetComponent<Pivot>();
		if (component1 != null && component1.Chest != null)
			vector3_1 = component1.Chest.transform.position;
		var vector3_2 = candidatPosition;
		var component2 = candidat.GetComponent<Pivot>();
		if (component2 != null && component2.Chest != null)
			vector3_2 = component2.Chest.transform.position;
		var instance = SPAudioListener.Instance;
		var originCell = SPCell.Find(vector3_2, instance.LayerMask);
		var destCell = SPCell.Find(vector3_1, instance.LayerMask);
		var location = Locator.Main.GetLocation(originCell, vector3_2, Vector3.zero, destCell, vector3_1, Vector3.zero,
			roughMaxCost);
		return location.PathFound &&
		       location.PathLength * (double)Mathf.Pow(2f, location.Filtering.Loss) < roughMaxCost;
	}

	public static Location GetHearLocation(
		GameObject go,
		GameObject candidat,
		float hearDistance,
		float noiseDistance) {
		var roughMaxCost = hearDistance + noiseDistance;
		var position1 = go.transform.position;
		var component1 = go.GetComponent<Pivot>();
		if (component1 != null && component1.Chest != null)
			position1 = component1.Chest.transform.position;
		var position2 = candidat.transform.position;
		var component2 = candidat.GetComponent<Pivot>();
		if (component2 != null && component2.Chest != null)
			position2 = component2.Chest.transform.position;
		var instance = SPAudioListener.Instance;
		var originCell = SPCell.Find(position2, instance.LayerMask);
		var destCell = SPCell.Find(position1, instance.LayerMask);
		return Locator.Main.GetLocation(originCell, position2, Vector3.zero, destCell, position1, Vector3.zero,
			roughMaxCost);
	}

	public static void GetCandidats(
		List<DetectableCandidatInfo> detectablies,
		DetectorComponent detector,
		ILocationItemComponent detectorItem,
		float maxDistance,
		Action<DetectableCandidatInfo> compute) {
		if (!detector.Owner.IsEnabledInHierarchy || !detector.IsEnabled || detectorItem == null ||
		    detectorItem.IsHibernation)
			return;
		var logicLocation1 = detectorItem.LogicLocation;
		if (logicLocation1 == null)
			return;
		var owner1 = (IEntityView)detector.Owner;
		if (!owner1.IsAttached)
			return;
		var position1 = owner1.Position;
		foreach (var detectably in detectablies) {
			var owner2 = (IEntityView)detectably.Detectable.Owner;
			if (owner2.IsAttached) {
				var position2 = owner2.Position;
				if (CheckDistance(position1, position2, maxDistance) &&
				    detectably.Detectable.Owner.IsEnabledInHierarchy && detectably.Detectable.IsEnabled &&
				    !detectably.LocationItem.IsHibernation) {
					var logicLocation2 = detectably.LocationItem.LogicLocation;
					if (logicLocation2 != null &&
					    ((LocationComponent)logicLocation1).LocationType ==
					    ((LocationComponent)logicLocation2).LocationType &&
					    (((LocationComponent)logicLocation1).LocationType != LocationType.Indoor ||
					     logicLocation1 == logicLocation2) && detector.Owner != detectably.Detectable.Owner &&
					    compute != null)
						compute(detectably);
				}
			}
		}
	}
}