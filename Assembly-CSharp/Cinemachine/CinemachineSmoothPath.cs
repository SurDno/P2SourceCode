using Cinemachine.Utility;
using System;
using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(18.5f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("Cinemachine/CinemachineSmoothPath")]
  [SaveDuringPlay]
  public class CinemachineSmoothPath : CinemachinePathBase
  {
    [Tooltip("If checked, then the path ends are joined to form a continuous loop.")]
    public bool m_Looped;
    [Tooltip("The waypoints that define the path.  They will be interpolated using a bezier curve.")]
    public CinemachineSmoothPath.Waypoint[] m_Waypoints = new CinemachineSmoothPath.Waypoint[0];
    private CinemachineSmoothPath.Waypoint[] m_ControlPoints1;
    private CinemachineSmoothPath.Waypoint[] m_ControlPoints2;
    private bool m_IsLoopedCache;

    public override float MinPos => 0.0f;

    public override float MaxPos
    {
      get
      {
        int num = this.m_Waypoints.Length - 1;
        return num < 1 ? 0.0f : (this.m_Looped ? (float) (num + 1) : (float) num);
      }
    }

    public override bool Looped => this.m_Looped;

    public override int DistanceCacheSampleStepsPerSegment => this.m_Resolution;

    private void OnValidate() => this.InvalidateDistanceCache();

    public override void InvalidateDistanceCache()
    {
      base.InvalidateDistanceCache();
      this.m_ControlPoints1 = (CinemachineSmoothPath.Waypoint[]) null;
      this.m_ControlPoints2 = (CinemachineSmoothPath.Waypoint[]) null;
    }

    private void UpdateControlPoints()
    {
      int length = this.m_Waypoints == null ? 0 : this.m_Waypoints.Length;
      if (length <= 1 || this.Looped == this.m_IsLoopedCache && this.m_ControlPoints1 != null && this.m_ControlPoints1.Length == length && this.m_ControlPoints2 != null && this.m_ControlPoints2.Length == length)
        return;
      Vector4[] ctrl1 = new Vector4[length];
      Vector4[] ctrl2 = new Vector4[length];
      Vector4[] knot = new Vector4[length];
      for (int index = 0; index < length; ++index)
        knot[index] = this.m_Waypoints[index].AsVector4;
      if (this.Looped)
        SplineHelpers.ComputeSmoothControlPointsLooped(ref knot, ref ctrl1, ref ctrl2);
      else
        SplineHelpers.ComputeSmoothControlPoints(ref knot, ref ctrl1, ref ctrl2);
      this.m_ControlPoints1 = new CinemachineSmoothPath.Waypoint[length];
      this.m_ControlPoints2 = new CinemachineSmoothPath.Waypoint[length];
      for (int index = 0; index < length; ++index)
      {
        this.m_ControlPoints1[index] = CinemachineSmoothPath.Waypoint.FromVector4(ctrl1[index]);
        this.m_ControlPoints2[index] = CinemachineSmoothPath.Waypoint.FromVector4(ctrl2[index]);
      }
      this.m_IsLoopedCache = this.Looped;
    }

    private float GetBoundingIndices(float pos, out int indexA, out int indexB)
    {
      pos = this.NormalizePos(pos);
      int length = this.m_Waypoints.Length;
      if (length < 2)
      {
        indexA = indexB = 0;
      }
      else
      {
        indexA = Mathf.FloorToInt(pos);
        if (indexA >= length)
        {
          pos -= this.MaxPos;
          indexA = 0;
        }
        indexB = indexA + 1;
        if (indexB == length)
        {
          if (this.Looped)
          {
            indexB = 0;
          }
          else
          {
            --indexB;
            --indexA;
          }
        }
      }
      return pos;
    }

    public override Vector3 EvaluatePosition(float pos)
    {
      Vector3 position = Vector3.zero;
      if (this.m_Waypoints.Length != 0)
      {
        this.UpdateControlPoints();
        int indexA;
        int indexB;
        pos = this.GetBoundingIndices(pos, out indexA, out indexB);
        position = indexA != indexB ? SplineHelpers.Bezier3(pos - (float) indexA, this.m_Waypoints[indexA].position, this.m_ControlPoints1[indexA].position, this.m_ControlPoints2[indexA].position, this.m_Waypoints[indexB].position) : this.m_Waypoints[indexA].position;
      }
      return this.transform.TransformPoint(position);
    }

    public override Vector3 EvaluateTangent(float pos)
    {
      Vector3 direction = this.transform.rotation * Vector3.forward;
      if (this.m_Waypoints.Length > 1)
      {
        this.UpdateControlPoints();
        int indexA;
        int indexB;
        pos = this.GetBoundingIndices(pos, out indexA, out indexB);
        if (!this.Looped && indexA == this.m_Waypoints.Length - 1)
          --indexA;
        direction = SplineHelpers.BezierTangent3(pos - (float) indexA, this.m_Waypoints[indexA].position, this.m_ControlPoints1[indexA].position, this.m_ControlPoints2[indexA].position, this.m_Waypoints[indexB].position);
      }
      return this.transform.TransformDirection(direction);
    }

    public override Quaternion EvaluateOrientation(float pos)
    {
      Quaternion orientation = this.transform.rotation;
      if (this.m_Waypoints.Length != 0)
      {
        int indexA;
        int indexB;
        pos = this.GetBoundingIndices(pos, out indexA, out indexB);
        float angle;
        if (indexA == indexB)
        {
          angle = this.m_Waypoints[indexA].roll;
        }
        else
        {
          this.UpdateControlPoints();
          angle = SplineHelpers.Bezier1(pos - (float) indexA, this.m_Waypoints[indexA].roll, this.m_ControlPoints1[indexA].roll, this.m_ControlPoints2[indexA].roll, this.m_Waypoints[indexB].roll);
        }
        Vector3 tangent = this.EvaluateTangent(pos);
        if (!tangent.AlmostZero())
        {
          Vector3 upwards = this.transform.rotation * Vector3.up;
          orientation = Quaternion.LookRotation(tangent, upwards) * Quaternion.AngleAxis(angle, Vector3.forward);
        }
      }
      return orientation;
    }

    [DocumentationSorting(18.7f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct Waypoint
    {
      [Tooltip("Position in path-local space")]
      public Vector3 position;
      [Tooltip("Defines the roll of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up.")]
      public float roll;

      internal Vector4 AsVector4
      {
        get => new Vector4(this.position.x, this.position.y, this.position.z, this.roll);
      }

      internal static CinemachineSmoothPath.Waypoint FromVector4(Vector4 v)
      {
        return new CinemachineSmoothPath.Waypoint()
        {
          position = new Vector3(v[0], v[1], v[2]),
          roll = v[3]
        };
      }
    }
  }
}
