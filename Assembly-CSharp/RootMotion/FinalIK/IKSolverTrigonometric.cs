// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverTrigonometric
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverTrigonometric : IKSolver
  {
    public Transform target;
    [Range(0.0f, 1f)]
    public float IKRotationWeight = 1f;
    public Quaternion IKRotation = Quaternion.identity;
    public Vector3 bendNormal = Vector3.right;
    public IKSolverTrigonometric.TrigonometricBone bone1 = new IKSolverTrigonometric.TrigonometricBone();
    public IKSolverTrigonometric.TrigonometricBone bone2 = new IKSolverTrigonometric.TrigonometricBone();
    public IKSolverTrigonometric.TrigonometricBone bone3 = new IKSolverTrigonometric.TrigonometricBone();
    protected Vector3 weightIKPosition;
    protected bool directHierarchy = true;

    public void SetBendGoalPosition(Vector3 goalPosition, float weight)
    {
      if (!this.initiated || (double) weight <= 0.0)
        return;
      Vector3 b = Vector3.Cross(goalPosition - this.bone1.transform.position, this.IKPosition - this.bone1.transform.position);
      if (!(b != Vector3.zero))
        return;
      if ((double) weight >= 1.0)
        this.bendNormal = b;
      else
        this.bendNormal = Vector3.Lerp(this.bendNormal, b, weight);
    }

    public void SetBendPlaneToCurrent()
    {
      if (!this.initiated)
        return;
      Vector3 vector3 = Vector3.Cross(this.bone2.transform.position - this.bone1.transform.position, this.bone3.transform.position - this.bone2.transform.position);
      if (!(vector3 != Vector3.zero))
        return;
      this.bendNormal = vector3;
    }

    public void SetIKRotation(Quaternion rotation) => this.IKRotation = rotation;

    public void SetIKRotationWeight(float weight)
    {
      this.IKRotationWeight = Mathf.Clamp(weight, 0.0f, 1f);
    }

    public Quaternion GetIKRotation() => this.IKRotation;

    public float GetIKRotationWeight() => this.IKRotationWeight;

    public override IKSolver.Point[] GetPoints()
    {
      return new IKSolver.Point[3]
      {
        (IKSolver.Point) this.bone1,
        (IKSolver.Point) this.bone2,
        (IKSolver.Point) this.bone3
      };
    }

    public override IKSolver.Point GetPoint(Transform transform)
    {
      if ((UnityEngine.Object) this.bone1.transform == (UnityEngine.Object) transform)
        return (IKSolver.Point) this.bone1;
      if ((UnityEngine.Object) this.bone2.transform == (UnityEngine.Object) transform)
        return (IKSolver.Point) this.bone2;
      return (UnityEngine.Object) this.bone3.transform == (UnityEngine.Object) transform ? (IKSolver.Point) this.bone3 : (IKSolver.Point) null;
    }

    public override void StoreDefaultLocalState()
    {
      this.bone1.StoreDefaultLocalState();
      this.bone2.StoreDefaultLocalState();
      this.bone3.StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!this.initiated)
        return;
      this.bone1.FixTransform();
      this.bone2.FixTransform();
      this.bone3.FixTransform();
    }

    public override bool IsValid(ref string message)
    {
      if ((UnityEngine.Object) this.bone1.transform == (UnityEngine.Object) null || (UnityEngine.Object) this.bone2.transform == (UnityEngine.Object) null || (UnityEngine.Object) this.bone3.transform == (UnityEngine.Object) null)
      {
        message = "Please assign all Bones to the IK solver.";
        return false;
      }
      Transform transform = (Transform) Hierarchy.ContainsDuplicate((UnityEngine.Object[]) new Transform[3]
      {
        this.bone1.transform,
        this.bone2.transform,
        this.bone3.transform
      });
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      {
        message = transform.name + " is represented multiple times in the Bones.";
        return false;
      }
      if (this.bone1.transform.position == this.bone2.transform.position)
      {
        message = "first bone position is the same as second bone position.";
        return false;
      }
      if (!(this.bone2.transform.position == this.bone3.transform.position))
        return true;
      message = "second bone position is the same as third bone position.";
      return false;
    }

    public bool SetChain(Transform bone1, Transform bone2, Transform bone3, Transform root)
    {
      this.bone1.transform = bone1;
      this.bone2.transform = bone2;
      this.bone3.transform = bone3;
      this.Initiate(root);
      return this.initiated;
    }

    public static void Solve(
      Transform bone1,
      Transform bone2,
      Transform bone3,
      Vector3 targetPosition,
      Vector3 bendNormal,
      float weight)
    {
      if ((double) weight <= 0.0)
        return;
      targetPosition = Vector3.Lerp(bone3.position, targetPosition, weight);
      Vector3 vector3_1 = targetPosition - bone1.position;
      float magnitude = vector3_1.magnitude;
      if ((double) magnitude == 0.0)
        return;
      Vector3 vector3_2 = bone2.position - bone1.position;
      float sqrMagnitude1 = vector3_2.sqrMagnitude;
      vector3_2 = bone3.position - bone2.position;
      float sqrMagnitude2 = vector3_2.sqrMagnitude;
      Vector3 bendDirection = Vector3.Cross(vector3_1, bendNormal);
      Vector3 directionToBendPoint = IKSolverTrigonometric.GetDirectionToBendPoint(vector3_1, magnitude, bendDirection, sqrMagnitude1, sqrMagnitude2);
      Quaternion b1 = Quaternion.FromToRotation(bone2.position - bone1.position, directionToBendPoint);
      if ((double) weight < 1.0)
        b1 = Quaternion.Lerp(Quaternion.identity, b1, weight);
      bone1.rotation = b1 * bone1.rotation;
      Quaternion b2 = Quaternion.FromToRotation(bone3.position - bone2.position, targetPosition - bone2.position);
      if ((double) weight < 1.0)
        b2 = Quaternion.Lerp(Quaternion.identity, b2, weight);
      bone2.rotation = b2 * bone2.rotation;
    }

    private static Vector3 GetDirectionToBendPoint(
      Vector3 direction,
      float directionMag,
      Vector3 bendDirection,
      float sqrMag1,
      float sqrMag2)
    {
      float z = (float) (((double) directionMag * (double) directionMag + ((double) sqrMag1 - (double) sqrMag2)) / 2.0) / directionMag;
      float y = (float) Math.Sqrt((double) Mathf.Clamp(sqrMag1 - z * z, 0.0f, float.PositiveInfinity));
      return direction == Vector3.zero ? Vector3.zero : Quaternion.LookRotation(direction, bendDirection) * new Vector3(0.0f, y, z);
    }

    protected override void OnInitiate()
    {
      if (this.bendNormal == Vector3.zero)
        this.bendNormal = Vector3.right;
      this.OnInitiateVirtual();
      this.IKPosition = this.bone3.transform.position;
      this.IKRotation = this.bone3.transform.rotation;
      this.InitiateBones();
      this.directHierarchy = this.IsDirectHierarchy();
    }

    private bool IsDirectHierarchy()
    {
      return !((UnityEngine.Object) this.bone3.transform.parent != (UnityEngine.Object) this.bone2.transform) && !((UnityEngine.Object) this.bone2.transform.parent != (UnityEngine.Object) this.bone1.transform);
    }

    private void InitiateBones()
    {
      this.bone1.Initiate(this.bone2.transform.position, this.bendNormal);
      this.bone2.Initiate(this.bone3.transform.position, this.bendNormal);
      this.SetBendPlaneToCurrent();
    }

    protected override void OnUpdate()
    {
      this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0.0f, 1f);
      this.IKRotationWeight = Mathf.Clamp(this.IKRotationWeight, 0.0f, 1f);
      if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
      {
        this.IKPosition = this.target.position;
        this.IKRotation = this.target.rotation;
      }
      this.OnUpdateVirtual();
      if ((double) this.IKPositionWeight > 0.0)
      {
        if (!this.directHierarchy)
        {
          this.bone1.Initiate(this.bone2.transform.position, this.bendNormal);
          this.bone2.Initiate(this.bone3.transform.position, this.bendNormal);
        }
        this.bone1.sqrMag = (this.bone2.transform.position - this.bone1.transform.position).sqrMagnitude;
        this.bone2.sqrMag = (this.bone3.transform.position - this.bone2.transform.position).sqrMagnitude;
        if (this.bendNormal == Vector3.zero && !Warning.logged)
          this.LogWarning("IKSolverTrigonometric Bend Normal is Vector3.zero.");
        this.weightIKPosition = Vector3.Lerp(this.bone3.transform.position, this.IKPosition, this.IKPositionWeight);
        Vector3 bendNormal = Vector3.Lerp(this.bone1.GetBendNormalFromCurrentRotation(), this.bendNormal, this.IKPositionWeight);
        Vector3 direction = Vector3.Lerp(this.bone2.transform.position - this.bone1.transform.position, this.GetBendDirection(this.weightIKPosition, bendNormal), this.IKPositionWeight);
        if (direction == Vector3.zero)
          direction = this.bone2.transform.position - this.bone1.transform.position;
        this.bone1.transform.rotation = this.bone1.GetRotation(direction, bendNormal);
        this.bone2.transform.rotation = this.bone2.GetRotation(this.weightIKPosition - this.bone2.transform.position, this.bone2.GetBendNormalFromCurrentRotation());
      }
      if ((double) this.IKRotationWeight > 0.0)
        this.bone3.transform.rotation = Quaternion.Slerp(this.bone3.transform.rotation, this.IKRotation, this.IKRotationWeight);
      this.OnPostSolveVirtual();
    }

    protected virtual void OnInitiateVirtual()
    {
    }

    protected virtual void OnUpdateVirtual()
    {
    }

    protected virtual void OnPostSolveVirtual()
    {
    }

    protected Vector3 GetBendDirection(Vector3 IKPosition, Vector3 bendNormal)
    {
      Vector3 vector3 = IKPosition - this.bone1.transform.position;
      if (vector3 == Vector3.zero)
        return Vector3.zero;
      float sqrMagnitude = vector3.sqrMagnitude;
      float num = (float) Math.Sqrt((double) sqrMagnitude);
      float z = (float) (((double) sqrMagnitude + (double) this.bone1.sqrMag - (double) this.bone2.sqrMag) / 2.0) / num;
      float y = (float) Math.Sqrt((double) Mathf.Clamp(this.bone1.sqrMag - z * z, 0.0f, float.PositiveInfinity));
      Vector3 upwards = Vector3.Cross(vector3, bendNormal);
      return Quaternion.LookRotation(vector3, upwards) * new Vector3(0.0f, y, z);
    }

    [Serializable]
    public class TrigonometricBone : IKSolver.Bone
    {
      private Quaternion targetToLocalSpace;
      private Vector3 defaultLocalBendNormal;

      public void Initiate(Vector3 childPosition, Vector3 bendNormal)
      {
        this.targetToLocalSpace = QuaTools.RotationToLocalSpace(this.transform.rotation, Quaternion.LookRotation(childPosition - this.transform.position, bendNormal));
        this.defaultLocalBendNormal = Quaternion.Inverse(this.transform.rotation) * bendNormal;
      }

      public Quaternion GetRotation(Vector3 direction, Vector3 bendNormal)
      {
        return Quaternion.LookRotation(direction, bendNormal) * this.targetToLocalSpace;
      }

      public Vector3 GetBendNormalFromCurrentRotation()
      {
        return this.transform.rotation * this.defaultLocalBendNormal;
      }
    }
  }
}
