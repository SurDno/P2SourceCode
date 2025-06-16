// Decompiled with JetBrains decompiler
// Type: PlagueZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
    PhysicsUtility.Raycast(PlagueZone.hits, new Vector3(worldPosition.x, 1000f, worldPosition.y), Vector3.down, 2000f);
    if (PlagueZone.hits.Count <= 0)
      return;
    for (int index = 0; index < PlagueZone.hits.Count; ++index)
    {
      PlagueZone componentNonAlloc = PlagueZone.hits[index].collider.GetComponentNonAlloc<PlagueZone>();
      if ((UnityEngine.Object) componentNonAlloc != (UnityEngine.Object) null)
        result.Add(new ZoneHit(componentNonAlloc, PlagueZone.hits[index].textureCoord.x, componentNonAlloc.importance, componentNonAlloc._level));
    }
    result.Sort(new Comparison<ZoneHit>(ZoneHit.Comparison));
  }

  public static float GetLevel(Vector2 worldPosition)
  {
    float level = 0.0f;
    float num = 1f;
    PlagueZone.GetSortedZoneHits(worldPosition, PlagueZone.tmp);
    for (int index = 0; index < PlagueZone.tmp.Count; ++index)
    {
      ZoneHit zoneHit = PlagueZone.tmp[index];
      level += zoneHit.Level * zoneHit.Opacity * num;
      num *= 1f - zoneHit.Opacity;
      if ((double) num == 0.0)
        break;
    }
    PlagueZone.tmp.Clear();
    return level;
  }

  public void ApplyLevel()
  {
    LOD[] loDs = this.GetComponent<LODGroup>().GetLODs();
    if ((double) this._level == 0.0)
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
      properties.SetFloat("_Level", this._level);
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

  private void OnEnable() => this.ApplyLevel();
}
