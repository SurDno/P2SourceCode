using UnityEngine;

public class POIPlane : POIBase
{
  public float Width;
  public float Length;
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
    Vector3 closestSurfacePosition;
    GetClosestSurfacePoint(currentPosition, out closestSurfacePosition, out Quaternion _);
    closestTargetRotation = character.transform.rotation;
    closestTargetPosition = transform.TransformPoint(transform.InverseTransformPoint(closestSurfacePosition));
    lastSurfacePosition = closestSurfacePosition;
    lastClosestTargetPosition = closestTargetPosition;
  }

  private void GetClosestSurfacePoint(
    Vector3 surfacePosition,
    out Vector3 closestSurfacePosition,
    out Quaternion closestSurfaceRotation)
  {
    Vector3 vector3 = transform.InverseTransformPoint(surfacePosition);
    Vector3 position = new Vector3(Mathf.Clamp(vector3.x, (float) (-(double) Width / 2.0), Width / 2f), 0.0f, Mathf.Clamp(vector3.z, (float) (-(double) Length / 2.0), Length / 2f));
    closestSurfacePosition = transform.TransformPoint(position);
    closestSurfaceRotation = transform.rotation;
  }

  public void GetRandomPointOnSurface(out Vector3 position, out Quaternion rotation)
  {
    rotation = transform.rotation;
    position = transform.position + transform.right * Width / 2f * (float) (2.0 * (Random.value - 0.5)) + transform.forward * Length / 2f * (float) (2.0 * (Random.value - 0.5));
  }

  public override void GetRandomTargetPoint(
    POIAnimationEnum animation,
    int animationIndex,
    POISetup character,
    out Vector3 targetPosition,
    out Quaternion targetRotation)
  {
    Vector3 position;
    Quaternion rotation;
    GetRandomPointOnSurface(out position, out rotation);
    targetRotation = rotation;
    targetPosition = position;
  }
}
