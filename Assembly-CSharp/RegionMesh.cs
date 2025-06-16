using System;
using Engine.Common.Components.Regions;
using Inspectors;
using UnityEngine;
using Random = UnityEngine.Random;

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

  public RegionEnum Region => region;

  [Inspected]
  public Vector3 Center => center;

  [Inspected]
  public float Radius => radius;

  [Inspected]
  public RegionMesh[] NearRegions => nearRegions;

  public Vector3[] Triangles => trianglesCache;

  public void Initialise()
  {
    MeshCollider component = GetComponent<MeshCollider>();
    if (component == null)
      return;
    Mesh sharedMesh = component.sharedMesh;
    int[] numArray = !(sharedMesh == null) ? sharedMesh.triangles : throw new Exception();
    int length = numArray.Length / 3;
    if (length == 0)
      throw new Exception();
    trianglesCache = new Vector3[numArray.Length];
    areaCache = new float[length];
    Vector3[] vertices = sharedMesh.vertices;
    float num = 0.0f;
    for (int index = 0; index < length; ++index)
    {
      Vector3 vector3_1 = vertices[numArray[index * 3]];
      Vector3 vector3_2 = vertices[numArray[index * 3 + 1]];
      Vector3 vector3_3 = vertices[numArray[index * 3 + 2]];
      num += Vector3.Cross(vector3_3 - vector3_1, vector3_2 - vector3_1).magnitude;
      areaCache[index] = num;
      trianglesCache[index * 3] = vector3_1;
      trianglesCache[index * 3 + 1] = vector3_2;
      trianglesCache[index * 3 + 2] = vector3_3;
    }
    for (int index = 0; index < length; ++index)
      areaCache[index] = areaCache[index] / num;
    areaCache[areaCache.Length - 1] = 1.0001f;
  }

  public Vector3 GetRandomPoint()
  {
    float num1 = Random.value;
    int index = 0;
    while (num1 > (double) areaCache[index])
      ++index;
    Vector3 vector3_1 = trianglesCache[index * 3];
    Vector3 vector3_2 = trianglesCache[index * 3 + 1];
    Vector3 vector3_3 = trianglesCache[index * 3 + 2];
    float num2 = Random.value;
    float num3 = Random.value;
    Vector3 worldPosition;
    if (num2 + (double) num3 < 1.0)
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
