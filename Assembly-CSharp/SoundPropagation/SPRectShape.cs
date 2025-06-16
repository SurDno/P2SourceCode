using UnityEngine;

namespace SoundPropagation
{
  public class SPRectShape : Shape
  {
    public Vector2 Size = new Vector2(1f, 2f);
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
      if ((double) segmentOnPlane.x > 1.0)
        segmentOnPlane.x = 1f;
      else if ((double) segmentOnPlane.x < -1.0)
        segmentOnPlane.x = -1f;
      if ((double) segmentOnPlane.y > 1.0)
        segmentOnPlane.y = 1f;
      else if ((double) segmentOnPlane.y < -1.0)
        segmentOnPlane.y = -1f;
      output = this.plane2world.MultiplyPoint3x4(segmentOnPlane);
      return true;
    }

    private void OnDrawGizmosSelected()
    {
      Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
      Vector2 vector2 = new Vector2(this.Size.x * 0.5f, this.Size.y * 0.5f);
      Vector3 vector3_1 = localToWorldMatrix.MultiplyPoint(new Vector3(-vector2.x, -vector2.y, 0.0f));
      Vector3 vector3_2 = localToWorldMatrix.MultiplyPoint(new Vector3(-vector2.x, vector2.y, 0.0f));
      Vector3 vector3_3 = localToWorldMatrix.MultiplyPoint(new Vector3(vector2.x, vector2.y, 0.0f));
      Vector3 vector3_4 = localToWorldMatrix.MultiplyPoint(new Vector3(vector2.x, -vector2.y, 0.0f));
      Gizmos.color = this.gizmoColor;
      Gizmos.DrawLine(vector3_1, vector3_2);
      Gizmos.DrawLine(vector3_2, vector3_3);
      Gizmos.DrawLine(vector3_3, vector3_4);
      Gizmos.DrawLine(vector3_4, vector3_1);
      Gizmos.DrawLine(vector3_1, vector3_3);
      Gizmos.DrawLine(vector3_2, vector3_4);
    }
  }
}
