using UnityEngine;

public class StaticMeshBuffers
{
  public Vector3[] Vertices;
  public Color32[] Colors;
  public Vector2[] UV;
  public Vector3[] Normals;
  public Vector4[] Tangents;
  public int[] Triangles;

  public int VertexCount => Vertices.Length;

  public int TriangleCount => Triangles.Length;

  public bool HasColors() => !IsNullOrEmpty(Colors);

  public bool HasUV() => !IsNullOrEmpty(UV);

  public bool HasNormals() => !IsNullOrEmpty(Normals);

  public bool HasTangents() => !IsNullOrEmpty(Tangents);

  private bool IsNullOrEmpty<T>(T[] array) => array == null || array.Length == 0;

  public StaticMeshBuffers(Mesh mesh)
  {
    Vertices = mesh.vertices;
    Colors = mesh.colors32;
    UV = mesh.uv;
    Normals = mesh.normals;
    Tangents = mesh.tangents;
    Triangles = mesh.triangles;
  }
}
