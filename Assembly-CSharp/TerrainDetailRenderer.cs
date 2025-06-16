using UnityEngine;
using UnityEngine.Rendering;

public class TerrainDetailRenderer : MonoBehaviour
{
  private static MaterialPropertyBlock materialProperties;
  private Texture2D placementMap;
  private bool visible;
  private const int seed = -1475502093;

  public void Show()
  {
    if (this.visible)
      return;
    this.visible = true;
    this.gameObject.SetActive(true);
  }

  public void Hide()
  {
    if (!this.visible)
      return;
    this.visible = false;
    this.gameObject.SetActive(false);
  }

  private void OnDestroy()
  {
    if ((Object) this.placementMap == (Object) null)
      return;
    Object.Destroy((Object) this.placementMap);
  }

  public static TerrainDetailRenderer Create(
    TerrainDetailRenderer prefab,
    TerrainDetailChunk chunk,
    int start,
    int count,
    out Vector3 min,
    out Vector3 max)
  {
    int chunkPosX = chunk.ChunkX * 104;
    int chunkPosY = chunk.ChunkY * 104;
    Vector2 detailTexelSize = chunk.Layer.DetailTexelSize;
    TerrainDetailRenderer terrainDetailRenderer = Object.Instantiate<TerrainDetailRenderer>(prefab, chunk.transform, false);
    GameObject gameObject = terrainDetailRenderer.gameObject;
    gameObject.name = "Renderer (" + (object) start + ", " + (object) count + ")";
    gameObject.layer = chunk.gameObject.layer;
    if (Application.isPlaying)
      gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
    else
      gameObject.hideFlags = HideFlags.HideAndDontSave;
    terrainDetailRenderer.visible = gameObject.activeSelf;
    int capacity;
    Mesh mesh = chunk.Layer.GetMesh(count, out capacity);
    Texture2D placementMap = TerrainDetailRenderer.CreatePlacementMap(chunk, start, count, capacity, chunkPosX, chunkPosY, out min, out max);
    Vector3 prototypeExtents = chunk.Layer.PrototypeExtents;
    min.x -= prototypeExtents.x;
    max.x += prototypeExtents.x;
    min.y -= prototypeExtents.z;
    max.y += prototypeExtents.y;
    min.z -= prototypeExtents.x;
    max.z += prototypeExtents.x;
    Vector3 position = chunk.Layer.Terrain.transform.position;
    if (TerrainDetailRenderer.materialProperties == null)
      TerrainDetailRenderer.materialProperties = new MaterialPropertyBlock();
    TerrainDetailRenderer.materialProperties.SetTexture("_PlacementMap", (Texture) placementMap);
    TerrainDetailRenderer.materialProperties.SetVector("_PlacementPosition", new Vector4(position.x + (float) chunkPosX * detailTexelSize.x, position.y, position.z + (float) chunkPosY * detailTexelSize.y, (float) count / (float) capacity));
    TerrainDetailRenderer.materialProperties.SetVector("_PlacementScale", new Vector4(detailTexelSize.x, chunk.Layer.TerrainData.size.y, detailTexelSize.y, 0.0f));
    gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
    Renderer component = (Renderer) gameObject.GetComponent<MeshRenderer>();
    component.sharedMaterial = chunk.Layer.Material;
    component.SetPropertyBlock(TerrainDetailRenderer.materialProperties);
    component.shadowCastingMode = chunk.Layer.CastShadows ? ShadowCastingMode.TwoSided : ShadowCastingMode.Off;
    gameObject.transform.SetParent(chunk.transform, false);
    gameObject.transform.localPosition = new Vector3(min.x - (float) chunkPosX * detailTexelSize.x, min.y, min.z - (float) chunkPosY * detailTexelSize.y);
    gameObject.transform.localScale = max - min;
    return terrainDetailRenderer;
  }

  public static Texture2D CreatePlacementMap(
    TerrainDetailChunk chunk,
    int pointStart,
    int pointCount,
    int capacity,
    int chunkPosX,
    int chunkPosY,
    out Vector3 min,
    out Vector3 max)
  {
    Random.State state = Random.state;
    Random.InitState(-1475502093);
    Texture2D placementMap = new Texture2D(capacity, 2, TextureFormat.RGBA32, false, true);
    placementMap.hideFlags = HideFlags.HideAndDontSave;
    placementMap.filterMode = FilterMode.Point;
    placementMap.wrapMode = TextureWrapMode.Clamp;
    Color32[] colors = new Color32[pointCount * 2];
    max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
    min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    Byte2[] points = chunk.Points;
    Vector2 detailTexelSize = chunk.Layer.DetailTexelSize;
    TerrainData terrainData = chunk.Layer.TerrainData;
    for (int index = 0; index < pointCount; ++index)
    {
      Byte2 byte2 = points[index + pointStart];
      byte b = (byte) Random.Range(0, 256);
      byte a = (byte) Random.Range(0, 256);
      colors[index] = new Color32(byte2.X, byte2.Y, b, a);
      float num1 = ((float) b / (float) byte.MaxValue + (float) byte2.X + (float) chunkPosX) * detailTexelSize.x;
      if ((double) min.x > (double) num1)
        min.x = num1;
      if ((double) max.x < (double) num1)
        max.x = num1;
      float num2 = ((float) a / (float) byte.MaxValue + (float) byte2.Y + (float) chunkPosY) * detailTexelSize.y;
      if ((double) min.z > (double) num2)
        min.z = num2;
      if ((double) max.z < (double) num2)
        max.z = num2;
      float x = num1 / terrainData.size.x;
      float y = num2 / terrainData.size.z;
      float interpolatedHeight = terrainData.GetInterpolatedHeight(x, y);
      if ((double) min.y > (double) interpolatedHeight)
        min.y = interpolatedHeight;
      if ((double) max.y < (double) interpolatedHeight)
        max.y = interpolatedHeight;
      float num3 = interpolatedHeight / terrainData.size.y;
      Vector3 interpolatedNormal = terrainData.GetInterpolatedNormal(x, y);
      ushort num4 = (ushort) Mathf.RoundToInt(num3 * (float) ushort.MaxValue);
      colors[index + pointCount] = new Color32((byte) ((uint) num4 / 256U), (byte) ((uint) num4 % 256U), (byte) (((double) interpolatedNormal.x + 1.0) * 128.0), (byte) (((double) interpolatedNormal.z + 1.0) * 128.0));
    }
    placementMap.SetPixels32(0, 0, pointCount, 2, colors);
    placementMap.Apply(false, true);
    Random.state = state;
    return placementMap;
  }
}
