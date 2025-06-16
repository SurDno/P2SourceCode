using Cinemachine.Utility;
using System;
using UnityEngine;

namespace Cinemachine
{
  public abstract class CinemachinePathBase : MonoBehaviour
  {
    [Tooltip("Path samples per waypoint.  This is used for calculating path distances.")]
    [Range(1f, 100f)]
    public int m_Resolution = 20;
    [Tooltip("The settings that control how the path will appear in the editor scene view.")]
    public CinemachinePathBase.Appearance m_Appearance = new CinemachinePathBase.Appearance();
    private float[] m_DistanceToPos;
    private float[] m_PosToDistance;
    private int m_CachedSampleSteps;
    private float m_PathLength;
    private float m_cachedPosStepSize;
    private float m_cachedDistanceStepSize;

    public abstract float MinPos { get; }

    public abstract float MaxPos { get; }

    public abstract bool Looped { get; }

    public virtual float NormalizePos(float pos)
    {
      if ((double) this.MaxPos == 0.0)
        return 0.0f;
      if (!this.Looped)
        return Mathf.Clamp(pos, 0.0f, this.MaxPos);
      pos %= this.MaxPos;
      if ((double) pos < 0.0)
        pos += this.MaxPos;
      return pos;
    }

    public abstract Vector3 EvaluatePosition(float pos);

    public abstract Vector3 EvaluateTangent(float pos);

    public abstract Quaternion EvaluateOrientation(float pos);

    public virtual float FindClosestPoint(
      Vector3 p,
      int startSegment,
      int searchRadius,
      int stepsPerSegment)
    {
      float num1 = this.MinPos;
      float a = this.MaxPos;
      if (searchRadius >= 0)
      {
        int num2 = Mathf.FloorToInt(Mathf.Min((float) searchRadius, (float) (((double) a - (double) num1) / 2.0)));
        num1 = (float) (startSegment - num2);
        a = (float) (startSegment + num2 + 1);
        if (!this.Looped)
        {
          num1 = Mathf.Max(num1, this.MinPos);
          a = Mathf.Max(a, this.MaxPos);
        }
      }
      stepsPerSegment = Mathf.RoundToInt(Mathf.Clamp((float) stepsPerSegment, 1f, 100f));
      float num3 = 1f / (float) stepsPerSegment;
      float closestPoint = (float) startSegment;
      float num4 = float.MaxValue;
      int num5 = stepsPerSegment == 1 ? 1 : 3;
      for (int index = 0; index < num5; ++index)
      {
        Vector3 vector3 = this.EvaluatePosition(num1);
        for (float pos = num1 + num3; (double) pos <= (double) a; pos += num3)
        {
          Vector3 position = this.EvaluatePosition(pos);
          float t = p.ClosestPointOnSegment(vector3, position);
          float num6 = Vector3.SqrMagnitude(p - Vector3.Lerp(vector3, position, t));
          if ((double) num6 < (double) num4)
          {
            num4 = num6;
            closestPoint = pos - (1f - t) * num3;
          }
          vector3 = position;
        }
        num1 = closestPoint - num3;
        a = closestPoint + num3;
        num3 /= (float) stepsPerSegment;
      }
      return closestPoint;
    }

    public float MinUnit(CinemachinePathBase.PositionUnits units)
    {
      return units == CinemachinePathBase.PositionUnits.Distance ? 0.0f : this.MinPos;
    }

    public float MaxUnit(CinemachinePathBase.PositionUnits units)
    {
      return units == CinemachinePathBase.PositionUnits.Distance ? this.PathLength : this.MaxPos;
    }

    public virtual float NormalizeUnit(float pos, CinemachinePathBase.PositionUnits units)
    {
      return units == CinemachinePathBase.PositionUnits.Distance ? this.NormalizePathDistance(pos) : this.NormalizePos(pos);
    }

    public Vector3 EvaluatePositionAtUnit(float pos, CinemachinePathBase.PositionUnits units)
    {
      if (units == CinemachinePathBase.PositionUnits.Distance)
        pos = this.GetPathPositionFromDistance(pos);
      return this.EvaluatePosition(pos);
    }

    public Vector3 EvaluateTangentAtUnit(float pos, CinemachinePathBase.PositionUnits units)
    {
      if (units == CinemachinePathBase.PositionUnits.Distance)
        pos = this.GetPathPositionFromDistance(pos);
      return this.EvaluateTangent(pos);
    }

    public Quaternion EvaluateOrientationAtUnit(float pos, CinemachinePathBase.PositionUnits units)
    {
      if (units == CinemachinePathBase.PositionUnits.Distance)
        pos = this.GetPathPositionFromDistance(pos);
      return this.EvaluateOrientation(pos);
    }

    public abstract int DistanceCacheSampleStepsPerSegment { get; }

