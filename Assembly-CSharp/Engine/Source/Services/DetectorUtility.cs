using System;
using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.Detectablies;
using SoundPropagation;

namespace Engine.Source.Services
{
  public static class DetectorUtility
  {
    public static bool CheckDistance(
      Vector3 detectorPosition,
      Vector3 detectablePosition,
      float distance)
    {
      return (double) detectorPosition.x + distance >= (double) detectablePosition.x && (double) detectorPosition.x - distance <= (double) detectablePosition.x && (double) detectorPosition.z + distance >= (double) detectablePosition.z && (double) detectorPosition.z - distance <= (double) detectablePosition.z;
    }

    public static bool CheckRadiusDistance(
      Vector3 detectorPosition,
      Vector3 detectablePosition,
      float distance)
    {
      return (double) detectorPosition.x + distance >= (double) detectablePosition.x && (double) detectorPosition.x - distance <= (double) detectablePosition.x && (double) detectorPosition.z + distance >= (double) detectablePosition.z && (double) detectorPosition.z - distance <= (double) detectablePosition.z && (double) (detectablePosition - detectorPosition).sqrMagnitude <= distance * (double) distance;
    }

    public static bool CanHear(
      GameObject go,
      Vector3 goPosition,
      GameObject candidat,
      Vector3 candidatPosition,
      float hearDistance,
      float noiseDistance)
    {
      float roughMaxCost = hearDistance + noiseDistance;
      Vector3 vector3_1 = goPosition;
      Pivot component1 = go.GetComponent<Pivot>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && (UnityEngine.Object) component1.Chest != (UnityEngine.Object) null)
        vector3_1 = component1.Chest.transform.position;
      Vector3 vector3_2 = candidatPosition;
      Pivot component2 = candidat.GetComponent<Pivot>();
      if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && (UnityEngine.Object) component2.Chest != (UnityEngine.Object) null)
        vector3_2 = component2.Chest.transform.position;
      SPAudioListener instance = SPAudioListener.Instance;
      SPCell originCell = SPCell.Find(vector3_2, instance.LayerMask);
      SPCell destCell = SPCell.Find(vector3_1, instance.LayerMask);
      Location location = Locator.Main.GetLocation(originCell, vector3_2, Vector3.zero, destCell, vector3_1, Vector3.zero, roughMaxCost);
      return location.PathFound && location.PathLength * (double) Mathf.Pow(2f, location.Filtering.Loss) < roughMaxCost;
    }

    public static Location GetHearLocation(
      GameObject go,
      GameObject candidat,
      float hearDistance,
      float noiseDistance)
    {
      float roughMaxCost = hearDistance + noiseDistance;
      Vector3 position1 = go.transform.position;
      Pivot component1 = go.GetComponent<Pivot>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && (UnityEngine.Object) component1.Chest != (UnityEngine.Object) null)
        position1 = component1.Chest.transform.position;
      Vector3 position2 = candidat.transform.position;
      Pivot component2 = candidat.GetComponent<Pivot>();
      if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && (UnityEngine.Object) component2.Chest != (UnityEngine.Object) null)
        position2 = component2.Chest.transform.position;
      SPAudioListener instance = SPAudioListener.Instance;
      SPCell originCell = SPCell.Find(position2, instance.LayerMask);
      SPCell destCell = SPCell.Find(position1, instance.LayerMask);
      return Locator.Main.GetLocation(originCell, position2, Vector3.zero, destCell, position1, Vector3.zero, roughMaxCost);
    }

    public static void GetCandidats(
      List<DetectableCandidatInfo> detectablies,
      DetectorComponent detector,
      ILocationItemComponent detectorItem,
      float maxDistance,
      Action<DetectableCandidatInfo> compute)
    {
      if (!detector.Owner.IsEnabledInHierarchy || !detector.IsEnabled || detectorItem == null || detectorItem.IsHibernation)
        return;
      ILocationComponent logicLocation1 = detectorItem.LogicLocation;
      if (logicLocation1 == null)
        return;
      IEntityView owner1 = (IEntityView) detector.Owner;
      if (!owner1.IsAttached)
        return;
      Vector3 position1 = owner1.Position;
      foreach (DetectableCandidatInfo detectably in detectablies)
      {
        IEntityView owner2 = (IEntityView) detectably.Detectable.Owner;
        if (owner2.IsAttached)
        {
          Vector3 position2 = owner2.Position;
          if (CheckDistance(position1, position2, maxDistance) && detectably.Detectable.Owner.IsEnabledInHierarchy && detectably.Detectable.IsEnabled && !detectably.LocationItem.IsHibernation)
          {
            ILocationComponent logicLocation2 = detectably.LocationItem.LogicLocation;
            if (logicLocation2 != null && ((LocationComponent) logicLocation1).LocationType == ((LocationComponent) logicLocation2).LocationType && (((LocationComponent) logicLocation1).LocationType != LocationType.Indoor || logicLocation1 == logicLocation2) && detector.Owner != detectably.Detectable.Owner && compute != null)
              compute(detectably);
          }
        }
      }
    }
  }
}
