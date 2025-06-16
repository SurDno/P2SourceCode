// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.RotationLimit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public abstract class RotationLimit : MonoBehaviour
  {
    public Vector3 axis = Vector3.forward;
    [HideInInspector]
    public Quaternion defaultLocalRotation;
    private bool initiated;
    private bool applicationQuit;
    private bool defaultLocalRotationSet;

    public void SetDefaultLocalRotation()
    {
      this.defaultLocalRotation = this.transform.localRotation;
      this.defaultLocalRotationSet = true;
    }

    public Quaternion GetLimitedLocalRotation(Quaternion localRotation, out bool changed)
    {
      if (!this.initiated)
        this.Awake();
      Quaternion rotation = Quaternion.Inverse(this.defaultLocalRotation) * localRotation;
      Quaternion quaternion = this.LimitRotation(rotation);
      changed = quaternion != rotation;
      return !changed ? localRotation : this.defaultLocalRotation * quaternion;
    }

    public bool Apply()
    {
      bool changed = false;
      this.transform.localRotation = this.GetLimitedLocalRotation(this.transform.localRotation, out changed);
      return changed;
    }

    public void Disable()
    {
      if (this.initiated)
      {
        this.enabled = false;
      }
      else
      {
        this.Awake();
        this.enabled = false;
      }
    }

    public Vector3 secondaryAxis => new Vector3(this.axis.y, this.axis.z, this.axis.x);

    public Vector3 crossAxis => Vector3.Cross(this.axis, this.secondaryAxis);

    protected abstract Quaternion LimitRotation(Quaternion rotation);

    private void Awake()
    {
      if (!this.defaultLocalRotationSet)
        this.SetDefaultLocalRotation();
      if (this.axis == Vector3.zero)
        Debug.LogError((object) "Axis is Vector3.zero.");
      this.initiated = true;
    }

    private void LateUpdate() => this.Apply();

    public void LogWarning(string message) => Warning.Log(message, this.transform);

    protected static Quaternion Limit1DOF(Quaternion rotation, Vector3 axis)
    {
      return Quaternion.FromToRotation(rotation * axis, axis) * rotation;
    }

    protected static Quaternion LimitTwist(
      Quaternion rotation,
      Vector3 axis,
      Vector3 orthoAxis,
      float twistLimit)
    {
      twistLimit = Mathf.Clamp(twistLimit, 0.0f, 180f);
      if ((double) twistLimit >= 180.0)
        return rotation;
      Vector3 normal = rotation * axis;
      Vector3 tangent1 = orthoAxis;
      Vector3.OrthoNormalize(ref normal, ref tangent1);
      Vector3 tangent2 = rotation * orthoAxis;
      Vector3.OrthoNormalize(ref normal, ref tangent2);
      Quaternion from = Quaternion.FromToRotation(tangent2, tangent1) * rotation;
      return (double) twistLimit <= 0.0 ? from : Quaternion.RotateTowards(from, rotation, twistLimit);
    }

    protected static float GetOrthogonalAngle(Vector3 v1, Vector3 v2, Vector3 normal)
    {
      Vector3.OrthoNormalize(ref normal, ref v1);
      Vector3.OrthoNormalize(ref normal, ref v2);
      return Vector3.Angle(v1, v2);
    }
  }
}
