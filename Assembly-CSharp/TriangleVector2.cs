using System;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using UnityEngine;

public static class TriangleVector2
{
  public static void Triangulate(
    Vector2[] polygon,
    float maxArea,
    out Vector2[] vertices,
    out int[] triangles)
  {
    InputGeometry inputGeometry = new InputGeometry();
    TriangleVector2.AddContour(inputGeometry, polygon, 0);
    TriangleNet.Mesh mesh = new TriangleNet.Mesh(new Behavior()
    {
      Quality = true,
      MaxArea = (double) maxArea,
      VarArea = true,
      Jettison = true
    });
    mesh.Triangulate(inputGeometry);
    Dictionary<int, Vertex> vertices1 = mesh.vertices;
    Dictionary<int, int> dictionary = new Dictionary<int, int>();
    int length = 0;
    foreach (int key in vertices1.Keys)
    {
      dictionary.Add(key, length);
      ++length;
    }
    vertices = new Vector2[length];
    foreach (KeyValuePair<int, Vertex> keyValuePair in vertices1)
    {
      Vertex vertex = keyValuePair.Value;
      vertices[dictionary[keyValuePair.Key]] = new Vector2(Convert.ToSingle(vertex.X), Convert.ToSingle(vertex.Y));
    }
    Dictionary<int, Triangle> triangles1 = mesh.triangles;
    int index1 = 0;
    triangles = new int[triangles1.Count * 3];
    foreach (Triangle triangle in triangles1.Values)
    {
      triangles[index1] = dictionary[triangle.P2];
      int index2 = index1 + 1;
      triangles[index2] = dictionary[triangle.P1];
      int index3 = index2 + 1;
      triangles[index3] = dictionary[triangle.P0];
      index1 = index3 + 1;
    }
  }

  public static void Triangulate(
    Vector2[][] innerContours,
    Vector2[][] outerContours,
    out Vector2[] vertices,
    out bool[] isInner,
    out int[] triangles)
  {
    InputGeometry inputGeometry = new InputGeometry();
    for (int index = 0; index < innerContours.Length; ++index)
      TriangleVector2.AddContour(inputGeometry, innerContours[index], 1);
    int count = inputGeometry.Count;
    if (outerContours != null)
    {
      for (int index = 0; index < outerContours.Length; ++index)
        TriangleVector2.AddContour(inputGeometry, outerContours[index], 0);
    }
    TriangleNet.Mesh mesh = new TriangleNet.Mesh();
    mesh.Triangulate(inputGeometry);
    Dictionary<int, Vertex> vertices1 = mesh.vertices;
    vertices = new Vector2[vertices1.Count];
    isInner = new bool[vertices.Length];
    for (int key = 0; key < vertices.Length; ++key)
    {
      vertices[key] = new Vector2(Convert.ToSingle(vertices1[key].X), Convert.ToSingle(vertices1[key].Y));
      isInner[key] = key < count;
    }
    Dictionary<int, Triangle> triangles1 = mesh.triangles;
    int index1 = 0;
    triangles = new int[triangles1.Count * 3];
    foreach (Triangle triangle in triangles1.Values)
    {
      triangles[index1] = triangle.P2;
      int index2 = index1 + 1;
      triangles[index2] = triangle.P1;
      int index3 = index2 + 1;
      triangles[index3] = triangle.P0;
      index1 = index3 + 1;
    }
  }

  private static void AddContour(InputGeometry inputGeometry, Vector2[] contour, int marker)
  {
    int count = inputGeometry.Count;
    for (int index = 0; index < contour.Length; ++index)
    {
      inputGeometry.AddPoint((double) contour[index].x, (double) contour[index].y, marker);
      inputGeometry.AddSegment(count + index, index == 0 ? count + contour.Length - 1 : count + index - 1, marker);
    }
  }
}
