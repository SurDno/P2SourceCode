using System;
using Cinemachine.Utility;
using UnityEngine;

namespace Cinemachine
{
  public abstract class CinemachinePathBase : MonoBehaviour
  {
    [Tooltip("Path samples per waypoint.  This is used for calculating path distances.")]
    [Range(1f, 100f)]
    public int m_Resolution = 20;
    [Tooltip("The settings that control how the path will appear in the editor scene view.")]
    public Appearance m_Appearance = new Appearance();
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
      if (MaxPos == 0.0)
        return 0.0f;
      if (!Looped)
        return Mathf.Clamp(pos, 0.0f, MaxPos);
      pos %= MaxPos;
      if (pos < 0.0)
        pos += MaxPos;
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
      float num1 = MinPos;
      float a = MaxPos;
      if (searchRadius >= 0)
      {
        int num2 = Mathf.FloorToInt(Mathf.Min(searchRadius, (float) ((a - (double) num1) / 2.0)));
        num1 = startSegment - num2;
        a = startSegment + num2 + 1;
        if (!Looped)
        {
          num1 = Mathf.Max(num1, MinPos);
          a = Mathf.Max(a, MaxPos);
        }
      }
      stepsPerSegment = Mathf.RoundToInt(Mathf.Clamp(stepsPerSegment, 1f, 100f));
      float num3 = 1f / stepsPerSegment;
      float closestPoint = startSegment;
      float num4 = float.MaxValue;
      int num5 = stepsPerSegment == 1 ? 1 : 3;
      for (int index = 0; index < num5; ++index)
      {
        Vector3 vector3 = EvaluatePosition(num1);
        for (float pos = num1 + num3; pos <= (double) a; pos += num3)
        {
          Vector3 position = EvaluatePosition(pos);
          float t = p.ClosestPointOnSegment(vector3, position);
          float num6 = Vector3.SqrMagnitude(p - Vector3.Lerp(vector3, position, t));
          if (num6 < (double) num4)
          {
            num4 = num6;
            closestPoint = pos - (1f - t) * num3;
          }
          vector3 = position;
        }
        num1 = closestPoint - num3;
        a = closestPoint + num3;
        num3 /= stepsPerSegment;
      }
      return closestPoint;
    }

    public float MinUnit(PositionUnits units)
    {
      return units == PositionUnits.Distance ? 0.0f : MinPos;
    }

    public float MaxUnit(PositionUnits units)
    {
      return units == PositionUnits.Distance ? PathLength : MaxPos;
    }

    public virtual float NormalizeUnit(float pos, PositionUnits units)
    {
      return units == PositionUnits.Distance ? NormalizePathDistance(pos) : NormalizePos(pos);
    }

    public Vector3 EvaluatePositionAtUnit(float pos, PositionUnits units)
    {
      if (units == PositionUnits.Distance)
        pos = GetPathPositionFromDistance(pos);
      return EvaluatePosition(pos);
    }

    public Vector3 EvaluateTangentAtUnit(float pos, PositionUnits units)
    {
      if (units == PositionUnits.Distance)
        pos = GetPathPositionFromDistance(pos);
      return EvaluateTangent(pos);
    }

    public Quaternion EvaluateOrientationAtUnit(float pos, PositionUnits units)
    {
      if (units == PositionUnits.Distance)
        pos = GetPathPositionFromDistance(pos);
      return EvaluateOrientation(pos);
    }

    public abstract int DistanceCacheSampleStepsPerSegment { get; }

    public virtual void InvalidateDistanceCache()
    {
      m_DistanceToPos = null;
      m_PosToDistance = null;
      m_CachedSampleSteps = 0;
      m_PathLength = 0.0f;
    }

    public bool DistanceCacheIsValid()
    {
      return MaxPos == (double) MinPos || m_DistanceToPos != null && m_PosToDistance != null && m_CachedSampleSteps == DistanceCacheSampleStepsPerSegment && m_CachedSampleSteps > 0;
    }

    public float PathLength
    {
      get
      {
        if (DistanceCacheSampleStepsPerSegment < 1)
          return 0.0f;
        if (!DistanceCacheIsValid())
          ResamplePath(DistanceCacheSampleStepsPerSegment);
        return m_PathLength;
      }
    }

    public float NormalizePathDistance(float distance)
    {
      float pathLength = PathLength;
      if (pathLength < 9.9999997473787516E-06)
        return 0.0f;
      if (Looped)
      {
        distance %= pathLength;
        if (distance < 0.0)
          distance += pathLength;
      }
      return Mathf.Clamp(distance, 0.0f, pathLength);
    }

    public float GetPathPositionFromDistance(float distance)
    {
      if (DistanceCacheSampleStepsPerSegment < 1 || PathLength < 9.9999997473787516E-05)
        return MinPos;
      distance = NormalizePathDistance(distance);
      float f = distance / m_cachedDistanceStepSize;
      int index = Mathf.FloorToInt(f);
      if (index >= m_DistanceToPos.Length - 1)
        return MaxPos;
      float t = f - index;
      return MinPos + Mathf.Lerp(m_DistanceToPos[index], m_DistanceToPos[index + 1], t);
    }

    public float GetPathDistanceFromPosition(float pos)
    {
      if (DistanceCacheSampleStepsPerSegment < 1 || PathLength < 9.9999997473787516E-05)
        return 0.0f;
      pos = NormalizePos(pos);
      float f = pos / m_cachedPosStepSize;
      int index = Mathf.FloorToInt(f);
      if (index >= m_PosToDistance.Length - 1)
        return m_PathLength;
      float t = f - index;
      return Mathf.Lerp(m_PosToDistance[index], m_PosToDistance[index + 1], t);
    }

    private void ResamplePath(int stepsPerSegment)
    {
      InvalidateDistanceCache();
      float minPos = MinPos;
      float maxPos = MaxPos;
      float num1 = 1f / Mathf.Max(1, stepsPerSegment);
      int length = Mathf.RoundToInt((maxPos - minPos) / num1) + 1;
      m_PosToDistance = new float[length];
      m_CachedSampleSteps = stepsPerSegment;
      m_cachedPosStepSize = num1;
      Vector3 a = EvaluatePosition(0.0f);
      m_PosToDistance[0] = 0.0f;
      float pos = minPos;
      for (int index = 1; index < length; ++index)
      {
        pos += num1;
        Vector3 position = EvaluatePosition(pos);
        m_PathLength += Vector3.Distance(a, position);
        a = position;
        m_PosToDistance[index] = m_PathLength;
      }
      m_DistanceToPos = new float[length];
      m_DistanceToPos[0] = 0.0f;
      if (length <= 1)
        return;
      float num2 = m_PathLength / (length - 1);
      m_cachedDistanceStepSize = num2;
      float num3 = 0.0f;
      int index1 = 1;
      for (int index2 = 1; index2 < length; ++index2)
      {
        num3 += num2;
        float num4 = m_PosToDistance[index1];
        while (num4 < (double) num3 && index1 < length - 1)
          num4 = m_PosToDistance[++index1];
        float num5 = m_PosToDistance[index1 - 1];
        float num6 = num4 - num5;
        float num7 = (num3 - num5) / num6;
        m_DistanceToPos[index2] = m_cachedPosStepSize * (float) (num7 + (double) index1 - 1.0);
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
