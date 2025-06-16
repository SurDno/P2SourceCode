namespace RootMotion.FinalIK
{
  public class BodyTilt : OffsetModifier
  {
    [Tooltip("Speed of tilting")]
    public float tiltSpeed = 6f;
    [Tooltip("Sensitivity of tilting")]
    public float tiltSensitivity = 0.07f;
    [Tooltip("The OffsetPose components")]
    public OffsetPose poseLeft;
    [Tooltip("The OffsetPose components")]
    public OffsetPose poseRight;
    private float tiltAngle;
    private Vector3 lastForward;

    protected override void Start()
    {
      base.Start();
      lastForward = this.transform.forward;
    }

    protected override void OnModifyOffset()
    {
      Quaternion rotation = Quaternion.FromToRotation(lastForward, this.transform.forward);
      float angle = 0.0f;
      Vector3 axis = Vector3.zero;
      rotation.ToAngleAxis(out angle, out axis);
      if ((double) axis.y > 0.0)
        angle = -angle;
      tiltAngle = Mathf.Lerp(tiltAngle, Mathf.Clamp(angle * (tiltSensitivity * 0.01f) / deltaTime, -1f, 1f), deltaTime * tiltSpeed);
      float weight = Mathf.Abs(tiltAngle) / 1f;
      if (tiltAngle < 0.0)
        poseRight.Apply(ik.solver, weight);
      else
        poseLeft.Apply(ik.solver, weight);
      lastForward = this.transform.forward;
    }
  }
}
