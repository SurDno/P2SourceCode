// Decompiled with JetBrains decompiler
// Type: TerrainDetailLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Settings.External;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

#nullable disable
[ExecuteInEditMode]
public class TerrainDetailLayer : MonoBehaviour
{
  private static DynamicMeshBuffers meshBuffers;
  private static int frame = -1;
  private static int chunksCreatedThisFrame = 0;
  [SerializeField]
  [FormerlySerializedAs("PrototypeMesh")]
  private Mesh prototypeMesh;
  [SerializeField]
  [FormerlySerializedAs("Material")]
  private Material material;
  [SerializeField]
  private TerrainDetailChunk chunkPrefab;
  [SerializeField]
  [FormerlySerializedAs("LayerIndex")]
  private int layerIndex = 0;
  [SerializeField]
  [FormerlySerializedAs("CastShadows")]
  private bool castShadows = false;
  [SerializeField]
  private TerrainDetailLayer.BendingCalculation bendingCalculation = TerrainDetailLayer.BendingCalculation.Off;
  private TerrainDetailChunk[,] chunks;
  private Vector2 chunkWorldSize;
  private Mesh[] chunkMeshes;
  private StaticMeshBuffers prototypeBuffers;
  private int minActiveChunkX = int.MaxValue;
  private int maxActiveChunkX = int.MinValue;
  private int minActiveChunkY = int.MaxValue;
  private int maxActiveChunkY = int.MinValue;
  private Coroutine preloading;
  private bool preloaded = false;

  public Vector2 DetailTexelSize { get; set; }

  public int MaxChunkCapacity { get; set; }

  public Vector3 PrototypeExtents { get; set; }

  public Terrain Terrain { get; set; }

  public TerrainData TerrainData { get; set; }

  public bool CastShadows => this.castShadows;

  public int LayerIndex => this.layerIndex;

  public Material Material => this.material;

  private IEnumerator Preload()
  {
    if ((Object) this.chunkPrefab != (Object) null)
    {
      int iLength = this.chunks.GetLength(0);
      int jLength = this.chunks.GetLength(1);
      for (int i = 0; i < iLength; ++i)
      {
        for (int j = 0; j < jLength; ++j)
        {
          TerrainDetailChunk chunk = this.chunks[i, j];
          if ((Object) chunk == (Object) null)
          {
            chunk = TerrainDetailChunk.Create(this.chunkPrefab, this, i, j);
            this.chunks[i, j] = chunk;
          }
          chunk.Preload();
          chunk = (TerrainDetailChunk) null;
        }
        yield return (object) null;
      }
    }
    this.preloading = (Coroutine) null;
  }

