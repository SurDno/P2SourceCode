using System;
using Cinemachine.Utility;
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
    public Waypoint[] m_Waypoints = [];
    private Waypoint[] m_ControlPoints1;
    private Waypoint[] m_ControlPoints2;
    private bool m_IsLoopedCache;

    public override float MinPos => 0.0f;

    public override float MaxPos
    {
      get
      {
        int num = m_Waypoints.Length - 1;
        return num < 1 ? 0.0f : (m_Looped ? num + 1 : (float) num);
      }
    }

    public override bool Looped => m_Looped;

    public override int DistanceCacheSampleStepsPerSegment => m_Resolution;

    private void OnValidate() => InvalidateDistanceCache();

    public override void InvalidateDistanceCache()
    {
      base.InvalidateDistanceCache();
      m_ControlPoints1 = null;
      m_ControlPoints2 = null;
    }

    private void UpdateControlPoints()
    {
      int length = m_Waypoints == null ? 0 : m_Waypoints.Length;
      if (length <= 1 || Looped == m_IsLoopedCache && m_ControlPoints1 != null && m_ControlPoints1.Length == length && m_ControlPoints2 != null && m_ControlPoints2.Length == length)
        return;
      Vector4[] ctrl1 = new Vector4[length];
      Vector4[] ctrl2 = new Vector4[length];
      Vector4[] knot = new Vector4[length];
      for (int index = 0; index < length; ++index)
        knot[index] = m_Waypoints[index].AsVector4;
      if (Looped)
        SplineHelpers.ComputeSmoothControlPointsLooped(ref knot, ref ctrl1, ref ctrl2);
      else
        SplineHelpers.ComputeSmoothControlPoints(ref knot, ref ctrl1, ref ctrl2);
      m_ControlPoints1 = new Waypoint[length];
      m_ControlPoints2 = new Waypoint[length];
      for (int index = 0; index < length; ++index)
      {
        m_ControlPoints1[index] = Waypoint.FromVector4(ctrl1[index]);
        m_ControlPoints2[index] = Waypoint.FromVector4(ctrl2[index]);
      }
      m_IsLoopedCache = Looped;
    }

    private float GetBoundingIndices(float pos, out int indexA, out int indexB)
    {
      pos = NormalizePos(pos);
      int length = m_Waypoints.Length;
      if (length < 2)
      {
        indexA = indexB = 0;
      }
      else
      {
        indexA = Mathf.FloorToInt(pos);
        if (indexA >= length)
        {
          pos -= MaxPos;
          indexA = 0;
        }
        indexB = indexA + 1;
        if (indexB == length)
        {
          if (Looped)
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
      if (m_Waypoints.Length != 0)
      {
        UpdateControlPoints();
        pos = GetBoundingIndices(pos, out int indexA, out int indexB);
        position = indexA != indexB ? SplineHelpers.Bezier3(pos - indexA, m_Waypoints[indexA].position, m_ControlPoints1[indexA].position, m_ControlPoints2[indexA].position, m_Waypoints[indexB].position) : m_Waypoints[indexA].position;
      }
      return transform.TransformPoint(position);
    }

    public override Vector3 EvaluateTangent(float pos)
    {
      Vector3 direction = transform.rotation * Vector3.forward;
      if (m_Waypoints.Length > 1)
      {
        UpdateControlPoints();
        pos = GetBoundingIndices(pos, out int indexA, out int indexB);
        if (!Looped && indexA == m_Waypoints.Length - 1)
          --indexA;
        direction = SplineHelpers.BezierTangent3(pos - indexA, m_Waypoints[indexA].position, m_ControlPoints1[indexA].position, m_ControlPoints2[indexA].position, m_Waypoints[indexB].position);
      }
      return transform.TransformDirection(direction);
    }

    public override Quaternion EvaluateOrientation(float pos)
    {
      Quaternion orientation = transform.rotation;
      if (m_Waypoints.Length != 0)
      {
        pos = GetBoundingIndices(pos, out int indexA, out int indexB);
        float angle;
        if (indexA == indexB)
        {
          angle = m_Waypoints[indexA].roll;
        }
        else
        {
          UpdateControlPoints();
          angle = SplineHelpers.Bezier1(pos - indexA, m_Waypoints[indexA].roll, m_ControlPoints1[indexA].roll, m_ControlPoints2[indexA].roll, m_Waypoints[indexB].roll);
        }
        Vector3 tangent = EvaluateTangent(pos);
        if (!tangent.AlmostZero())
        {
          Vector3 upwards = transform.rotation * Vector3.up;
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

      internal Vector4 AsVector4 => new(position.x, position.y, position.z, roll);

      internal static Waypoint FromVector4(Vector4 v)
      {
        return new Waypoint {
          position = new Vector3(v[0], v[1], v[2]),
          roll = v[3]
        };
      }
    }
  }
}
