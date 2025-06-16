using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class AimPoser : MonoBehaviour
  {
    public float angleBuffer = 5f;
    public Pose[] poses = new Pose[0];

    public Pose GetPose(Vector3 localDirection)
    {
      if (poses.Length == 0)
        return null;
      for (int index = 0; index < poses.Length - 1; ++index)
      {
        if (poses[index].IsInDirection(localDirection))
          return poses[index];
      }
      return poses[poses.Length - 1];
    }

    public void SetPoseActive(Pose pose)
    {
      for (int index = 0; index < poses.Length; ++index)
        poses[index].SetAngleBuffer(poses[index] == pose ? angleBuffer : 0.0f);
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
        if (direction == Vector3.zero || yaw <= 0.0 || pitch <= 0.0)
          return false;
        if (yaw < 180.0)
        {
          Vector3 to = new Vector3(direction.x, 0.0f, direction.z);
          if (to == Vector3.zero)
            to = Vector3.forward;
          if (Vector3.Angle(new Vector3(d.x, 0.0f, d.z), to) > yaw + (double) angleBuffer)
            return false;
        }
        if (pitch >= 180.0)
          return true;
        float num = Vector3.Angle(Vector3.up, direction);
        return Mathf.Abs(Vector3.Angle(Vector3.up, d) - num) < pitch + (double) angleBuffer;
      }

      public void SetAngleBuffer(float value) => angleBuffer = value;
    }
  }
}
