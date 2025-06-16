// Decompiled with JetBrains decompiler
// Type: Rain.PuddleCutter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Rain
{
  public class PuddleCutter : MonoBehaviour
  {
    private const string DebugPrefix = "Puddle Cutter : ";
    [Header("Source")]
    public MeshFilter SourceMeshFilter;
    public PuddleCutter.UVSet SourceUVSet = PuddleCutter.UVSet.UV0;
    [Header("Settings")]
    public Vector3 Size = new Vector3(5f, 1f, 5f);
    public Color ColorMask = Color.white;
    public float Elevation = 0.01f;
    [Range(0.0f, 1f)]
    public float MinYNormal = 0.5f;

    private void AddTriangle(
      List<Vector3> outputVertices,
      List<Vector2> outputPrimaryUV,
      List<Vector2> outputSecondaryUV,
      List<Vector3> outputNormals,
      List<int> outputIndices,
      Vector3[] triangleVertices,
      Vector2[] triangleUV,
      Vector3[] triangleNormals)
    {
      int count = outputVertices.Count;
      outputIndices.Add(count);
      int num1 = count + 1;
      outputIndices.Add(num1);
      int num2 = num1 + 1;
      outputIndices.Add(num2);
      for (int index = 0; index < 3; ++index)
      {
        outputVertices.Add(triangleVertices[index]);
        outputPrimaryUV.Add(new Vector2((float) ((double) triangleVertices[index].x / (double) this.Size.x + 0.5), (float) ((double) triangleVertices[index].z / (double) this.Size.z + 0.5)));
      }
      if (outputSecondaryUV != null)
      {
        for (int index = 0; index < 3; ++index)
          outputSecondaryUV.Add(triangleUV[index]);
      }
      if (outputNormals == null)
        return;
      for (int index = 0; index < 3; ++index)
        outputNormals.Add(triangleNormals[index]);
    }

    public void Build()
    {
      if ((double) this.Size.x * 0.5 <= 0.0 || (double) this.Size.y * 0.5 <= 0.0 || (double) this.Size.z * 0.5 <= 0.0 || (UnityEngine.Object) this.SourceMeshFilter == (UnityEngine.Object) null)
        return;
      Mesh sharedMesh = this.SourceMeshFilter.sharedMesh;
      if ((UnityEngine.Object) sharedMesh == (UnityEngine.Object) null)
        return;
      if (!sharedMesh.isReadable)
      {
        Debug.LogWarning((object) ("Puddle Cutter : Mesh \"" + sharedMesh.name + "\" is not readable."));
      }
      else
      {
        Vector3[] vertices = sharedMesh.vertices;
        int[] triangles = sharedMesh.triangles;
        Vector3[] normals = sharedMesh.normals;
        if (vertices == null || vertices.Length == 0 || triangles == null || triangles.Length == 0)
          return;
        Matrix4x4 matrix4x4 = this.transform.worldToLocalMatrix * this.SourceMeshFilter.transform.localToWorldMatrix;
        for (int index = 0; index < vertices.Length; ++index)
        {
          vertices[index] = matrix4x4.MultiplyPoint(vertices[index]);
          vertices[index].y += this.Elevation;
          if (normals != null)
            normals[index] = matrix4x4.MultiplyVector(normals[index]);
        }
        Vector2[] vector2Array = (Vector2[]) null;
        switch (this.SourceUVSet)
        {
          case PuddleCutter.UVSet.UV0:
            vector2Array = sharedMesh.uv;
            break;
          case PuddleCutter.UVSet.UV1:
            vector2Array = sharedMesh.uv2;
            break;
          case PuddleCutter.UVSet.UV2:
            vector2Array = sharedMesh.uv3;
            break;
          case PuddleCutter.UVSet.UV3:
            vector2Array = sharedMesh.uv4;
            break;
        }
        if (this.SourceUVSet != PuddleCutter.UVSet.None && vector2Array == null)
          Debug.LogWarning((object) ("Puddle Cutter : UV set of mesh \"" + sharedMesh.name + "\" is empty. None will be used."));
        List<Vector3> outputVertices = new List<Vector3>();
        List<Vector2> outputPrimaryUV = new List<Vector2>();
        List<Vector2> outputSecondaryUV = vector2Array == null ? (List<Vector2>) null : new List<Vector2>();
        List<Vector3> outputNormals = normals == null ? (List<Vector3>) null : new List<Vector3>();
        List<int> outputIndices = new List<int>();
        List<Vector3> polygonVertices = new List<Vector3>(3);
        List<Vector2> polygonUV = vector2Array == null ? (List<Vector2>) null : new List<Vector2>(3);
        List<Vector3> polygonNormals = normals == null ? (List<Vector3>) null : new List<Vector3>(3);
        for (int index = 0; index < triangles.Length; index += 3)
        {
          polygonVertices.Clear();
          polygonVertices.Add(vertices[triangles[index]]);
          polygonVertices.Add(vertices[triangles[index + 1]]);
          polygonVertices.Add(vertices[triangles[index + 2]]);
          if (vector2Array != null)
          {
            polygonUV.Clear();
            polygonUV.Add(vector2Array[triangles[index]]);
            polygonUV.Add(vector2Array[triangles[index + 1]]);
            polygonUV.Add(vector2Array[triangles[index + 2]]);
          }
          if (normals != null)
          {
            polygonNormals.Clear();
            polygonNormals.Add(normals[triangles[index]]);
            polygonNormals.Add(normals[triangles[index + 1]]);
            polygonNormals.Add(normals[triangles[index + 2]]);
          }
          this.ProcessTriangle(outputVertices, outputPrimaryUV, outputSecondaryUV, outputNormals, outputIndices, polygonVertices, polygonUV, polygonNormals);
        }
        Mesh mesh = new Mesh();
        mesh.SetVertices(outputVertices);
        mesh.SetTriangles(outputIndices, 0, true);
        mesh.SetUVs(0, outputPrimaryUV);
        if (outputSecondaryUV != null)
          mesh.SetUVs(1, outputSecondaryUV);
        if (outputNormals != null)
        {
          mesh.SetNormals(outputNormals);
          mesh.RecalculateTangents();
        }
        if (this.ColorMask != Color.white)
        {
          Color[] colorArray = new Color[outputVertices.Count];
          for (int index = 0; index < colorArray.Length; ++index)
            colorArray[index] = this.ColorMask;
          mesh.colors = colorArray;
        }
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        if ((UnityEngine.Object) meshFilter == (UnityEngine.Object) null)
          meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        MeshCollider component = this.GetComponent<MeshCollider>();
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
          return;
        component.sharedMesh = mesh;
      }
    }

    private void CutPolygonPositiveSides(
      List<Vector3> polygonVertices,
      List<Vector2> polygonUV,
      List<Vector3> polygonNormals,
      Vector3 extents)
    {
      this.CutPolygonSide(polygonVertices, polygonUV, polygonNormals, 0, extents.x);
      if (polygonVertices.Count == 0)
        return;
      this.CutPolygonSide(polygonVertices, polygonUV, polygonNormals, 2, extents.z);
      if (polygonVertices.Count != 0)
        ;
    }

    private void CutPolygonSide(
      List<Vector3> polygonVertices,
      List<Vector2> polygonUV,
      List<Vector3> polygonNormals,
      int axisIndex,
      float extent)
    {
      float num1 = polygonVertices[0][axisIndex];
      float num2 = polygonVertices[0][axisIndex];
      Vector3 polygonVertex;
      for (int index = 1; index < polygonVertices.Count; ++index)
      {
        polygonVertex = polygonVertices[index];
        if ((double) polygonVertex[axisIndex] < (double) num1)
        {
          polygonVertex = polygonVertices[index];
          num1 = polygonVertex[axisIndex];
        }
        polygonVertex = polygonVertices[index];
        if ((double) polygonVertex[axisIndex] > (double) num2)
        {
          polygonVertex = polygonVertices[index];
          num2 = polygonVertex[axisIndex];
        }
      }
      if ((double) num2 <= (double) extent)
        return;
      if ((double) num1 >= (double) extent)
      {
        polygonVertices.Clear();
        polygonUV?.Clear();
        polygonNormals?.Clear();
      }
      else
      {
        for (int index1 = 0; index1 < polygonVertices.Count; ++index1)
        {
          polygonVertex = polygonVertices[index1];
          float f1 = polygonVertex[axisIndex] - extent;
          int index2 = index1 + 1;
          if (index2 == polygonVertices.Count)
            index2 = 0;
          polygonVertex = polygonVertices[index2];
          float f2 = polygonVertex[axisIndex] - extent;
          if ((double) Mathf.Sign(f1 * f2) < 0.0)
          {
            float num3 = Mathf.Abs(f1);
            float num4 = Mathf.Abs(f2);
            float num5 = num3 + num4;
            Vector3 vector3_1 = (polygonVertices[index1] * num4 + polygonVertices[index2] * num3) / num5;
            vector3_1[axisIndex] = extent;
            polygonVertices.Insert(index1 + 1, vector3_1);
            if (polygonUV != null)
            {
              Vector2 vector2 = (polygonUV[index1] * num4 + polygonUV[index2] * num3) / num5;
              polygonUV.Insert(index1 + 1, vector2);
            }
            if (polygonNormals != null)
            {
              Vector3 vector3_2 = (polygonNormals[index1] * num4 + polygonNormals[index2] * num3) / num5;
              polygonNormals.Insert(index1 + 1, vector3_2);
            }
            ++index1;
          }
        }
        for (int index = 0; index < polygonVertices.Count; ++index)
        {
          polygonVertex = polygonVertices[index];
          if ((double) polygonVertex[axisIndex] > (double) extent)
          {
            polygonVertices.RemoveAt(index);
            polygonUV?.RemoveAt(index);
            polygonNormals?.RemoveAt(index);
            --index;
          }
        }
      }
    }

    private void FlipPolygon(List<Vector3> polygonVertices)
    {
      for (int index = 0; index < polygonVertices.Count; ++index)
        polygonVertices[index] = -polygonVertices[index];
    }

    private void OnDrawGizmosSelected()
    {
      Vector3 vector3_1 = this.Size * 0.5f;
      Vector3 point1 = new Vector3(-vector3_1.x, -vector3_1.y, -vector3_1.z);
      Vector3 point2 = new Vector3(-vector3_1.x, -vector3_1.y, vector3_1.z);
      Vector3 point3 = new Vector3(-vector3_1.x, vector3_1.y, vector3_1.z);
      Vector3 point4 = new Vector3(-vector3_1.x, vector3_1.y, -vector3_1.z);
      Vector3 point5 = new Vector3(vector3_1.x, -vector3_1.y, -vector3_1.z);
      Vector3 point6 = new Vector3(vector3_1.x, -vector3_1.y, vector3_1.z);
      Vector3 point7 = new Vector3(vector3_1.x, vector3_1.y, vector3_1.z);
      Vector3 point8 = new Vector3(vector3_1.x, vector3_1.y, -vector3_1.z);
      Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
      Vector3 vector3_2 = localToWorldMatrix.MultiplyPoint3x4(point1);
      Vector3 vector3_3 = localToWorldMatrix.MultiplyPoint3x4(point2);
      Vector3 vector3_4 = localToWorldMatrix.MultiplyPoint3x4(point3);
      Vector3 vector3_5 = localToWorldMatrix.MultiplyPoint3x4(point4);
      Vector3 vector3_6 = localToWorldMatrix.MultiplyPoint3x4(point5);
      Vector3 vector3_7 = localToWorldMatrix.MultiplyPoint3x4(point6);
      Vector3 vector3_8 = localToWorldMatrix.MultiplyPoint3x4(point7);
      Vector3 vector3_9 = localToWorldMatrix.MultiplyPoint3x4(point8);
      Gizmos.color = new Color(0.0f, 0.5f, 1f, 0.75f);
      Gizmos.DrawLine(vector3_2, vector3_3);
      Gizmos.DrawLine(vector3_3, vector3_4);
      Gizmos.DrawLine(vector3_4, vector3_5);
      Gizmos.DrawLine(vector3_5, vector3_2);
      Gizmos.DrawLine(vector3_6, vector3_7);
      Gizmos.DrawLine(vector3_7, vector3_8);
      Gizmos.DrawLine(vector3_8, vector3_9);
      Gizmos.DrawLine(vector3_9, vector3_6);
      Gizmos.DrawLine(vector3_2, vector3_6);
      Gizmos.DrawLine(vector3_3, vector3_7);
      Gizmos.DrawLine(vector3_4, vector3_8);
      Gizmos.DrawLine(vector3_5, vector3_9);
    }

    private void OnValidate()
    {
      if (this.hideFlags == HideFlags.DontSaveInBuild)
        this.hideFlags = HideFlags.None;
      if ((double) this.Size.x < 0.0)
        this.Size.x = 0.0f;
      if ((double) this.Size.y < 0.0)
        this.Size.y = 0.0f;
      if ((double) this.Size.z >= 0.0)
        return;
      this.Size.z = 0.0f;
    }

    private void ProcessTriangle(
      List<Vector3> outputVertices,
      List<Vector2> outputPrimaryUV,
      List<Vector2> outputSecondaryUV,
      List<Vector3> outputNormals,
      List<int> outputIndices,
      List<Vector3> polygonVertices,
      List<Vector2> polygonUV,
      List<Vector3> polygonNormals)
    {
      Vector3 vector3_1 = Vector3.Cross(polygonVertices[1] - polygonVertices[0], polygonVertices[2] - polygonVertices[0]);
      vector3_1.Normalize();
      if ((double) vector3_1.y < (double) this.MinYNormal)
        return;
      Vector3 vector3_2 = new Vector3(polygonVertices[0].x, polygonVertices[0].y, polygonVertices[0].z);
      Vector3 vector3_3 = vector3_2;
      for (int index1 = 1; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < 3; ++index2)
        {
          if ((double) polygonVertices[index1][index2] < (double) vector3_2[index2])
            vector3_2[index2] = polygonVertices[index1][index2];
          if ((double) polygonVertices[index1][index2] > (double) vector3_3[index2])
            vector3_3[index2] = polygonVertices[index1][index2];
        }
      }
      if (!new Bounds((vector3_2 + vector3_3) * 0.5f, vector3_3 - vector3_2).Intersects(new Bounds(Vector3.zero, this.Size)))
        return;
      Vector3 extents = this.Size * 0.5f;
      this.CutPolygonPositiveSides(polygonVertices, polygonUV, polygonNormals, extents);
      if (polygonVertices.Count == 0)
        return;
      this.FlipPolygon(polygonVertices);
      this.CutPolygonPositiveSides(polygonVertices, polygonUV, polygonNormals, extents);
      if (polygonVertices.Count == 0)
        return;
      this.FlipPolygon(polygonVertices);
      Vector3[] triangleVertices = new Vector3[3];
      Vector2[] triangleUV = outputSecondaryUV == null ? (Vector2[]) null : new Vector2[3];
      Vector3[] triangleNormals = new Vector3[3];
      while (polygonVertices.Count > 3)
      {
        int index3 = polygonVertices.Count - 1;
        int index4 = 0;
        int index5 = 1;
        float num1 = Vector3.Distance(polygonVertices[1], polygonVertices[polygonVertices.Count - 1]);
        for (int index6 = 1; index6 < polygonVertices.Count; ++index6)
        {
          int index7 = index6 - 1;
          int index8 = index6 + 1;
          if (index8 == polygonVertices.Count)
            index8 = 0;
          float num2 = Vector3.Distance(polygonVertices[index7], polygonVertices[index8]);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            index3 = index7;
            index4 = index6;
            index5 = index8;
          }
        }
        triangleVertices[0] = polygonVertices[index3];
        triangleVertices[1] = polygonVertices[index4];
        triangleVertices[2] = polygonVertices[index5];
        polygonVertices.RemoveAt(index4);
        if (outputSecondaryUV != null)
        {
          triangleUV[0] = polygonUV[index3];
          triangleUV[1] = polygonUV[index4];
          triangleUV[2] = polygonUV[index5];
          polygonUV.RemoveAt(index4);
        }
        if (outputNormals != null)
        {
          triangleNormals[0] = polygonNormals[index3];
          triangleNormals[1] = polygonNormals[index4];
          triangleNormals[2] = polygonNormals[index5];
          polygonNormals.RemoveAt(index4);
        }
        this.AddTriangle(outputVertices, outputPrimaryUV, outputSecondaryUV, outputNormals, outputIndices, triangleVertices, triangleUV, triangleNormals);
      }
      if (polygonVertices.Count != 3)
        return;
      triangleVertices[0] = polygonVertices[0];
      triangleVertices[1] = polygonVertices[1];
      triangleVertices[2] = polygonVertices[2];
      if (outputSecondaryUV != null)
      {
        triangleUV[0] = polygonUV[0];
        triangleUV[1] = polygonUV[1];
        triangleUV[2] = polygonUV[2];
      }
      if (outputNormals != null)
      {
        triangleNormals[0] = polygonNormals[0];
        triangleNormals[1] = polygonNormals[1];
        triangleNormals[2] = polygonNormals[2];
      }
      this.AddTriangle(outputVertices, outputPrimaryUV, outputSecondaryUV, outputNormals, outputIndices, triangleVertices, triangleUV, triangleNormals);
    }

    [Serializable]
    public enum UVSet : byte
    {
      UV0 = 0,
      UV1 = 1,
      UV2 = 2,
      UV3 = 3,
      None = 255, // 0xFF
    }
  }
}
