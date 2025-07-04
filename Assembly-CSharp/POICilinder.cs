﻿using UnityEngine;

public class POICilinder : POIBase
{
  public float AngleOffset;
  public float Angle;
  public float Height;
  public float Radius;
  private Vector3 lastSurfacePosition;
  private Vector3 lastClosestTargetPosition;

  public override void GetClosestTargetPoint(
    POIAnimationEnum animation,
    int animationIndex,
    POISetup character,
    Vector3 currentPosition,
    out Vector3 closestTargetPosition,
    out Quaternion closestTargetRotation)
  {
    Vector3 animationOffset = character.GetAnimationOffset(animation, animationIndex);
    GetClosestSurfacePoint(transform.TransformPoint(transform.InverseTransformPoint(currentPosition)), out Vector3 closestSurfacePosition, out Quaternion closestSurfaceRotation);
    closestTargetRotation = closestSurfaceRotation;
    Vector3 vector3 = Quaternion.LookRotation(closestSurfacePosition - transform.position) * animationOffset;
    closestTargetPosition = closestSurfacePosition + vector3;
    lastSurfacePosition = closestSurfacePosition;
    lastClosestTargetPosition = closestTargetPosition;
  }

  private void GetClosestSurfacePoint(
    Vector3 surfacePosition,
    out Vector3 closestSurfacePosition,
    out Quaternion closestSurfaceRotation)
  {
    closestSurfacePosition = surfacePosition;
    closestSurfaceRotation = Quaternion.LookRotation(transform.position - closestSurfacePosition);
    Quaternion quaternion1 = Quaternion.LookRotation(closestSurfacePosition - transform.position);
    if (Angle < 360.0)
    {
      Quaternion quaternion2 = transform.rotation * Quaternion.Euler(Vector3.up * (float) (-(double) Angle / 2.0));
      Quaternion quaternion3 = transform.rotation * Quaternion.Euler(Vector3.up * (Angle / 2f));
    }
    closestSurfacePosition = transform.position + quaternion1 * Vector3.forward * Radius;
  }

  public void GetRandomPointOnSurface(out Vector3 position, out Quaternion rotation)
  {
    float angle = (float) (Angle / 2.0 - Angle * (double) Random.value);
    rotation = transform.rotation * Quaternion.Euler(Vector3.up * angle);
    Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
    position = transform.position + quaternion * transform.forward * Radius;
  }

  private float ClampAngle(float a, float min, float max)
  {
    if (max < (double) min)
      max += 360f;
    if (a > (double) max)
      a -= 360f;
    if (a < (double) min)
      a += 360f;
    if (a <= (double) max)
      return a;
    if (a - (max + (double) min) * 0.5 >= 180.0)
      return min;
    if (max >= 360.0)
      max -= 360f;
    return max;
  }

  public override void GetRandomTargetPoint(
    POIAnimationEnum animation,
    int animationIndex,
    POISetup character,
    out Vector3 targetPosition,
    out Quaternion targetRotation)
  {
    GetRandomPointOnSurface(out Vector3 position, out Quaternion rotation);
    targetRotation = rotation;
    Vector3 animationOffset = character.GetAnimationOffset(animation, animationIndex);
    targetPosition = transform.TransformPoint(transform.InverseTransformPoint(position));
    targetPosition += rotation * animationOffset;
  }
}
