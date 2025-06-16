using UnityEngine;

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
    Vector3 closestSurfacePosition;
    Quaternion closestSurfaceRotation;
    this.GetClosestSurfacePoint(this.transform.TransformPoint(this.transform.InverseTransformPoint(currentPosition)), out closestSurfacePosition, out closestSurfaceRotation);
    closestTargetRotation = closestSurfaceRotation;
    Vector3 vector3 = Quaternion.LookRotation(closestSurfacePosition - this.transform.position) * animationOffset;
    closestTargetPosition = closestSurfacePosition + vector3;
    this.lastSurfacePosition = closestSurfacePosition;
    this.lastClosestTargetPosition = closestTargetPosition;
  }

  private void GetClosestSurfacePoint(
    Vector3 surfacePosition,
    out Vector3 closestSurfacePosition,
    out Quaternion closestSurfaceRotation)
  {
    closestSurfacePosition = surfacePosition;
    closestSurfaceRotation = Quaternion.LookRotation(this.transform.position - closestSurfacePosition);
    Quaternion quaternion1 = Quaternion.LookRotation(closestSurfacePosition - this.transform.position);
    if ((double) this.Angle < 360.0)
    {
      Quaternion quaternion2 = this.transform.rotation * Quaternion.Euler(Vector3.up * (float) (-(double) this.Angle / 2.0));
      Quaternion quaternion3 = this.transform.rotation * Quaternion.Euler(Vector3.up * (this.Angle / 2f));
    }
    closestSurfacePosition = this.transform.position + quaternion1 * Vector3.forward * this.Radius;
  }

  public void GetRandomPointOnSurface(out Vector3 position, out Quaternion rotation)
  {
    float angle = (float) ((double) this.Angle / 2.0 - (double) this.Angle * (double) Random.value);
    rotation = this.transform.rotation * Quaternion.Euler(Vector3.up * angle);
    Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
    position = this.transform.position + quaternion * this.transform.forward * this.Radius;
  }

  private float ClampAngle(float a, float min, float max)
  {
    if ((double) max < (double) min)
      max += 360f;
    if ((double) a > (double) max)
      a -= 360f;
    if ((double) a < (double) min)
      a += 360f;
    if ((double) a <= (double) max)
      return a;
    if ((double) a - ((double) max + (double) min) * 0.5 >= 180.0)
      return min;
    if ((double) max >= 360.0)
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
    Vector3 position;
    Quaternion rotation;
    this.GetRandomPointOnSurface(out position, out rotation);
    targetRotation = rotation;
    Vector3 animationOffset = character.GetAnimationOffset(animation, animationIndex);
    targetPosition = this.transform.TransformPoint(this.transform.InverseTransformPoint(position));
    targetPosition += rotation * animationOffset;
  }
}
