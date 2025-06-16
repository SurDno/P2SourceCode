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
    Vector3 closestSurfacePosition;
    GetClosestSurfacePoint(this.transform.TransformPoint(this.transform.InverseTransformPoint(currentPosition) + animationOffset), out closestSurfacePosition, out Quaternion _);
    closestTargetRotation = this.transform.rotation;
    closestTargetPosition = this.transform.TransformPoint(this.transform.InverseTransformPoint(closestSurfacePosition) - animationOffset);
    lastSurfacePosition = closestSurfacePosition;
    lastClosestTargetPosition = closestTargetPosition;
  }

  private void GetClosestSurfacePoint(
    Vector3 surfacePosition,
    out Vector3 closestSurfacePosition,
    out Quaternion closestSurfaceRotation)
  {
    Vector3 vector3_1 = this.transform.position - this.transform.right * Width / 2f;
    Vector3 vector3_2 = this.transform.position + this.transform.right * Width / 2f;
    Vector3 rhs = vector3_2 - vector3_1;
    float num = Vector3.Dot(surfacePosition - vector3_1, rhs) / rhs.sqrMagnitude;
    closestSurfacePosition = num >= 0.0 ? (num <= 1.0 ? vector3_1 + rhs * num : vector3_2) : vector3_1;
    closestSurfaceRotation = this.transform.rotation;
  }

  public void GetRandomPointOnSurface(out Vector3 position, out Quaternion rotation)
  {
    rotation = this.transform.rotation;
    position = this.transform.position + this.transform.right * Width * (Random.value - 0.5f);
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
    Vector3 animationOffset = character.GetAnimationOffset(animation, animationIndex);
    targetPosition = this.transform.TransformPoint(this.transform.InverseTransformPoint(position) - animationOffset);
  }
}
