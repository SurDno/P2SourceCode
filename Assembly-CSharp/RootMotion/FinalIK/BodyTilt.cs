// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.BodyTilt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
      this.lastForward = this.transform.forward;
    }

    protected override void OnModifyOffset()
    {
      Quaternion rotation = Quaternion.FromToRotation(this.lastForward, this.transform.forward);
      float angle = 0.0f;
      Vector3 axis = Vector3.zero;
      rotation.ToAngleAxis(out angle, out axis);
      if ((double) axis.y > 0.0)
        angle = -angle;
      this.tiltAngle = Mathf.Lerp(this.tiltAngle, Mathf.Clamp(angle * (this.tiltSensitivity * 0.01f) / this.deltaTime, -1f, 1f), this.deltaTime * this.tiltSpeed);
      float weight = Mathf.Abs(this.tiltAngle) / 1f;
      if ((double) this.tiltAngle < 0.0)
        this.poseRight.Apply(this.ik.solver, weight);
      else
        this.poseLeft.Apply(this.ik.solver, weight);
      this.lastForward = this.transform.forward;
    }
  }
}
