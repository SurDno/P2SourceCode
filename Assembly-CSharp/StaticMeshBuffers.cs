// Decompiled with JetBrains decompiler
// Type: StaticMeshBuffers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class StaticMeshBuffers
{
  public Vector3[] Vertices;
  public Color32[] Colors;
  public Vector2[] UV;
  public Vector3[] Normals;
  public Vector4[] Tangents;
  public int[] Triangles;

  public int VertexCount => this.Vertices.Length;

  public int TriangleCount => this.Triangles.Length;

  public bool HasColors() => !this.IsNullOrEmpty<Color32>(this.Colors);

  public bool HasUV() => !this.IsNullOrEmpty<Vector2>(this.UV);

  public bool HasNormals() => !this.IsNullOrEmpty<Vector3>(this.Normals);

  public bool HasTangents() => !this.IsNullOrEmpty<Vector4>(this.Tangents);

  private bool IsNullOrEmpty<T>(T[] array) => array == null || array.Length == 0;

  public StaticMeshBuffers(Mesh mesh)
  {
    this.Vertices = mesh.vertices;
    this.Colors = mesh.colors32;
    this.UV = mesh.uv;
    this.Normals = mesh.normals;
    this.Tangents = mesh.tangents;
    this.Triangles = mesh.triangles;
  }
}
