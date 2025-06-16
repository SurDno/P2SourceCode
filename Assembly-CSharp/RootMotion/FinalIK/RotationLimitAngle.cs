// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.RotationLimitAngle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page12.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Rotation Limits/Rotation Limit Angle")]
  public class RotationLimitAngle : RotationLimit
  {
    [Range(0.0f, 180f)]
    public float limit = 45f;
    [Range(0.0f, 180f)]
    public float twistLimit = 180f;

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page12.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_rotation_limit_angle.html");
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
      return RotationLimit.LimitTwist(this.LimitSwing(rotation), this.axis, this.secondaryAxis, this.twistLimit);
    }

    private Quaternion LimitSwing(Quaternion rotation)
    {
      if (this.axis == Vector3.zero || rotation == Quaternion.identity || (double) this.limit >= 180.0)
        return rotation;
      Vector3 vector3 = rotation * this.axis;
      Quaternion quaternion = Quaternion.RotateTowards(Quaternion.identity, Quaternion.FromToRotation(this.axis, vector3), this.limit);
      return Quaternion.FromToRotation(vector3, quaternion * this.axis) * rotation;
    }
  }
}
