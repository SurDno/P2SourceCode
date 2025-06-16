using Engine.Common.Components.Regions;
using System.Collections.Generic;
using UnityEngine;

public class RegionLocator : MonoBehaviour
{
  private static RegionLocator instance;
  private Dictionary<RegionEnum, RegionMesh> regionsCache = new Dictionary<RegionEnum, RegionMesh>();

  public static RegionEnum GetRegionName(Vector3 position)
  {
    return RegionLocator.instance.GetRegionNameInternal(position);
  }

  private void Initialise()
  {
    int childCount = RegionLocator.instance.transform.childCount;
    for (int index = 0; index < childCount; ++index)
    {
      RegionMesh component = RegionLocator.instance.transform.GetChild(index).gameObject.GetComponent<RegionMesh>();
      if (!((Object) component == (Object) null))
      {
        component.Initialise();
        this.regionsCache[component.Region] = component;
      }
    }
  }

  public static RegionMesh GetRegionMesh(RegionEnum regionName)
  {
    RegionMesh regionMesh;
    RegionLocator.instance.regionsCache.TryGetValue(regionName, out regionMesh);
    return regionMesh;
  }

  private void Awake()
  {
    RegionLocator.instance = this;
    this.Initialise();
  }

  private RegionEnum GetRegionNameInternal(Vector3 position)
  {
    int layerMask = 1 << this.gameObject.layer;
    RaycastHit hitInfo;
    if (!Physics.Raycast(new Ray(new Vector3(position.x, 2f, position.z), Vector3.down), out hitInfo, 3f, layerMask, QueryTriggerInteraction.Collide))
      return RegionEnum.None;
    RegionMesh component = hitInfo.collider.GetComponent<RegionMesh>();
    return (Object) component == (Object) null ? RegionEnum.None : component.Region;
  }
}
