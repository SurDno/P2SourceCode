using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (MeshCollider), typeof (LODGroup))]
[ExecuteInEditMode]
public class PlagueZone : MonoBehaviour
{
  private static List<ZoneHit> tmp = new List<ZoneHit>();
  private static List<RaycastHit> hits = new List<RaycastHit>();
  [SerializeField]
  private float _level;
  [SerializeField]
  private byte importance;

  private static void GetSortedZoneHits(Vector2 worldPosition, List<ZoneHit> result)
  {
    result.Clear();
    PhysicsUtility.Raycast(hits, new Vector3(worldPosition.x, 1000f, worldPosition.y), Vector3.down, 2000f);
    if (hits.Count <= 0)
      return;
    for (int index = 0; index < hits.Count; ++index)
    {
      PlagueZone componentNonAlloc = hits[index].collider.GetComponentNonAlloc<PlagueZone>();
      if (componentNonAlloc != null)
        result.Add(new ZoneHit(componentNonAlloc, hits[index].textureCoord.x, componentNonAlloc.importance, componentNonAlloc._level));
    }
    result.Sort(ZoneHit.Comparison);
  }

  public static float GetLevel(Vector2 worldPosition)
  {
    float level = 0.0f;
    float num = 1f;
    GetSortedZoneHits(worldPosition, tmp);
    for (int index = 0; index < tmp.Count; ++index)
    {
      ZoneHit zoneHit = tmp[index];
      level += zoneHit.Level * zoneHit.Opacity * num;
      num *= 1f - zoneHit.Opacity;
      if (num == 0.0)
        break;
    }
    tmp.Clear();
    return level;
  }

  public void ApplyLevel()
  {
    LOD[] loDs = GetComponent<LODGroup>().GetLODs();
    if (_level == 0.0)
    {
      for (int index1 = 0; index1 < loDs.Length; ++index1)
      {
        for (int index2 = 0; index2 < loDs[index1].renderers.Length; ++index2)
          loDs[index1].renderers[index2].enabled = false;
      }
    }
    else
    {
      MaterialPropertyBlock properties = new MaterialPropertyBlock();
      properties.SetFloat("_Level", _level);
      for (int index3 = 0; index3 < loDs.Length; ++index3)
      {
        for (int index4 = 0; index4 < loDs[index3].renderers.Length; ++index4)
        {
          Renderer renderer = loDs[index3].renderers[index4];
          renderer.SetPropertyBlock(properties);
          renderer.enabled = true;
        }
      }
    }
  }

  private void OnEnable() => ApplyLevel();
}
