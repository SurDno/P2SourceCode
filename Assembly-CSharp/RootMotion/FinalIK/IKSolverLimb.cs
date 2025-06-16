// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverLimb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverLimb : IKSolverTrigonometric
  {
    public AvatarIKGoal goal;
    public IKSolverLimb.BendModifier bendModifier;
    [Range(0.0f, 1f)]
    public float maintainRotationWeight;
    [Range(0.0f, 1f)]
    public float bendModifierWeight = 1f;
    public Transform bendGoal;
    private bool maintainBendFor1Frame;
    private bool maintainRotationFor1Frame;
    private Quaternion defaultRootRotation;
    private Quaternion parentDefaultRotation;
    private Quaternion bone3RotationBeforeSolve;
    private Quaternion maintainRotation;
    private Quaternion bone3DefaultRotation;
    private Vector3 _bendNormal;
    private Vector3 animationNormal;
    private IKSolverLimb.AxisDirection[] axisDirectionsLeft = new IKSolverLimb.AxisDirection[4];
    private IKSolverLimb.AxisDirection[] axisDirectionsRight = new IKSolverLimb.AxisDirection[4];

    public void MaintainRotation()
    {
      if (!this.initiated)
        return;
      this.maintainRotation = this.bone3.transform.rotation;
      this.maintainRotationFor1Frame = true;
    }

    public void MaintainBend()
    {
      if (!this.initiated)
        return;
      this.animationNormal = this.bone1.GetBendNormalFromCurrentRotation();
      this.maintainBendFor1Frame = true;
    }

    protected override void OnInitiateVirtual()
    {
      this.defaultRootRotation = this.root.rotation;
      if ((UnityEngine.Object) this.bone1.transform.parent != (UnityEngine.Object) null)
        this.parentDefaultRotation = Quaternion.Inverse(this.defaultRootRotation) * this.bone1.transform.parent.rotation;
      if ((UnityEngine.Object) this.bone3.rotationLimit != (UnityEngine.Object) null)
        this.bone3.rotationLimit.Disable();
      this.bone3DefaultRotation = this.bone3.transform.rotation;
      Vector3 vector3 = Vector3.Cross(this.bone2.transform.position - this.bone1.transform.position, this.bone3.transform.position - this.bone2.transform.position);
      if (vector3 != Vector3.zero)
        this.bendNormal = vector3;
      this.animationNormal = this.bendNormal;
      this.StoreAxisDirections(ref this.axisDirectionsLeft);
      this.StoreAxisDirections(ref this.axisDirectionsRight);
    }

    protected override void OnUpdateVirtual()
    {
      if ((double) this.IKPositionWeight > 0.0)
      {
        this.bendModifierWeight = Mathf.Clamp(this.bendModifierWeight, 0.0f, 1f);
        this.maintainRotationWeight = Mathf.Clamp(this.maintainRotationWeight, 0.0f, 1f);
        this._bendNormal = this.bendNormal;
        this.bendNormal = this.GetModifiedBendNormal();
      }
      if ((double) this.maintainRotationWeight * (double) this.IKPositionWeight <= 0.0)
        return;
      this.bone3RotationBeforeSolve = this.maintainRotationFor1Frame ? this.maintainRotation : this.bone3.transform.rotation;
      this.maintainRotationFor1Frame = false;
    }

    protected override void OnPostSolveVirtual()
    {
      if ((double) this.IKPositionWeight > 0.0)
        this.bendNormal = this._bendNormal;
      if ((double) this.maintainRotationWeight * (double) this.IKPositionWeight <= 0.0)
        return;
      this.bone3.transform.rotation = Quaternion.Slerp(this.bone3.transform.rotation, this.bone3RotationBeforeSolve, this.maintainRotationWeight * this.IKPositionWeight);
    }

    public IKSolverLimb()
    {
    }

    public IKSolverLimb(AvatarIKGoal goal) => this.goal = goal;

    private IKSolverLimb.AxisDirection[] axisDirections
    {
      get
      {
        return this.goal == AvatarIKGoal.LeftHand ? this.axisDirectionsLeft : this.axisDirectionsRight;
      }
    }

    private void StoreAxisDirections(ref IKSolverLimb.AxisDirection[] axisDirections)
    {
      axisDirections[0] = new IKSolverLimb.AxisDirection(Vector3.zero, new Vector3(-1f, 0.0f, 0.0f));
      axisDirections[1] = new IKSolverLimb.AxisDirection(new Vector3(0.5f, 0.0f, -0.2f), new Vector3(-0.5f, -1f, 1f));
      axisDirections[2] = new IKSolverLimb.AxisDirection(new Vector3(-0.5f, -1f, -0.2f), new Vector3(0.0f, 0.5f, -1f));
      axisDirections[3] = new IKSolverLimb.AxisDirection(new Vector3(-0.5f, -0.5f, 1f), new Vector3(-1f, -1f, -1f));
    }

    private Vector3 GetModifiedBendNormal()
    {
      float bendModifierWeight = this.bendModifierWeight;
      if ((double) bendModifierWeight <= 0.0)
        return this.bendNormal;
      switch (this.bendModifier)
      {
        case IKSolverLimb.BendModifier.Animation:
          if (!this.maintainBendFor1Frame)
            this.MaintainBend();
          this.maintainBendFor1Frame = false;
          return Vector3.Lerp(this.bendNormal, this.animationNormal, bendModifierWeight);
        case IKSolverLimb.BendModifier.Target:
          return Quaternion.Slerp(Quaternion.identity, this.IKRotation * Quaternion.Inverse(this.bone3DefaultRotation), bendModifierWeight) * this.bendNormal;
        case IKSolverLimb.BendModifier.Parent:
          return (UnityEngine.Object) this.bone1.transform.parent == (UnityEngine.Object) null ? this.bendNormal : Quaternion.Slerp(Quaternion.identity, this.bone1.transform.parent.rotation * Quaternion.Inverse(this.parentDefaultRotation) * Quaternion.Inverse(this.defaultRootRotation), bendModifierWeight) * this.bendNormal;
        case IKSolverLimb.BendModifier.Arm:
          if ((UnityEngine.Object) this.bone1.transform.parent == (UnityEngine.Object) null)
            return this.bendNormal;
          if (this.goal == AvatarIKGoal.LeftFoot || this.goal == AvatarIKGoal.RightFoot)
          {
            if (!Warning.logged)
              this.LogWarning("Trying to use the 'Arm' bend modifier on a leg.");
            return this.bendNormal;
          }
          Vector3 normalized = (this.IKPosition - this.bone1.transform.position).normalized;
          Vector3 rhs = Quaternion.Inverse(this.bone1.transform.parent.rotation * Quaternion.Inverse(this.parentDefaultRotation)) * normalized;
          if (this.goal == AvatarIKGoal.LeftHand)
            rhs.x = -rhs.x;
          for (int index = 1; index < this.axisDirections.Length; ++index)
          {
            this.axisDirections[index].dot = Mathf.Clamp(Vector3.Dot(this.axisDirections[index].direction, rhs), 0.0f, 1f);
            this.axisDirections[index].dot = Interp.Float(this.axisDirections[index].dot, InterpolationMode.InOutQuintic);
          }
          Vector3 a = this.axisDirections[0].axis;
          for (int index = 1; index < this.axisDirections.Length; ++index)
            a = Vector3.Slerp(a, this.axisDirections[index].axis, this.axisDirections[index].dot);
          if (this.goal == AvatarIKGoal.LeftHand)
          {
            a.x = -a.x;
            a = -a;
          }
          Vector3 b1 = this.bone1.transform.parent.rotation * Quaternion.Inverse(this.parentDefaultRotation) * a;
          return (double) bendModifierWeight >= 1.0 ? b1 : Vector3.Lerp(this.bendNormal, b1, bendModifierWeight);
        case IKSolverLimb.BendModifier.Goal:
          if ((UnityEngine.Object) this.bendGoal == (UnityEngine.Object) null)
          {
            if (!Warning.logged)
              this.LogWarning("Trying to use the 'Goal' Bend Modifier, but the Bend Goal is unassigned.");
            return this.bendNormal;
          }
          Vector3 b2 = Vector3.Cross(this.bendGoal.position - this.bone1.transform.position, this.IKPosition - this.bone1.transform.position);
          if (b2 == Vector3.zero)
            return this.bendNormal;
          return (double) bendModifierWeight >= 1.0 ? b2 : Vector3.Lerp(this.bendNormal, b2, bendModifierWeight);
        default:
          return this.bendNormal;
      }
    }

    [Serializable]
    public enum BendModifier
    {
      Animation,
      Target,
      Parent,
      Arm,
      Goal,
    }

    [Serializable]
    public struct AxisDirection
    {
      public Vector3 direction;
      public Vector3 axis;
      public float dot;

      public AxisDirection(Vector3 direction, Vector3 axis)
      {
        this.direction = direction.normalized;
        this.axis = axis.normalized;
        this.dot = 0.0f;
      }
    }
  }
}
