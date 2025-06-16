using UnityEngine;

namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page12.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Rotation Limits/Rotation Limit Hinge")]
  public class RotationLimitHinge : RotationLimit
  {
    public bool useLimits = true;
    public float min = -45f;
    public float max = 90f;
    [HideInInspector]
    public float zeroAxisDisplayOffset;
    private Quaternion lastRotation = Quaternion.identity;
    private float lastAngle;

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page12.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_rotation_limit_hinge.html");
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

    protected override Quaternion LimitRotation(Quaternion rotation)
    {
      this.lastRotation = this.LimitHinge(rotation);
      return this.lastRotation;
    }

    private Quaternion LimitHinge(Quaternion rotation)
    {
      if ((double) this.min == 0.0 && (double) this.max == 0.0 && this.useLimits)
        return Quaternion.AngleAxis(0.0f, this.axis);
      Quaternion quaternion = RotationLimit.Limit1DOF(rotation, this.axis);
      if (!this.useLimits)
        return quaternion;
      Quaternion b = quaternion * Quaternion.Inverse(this.lastRotation);
      float num = Quaternion.Angle(Quaternion.identity, b);
      Vector3 lhs = new Vector3(this.axis.z, this.axis.x, this.axis.y);
      Vector3 rhs = Vector3.Cross(lhs, this.axis);
      if ((double) Vector3.Dot(b * lhs, rhs) > 0.0)
        num = -num;
      this.lastAngle = Mathf.Clamp(this.lastAngle + num, this.min, this.max);
      return Quaternion.AngleAxis(this.lastAngle, this.axis);
    }
  }
}
