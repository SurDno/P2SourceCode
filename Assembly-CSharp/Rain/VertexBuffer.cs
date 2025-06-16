using System.Collections.Generic;

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
