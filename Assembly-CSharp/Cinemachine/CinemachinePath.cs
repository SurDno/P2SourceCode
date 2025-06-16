using Cinemachine.Utility;
using System;
using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(18f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("Cinemachine/CinemachinePath")]
  [SaveDuringPlay]
  public class CinemachinePath : CinemachinePathBase
  {
    [Tooltip("If checked, then the path ends are joined to form a continuous loop.")]
    public bool m_Looped;
    [Tooltip("The waypoints that define the path.  They will be interpolated using a bezier curve.")]
    public CinemachinePath.Waypoint[] m_Waypoints = new CinemachinePath.Waypoint[0];

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

    private float GetBoundingIndices(float pos, out int indexA, out int indexB)
    {
      pos = this.NormalizePos(pos);
      int num = Mathf.RoundToInt(pos);
      if ((double) Mathf.Abs(pos - (float) num) < 9.9999997473787516E-05)
      {
        indexA = indexB = num == this.m_Waypoints.Length ? 0 : num;
      }
      else
      {
        indexA = Mathf.FloorToInt(pos);
        if (indexA >= this.m_Waypoints.Length)
        {
          pos -= this.MaxPos;
          indexA = 0;
        }
        indexB = Mathf.CeilToInt(pos);
        if (indexB >= this.m_Waypoints.Length)
          indexB = 0;
      }
      return pos;
    }

    public override Vector3 EvaluatePosition(float pos)
    {
      Vector3 vector3 = new Vector3();
      Vector3 position;
      if (this.m_Waypoints.Length == 0)
      {
        position = this.transform.position;
      }
      else
      {
        int indexA;
        int indexB;
        pos = this.GetBoundingIndices(pos, out indexA, out indexB);
        if (indexA == indexB)
        {
          position = this.m_Waypoints[indexA].position;
        }
        else
        {
          CinemachinePath.Waypoint waypoint1 = this.m_Waypoints[indexA];
          CinemachinePath.Waypoint waypoint2 = this.m_Waypoints[indexB];
          position = SplineHelpers.Bezier3(pos - (float) indexA, this.m_Waypoints[indexA].position, waypoint1.position + waypoint1.tangent, waypoint2.position - waypoint2.tangent, waypoint2.position);
        }
      }
      return this.transform.TransformPoint(position);
    }

    public override Vector3 EvaluateTangent(float pos)
    {
      Vector3 vector3 = new Vector3();
      Vector3 direction;
      if (this.m_Waypoints.Length == 0)
      {
        direction = this.transform.rotation * Vector3.forward;
      }
      else
      {
        int indexA;
        int indexB;
        pos = this.GetBoundingIndices(pos, out indexA, out indexB);
        if (indexA == indexB)
        {
          direction = this.m_Waypoints[indexA].tangent;
        }
        else
        {
          CinemachinePath.Waypoint waypoint1 = this.m_Waypoints[indexA];
          CinemachinePath.Waypoint waypoint2 = this.m_Waypoints[indexB];
          direction = SplineHelpers.BezierTangent3(pos - (float) indexA, this.m_Waypoints[indexA].position, waypoint1.position + waypoint1.tangent, waypoint2.position - waypoint2.tangent, waypoint2.position);
        }
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
          float roll1 = this.m_Waypoints[indexA].roll;
          float roll2 = this.m_Waypoints[indexB].roll;
          if (indexB == 0)
          {
            roll1 %= 360f;
            roll2 %= 360f;
          }
          angle = Mathf.Lerp(roll1, roll2, pos - (float) indexA);
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

    private void OnValidate() => this.InvalidateDistanceCache();

    [DocumentationSorting(18.2f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct Waypoint
    {
      [Tooltip("Position in path-local space")]
      public Vector3 position;
      [Tooltip("Offset from the position, which defines the tangent of the curve at the waypoint.  The length of the tangent encodes the strength of the bezier handle.  The same handle is used symmetrically on both sides of the waypoint, to ensure smoothness.")]
      public Vector3 tangent;
      [Tooltip("Defines the roll of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up.")]
      public float roll;
    }
  }
}
