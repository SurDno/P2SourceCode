using Engine.Source.Settings.External;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetailChunk : MonoBehaviour
{
  private static List<Byte2> pointList;
  private static List<int> indexList;
  private static List<Vector3> boundList;
  private static int frame = -1;
  private static int renderersCreatedThisFrame = 0;
  [SerializeField]
  private TerrainDetailRenderer rendererPrefab;
  private TerrainDetailRenderer[] renderers;
  private byte[] invisibilityFrameCount;
  private int[] indices;
  private Vector3[] bounds;
  private bool needsUpdate = false;

  public TerrainDetailLayer Layer { get; set; }

  public int ChunkX { get; set; }

  public int ChunkY { get; set; }

  public Byte2[] Points { get; set; }

  public void Preload()
  {
    if ((Object) this.rendererPrefab == (Object) null)
      return;
    for (int index = 0; index < this.renderers.Length; ++index)
    {
      if ((Object) this.renderers[index] == (Object) null)
      {
        TerrainDetailRenderer terrainDetailRenderer = TerrainDetailRenderer.Create(this.rendererPrefab, this, this.indices[index * 2], this.indices[index * 2 + 1], out this.bounds[index * 2], out this.bounds[index * 2 + 1]);
        this.renderers[index] = terrainDetailRenderer;
        terrainDetailRenderer.Hide();
      }
    }
  }

  public bool UpdateVisibility(Vector3 terrainSpaceCameraPosition, float drawDistance)
  {
    if ((Object) this.rendererPrefab == (Object) null)
      return false;
    this.needsUpdate = false;
    for (int index1 = 0; index1 < this.renderers.Length; ++index1)
    {
      int index2 = index1;
      Vector3 b = terrainSpaceCameraPosition;
      Vector3 bound1 = this.bounds[index1 * 2];
      Vector3 bound2 = this.bounds[index1 * 2 + 1];
      if ((double) b.x < (double) bound1.x)
        b.x = bound1.x;
      if ((double) b.y < (double) bound1.y)
        b.y = bound1.y;
      if ((double) b.z < (double) bound1.z)
        b.z = bound1.z;
      if ((double) b.x > (double) bound2.x)
        b.x = bound2.x;
      if ((double) b.y > (double) bound2.y)
        b.y = bound2.y;
      if ((double) b.z > (double) bound2.z)
        b.z = bound2.z;
      float num = Vector3.Distance(terrainSpaceCameraPosition, b);
      if ((double) num < (double) drawDistance)
      {
        if ((Object) this.renderers[index2] == (Object) null)
        {
          if (TerrainDetailChunk.frame != Time.frameCount)
          {
            TerrainDetailChunk.frame = Time.frameCount;
            TerrainDetailChunk.renderersCreatedThisFrame = 0;
          }
          if ((double) num < 3.0 || TerrainDetailChunk.renderersCreatedThisFrame < 1)
          {
            ++TerrainDetailChunk.renderersCreatedThisFrame;
            this.renderers[index2] = TerrainDetailRenderer.Create(this.rendererPrefab, this, this.indices[index2 * 2], this.indices[index2 * 2 + 1], out this.bounds[index2 * 2], out this.bounds[index2 * 2 + 1]);
            this.needsUpdate = true;
          }
        }
        else
        {
          this.renderers[index2].Show();
          this.needsUpdate = true;
        }
        this.invisibilityFrameCount[index2] = (byte) 0;
      }
      else
        this.needsUpdate = this.HideRenderer(index2) || this.needsUpdate;
    }
    return this.needsUpdate;
  }

  public bool HideRenderer(int index)
  {
    if (!((Object) this.renderers[index] != (Object) null))
      return false;
    if (Application.isPlaying && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadGrass)
    {
      this.renderers[index].Hide();
      return false;
    }
    if (this.invisibilityFrameCount[index] == (byte) 0)
    {
      this.renderers[index].Hide();
      this.invisibilityFrameCount[index] = (byte) 1;
      return true;
    }
    if (this.invisibilityFrameCount[index] < (byte) 128)
    {
      ++this.invisibilityFrameCount[index];
      return true;
    }
    Object.Destroy((Object) this.renderers[index].gameObject);
    this.renderers[index] = (TerrainDetailRenderer) null;
    return false;
  }

  public bool HideAll(bool forceDestroy = false)
  {
    if (!this.needsUpdate)
      return false;
    this.needsUpdate = false;
    for (int index = 0; index < this.renderers.Length; ++index)
    {
      if (forceDestroy)
      {
        if ((Object) this.renderers[index] != (Object) null)
        {
          Object.Destroy((Object) this.renderers[index].gameObject);
          this.renderers[index] = (TerrainDetailRenderer) null;
        }
      }
      else
        this.needsUpdate = this.HideRenderer(index) || this.needsUpdate;
    }
    return this.needsUpdate;
  }

  public static TerrainDetailChunk Create(
    TerrainDetailChunk prefab,
    TerrainDetailLayer layer,
    int chunkX,
    int chunkY)
  {
    TerrainDetailLayer.PrepareClearList<Byte2>(ref TerrainDetailChunk.pointList);
    TerrainDetailLayer.PrepareClearList<int>(ref TerrainDetailChunk.indexList);
    TerrainDetailLayer.PrepareClearList<Vector3>(ref TerrainDetailChunk.boundList);
    int xBase = chunkX * 104;
    int yBase = chunkY * 104;
    int num1 = (chunkX + 1) * 104;
    if (num1 > layer.TerrainData.detailWidth)
      num1 = layer.TerrainData.detailWidth;
    int width = num1 - xBase;
    int num2 = (chunkY + 1) * 104;
    if (num2 > layer.TerrainData.detailHeight)
      num2 = layer.TerrainData.detailHeight;
    int height = num2 - yBase;
    int[,] detailLayer = layer.TerrainData.GetDetailLayer(xBase, yBase, width, height, layer.LayerIndex);
    for (int x = 0; x < width; ++x)
    {
      for (int y = 0; y < height; ++y)
      {
        int num3 = detailLayer[y, x];
        if (num3 != 0)
        {
          for (int index = 0; index < num3; ++index)
            TerrainDetailChunk.pointList.Add(new Byte2((byte) x, (byte) y));
        }
      }
    }
    int f = TerrainDetailChunk.pointList.Count / layer.MaxChunkCapacity;
    if (TerrainDetailChunk.pointList.Count % layer.MaxChunkCapacity > 0)
      ++f;
    int num4 = (int) Mathf.Sqrt((float) f);
    for (int index1 = 0; index1 < num4; ++index1)
    {
      int num5 = index1 * f / num4;
      int num6 = (index1 + 1) * f / num4;
      int index2 = num5 * TerrainDetailChunk.pointList.Count / f;
      int count = num6 * TerrainDetailChunk.pointList.Count / f - index2;
      TerrainDetailChunk.pointList.Sort(index2, count, (IComparer<Byte2>) new Byte2.YComparer());
      for (int index3 = num5; index3 < num6; ++index3)
      {
        int num7 = index3 * TerrainDetailChunk.pointList.Count / f;
        int num8 = (index3 + 1) * TerrainDetailChunk.pointList.Count / f - num7;
        if (num8 > layer.MaxChunkCapacity)
        {
          Debug.LogWarning((object) ("Grass : Renderer exceeds mesh capacity (points.Count = " + (object) TerrainDetailChunk.pointList.Count + ", maxChunkCapacity = " + (object) layer.MaxChunkCapacity + ", rendererCount = " + (object) f + ", cols = " + (object) num4 + ", colFirstRenderer = " + (object) num5 + ", colAfterLastRenderer = " + (object) num6 + ", colFirstPoint = " + (object) index2 + ", colPointCount = " + (object) count + ", rendererFirstPoint = " + (object) num7 + ", rendererPointCount = " + (object) num8));
          num8 = layer.MaxChunkCapacity;
        }
        byte num9 = byte.MaxValue;
        byte num10 = 0;
        byte num11 = byte.MaxValue;
        byte num12 = 0;
        for (int index4 = 0; index4 < num8; ++index4)
        {
          Byte2 point = TerrainDetailChunk.pointList[index4 + num7];
          if ((int) num9 > (int) point.X)
            num9 = point.X;
          if ((int) num11 > (int) point.Y)
            num11 = point.Y;
          if ((int) num10 < (int) point.X)
            num10 = point.X;
          if ((int) num12 < (int) point.Y)
            num12 = point.Y;
        }
        TerrainDetailChunk.indexList.Add(num7);
        TerrainDetailChunk.indexList.Add(num8);
        TerrainDetailChunk.boundList.Add(new Vector3((float) (xBase + (int) num9) * layer.DetailTexelSize.x - layer.PrototypeExtents.x, -layer.PrototypeExtents.z, (float) (yBase + (int) num11) * layer.DetailTexelSize.y - layer.PrototypeExtents.x));
        TerrainDetailChunk.boundList.Add(new Vector3((float) (xBase + (int) num10 + 1) * layer.DetailTexelSize.x + layer.PrototypeExtents.x, layer.TerrainData.size.y + layer.PrototypeExtents.y, (float) (yBase + (int) num12 + 1) * layer.DetailTexelSize.y + layer.PrototypeExtents.x));
      }
    }
    TerrainDetailChunk terrainDetailChunk = Object.Instantiate<TerrainDetailChunk>(prefab, layer.transform, false);
    GameObject gameObject = terrainDetailChunk.gameObject;
    gameObject.transform.localPosition = new Vector3((float) xBase * layer.DetailTexelSize.x, 0.0f, (float) yBase * layer.DetailTexelSize.y);
    gameObject.name = "Chunk (" + (object) chunkX + ", " + (object) chunkY + ")";
    gameObject.layer = layer.gameObject.layer;
    if (Application.isPlaying)
      gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
    else
      gameObject.hideFlags = HideFlags.HideAndDontSave;
    terrainDetailChunk.renderers = new TerrainDetailRenderer[f];
    terrainDetailChunk.invisibilityFrameCount = new byte[f];
    terrainDetailChunk.Points = TerrainDetailChunk.pointList.ToArray();
    terrainDetailChunk.indices = TerrainDetailChunk.indexList.ToArray();
    terrainDetailChunk.bounds = TerrainDetailChunk.boundList.ToArray();
    terrainDetailChunk.Layer = layer;
    terrainDetailChunk.ChunkX = chunkX;
    terrainDetailChunk.ChunkY = chunkY;
    return terrainDetailChunk;
  }
}
