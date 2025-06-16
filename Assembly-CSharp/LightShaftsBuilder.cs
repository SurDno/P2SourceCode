using System.Collections.Generic;
using UnityEngine;

public class LightShaftsBuilder
{
  private LightShafts lightShafts;
  private List<Vector3> normals = new List<Vector3>();
  private int occupants;
  private float opacity;
  private List<float> rayLengths = new List<float>();
  private List<Vector3> rayOrigins = new List<Vector3>();
  private List<int> triangles = new List<int>();
  private Vector3 up;
  private List<Vector2> uvs = new List<Vector2>();
  private List<Vector3> vertices = new List<Vector3>();
  private Matrix4x4 w2l;

  public static LightShaftsBuilder Instance { get; private set; }

  public static void Occupy()
  {
    if (Instance == null)
    {
      Instance = new LightShaftsBuilder();
      Instance.occupants = 1;
    }
    else
      ++Instance.occupants;
  }

  public static void Vacate()
  {
    --Instance.occupants;
    if (Instance.occupants > 0)
      return;
    Instance = null;
  }

  public void AddRay(Vector3 origin, float length)
  {
    rayOrigins.Add(origin);
    rayLengths.Add(length);
  }

  private void AddTriangle(int index0, int index1, int index2)
  {
    triangles.Add(index0);
    triangles.Add(index1);
    triangles.Add(index2);
  }

  private void AddTriangles()
  {
    AddTriangle(vertices.Count - 1, vertices.Count + 1, vertices.Count - 2);
    AddTriangle(vertices.Count + 1, vertices.Count, vertices.Count - 2);
  }

  private void AddVertex(Vector3 position, Vector2 uv)
  {
    vertices.Add(w2l.MultiplyPoint(position));
    normals.Add(w2l.MultiplyVector(LightShafts.LightDirection));
    uvs.Add(uv);
  }

  private void AddVertices(Vector3 position, float opacity)
  {
    AddVertex(position - up, new Vector2(-lightShafts.radius, opacity));
    AddVertex(position + up, new Vector2(lightShafts.radius, opacity));
  }

  public void BuildTo(Mesh mesh)
  {
    int num1 = 0;
    int num2 = 0;
    for (int index = 0; index < rayLengths.Count; ++index)
    {
      if (rayLengths[index] > 0.0)
      {
        if (rayLengths[index] == (double) lightShafts.length || lightShafts.length <= lightShafts.fadeIn + (double) lightShafts.radius)
        {
          num1 += 6;
          num2 += 12;
        }
        else
        {
          num1 += 8;
          num2 += 18;
        }
      }
    }
    vertices.Clear();
    if (vertices.Capacity < num1)
      vertices.Capacity = num1;
    normals.Clear();
    if (normals.Capacity < num1)
      normals.Capacity = num1;
    uvs.Clear();
    if (uvs.Capacity < num1)
      uvs.Capacity = num1;
    triangles.Clear();
    if (triangles.Capacity < num2)
      triangles.Capacity = num2;
    w2l = lightShafts.transform.worldToLocalMatrix;
    up = Vector3.Cross(LightShafts.LightDirection, Vector3.Cross(LightShafts.LightDirection, Vector3.down).normalized).normalized * (1f / 1000f);
    for (int index = 0; index < rayLengths.Count; ++index)
    {
      if (rayLengths[index] != 0.0)
      {
        AddVertices(rayOrigins[index], 0.0f);
        if (rayLengths[index] == (double) lightShafts.length)
        {
          AddTriangles();
          AddVertices(rayOrigins[index] + LightShafts.LightDirection * lightShafts.fadeIn, opacity);
          AddTriangles();
          AddVertices(rayOrigins[index] + LightShafts.LightDirection * rayLengths[index], 0.0f);
        }
        else if (rayLengths[index] <= lightShafts.fadeIn + (double) lightShafts.radius)
        {
          float num3 = (rayLengths[index] - lightShafts.radius) / lightShafts.fadeIn;
          AddTriangles();
          AddVertices(rayOrigins[index] + LightShafts.LightDirection * (rayLengths[index] - lightShafts.radius), opacity * num3);
          AddTriangles();
          AddVertices(rayOrigins[index] + LightShafts.LightDirection * rayLengths[index], 0.0f);
        }
        else
        {
          float num4 = (float) ((rayLengths[index] - (double) lightShafts.radius) / (lightShafts.length - (double) lightShafts.fadeIn));
          AddTriangles();
          AddVertices(rayOrigins[index] + LightShafts.LightDirection * lightShafts.fadeIn, opacity);
          AddTriangles();
          AddVertices(rayOrigins[index] + LightShafts.LightDirection * (rayLengths[index] - lightShafts.radius), opacity * num4);
          AddTriangles();
          AddVertices(rayOrigins[index] + LightShafts.LightDirection * rayLengths[index], 0.0f);
        }
      }
    }
    mesh.Clear();
    mesh.SetVertices(vertices);
    mesh.SetNormals(normals);
    mesh.SetUVs(0, uvs);
    mesh.SetTriangles(triangles, 0);
    mesh.RecalculateBounds();
  }

  public void Prepare(LightShafts lightShafts, float opacity, int pointCount)
  {
    this.lightShafts = lightShafts;
    this.opacity = opacity;
    rayOrigins.Clear();
    if (rayOrigins.Capacity < pointCount)
      rayOrigins.Capacity = pointCount;
    rayLengths.Clear();
    if (rayLengths.Capacity >= pointCount)
      return;
    rayLengths.Capacity = pointCount;
  }
}
