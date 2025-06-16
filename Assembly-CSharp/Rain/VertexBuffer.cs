using System.Collections.Generic;
using UnityEngine;

namespace Rain
{
  public class VertexBuffer
  {
    public List<Vector3> vertices = new List<Vector3>();
    public List<Color32> colors = new List<Color32>();
    public List<Vector2> uvs = new List<Vector2>();
    public List<Vector3> normals = new List<Vector3>();
    public List<int> triangles = new List<int>();

    public void SetCapacity(int vertexCount, int indexCount)
    {
      if (this.vertices.Capacity < vertexCount)
      {
        this.vertices.Capacity = vertexCount;
        this.colors.Capacity = vertexCount;
        this.uvs.Capacity = vertexCount;
        this.normals.Capacity = vertexCount;
      }
      if (this.triangles.Capacity >= indexCount)
        return;
      this.triangles.Capacity = indexCount;
    }

    public void Clear()
    {
      this.vertices.Clear();
      this.colors.Clear();
      this.uvs.Clear();
      this.normals.Clear();
      this.triangles.Clear();
    }
  }
}
