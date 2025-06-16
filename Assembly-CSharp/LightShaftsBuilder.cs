// Decompiled with JetBrains decompiler
// Type: LightShaftsBuilder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
    if (LightShaftsBuilder.Instance == null)
    {
      LightShaftsBuilder.Instance = new LightShaftsBuilder();
      LightShaftsBuilder.Instance.occupants = 1;
    }
    else
      ++LightShaftsBuilder.Instance.occupants;
  }

  public static void Vacate()
  {
    --LightShaftsBuilder.Instance.occupants;
    if (LightShaftsBuilder.Instance.occupants > 0)
      return;
    LightShaftsBuilder.Instance = (LightShaftsBuilder) null;
  }

  public void AddRay(Vector3 origin, float length)
  {
    this.rayOrigins.Add(origin);
    this.rayLengths.Add(length);
  }

  private void AddTriangle(int index0, int index1, int index2)
  {
    this.triangles.Add(index0);
    this.triangles.Add(index1);
    this.triangles.Add(index2);
  }

  private void AddTriangles()
  {
    this.AddTriangle(this.vertices.Count - 1, this.vertices.Count + 1, this.vertices.Count - 2);
    this.AddTriangle(this.vertices.Count + 1, this.vertices.Count, this.vertices.Count - 2);
  }

  private void AddVertex(Vector3 position, Vector2 uv)
  {
    this.vertices.Add(this.w2l.MultiplyPoint(position));
    this.normals.Add(this.w2l.MultiplyVector(LightShafts.LightDirection));
    this.uvs.Add(uv);
  }

  private void AddVertices(Vector3 position, float opacity)
  {
    this.AddVertex(position - this.up, new Vector2(-this.lightShafts.radius, opacity));
    this.AddVertex(position + this.up, new Vector2(this.lightShafts.radius, opacity));
  }

  public void BuildTo(Mesh mesh)
  {
    int num1 = 0;
    int num2 = 0;
    for (int index = 0; index < this.rayLengths.Count; ++index)
    {
      if ((double) this.rayLengths[index] > 0.0)
      {
        if ((double) this.rayLengths[index] == (double) this.lightShafts.length || (double) this.lightShafts.length <= (double) this.lightShafts.fadeIn + (double) this.lightShafts.radius)
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
    this.vertices.Clear();
    if (this.vertices.Capacity < num1)
      this.vertices.Capacity = num1;
    this.normals.Clear();
    if (this.normals.Capacity < num1)
      this.normals.Capacity = num1;
    this.uvs.Clear();
    if (this.uvs.Capacity < num1)
      this.uvs.Capacity = num1;
    this.triangles.Clear();
    if (this.triangles.Capacity < num2)
      this.triangles.Capacity = num2;
    this.w2l = this.lightShafts.transform.worldToLocalMatrix;
    this.up = Vector3.Cross(LightShafts.LightDirection, Vector3.Cross(LightShafts.LightDirection, Vector3.down).normalized).normalized * (1f / 1000f);
    for (int index = 0; index < this.rayLengths.Count; ++index)
    {
      if ((double) this.rayLengths[index] != 0.0)
      {
        this.AddVertices(this.rayOrigins[index], 0.0f);
        if ((double) this.rayLengths[index] == (double) this.lightShafts.length)
        {
          this.AddTriangles();
          this.AddVertices(this.rayOrigins[index] + LightShafts.LightDirection * this.lightShafts.fadeIn, this.opacity);
          this.AddTriangles();
          this.AddVertices(this.rayOrigins[index] + LightShafts.LightDirection * this.rayLengths[index], 0.0f);
        }
        else if ((double) this.rayLengths[index] <= (double) this.lightShafts.fadeIn + (double) this.lightShafts.radius)
        {
          float num3 = (this.rayLengths[index] - this.lightShafts.radius) / this.lightShafts.fadeIn;
          this.AddTriangles();
          this.AddVertices(this.rayOrigins[index] + LightShafts.LightDirection * (this.rayLengths[index] - this.lightShafts.radius), this.opacity * num3);
          this.AddTriangles();
          this.AddVertices(this.rayOrigins[index] + LightShafts.LightDirection * this.rayLengths[index], 0.0f);
        }
        else
        {
          float num4 = (float) (((double) this.rayLengths[index] - (double) this.lightShafts.radius) / ((double) this.lightShafts.length - (double) this.lightShafts.fadeIn));
          this.AddTriangles();
          this.AddVertices(this.rayOrigins[index] + LightShafts.LightDirection * this.lightShafts.fadeIn, this.opacity);
          this.AddTriangles();
          this.AddVertices(this.rayOrigins[index] + LightShafts.LightDirection * (this.rayLengths[index] - this.lightShafts.radius), this.opacity * num4);
          this.AddTriangles();
          this.AddVertices(this.rayOrigins[index] + LightShafts.LightDirection * this.rayLengths[index], 0.0f);
        }
      }
    }
    mesh.Clear();
    mesh.SetVertices(this.vertices);
    mesh.SetNormals(this.normals);
    mesh.SetUVs(0, this.uvs);
    mesh.SetTriangles(this.triangles, 0);
    mesh.RecalculateBounds();
  }

  public void Prepare(LightShafts lightShafts, float opacity, int pointCount)
  {
    this.lightShafts = lightShafts;
    this.opacity = opacity;
    this.rayOrigins.Clear();
    if (this.rayOrigins.Capacity < pointCount)
      this.rayOrigins.Capacity = pointCount;
    this.rayLengths.Clear();
    if (this.rayLengths.Capacity >= pointCount)
      return;
    this.rayLengths.Capacity = pointCount;
  }
}
