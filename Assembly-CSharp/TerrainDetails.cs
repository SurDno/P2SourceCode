// Decompiled with JetBrains decompiler
// Type: TerrainDetails
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class TerrainDetails : MonoBehaviour
{
  public const int ChunkSize = 104;
  public const float ForceBuildDistance = 3f;
  public const int MaxChunkCreationPerFrame = 1;
  public const byte MaxInvisibilityFrameCount = 128;
  public const int MaxMeshVertexCount = 8192;
  public const int MaxRendererCreationPerFrame = 1;
  public const int MeshCount = 8;
  public Terrain Terrain;

  private void OnEnable()
  {
    if ((Object) this.Terrain == (Object) null)
      return;
    foreach (Component component1 in this.transform)
    {
      TerrainDetailLayer component2 = component1.GetComponent<TerrainDetailLayer>();
      if ((Object) component2 != (Object) null)
        component2.Terrain = this.Terrain;
    }
    this.Terrain.drawTreesAndFoliage = false;
  }

  private void OnDisable()
  {
    if ((Object) this.Terrain == (Object) null)
      return;
    this.Terrain.drawTreesAndFoliage = true;
  }
}