    public virtual void InvalidateDistanceCache()
    {
      this.m_DistanceToPos = (float[]) null;
      this.m_PosToDistance = (float[]) null;
      this.m_CachedSampleSteps = 0;
      this.m_PathLength = 0.0f;
    }

    public bool DistanceCacheIsValid()
    {
      return (double) this.MaxPos == (double) this.MinPos || this.m_DistanceToPos != null && this.m_PosToDistance != null && this.m_CachedSampleSteps == this.DistanceCacheSampleStepsPerSegment && this.m_CachedSampleSteps > 0;
    }

    public float PathLength
    {
      get
      {
        if (this.DistanceCacheSampleStepsPerSegment < 1)
          return 0.0f;
        if (!this.DistanceCacheIsValid())
          this.ResamplePath(this.DistanceCacheSampleStepsPerSegment);
        return this.m_PathLength;
      }
    }

    public float NormalizePathDistance(float distance)
    {
      float pathLength = this.PathLength;
      if ((double) pathLength < 9.9999997473787516E-06)
        return 0.0f;
      if (this.Looped)
      {
        distance %= pathLength;
        if ((double) distance < 0.0)
          distance += pathLength;
      }
      return Mathf.Clamp(distance, 0.0f, pathLength);
    }

    public float GetPathPositionFromDistance(float distance)
    {
      if (this.DistanceCacheSampleStepsPerSegment < 1 || (double) this.PathLength < 9.9999997473787516E-05)
        return this.MinPos;
      distance = this.NormalizePathDistance(distance);
      float f = distance / this.m_cachedDistanceStepSize;
      int index = Mathf.FloorToInt(f);
      if (index >= this.m_DistanceToPos.Length - 1)
        return this.MaxPos;
      float t = f - (float) index;
      return this.MinPos + Mathf.Lerp(this.m_DistanceToPos[index], this.m_DistanceToPos[index + 1], t);
    }

    public float GetPathDistanceFromPosition(float pos)
    {
      if (this.DistanceCacheSampleStepsPerSegment < 1 || (double) this.PathLength < 9.9999997473787516E-05)
        return 0.0f;
      pos = this.NormalizePos(pos);
      float f = pos / this.m_cachedPosStepSize;
      int index = Mathf.FloorToInt(f);
      if (index >= this.m_PosToDistance.Length - 1)
        return this.m_PathLength;
      float t = f - (float) index;
      return Mathf.Lerp(this.m_PosToDistance[index], this.m_PosToDistance[index + 1], t);
    }

    private void ResamplePath(int stepsPerSegment)
    {
      this.InvalidateDistanceCache();
      float minPos = this.MinPos;
      float maxPos = this.MaxPos;
      float num1 = 1f / (float) Mathf.Max(1, stepsPerSegment);
      int length = Mathf.RoundToInt((maxPos - minPos) / num1) + 1;
      this.m_PosToDistance = new float[length];
      this.m_CachedSampleSteps = stepsPerSegment;
      this.m_cachedPosStepSize = num1;
      Vector3 a = this.EvaluatePosition(0.0f);
      this.m_PosToDistance[0] = 0.0f;
      float pos = minPos;
      for (int index = 1; index < length; ++index)
      {
        pos += num1;
        Vector3 position = this.EvaluatePosition(pos);
        this.m_PathLength += Vector3.Distance(a, position);
        a = position;
        this.m_PosToDistance[index] = this.m_PathLength;
      }
      this.m_DistanceToPos = new float[length];
      this.m_DistanceToPos[0] = 0.0f;
      if (length <= 1)
        return;
      float num2 = this.m_PathLength / (float) (length - 1);
      this.m_cachedDistanceStepSize = num2;
      float num3 = 0.0f;
      int index1 = 1;
      for (int index2 = 1; index2 < length; ++index2)
      {
        num3 += num2;
        float num4 = this.m_PosToDistance[index1];
        while ((double) num4 < (double) num3 && index1 < length - 1)
          num4 = this.m_PosToDistance[++index1];
        float num5 = this.m_PosToDistance[index1 - 1];
        float num6 = num4 - num5;
        float num7 = (num3 - num5) / num6;
        this.m_DistanceToPos[index2] = this.m_cachedPosStepSize * (float) ((double) num7 + (double) index1 - 1.0);
      }
    }

    [DocumentationSorting(18.1f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public class Appearance
    {
      [Tooltip("The color of the path itself when it is active in the editor")]
      public Color pathColor = Color.green;
      [Tooltip("The color of the path itself when it is inactive in the editor")]
      public Color inactivePathColor = Color.gray;
      [Tooltip("The width of the railroad-tracks that are drawn to represent the path")]
      [Range(0.0f, 10f)]
      public float width = 0.2f;
    }

    public enum PositionUnits
    {
      PathUnits,
      Distance,
    }
  }
}
