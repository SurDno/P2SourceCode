// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.TwistRelaxer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public class TwistRelaxer : MonoBehaviour
  {
    public IK ik;
    [Tooltip("The weight of relaxing the twist of this Transform")]
    [Range(0.0f, 1f)]
    public float weight = 1f;
    [Tooltip("If 0.5, this Transform will be twisted half way from parent to child. If 1, the twist angle will be locked to the child and will rotate with along with it.")]
    [Range(0.0f, 1f)]
    public float parentChildCrossfade = 0.5f;
    [Tooltip("Rotation offset around the twist axis.")]
    [Range(-180f, 180f)]
    public float twistAngleOffset;
    private Vector3 twistAxis = Vector3.right;
    private Vector3 axis = Vector3.forward;
    private Vector3 axisRelativeToParentDefault;
    private Vector3 axisRelativeToChildDefault;
    private Transform parent;
    private Transform child;

    public void Relax()
    {
      if ((double) this.weight <= 0.0)
        return;
      Quaternion rotation1 = this.transform.rotation;
      Quaternion quaternion1 = Quaternion.AngleAxis(this.twistAngleOffset, rotation1 * this.twistAxis);
      Quaternion quaternion2 = quaternion1 * rotation1;
      Vector3 vector3_1 = Vector3.Slerp(quaternion1 * this.parent.rotation * this.axisRelativeToParentDefault, quaternion1 * this.child.rotation * this.axisRelativeToChildDefault, this.parentChildCrossfade);
      Vector3 vector3_2 = Quaternion.Inverse(Quaternion.LookRotation(quaternion2 * this.axis, quaternion2 * this.twistAxis)) * vector3_1;
      float num = Mathf.Atan2(vector3_2.x, vector3_2.z) * 57.29578f;
      Quaternion rotation2 = this.child.rotation;
      this.transform.rotation = Quaternion.AngleAxis(num * this.weight, quaternion2 * this.twistAxis) * quaternion2;
      this.child.rotation = rotation2;
    }

    private void Start()
    {
      this.parent = this.transform.parent;
      if (this.transform.childCount == 0)
      {
        Transform[] componentsInChildren = this.parent.GetComponentsInChildren<Transform>();
        for (int index = 1; index < componentsInChildren.Length; ++index)
        {
          if ((Object) componentsInChildren[index] != (Object) this.transform)
          {
            this.child = componentsInChildren[index];
            break;
          }
        }
      }
      else
        this.child = this.transform.GetChild(0);
      this.twistAxis = this.transform.InverseTransformDirection(this.child.position - this.transform.position);
      this.axis = new Vector3(this.twistAxis.y, this.twistAxis.z, this.twistAxis.x);
      Vector3 vector3 = this.transform.rotation * this.axis;
      this.axisRelativeToParentDefault = Quaternion.Inverse(this.parent.rotation) * vector3;
      this.axisRelativeToChildDefault = Quaternion.Inverse(this.child.rotation) * vector3;
      if (!((Object) this.ik != (Object) null))
        return;
      this.ik.GetIKSolver().OnPostUpdate += new IKSolver.UpdateDelegate(this.OnPostUpdate);
    }

    private void OnPostUpdate()
    {
      if (!((Object) this.ik != (Object) null))
        return;
      this.Relax();
    }

    private void LateUpdate()
    {
      if (!((Object) this.ik == (Object) null))
        return;
      this.Relax();
    }

    private void OnDestroy()
    {
      if (!((Object) this.ik != (Object) null))
        return;
      this.ik.GetIKSolver().OnPostUpdate -= new IKSolver.UpdateDelegate(this.OnPostUpdate);
    }
  }
}
