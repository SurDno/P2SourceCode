using UnityEngine;

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
    if (Terrain == null)
      return;
    foreach (Component component1 in transform)
    {
      TerrainDetailLayer component2 = component1.GetComponent<TerrainDetailLayer>();
      if (component2 != null)
        component2.Terrain = Terrain;
    }
    Terrain.drawTreesAndFoliage = false;
  }

  private void OnDisable()
  {
    if (Terrain == null)
      return;
    Terrain.drawTreesAndFoliage = true;
  }
}
