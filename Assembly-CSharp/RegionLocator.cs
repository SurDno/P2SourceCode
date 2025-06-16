using System.Collections.Generic;
using Engine.Common.Components.Regions;

public class RegionLocator : MonoBehaviour
{
  private static RegionLocator instance;
  private Dictionary<RegionEnum, RegionMesh> regionsCache = new Dictionary<RegionEnum, RegionMesh>();

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
      if (!((Object) component == (Object) null))
      {
        component.Initialise();
        regionsCache[component.Region] = component;
      }
    }
  }

  public static RegionMesh GetRegionMesh(RegionEnum regionName)
  {
    RegionMesh regionMesh;
    instance.regionsCache.TryGetValue(regionName, out regionMesh);
    return regionMesh;
  }

  private void Awake()
  {
    instance = this;
    Initialise();
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