  private Mesh CreateMesh(int capacity)
  {
    bool flag1 = this.prototypeBuffers.HasColors();
    bool flag2 = this.prototypeBuffers.HasUV();
    bool flag3 = this.prototypeBuffers.HasNormals();
    bool flag4 = this.prototypeBuffers.HasTangents();
    Mesh mesh = new Mesh();
    mesh.hideFlags = HideFlags.HideAndDontSave;
    mesh.name = "Terrain Detail " + (object) this.layerIndex + " (" + (object) capacity + ")";
    if (TerrainDetailLayer.meshBuffers == null)
      TerrainDetailLayer.meshBuffers = new DynamicMeshBuffers();
    TerrainDetailLayer.PrepareClearList<Vector3>(ref TerrainDetailLayer.meshBuffers.Vertices);
    if (flag1)
      TerrainDetailLayer.PrepareClearList<Color32>(ref TerrainDetailLayer.meshBuffers.Colors);
    if (flag2)
      TerrainDetailLayer.PrepareClearList<Vector2>(ref TerrainDetailLayer.meshBuffers.UV);
    TerrainDetailLayer.PrepareClearList<Vector2>(ref TerrainDetailLayer.meshBuffers.UV2);
    if (flag3)
      TerrainDetailLayer.PrepareClearList<Vector3>(ref TerrainDetailLayer.meshBuffers.Normals);
    if (flag4)
      TerrainDetailLayer.PrepareClearList<Vector4>(ref TerrainDetailLayer.meshBuffers.Tangents);
    TerrainDetailLayer.PrepareClearList<int>(ref TerrainDetailLayer.meshBuffers.Triangles);
    for (int index1 = 0; index1 < capacity; ++index1)
    {
      for (int index2 = 0; index2 < this.prototypeBuffers.TriangleCount; ++index2)
        TerrainDetailLayer.meshBuffers.Triangles.Add(this.prototypeBuffers.Triangles[index2] + TerrainDetailLayer.meshBuffers.Vertices.Count);
      for (int index3 = 0; index3 < this.prototypeBuffers.VertexCount; ++index3)
      {
        TerrainDetailLayer.meshBuffers.Vertices.Add(this.prototypeBuffers.Vertices[index3]);
        TerrainDetailLayer.meshBuffers.UV2.Add(new Vector2((0.5f + (float) index1) / (float) capacity, 0.0f));
        if (flag1)
          TerrainDetailLayer.meshBuffers.Colors.Add(this.prototypeBuffers.Colors[index3]);
        if (flag2)
          TerrainDetailLayer.meshBuffers.UV.Add(this.prototypeBuffers.UV[index3]);
        if (flag3)
          TerrainDetailLayer.meshBuffers.Normals.Add(this.prototypeBuffers.Normals[index3]);
        if (flag4)
          TerrainDetailLayer.meshBuffers.Tangents.Add(this.prototypeBuffers.Tangents[index3]);
      }
    }
    mesh.SetVertices(TerrainDetailLayer.meshBuffers.Vertices);
    mesh.SetUVs(1, TerrainDetailLayer.meshBuffers.UV2);
    mesh.SetTriangles(TerrainDetailLayer.meshBuffers.Triangles, 0, false);
    if (flag1)
      mesh.SetColors(TerrainDetailLayer.meshBuffers.Colors);
    if (flag2)
      mesh.SetUVs(0, TerrainDetailLayer.meshBuffers.UV);
    if (flag3)
      mesh.SetNormals(TerrainDetailLayer.meshBuffers.Normals);
    if (flag4)
      mesh.SetTangents(TerrainDetailLayer.meshBuffers.Tangents);
    mesh.bounds = new Bounds(new Vector3(0.5f, 0.5f, 0.5f), Vector3.one);
    mesh.UploadMeshData(true);
    return mesh;
  }

  public Mesh GetMesh(int pointCount, out int capacity)
  {
    int num = pointCount * 8;
    int index = num / this.MaxChunkCapacity - 1;
    if (num % this.MaxChunkCapacity > 0)
      ++index;
    capacity = (index + 1) * this.MaxChunkCapacity / 8;
    if ((Object) this.chunkMeshes[index] == (Object) null)
      this.chunkMeshes[index] = this.CreateMesh(capacity);
    return this.chunkMeshes[index];
  }

  private void OnEnable()
  {
    Camera.onPreCull -= new Camera.CameraCallback(this.OnPreCullEvent);
    Camera.onPreCull += new Camera.CameraCallback(this.OnPreCullEvent);
  }

  private void OnDisable()
  {
    if (this.preloading != null)
    {
      this.StopCoroutine(this.preloading);
      this.preloading = (Coroutine) null;
    }
    this.preloaded = false;
    Camera.onPreCull -= new Camera.CameraCallback(this.OnPreCullEvent);
    this.TerrainData = (TerrainData) null;
    if (this.chunks != null)
    {
      int length1 = this.chunks.GetLength(0);
      int length2 = this.chunks.GetLength(1);
      for (int index1 = 0; index1 < length1; ++index1)
      {
        for (int index2 = 0; index2 < length2; ++index2)
        {
          if ((Object) this.chunks[index1, index2] != (Object) null)
          {
            Object.Destroy((Object) this.chunks[index1, index2].gameObject);
            this.chunks[index1, index2] = (TerrainDetailChunk) null;
          }
        }
      }
      this.chunks = (TerrainDetailChunk[,]) null;
    }
    if (this.chunkMeshes != null)
    {
      for (int index = 0; index < this.chunkMeshes.Length; ++index)
      {
        Object.Destroy((Object) this.chunkMeshes[index]);
        this.chunkMeshes[index] = (Mesh) null;
      }
      this.chunkMeshes = (Mesh[]) null;
    }
    this.prototypeBuffers = (StaticMeshBuffers) null;
  }

