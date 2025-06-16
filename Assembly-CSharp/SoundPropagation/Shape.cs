using UnityEngine;

namespace SoundPropagation
{
  public abstract class Shape : MonoBehaviour
  {
    private bool initialized = false;

    public bool ClosestPointToSegment(Vector3 pointA, Vector3 pointB, out Vector3 output)
    {
      if (!this.initialized)
      {
        this.Initialize();
        this.initialized = true;
      }
      return this.ClosestPointToSegmentInternal(pointA, pointB, out output);
    }

    protected abstract bool ClosestPointToSegmentInternal(
      Vector3 pointA,
      Vector3 pointB,
      out Vector3 output);

    protected Vector3 ClosestToSegmentOnPlane(
      Matrix4x4 world2plane,
      Vector3 pointA,
      Vector3 pointB)
    {
      pointA = world2plane.MultiplyPoint3x4(pointA);
      pointB = world2plane.MultiplyPoint3x4(pointB);
      Vector3 segmentOnPlane;
      if ((double) pointA.z == (double) pointB.z)
        segmentOnPlane = (pointA + pointB) * 0.5f;
      else if ((double) pointA.z >= 0.0 && (double) pointB.z >= 0.0)
        segmentOnPlane = (double) pointA.z >= (double) pointB.z ? pointB : pointA;
      else if ((double) pointA.z <= 0.0 && (double) pointB.z <= 0.0)
      {
        segmentOnPlane = (double) pointA.z <= (double) pointB.z ? pointB : pointA;
      }
      else
      {
        float num = pointA.z / (pointA.z - pointB.z);
        segmentOnPlane = pointB * num + pointA * (1f - num);
      }
      segmentOnPlane.z = 0.0f;
      return segmentOnPlane;
    }

    protected Color gizmoColor => new Color(0.0f, 0.5f, 1f, 0.75f);

    protected abstract void Initialize();
  }
}
