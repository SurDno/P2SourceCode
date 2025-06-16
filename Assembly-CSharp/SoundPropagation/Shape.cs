// Decompiled with JetBrains decompiler
// Type: SoundPropagation.Shape
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
