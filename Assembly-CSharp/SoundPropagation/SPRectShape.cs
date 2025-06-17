using UnityEngine;

namespace SoundPropagation
{
  public class SPRectShape : Shape
  {
    public Vector2 Size = new(1f, 2f);
    private Matrix4x4 plane2world;
    private Matrix4x4 world2plane;

    protected override void Initialize()
    {
      plane2world = transform.localToWorldMatrix;
      Vector2 vector2 = Size * 0.5f;
      plane2world.m00 *= vector2.x;
      plane2world.m10 *= vector2.x;
      plane2world.m20 *= vector2.x;
      plane2world.m01 *= vector2.y;
      plane2world.m11 *= vector2.y;
      plane2world.m21 *= vector2.y;
      world2plane = plane2world.inverse;
    }

    protected override bool ClosestPointToSegmentInternal(
      Vector3 pointA,
      Vector3 pointB,
      out Vector3 output)
    {
      Vector3 segmentOnPlane = ClosestToSegmentOnPlane(world2plane, pointA, pointB);
      if (segmentOnPlane.x > 1.0)
        segmentOnPlane.x = 1f;
      else if (segmentOnPlane.x < -1.0)
        segmentOnPlane.x = -1f;
      if (segmentOnPlane.y > 1.0)
        segmentOnPlane.y = 1f;
      else if (segmentOnPlane.y < -1.0)
        segmentOnPlane.y = -1f;
      output = plane2world.MultiplyPoint3x4(segmentOnPlane);
      return true;
    }

    private void OnDrawGizmosSelected()
    {
      Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
      Vector2 vector2 = new Vector2(Size.x * 0.5f, Size.y * 0.5f);
      Vector3 vector3_1 = localToWorldMatrix.MultiplyPoint(new Vector3(-vector2.x, -vector2.y, 0.0f));
      Vector3 vector3_2 = localToWorldMatrix.MultiplyPoint(new Vector3(-vector2.x, vector2.y, 0.0f));
      Vector3 vector3_3 = localToWorldMatrix.MultiplyPoint(new Vector3(vector2.x, vector2.y, 0.0f));
      Vector3 vector3_4 = localToWorldMatrix.MultiplyPoint(new Vector3(vector2.x, -vector2.y, 0.0f));
      Gizmos.color = gizmoColor;
      Gizmos.DrawLine(vector3_1, vector3_2);
      Gizmos.DrawLine(vector3_2, vector3_3);
      Gizmos.DrawLine(vector3_3, vector3_4);
      Gizmos.DrawLine(vector3_4, vector3_1);
      Gizmos.DrawLine(vector3_1, vector3_3);
      Gizmos.DrawLine(vector3_2, vector3_4);
    }
  }
}