  private void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (TerrainDetailLayer));
    this.OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void OnPreCullEvent2(Camera camera)
  {
    if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableGrass || (Object) this.prototypeMesh == (Object) null || (Object) this.material == (Object) null || (Object) this.Terrain == (Object) null || (Object) this.chunkPrefab == (Object) null || (Object) camera == (Object) null || (camera.cullingMask & 1 << this.gameObject.layer) == 0)
      return;
    if (this.chunks == null)
    {
      this.TerrainData = this.Terrain.terrainData;
      this.DetailTexelSize = new Vector2(this.TerrainData.size.x / (float) this.TerrainData.detailWidth, this.TerrainData.size.z / (float) this.TerrainData.detailHeight);
      this.chunkWorldSize = this.DetailTexelSize * 104f;
      int length1 = this.TerrainData.detailWidth / 104;
      if (this.TerrainData.detailWidth % 104 > 0)
        ++length1;
      int length2 = this.TerrainData.detailHeight / 104;
      if (this.TerrainData.detailHeight % 104 > 0)
        ++length2;
      this.chunks = new TerrainDetailChunk[length1, length2];
      this.minActiveChunkX = int.MaxValue;
      this.maxActiveChunkX = int.MinValue;
      this.minActiveChunkY = int.MaxValue;
      this.maxActiveChunkY = int.MinValue;
      this.chunkMeshes = new Mesh[8];
      this.prototypeBuffers = new StaticMeshBuffers(this.prototypeMesh);
      this.MaxChunkCapacity = 8192 / this.prototypeBuffers.VertexCount;
      Bounds bounds = this.prototypeMesh.bounds;
      Vector3 vector3 = -bounds.min;
      Vector3 max = bounds.max;
      this.PrototypeExtents = new Vector3(Mathf.Max(Mathf.Max(vector3.x, vector3.z), Mathf.Max(max.x, max.z)), max.y, vector3.y);
      if (this.bendingCalculation != 0)
      {
        Vector3[] vertices = this.prototypeBuffers.Vertices;
        Color32[] color32Array = this.prototypeBuffers.Colors;
        if (color32Array == null)
        {
          color32Array = new Color32[this.prototypeBuffers.VertexCount];
          this.prototypeBuffers.Colors = color32Array;
        }
        bool flag = this.bendingCalculation == TerrainDetailLayer.BendingCalculation.Vertical;
        float num1 = !flag ? Mathf.Max(this.PrototypeExtents.y, this.PrototypeExtents.x) : this.PrototypeExtents.y;
        for (int index = 0; index < color32Array.Length; ++index)
        {
          Color32 color32 = color32Array[index];
          float num2 = !flag ? vertices[index].magnitude : vertices[index].y;
          color32.a = (byte) ((double) Mathf.Clamp01(num2 / num1) * (double) byte.MaxValue + 0.44999998807907104);
          color32Array[index] = color32;
        }
      }
    }
    if (!this.preloaded && Application.isPlaying && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PreloadGrass)
    {
      this.preloaded = true;
      this.preloading = this.StartCoroutine(this.Preload());
    }
    Vector4 vector = this.material.GetVector("_DrawDistance");
    float drawDistance = vector.x + vector.y;
    Vector3 vector3_1 = camera.transform.position - this.Terrain.transform.position;
    int num3 = Mathf.FloorToInt((vector3_1.x - drawDistance) / this.chunkWorldSize.x);
    int num4 = Mathf.FloorToInt((vector3_1.x + drawDistance) / this.chunkWorldSize.x);
    int num5 = Mathf.FloorToInt((vector3_1.z - drawDistance) / this.chunkWorldSize.y);
    int num6 = Mathf.FloorToInt((vector3_1.z + drawDistance) / this.chunkWorldSize.y);
    int upperBound1 = this.chunks.GetUpperBound(0);
    int upperBound2 = this.chunks.GetUpperBound(1);
    if (num3 < 0)
      num3 = 0;
    if (num4 > upperBound1)
      num4 = upperBound1;
    if (num5 < 0)
      num5 = 0;
    if (num6 > upperBound2)
      num6 = upperBound2;
    if (this.minActiveChunkX < num3)
      num3 = this.minActiveChunkX;
    if (this.maxActiveChunkX > num4)
      num4 = this.maxActiveChunkX;
    if (this.minActiveChunkY < num5)
      num5 = this.minActiveChunkY;
    if (this.maxActiveChunkY > num6)
      num6 = this.maxActiveChunkY;
    this.minActiveChunkX = int.MaxValue;
    this.maxActiveChunkX = int.MinValue;
    this.minActiveChunkY = int.MaxValue;
    this.maxActiveChunkY = int.MinValue;
    for (int chunkX = num3; chunkX <= num4; ++chunkX)
    {
      for (int chunkY = num5; chunkY <= num6; ++chunkY)
      {
        if (chunkX < 0 || chunkX > upperBound1 || chunkY < 0 || chunkY > upperBound2)
        {
          Debug.LogError((object) ("TerrainDetailLayer : Chunk [" + (object) chunkX + ", " + (object) chunkY + "] is out of bounds [" + (object) upperBound1 + ", " + (object) upperBound2 + "]."));
        }
        else
        {
          Vector3 vector3_2 = new Vector3((float) chunkX * this.chunkWorldSize.x - this.PrototypeExtents.x, -this.PrototypeExtents.z, (float) chunkY * this.chunkWorldSize.y - this.PrototypeExtents.x);
          Vector3 vector3_3 = new Vector3((float) (chunkX + 1) * this.chunkWorldSize.x + this.PrototypeExtents.x, this.TerrainData.size.y + this.PrototypeExtents.y, (float) (chunkY + 1) * this.chunkWorldSize.y + this.PrototypeExtents.x);
          Vector3 b = vector3_1;
          if ((double) b.x < (double) vector3_2.x)
            b.x = vector3_2.x;
          if ((double) b.y < (double) vector3_2.y)
            b.y = vector3_2.y;
          if ((double) b.z < (double) vector3_2.z)
            b.z = vector3_2.z;
          if ((double) b.x > (double) vector3_3.x)
            b.x = vector3_3.x;
          if ((double) b.y > (double) vector3_3.y)
            b.y = vector3_3.y;
          if ((double) b.z > (double) vector3_3.z)
            b.z = vector3_3.z;
          float num7 = Vector3.Distance(vector3_1, b);
          bool flag = (double) num7 < (double) drawDistance;
          if (flag)
          {
            if ((Object) this.chunks[chunkX, chunkY] == (Object) null)
            {
              if (TerrainDetailLayer.frame != Time.frameCount)
              {
                TerrainDetailLayer.frame = Time.frameCount;
                TerrainDetailLayer.chunksCreatedThisFrame = 0;
              }
              if ((double) num7 < 3.0 || TerrainDetailLayer.chunksCreatedThisFrame < 1)
              {
                ++TerrainDetailLayer.chunksCreatedThisFrame;
                this.chunks[chunkX, chunkY] = TerrainDetailChunk.Create(this.chunkPrefab, this, chunkX, chunkY);
                flag = this.chunks[chunkX, chunkY].UpdateVisibility(vector3_1, drawDistance);
              }
              else
                flag = false;
            }
            else
              flag = this.chunks[chunkX, chunkY].UpdateVisibility(vector3_1, drawDistance);
          }
          else if ((Object) this.chunks[chunkX, chunkY] != (Object) null)
            flag = this.chunks[chunkX, chunkY].HideAll();
          if (flag)
          {
            if (chunkX < this.minActiveChunkX)
              this.minActiveChunkX = chunkX;
            if (chunkX > this.maxActiveChunkX)
              this.maxActiveChunkX = chunkX;
            if (chunkY < this.minActiveChunkY)
              this.minActiveChunkY = chunkY;
            if (chunkY > this.maxActiveChunkY)
              this.maxActiveChunkY = chunkY;
          }
        }
      }
    }
  }

  public static void PrepareClearList<T>(ref List<T> list)
  {
    if (list == null)
      list = new List<T>();
    else
      list.Clear();
  }

  public enum BendingCalculation : byte
  {
    Off,
    Vertical,
    Spherical,
  }
}
