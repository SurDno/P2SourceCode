using Engine.Common.Components.Regions;
using Inspectors;
using System;
using UnityEngine;

public class RegionMesh : MonoBehaviour
{
  [SerializeField]
  [HideInInspector]
  private Vector3 center;
  [SerializeField]
  [HideInInspector]
  private float radius;
  [SerializeField]
  private RegionEnum region;
  [SerializeField]
  private RegionMesh[] nearRegions;
  private Vector3[] trianglesCache = new Vector3[0];
  private float[] areaCache = new float[0];

  public RegionEnum Region => this.region;

  [Inspected]
  public Vector3 Center => this.center;

  [Inspected]
  public float Radius => this.radius;

  [Inspected]
  public RegionMesh[] NearRegions => this.nearRegions;

  public Vector3[] Triangles => this.trianglesCache;

  public void Initialise()
  {
    MeshCollider component = this.GetComponent<MeshCollider>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    Mesh sharedMesh = component.sharedMesh;
    int[] numArray = !((UnityEngine.Object) sharedMesh == (UnityEngine.Object) null) ? sharedMesh.triangles : throw new Exception();
    int length = numArray.Length / 3;
    if (length == 0)
      throw new Exception();
    this.trianglesCache = new Vector3[numArray.Length];
    this.areaCache = new float[length];
    Vector3[] vertices = sharedMesh.vertices;
    float num = 0.0f;
    for (int index = 0; index < length; ++index)
    {
      Vector3 vector3_1 = vertices[numArray[index * 3]];
      Vector3 vector3_2 = vertices[numArray[index * 3 + 1]];
      Vector3 vector3_3 = vertices[numArray[index * 3 + 2]];
      num += Vector3.Cross(vector3_3 - vector3_1, vector3_2 - vector3_1).magnitude;
      this.areaCache[index] = num;
      this.trianglesCache[index * 3] = vector3_1;
      this.trianglesCache[index * 3 + 1] = vector3_2;
      this.trianglesCache[index * 3 + 2] = vector3_3;
    }
    for (int index = 0; index < length; ++index)
      this.areaCache[index] = this.areaCache[index] / num;
    this.areaCache[this.areaCache.Length - 1] = 1.0001f;
  }

  public Vector3 GetRandomPoint()
  {
    float num1 = UnityEngine.Random.value;
    int index = 0;
    while ((double) num1 > (double) this.areaCache[index])
      ++index;
    Vector3 vector3_1 = this.trianglesCache[index * 3];
    Vector3 vector3_2 = this.trianglesCache[index * 3 + 1];
    Vector3 vector3_3 = this.trianglesCache[index * 3 + 2];
    float num2 = UnityEngine.Random.value;
    float num3 = UnityEngine.Random.value;
    Vector3 worldPosition;
    if ((double) num2 + (double) num3 < 1.0)
    {
      worldPosition = vector3_1 + (vector3_2 - vector3_1) * num2 + (vector3_3 - vector3_1) * num3;
    }
    else
    {
      Vector3 vector3_4 = vector3_1 + (vector3_2 - vector3_1) * 1f + (vector3_3 - vector3_1) * 1f;
      worldPosition = vector3_4 + (vector3_2 - vector3_4) * num2 + (vector3_3 - vector3_4) * num3;
    }
    worldPosition.y = Terrain.activeTerrain.SampleHeight(worldPosition);
    return worldPosition;
  }
}
