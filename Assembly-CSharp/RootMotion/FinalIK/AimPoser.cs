using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class AimPoser : MonoBehaviour
  {
    public float angleBuffer = 5f;
    public AimPoser.Pose[] poses = new AimPoser.Pose[0];

    public AimPoser.Pose GetPose(Vector3 localDirection)
    {
      if (this.poses.Length == 0)
        return (AimPoser.Pose) null;
      for (int index = 0; index < this.poses.Length - 1; ++index)
      {
        if (this.poses[index].IsInDirection(localDirection))
          return this.poses[index];
      }
      return this.poses[this.poses.Length - 1];
    }

    public void SetPoseActive(AimPoser.Pose pose)
    {
      for (int index = 0; index < this.poses.Length; ++index)
        this.poses[index].SetAngleBuffer(this.poses[index] == pose ? this.angleBuffer : 0.0f);
    }

    [Serializable]
    public class Pose
    {
      public bool visualize = true;
      public string name;
      public Vector3 direction;
      public float yaw = 75f;
      public float pitch = 45f;
      private float angleBuffer;

      public bool IsInDirection(Vector3 d)
      {
        if (this.direction == Vector3.zero || (double) this.yaw <= 0.0 || (double) this.pitch <= 0.0)
          return false;
        if ((double) this.yaw < 180.0)
        {
          Vector3 to = new Vector3(this.direction.x, 0.0f, this.direction.z);
          if (to == Vector3.zero)
            to = Vector3.forward;
          if ((double) Vector3.Angle(new Vector3(d.x, 0.0f, d.z), to) > (double) this.yaw + (double) this.angleBuffer)
            return false;
        }
        if ((double) this.pitch >= 180.0)
          return true;
        float num = Vector3.Angle(Vector3.up, this.direction);
        return (double) Mathf.Abs(Vector3.Angle(Vector3.up, d) - num) < (double) this.pitch + (double) this.angleBuffer;
      }

      public void SetAngleBuffer(float value) => this.angleBuffer = value;
    }
  }
}
