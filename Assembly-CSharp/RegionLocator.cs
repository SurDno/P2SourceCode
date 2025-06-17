using System.Collections.Generic;
using Engine.Common.Components.Regions;
using UnityEngine;

public class RegionLocator : MonoBehaviour
{
  private static RegionLocator instance;
  private Dictionary<RegionEnum, RegionMesh> regionsCache = new();

  public static RegionEnum GetRegionName(Vector3 position)
  {
    return instance.GetRegionNameInternal(position);
  }

  private void Initialise()
  {
    int childCount = instance.transform.childCount;
    for (int index = 0; index < childCount; ++index)
    {
      RegionMesh component = instance.transform.GetChild(index).gameObject.GetComponent<RegionMesh>();
      if (!(component == null))
      {
        component.Initialise();
        regionsCache[component.Region] = component;
      }
    }
  }

  public static RegionMesh GetRegionMesh(RegionEnum regionName)
  {
    instance.regionsCache.TryGetValue(regionName, out RegionMesh regionMesh);
    return regionMesh;
  }

  private void Awake()
  {
    instance = this;
    Initialise();
  }

  private RegionEnum GetRegionNameInternal(Vector3 position)
  {
    int layerMask = 1 << gameObject.layer;
    if (!Physics.Raycast(new Ray(new Vector3(position.x, 2f, position.z), Vector3.down), out RaycastHit hitInfo, 3f, layerMask, QueryTriggerInteraction.Collide))
      return RegionEnum.None;
    RegionMesh component = hitInfo.collider.GetComponent<RegionMesh>();
    return component == null ? RegionEnum.None : component.Region;
  }
}
