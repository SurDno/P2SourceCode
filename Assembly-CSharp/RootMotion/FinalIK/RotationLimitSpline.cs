using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page12.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Rotation Limits/Rotation Limit Spline")]
  public class RotationLimitSpline : RotationLimit
  {
    [Range(0.0f, 180f)]
    public float twistLimit = 180f;
    [SerializeField]
    [HideInInspector]
    public AnimationCurve spline;

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page12.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_rotation_limit_spline.html");
    }

    [ContextMenu("Support Group")]
    private void SupportGroup()
    {
      Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
    }

    [ContextMenu("Asset Store Thread")]
    private void ASThread()
    {
      Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
    }

    public void SetSpline(Keyframe[] keyframes) => spline.keys = keyframes;

    protected override Quaternion LimitRotation(Quaternion rotation)
    {
      return LimitTwist(LimitSwing(rotation), axis, secondaryAxis, twistLimit);
    }

    public Quaternion LimitSwing(Quaternion rotation)
    {
      if (axis == Vector3.zero || rotation == Quaternion.identity)
        return rotation;
      Vector3 vector3 = rotation * axis;
      float time = GetOrthogonalAngle(vector3, secondaryAxis, axis);
      if (Vector3.Dot(vector3, crossAxis) < 0.0)
        time = (float) (180.0 + (180.0 - time));
      float maxDegreesDelta = spline.Evaluate(time);
      Quaternion quaternion = Quaternion.RotateTowards(Quaternion.identity, Quaternion.FromToRotation(axis, vector3), maxDegreesDelta);
      return Quaternion.FromToRotation(vector3, quaternion * axis) * rotation;
    }
  }
}
