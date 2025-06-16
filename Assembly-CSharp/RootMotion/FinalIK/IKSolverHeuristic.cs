// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverHeuristic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverHeuristic : IKSolver
  {
    public Transform target;
    public float tolerance = 0.0f;
    public int maxIterations = 4;
    public bool useRotationLimits = true;
    public bool XY;
    public IKSolver.Bone[] bones = new IKSolver.Bone[0];
    protected Vector3 lastLocalDirection;
    protected float chainLength;

    public bool SetChain(Transform[] hierarchy, Transform root)
    {
      if (this.bones == null || this.bones.Length != hierarchy.Length)
        this.bones = new IKSolver.Bone[hierarchy.Length];
      for (int index = 0; index < hierarchy.Length; ++index)
      {
        if (this.bones[index] == null)
          this.bones[index] = new IKSolver.Bone();
        this.bones[index].transform = hierarchy[index];
      }
      this.Initiate(root);
      return this.initiated;
    }

    public void AddBone(Transform bone)
    {
      Transform[] hierarchy = new Transform[this.bones.Length + 1];
      for (int index = 0; index < this.bones.Length; ++index)
        hierarchy[index] = this.bones[index].transform;
      hierarchy[hierarchy.Length - 1] = bone;
      this.SetChain(hierarchy, this.root);
    }

    public override void StoreDefaultLocalState()
    {
      for (int index = 0; index < this.bones.Length; ++index)
        this.bones[index].StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!this.initiated || (double) this.IKPositionWeight <= 0.0)
        return;
      for (int index = 0; index < this.bones.Length; ++index)
        this.bones[index].FixTransform();
    }

    public override bool IsValid(ref string message)
    {
      if (this.bones.Length == 0)
      {
        message = "IK chain has no Bones.";
        return false;
      }
      if (this.bones.Length < this.minBones)
      {
        message = "IK chain has less than " + (object) this.minBones + " Bones.";
        return false;
      }
      foreach (IKSolver.Point bone in this.bones)
      {
        if ((UnityEngine.Object) bone.transform == (UnityEngine.Object) null)
        {
          message = "One of the Bones is null.";
          return false;
        }
      }
      Transform transform = IKSolver.ContainsDuplicateBone(this.bones);
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      {
        message = transform.name + " is represented multiple times in the Bones.";
        return false;
      }
      if (!this.allowCommonParent && !IKSolver.HierarchyIsValid(this.bones))
      {
        message = "Invalid bone hierarchy detected. IK requires for it's bones to be parented to each other in descending order.";
        return false;
      }
      if (!this.boneLengthCanBeZero)
      {
        for (int index = 0; index < this.bones.Length - 1; ++index)
        {
          if ((double) (this.bones[index].transform.position - this.bones[index + 1].transform.position).magnitude == 0.0)
          {
            message = "Bone " + (object) index + " length is zero.";
            return false;
          }
        }
      }
      return true;
    }

    public override IKSolver.Point[] GetPoints() => (IKSolver.Point[]) this.bones;

    public override IKSolver.Point GetPoint(Transform transform)
    {
      for (int index = 0; index < this.bones.Length; ++index)
      {
        if ((UnityEngine.Object) this.bones[index].transform == (UnityEngine.Object) transform)
          return (IKSolver.Point) this.bones[index];
      }
      return (IKSolver.Point) null;
    }

    protected virtual int minBones => 2;

    protected virtual bool boneLengthCanBeZero => true;

    protected virtual bool allowCommonParent => false;

    protected override void OnInitiate()
    {
    }

    protected override void OnUpdate()
    {
    }

    protected void InitiateBones()
    {
      this.chainLength = 0.0f;
      for (int index = 0; index < this.bones.Length; ++index)
      {
        if (index < this.bones.Length - 1)
        {
          this.bones[index].length = (this.bones[index].transform.position - this.bones[index + 1].transform.position).magnitude;
          this.chainLength += this.bones[index].length;
          Vector3 position = this.bones[index + 1].transform.position;
          this.bones[index].axis = Quaternion.Inverse(this.bones[index].transform.rotation) * (position - this.bones[index].transform.position);
          if ((UnityEngine.Object) this.bones[index].rotationLimit != (UnityEngine.Object) null)
          {
            if (this.XY && !(this.bones[index].rotationLimit is RotationLimitHinge))
              Warning.Log("Only Hinge Rotation Limits should be used on 2D IK solvers.", this.bones[index].transform);
            this.bones[index].rotationLimit.Disable();
          }
        }
        else
          this.bones[index].axis = Quaternion.Inverse(this.bones[index].transform.rotation) * (this.bones[this.bones.Length - 1].transform.position - this.bones[0].transform.position);
      }
    }

    protected virtual Vector3 localDirection
    {
      get
      {
        return this.bones[0].transform.InverseTransformDirection(this.bones[this.bones.Length - 1].transform.position - this.bones[0].transform.position);
      }
    }

    protected float positionOffset
    {
      get => Vector3.SqrMagnitude(this.localDirection - this.lastLocalDirection);
    }

    protected Vector3 GetSingularityOffset()
    {
      if (!this.SingularityDetected())
        return Vector3.zero;
      Vector3 normalized = (this.IKPosition - this.bones[0].transform.position).normalized;
      Vector3 rhs = new Vector3(normalized.y, normalized.z, normalized.x);
      if (this.useRotationLimits && (UnityEngine.Object) this.bones[this.bones.Length - 2].rotationLimit != (UnityEngine.Object) null && this.bones[this.bones.Length - 2].rotationLimit is RotationLimitHinge)
        rhs = this.bones[this.bones.Length - 2].transform.rotation * this.bones[this.bones.Length - 2].rotationLimit.axis;
      return Vector3.Cross(normalized, rhs) * this.bones[this.bones.Length - 2].length * 0.5f;
    }

    private bool SingularityDetected()
    {
      if (!this.initiated)
        return false;
      Vector3 vector3_1 = this.bones[this.bones.Length - 1].transform.position - this.bones[0].transform.position;
      Vector3 vector3_2 = this.IKPosition - this.bones[0].transform.position;
      float magnitude1 = vector3_1.magnitude;
      float magnitude2 = vector3_2.magnitude;
      return (double) magnitude1 >= (double) magnitude2 && (double) magnitude1 >= (double) this.chainLength - (double) this.bones[this.bones.Length - 2].length * 0.10000000149011612 && (double) magnitude1 != 0.0 && (double) magnitude2 != 0.0 && (double) magnitude2 <= (double) magnitude1 && (double) Vector3.Dot(vector3_1 / magnitude1, vector3_2 / magnitude2) >= 0.99900001287460327;
    }
  }
}
