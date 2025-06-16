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
      if (weight <= 0.0)
        return;
      Quaternion rotation1 = this.transform.rotation;
      Quaternion quaternion1 = Quaternion.AngleAxis(twistAngleOffset, rotation1 * twistAxis);
      Quaternion quaternion2 = quaternion1 * rotation1;
      Vector3 vector3_1 = Vector3.Slerp(quaternion1 * parent.rotation * axisRelativeToParentDefault, quaternion1 * child.rotation * axisRelativeToChildDefault, parentChildCrossfade);
      Vector3 vector3_2 = Quaternion.Inverse(Quaternion.LookRotation(quaternion2 * axis, quaternion2 * twistAxis)) * vector3_1;
      float num = Mathf.Atan2(vector3_2.x, vector3_2.z) * 57.29578f;
      Quaternion rotation2 = child.rotation;
      this.transform.rotation = Quaternion.AngleAxis(num * weight, quaternion2 * twistAxis) * quaternion2;
      child.rotation = rotation2;
    }

    private void Start()
    {
      parent = this.transform.parent;
      if (this.transform.childCount == 0)
      {
        Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
        for (int index = 1; index < componentsInChildren.Length; ++index)
        {
          if ((Object) componentsInChildren[index] != (Object) this.transform)
          {
            child = componentsInChildren[index];
            break;
          }
        }
      }
      else
        child = this.transform.GetChild(0);
      twistAxis = this.transform.InverseTransformDirection(child.position - this.transform.position);
      axis = new Vector3(twistAxis.y, twistAxis.z, twistAxis.x);
      Vector3 vector3 = this.transform.rotation * axis;
      axisRelativeToParentDefault = Quaternion.Inverse(parent.rotation) * vector3;
      axisRelativeToChildDefault = Quaternion.Inverse(child.rotation) * vector3;
      if (!((Object) ik != (Object) null))
        return;
      ik.GetIKSolver().OnPostUpdate += OnPostUpdate;
    }

    private void OnPostUpdate()
    {
      if (!((Object) ik != (Object) null))
        return;
      Relax();
    }

    private void LateUpdate()
    {
      if (!((Object) ik == (Object) null))
        return;
      Relax();
    }

    private void OnDestroy()
    {
      if (!((Object) ik != (Object) null))
        return;
      ik.GetIKSolver().OnPostUpdate -= OnPostUpdate;
    }
  }
}
