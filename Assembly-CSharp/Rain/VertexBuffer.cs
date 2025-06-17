using System.Collections.Generic;
using UnityEngine;

namespace Rain
{
  public class VertexBuffer
  {
    public List<Vector3> vertices = [];
    public List<Color32> colors = [];
    public List<Vector2> uvs = [];
    public List<Vector3> normals = [];
    public List<int> triangles = [];

    public void SetCapacity(int vertexCount, int indexCount)
    {
      if (vertices.Capacity < vertexCount)
      {
        vertices.Capacity = vertexCount;
        colors.Capacity = vertexCount;
        uvs.Capacity = vertexCount;
        normals.Capacity = vertexCount;
      }
      if (triangles.Capacity >= indexCount)
        return;
      triangles.Capacity = indexCount;
    }

    public void Clear()
    {
      vertices.Clear();
      colors.Clear();
      uvs.Clear();
      normals.Clear();
      triangles.Clear();
    }
  }
}
