using UnityEngine;

namespace RootMotion
{
  public static class V3Tools
  {
    public static Vector3 Lerp(Vector3 fromVector, Vector3 toVector, float weight)
    {
      if ((double) weight <= 0.0)
        return fromVector;
      return (double) weight >= 1.0 ? toVector : Vector3.Lerp(fromVector, toVector, weight);
    }

    public static Vector3 Slerp(Vector3 fromVector, Vector3 toVector, float weight)
    {
      if ((double) weight <= 0.0)
        return fromVector;
      return (double) weight >= 1.0 ? toVector : Vector3.Slerp(fromVector, toVector, weight);
    }

    public static Vector3 ExtractVertical(Vector3 v, Vector3 verticalAxis, float weight)
    {
      return (double) weight == 0.0 ? Vector3.zero : Vector3.Project(v, verticalAxis) * weight;
    }

    public static Vector3 ExtractHorizontal(Vector3 v, Vector3 normal, float weight)
    {
      if ((double) weight == 0.0)
        return Vector3.zero;
      Vector3 tangent = v;
      Vector3.OrthoNormalize(ref normal, ref tangent);
      return Vector3.Project(v, tangent) * weight;
    }

    public static Vector3 ClampDirection(
      Vector3 direction,
      Vector3 normalDirection,
      float clampWeight,
      int clampSmoothing,
      out bool changed)
    {
      changed = false;
      if ((double) clampWeight <= 0.0)
        return direction;
      if ((double) clampWeight >= 1.0)
      {
        changed = true;
        return normalDirection;
      }
      float num1 = (float) (1.0 - (double) Vector3.Angle(normalDirection, direction) / 180.0);
      if ((double) num1 > (double) clampWeight)
        return direction;
      changed = true;
      float num2 = (double) clampWeight > 0.0 ? Mathf.Clamp((float) (1.0 - ((double) clampWeight - (double) num1) / (1.0 - (double) num1)), 0.0f, 1f) : 1f;
      float num3 = (double) clampWeight > 0.0 ? Mathf.Clamp(num1 / clampWeight, 0.0f, 1f) : 1f;
      for (int index = 0; index < clampSmoothing; ++index)
        num3 = Mathf.Sin((float) ((double) num3 * 3.1415927410125732 * 0.5));
      return Vector3.Slerp(normalDirection, direction, num3 * num2);
    }

    public static Vector3 ClampDirection(
      Vector3 direction,
      Vector3 normalDirection,
      float clampWeight,
      int clampSmoothing,
      out float clampValue)
    {
      clampValue = 1f;
      if ((double) clampWeight <= 0.0)
        return direction;
      if ((double) clampWeight >= 1.0)
        return normalDirection;
      float num1 = (float) (1.0 - (double) Vector3.Angle(normalDirection, direction) / 180.0);
      if ((double) num1 > (double) clampWeight)
      {
        clampValue = 0.0f;
        return direction;
      }
      float num2 = (double) clampWeight > 0.0 ? Mathf.Clamp((float) (1.0 - ((double) clampWeight - (double) num1) / (1.0 - (double) num1)), 0.0f, 1f) : 1f;
      float num3 = (double) clampWeight > 0.0 ? Mathf.Clamp(num1 / clampWeight, 0.0f, 1f) : 1f;
      for (int index = 0; index < clampSmoothing; ++index)
        num3 = Mathf.Sin((float) ((double) num3 * 3.1415927410125732 * 0.5));
      float t = num3 * num2;
      clampValue = 1f - t;
      return Vector3.Slerp(normalDirection, direction, t);
    }

    public static Vector3 LineToPlane(
      Vector3 origin,
      Vector3 direction,
      Vector3 planeNormal,
      Vector3 planePoint)
    {
      float num1 = Vector3.Dot(planePoint - origin, planeNormal);
      float num2 = Vector3.Dot(direction, planeNormal);
      if ((double) num2 == 0.0)
        return Vector3.zero;
      float num3 = num1 / num2;
      return origin + direction.normalized * num3;
    }

    public static Vector3 PointToPlane(Vector3 point, Vector3 planePosition, Vector3 planeNormal)
    {
      if (planeNormal == Vector3.up)
        return new Vector3(point.x, planePosition.y, point.z);
      Vector3 tangent = point - planePosition;
      Vector3 normal = planeNormal;
      Vector3.OrthoNormalize(ref normal, ref tangent);
      return planePosition + Vector3.Project(point - planePosition, tangent);
    }
  }
}
