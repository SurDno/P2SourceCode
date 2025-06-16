// Decompiled with JetBrains decompiler
// Type: SoundPropagation.SPEllipseShape
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  public class SPEllipseShape : Shape
  {
    public Vector2 Size = new Vector2(1f, 1f);
    private Matrix4x4 plane2world;
    private Matrix4x4 world2plane;

    protected override void Initialize()
    {
      this.plane2world = this.transform.localToWorldMatrix;
      Vector2 vector2 = this.Size * 0.5f;
      this.plane2world.m00 *= vector2.x;
      this.plane2world.m10 *= vector2.x;
      this.plane2world.m20 *= vector2.x;
      this.plane2world.m01 *= vector2.y;
      this.plane2world.m11 *= vector2.y;
      this.plane2world.m21 *= vector2.y;
      this.world2plane = this.plane2world.inverse;
    }

    protected override bool ClosestPointToSegmentInternal(
      Vector3 pointA,
      Vector3 pointB,
      out Vector3 output)
    {
      Vector3 segmentOnPlane = this.ClosestToSegmentOnPlane(this.world2plane, pointA, pointB);
      float f = (float) ((double) segmentOnPlane.x * (double) segmentOnPlane.x + (double) segmentOnPlane.y * (double) segmentOnPlane.y);
      if ((double) f > 1.0)
      {
        float num = Mathf.Sqrt(f);
        segmentOnPlane.x /= num;
        segmentOnPlane.y /= num;
      }
      output = this.plane2world.MultiplyPoint3x4(segmentOnPlane);
      return true;
    }

    private void OnDrawGizmosSelected()
    {
      this.plane2world = this.transform.localToWorldMatrix;
      Vector2 vector2 = this.Size * 0.5f;
      this.plane2world.m00 *= vector2.x;
      this.plane2world.m10 *= vector2.x;
      this.plane2world.m20 *= vector2.x;
      this.plane2world.m01 *= vector2.y;
      this.plane2world.m11 *= vector2.y;
      this.plane2world.m21 *= vector2.y;
      Gizmos.color = this.gizmoColor;
      Vector3 vector3_1 = this.plane2world.MultiplyPoint(new Vector3(-1f, 0.0f, 0.0f));
      Vector3 vector3_2 = this.plane2world.MultiplyPoint(new Vector3(-0.866f, 0.5f, 0.0f));
      Gizmos.DrawLine(vector3_1, vector3_2);
      Vector3 vector3_3 = this.plane2world.MultiplyPoint(new Vector3(-0.5f, 0.866f, 0.0f));
      Gizmos.DrawLine(vector3_2, vector3_3);
      Vector3 vector3_4 = this.plane2world.MultiplyPoint(new Vector3(0.0f, 1f, 0.0f));
      Gizmos.DrawLine(vector3_3, vector3_4);
      Vector3 vector3_5 = this.plane2world.MultiplyPoint(new Vector3(0.5f, 0.866f, 0.0f));
      Gizmos.DrawLine(vector3_4, vector3_5);
      Vector3 vector3_6 = this.plane2world.MultiplyPoint(new Vector3(0.866f, 0.5f, 0.0f));
      Gizmos.DrawLine(vector3_5, vector3_6);
      Vector3 vector3_7 = this.plane2world.MultiplyPoint(new Vector3(1f, 0.0f, 0.0f));
      Gizmos.DrawLine(vector3_6, vector3_7);
      Vector3 vector3_8 = this.plane2world.MultiplyPoint(new Vector3(0.866f, -0.5f, 0.0f));
      Gizmos.DrawLine(vector3_7, vector3_8);
      Vector3 vector3_9 = this.plane2world.MultiplyPoint(new Vector3(0.5f, -0.866f, 0.0f));
      Gizmos.DrawLine(vector3_8, vector3_9);
      Vector3 vector3_10 = this.plane2world.MultiplyPoint(new Vector3(0.0f, -1f, 0.0f));
      Gizmos.DrawLine(vector3_9, vector3_10);
      Vector3 vector3_11 = this.plane2world.MultiplyPoint(new Vector3(-0.5f, -0.866f, 0.0f));
      Gizmos.DrawLine(vector3_10, vector3_11);
      Vector3 vector3_12 = this.plane2world.MultiplyPoint(new Vector3(-0.866f, -0.5f, 0.0f));
      Gizmos.DrawLine(vector3_11, vector3_12);
      Gizmos.DrawLine(vector3_12, vector3_1);
    }
  }
}
