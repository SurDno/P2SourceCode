using UnityEngine;

public class StaticMeshBuffers(Mesh mesh) {
  public Vector3[] Vertices = mesh.vertices;
  public Color32[] Colors = mesh.colors32;
  public Vector2[] UV = mesh.uv;
  public Vector3[] Normals = mesh.normals;
  public Vector4[] Tangents = mesh.tangents;
  public int[] Triangles = mesh.triangles;

  public int VertexCount => Vertices.Length;

  public int TriangleCount => Triangles.Length;

  public bool HasColors() => !IsNullOrEmpty(Colors);

  public bool HasUV() => !IsNullOrEmpty(UV);

  public bool HasNormals() => !IsNullOrEmpty(Normals);

  public bool HasTangents() => !IsNullOrEmpty(Tangents);

  private bool IsNullOrEmpty<T>(T[] array) => array == null || array.Length == 0;
}
