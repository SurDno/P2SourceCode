// Decompiled with JetBrains decompiler
// Type: Cinemachine.Utility.UnityQuaternionExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Cinemachine.Utility
{
  public static class UnityQuaternionExtensions
  {
    public static Quaternion SlerpWithReferenceUp(
      Quaternion qA,
      Quaternion qB,
      float t,
      Vector3 up)
    {
      Vector3 vector3 = (qA * Vector3.forward).ProjectOntoPlane(up);
      Vector3 v = (qB * Vector3.forward).ProjectOntoPlane(up);
      if (vector3.AlmostZero() || v.AlmostZero())
        return Quaternion.Slerp(qA, qB, t);
      Quaternion rotation = Quaternion.LookRotation(vector3, up);
      Quaternion quaternion1 = Quaternion.Inverse(rotation) * qA;
      Quaternion quaternion2 = Quaternion.Inverse(rotation) * qB;
      Vector3 eulerAngles1 = quaternion1.eulerAngles;
      Vector3 eulerAngles2 = quaternion2.eulerAngles;
      return rotation * Quaternion.Euler(Mathf.LerpAngle(eulerAngles1.x, eulerAngles2.x, t), Mathf.LerpAngle(eulerAngles1.y, eulerAngles2.y, t), Mathf.LerpAngle(eulerAngles1.z, eulerAngles2.z, t));
    }

    public static Quaternion Normalized(this Quaternion q)
    {
      Vector4 normalized = new Vector4(q.x, q.y, q.z, q.w).normalized;
      return new Quaternion(normalized.x, normalized.y, normalized.z, normalized.w);
    }

    public static Vector2 GetCameraRotationToTarget(
      this Quaternion orient,
      Vector3 lookAtDir,
      Vector3 worldUp)
    {
      if (lookAtDir.AlmostZero())
        return Vector2.zero;
      Quaternion quaternion1 = Quaternion.Inverse(orient);
      Vector3 vector3_1 = quaternion1 * worldUp;
      lookAtDir = quaternion1 * lookAtDir;
      float num = 0.0f;
      Vector3 vector3_2 = lookAtDir.ProjectOntoPlane(vector3_1);
      if (!vector3_2.AlmostZero())
      {
        Vector3 vector3_3 = Vector3.forward.ProjectOntoPlane(vector3_1);
        if (vector3_3.AlmostZero())
          vector3_3 = (double) Vector3.Dot(vector3_3, vector3_1) <= 0.0 ? Vector3.up.ProjectOntoPlane(vector3_1) : Vector3.down.ProjectOntoPlane(vector3_1);
        num = UnityVectorExtensions.SignedAngle(vector3_3, vector3_2, vector3_1);
      }
      Quaternion quaternion2 = Quaternion.AngleAxis(num, vector3_1);
      return new Vector2(UnityVectorExtensions.SignedAngle(quaternion2 * Vector3.forward, lookAtDir, quaternion2 * Vector3.right), num);
    }

    public static Quaternion ApplyCameraRotation(
      this Quaternion orient,
      Vector2 rot,
      Vector3 worldUp)
    {
      Quaternion quaternion = Quaternion.AngleAxis(rot.x, Vector3.right);
      return Quaternion.AngleAxis(rot.y, worldUp) * orient * quaternion;
    }
  }
}
