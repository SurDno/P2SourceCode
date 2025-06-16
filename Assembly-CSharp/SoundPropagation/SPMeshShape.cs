using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundPropagation
{
  public class SPMeshShape : Shape
  {
    public Vector3[] Vertices;
    public IndexTuple[] Triangles;
    private Matrix4x4[] world2planesMatrices;
    private Matrix4x4[] planes2worldMatrices;

    private Vector3 ClampToTriangle(Vector3 point)
    {
      if (point.x <= 0.0)
      {
        point.x = 0.0f;
        if (point.y <= 0.0)
          point.y = 0.0f;
        else if (point.y > 1.0)
          point.y = 1f;
      }
      else if (point.y <= 0.0)
      {
        point.y = 0.0f;
        if (point.x > 1.0)
          point.x = 1f;
      }
      else if (point.y - 1.0 >= point.x)
      {
        point.y = 1f;
        point.x = 0.0f;
      }
      else if (point.x - 1.0 >= point.y)
      {
        point.y = 0.0f;
        point.x = 1f;
      }
      else
      {
        float num1 = point.x + point.y;
        if (num1 > 1.0)
        {
          float num2 = (float) ((num1 - 1.0) * 0.5);
          point.x -= num2;
          point.y -= num2;
        }
      }
      return point;
    }

    protected override bool ClosestPointToSegmentInternal(
      Vector3 pointA,
      Vector3 pointB,
      out Vector3 output)
    {
      if (world2planesMatrices.Length == 0)
      {
        output = pointA;
        return false;
      }
      if (world2planesMatrices.Length == 1)
      {
        Vector3 triangle = ClampToTriangle(ClosestToSegmentOnPlane(world2planesMatrices[0], pointA, pointB));
        output = planes2worldMatrices[0].MultiplyPoint3x4(triangle);
        return true;
      }
      float num1 = float.MaxValue;
      Vector3 vector3_1 = pointA;
      for (int index = 0; index < world2planesMatrices.Length; ++index)
      {
        Vector3 triangle = ClampToTriangle(ClosestToSegmentOnPlane(world2planesMatrices[index], pointA, pointB));
        Vector3 vector3_2 = planes2worldMatrices[index].MultiplyPoint3x4(triangle);
        float num2 = Vector3.Distance(pointA, vector3_2) + Vector3.Distance(vector3_2, pointB);
        if (index == 0 || num2 < (double) num1)
        {
          vector3_1 = vector3_2;
          num1 = num2;
        }
      }
      output = vector3_1;
      return true;
    }

    private Vector3 GetVertex(int index)
    {
      if (index < 0)
        index = 0;
      else if (index >= Vertices.Length)
        index = Vertices.Length - 1;
      return Vertices[index];
    }

    public void ImportMesh(Mesh mesh, float weldThreshold)
    {
      int[] triangles = mesh.triangles;
      Vector3[] vertices = mesh.vertices;
      List<IndexTuple> indexTupleList = new List<IndexTuple>(triangles.Length / 3);
      List<Vector3> list = new List<Vector3>(vertices.Length);
      int index1;
      for (int index2 = 0; index2 < triangles.Length; index2 = index1 + 1)
      {
        Vector3 vertex1 = vertices[triangles[index2]];
        int index3 = index2 + 1;
        Vector3 vertex2 = vertices[triangles[index3]];
        index1 = index3 + 1;
        Vector3 vertex3 = vertices[triangles[index1]];
        IndexTuple indexTuple = new IndexTuple();
        indexTuple.A = AddVertex(list, vertex1, weldThreshold);
        indexTuple.B = AddVertex(list, vertex2, weldThreshold);
        indexTuple.C = AddVertex(list, vertex3, weldThreshold);
        if (indexTuple.A != indexTuple.B && indexTuple.B != indexTuple.C && indexTuple.C != indexTuple.A)
          indexTupleList.Add(indexTuple);
      }
      Triangles = indexTupleList.ToArray();
      Vertices = list.ToArray();
    }

    protected override void Initialize()
    {
      if (Vertices.Length == 0)
      {
        world2planesMatrices = new Matrix4x4[0];
        planes2worldMatrices = new Matrix4x4[0];
      }
      else
      {
        Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
        for (int index = 0; index < Vertices.Length; ++index)
          Vertices[index] = localToWorldMatrix.MultiplyPoint3x4(Vertices[index]);
        planes2worldMatrices = new Matrix4x4[Triangles.Length];
        world2planesMatrices = new Matrix4x4[Triangles.Length];
        for (int index = 0; index < Triangles.Length; ++index)
        {
          Vector3 vertex1 = GetVertex(Triangles[index].A);
          Vector3 vertex2 = GetVertex(Triangles[index].B);
          Vector3 vertex3 = GetVertex(Triangles[index].C);
          Vector3 vector3_1 = vertex2 - vertex1;
          Vector3 vector3_2 = vertex3 - vertex2;
          Vector3 vector3_3 = vertex1 - vertex3;
          float sqrMagnitude1 = vector3_1.sqrMagnitude;
          float sqrMagnitude2 = vector3_2.sqrMagnitude;
          float sqrMagnitude3 = vector3_3.sqrMagnitude;
          Vector3 vector3_4;
          Vector3 lhs;
          Vector3 rhs;
          if (sqrMagnitude1 >= (double) sqrMagnitude2)
          {
            if (sqrMagnitude1 >= (double) sqrMagnitude3)
            {
              vector3_4 = vertex3;
              lhs = vector3_3;
              rhs = -vector3_2;
            }
            else
            {
              vector3_4 = vertex2;
              lhs = vector3_2;
              rhs = -vector3_1;
            }
          }
          else if (sqrMagnitude2 >= (double) sqrMagnitude3)
          {
            vector3_4 = vertex1;
            lhs = vector3_1;
            rhs = -vector3_3;
          }
          else
          {
            vector3_4 = vertex2;
            lhs = vector3_2;
            rhs = -vector3_1;
          }
          Vector3 vector3_5 = Vector3.Cross(lhs, rhs);
          Matrix4x4 matrix4x4 = new Matrix4x4();
          matrix4x4.m00 = lhs.x;
          matrix4x4.m10 = lhs.y;
          matrix4x4.m20 = lhs.z;
          matrix4x4.m01 = rhs.x;
          matrix4x4.m11 = rhs.y;
          matrix4x4.m21 = rhs.z;
          matrix4x4.m02 = vector3_5.x;
          matrix4x4.m12 = vector3_5.y;
          matrix4x4.m22 = vector3_5.z;
          matrix4x4.m03 = vector3_4.x;
          matrix4x4.m13 = vector3_4.y;
          matrix4x4.m23 = vector3_4.z;
          matrix4x4.m33 = 1f;
          planes2worldMatrices[index] = matrix4x4;
          world2planesMatrices[index] = matrix4x4.inverse;
        }
      }
    }

    private void OnDrawGizmosSelected()
    {
      if (Vertices == null || Vertices.Length == 0)
        return;
      Gizmos.color = gizmoColor;
      if (planes2worldMatrices != null && planes2worldMatrices.Length != 0)
      {
        for (int index = 0; index < planes2worldMatrices.Length; ++index)
        {
          Matrix4x4 planes2worldMatrix = planes2worldMatrices[index];
          Vector3 vector3_1 = planes2worldMatrix.MultiplyPoint3x4(new Vector3(0.0f, 0.0f));
          Vector3 vector3_2 = planes2worldMatrix.MultiplyPoint3x4(new Vector3(0.0f, 1f));
          Vector3 vector3_3 = planes2worldMatrix.MultiplyPoint3x4(new Vector3(1f, 0.0f));
          Gizmos.DrawLine(vector3_1, vector3_2);
          Gizmos.DrawLine(vector3_2, vector3_3);
          Gizmos.DrawLine(vector3_3, vector3_1);
        }
      }
      else
      {
        if (Triangles == null || Triangles.Length == 0)
          return;
        Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
        for (int index = 0; index < Triangles.Length; ++index)
        {
          Vector3 vector3_4 = localToWorldMatrix.MultiplyPoint3x4(GetVertex(Triangles[index].A));
          Vector3 vector3_5 = localToWorldMatrix.MultiplyPoint3x4(GetVertex(Triangles[index].B));
          Vector3 vector3_6 = localToWorldMatrix.MultiplyPoint3x4(GetVertex(Triangles[index].C));
          Gizmos.DrawLine(vector3_4, vector3_5);
          Gizmos.DrawLine(vector3_5, vector3_6);
          Gizmos.DrawLine(vector3_6, vector3_4);
        }
      }
    }

    private int AddVertex(List<Vector3> list, Vector3 vertex, float weldThreshold)
    {
      int num = -1;
      for (int index = 0; index < list.Count; ++index)
      {
        if (Vector3.Distance(list[index], vertex) <= (double) weldThreshold)
        {
          list[index] = (list[index] + vertex) * 0.5f;
          num = index;
          break;
        }
      }
      if (num == -1)
      {
        num = list.Count;
        list.Add(vertex);
      }
      return num;
    }

    [Serializable]
    public struct IndexTuple
    {
      public int A;
      public int B;
      public int C;
    }
  }
}
