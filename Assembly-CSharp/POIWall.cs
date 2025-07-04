﻿using UnityEngine;

public class POIWall : POIBase
{
  public float Width;
  public float Height;
  public float VerticalOffset;
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
    GetClosestSurfacePoint(transform.TransformPoint(transform.InverseTransformPoint(currentPosition) + animationOffset), out Vector3 closestSurfacePosition, out Quaternion _);
    closestTargetRotation = transform.rotation;
    closestTargetPosition = transform.TransformPoint(transform.InverseTransformPoint(closestSurfacePosition) - animationOffset);
    lastSurfacePosition = closestSurfacePosition;
    lastClosestTargetPosition = closestTargetPosition;
  }

  private void GetClosestSurfacePoint(
    Vector3 surfacePosition,
    out Vector3 closestSurfacePosition,
    out Quaternion closestSurfaceRotation)
  {
    Vector3 vector3_1 = transform.position - transform.right * Width / 2f;
    Vector3 vector3_2 = transform.position + transform.right * Width / 2f;
    Vector3 rhs = vector3_2 - vector3_1;
    float num = Vector3.Dot(surfacePosition - vector3_1, rhs) / rhs.sqrMagnitude;
    closestSurfacePosition = num >= 0.0 ? (num <= 1.0 ? vector3_1 + rhs * num : vector3_2) : vector3_1;
    closestSurfaceRotation = transform.rotation;
  }

  public void GetRandomPointOnSurface(out Vector3 position, out Quaternion rotation)
  {
    rotation = transform.rotation;
    position = transform.position + transform.right * Width * (Random.value - 0.5f);
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
    targetPosition = transform.TransformPoint(transform.InverseTransformPoint(position) - animationOffset);
  }
}
