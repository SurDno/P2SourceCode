// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverFABRIK
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverFABRIK : IKSolverHeuristic
  {
    public IKSolver.IterationDelegate OnPreIteration;
    private bool[] limitedBones = new bool[0];
    private Vector3[] solverLocalPositions = new Vector3[0];

    public void SolveForward(Vector3 position)
    {
      if (!this.initiated)
      {
        if (Warning.logged)
          return;
        this.LogWarning("Trying to solve uninitiated FABRIK chain.");
      }
      else
      {
        this.OnPreSolve();
        this.ForwardReach(position);
      }
    }

    public void SolveBackward(Vector3 position)
    {
      if (!this.initiated)
      {
        if (Warning.logged)
          return;
        this.LogWarning("Trying to solve uninitiated FABRIK chain.");
      }
      else
      {
        this.BackwardReach(position);
        this.OnPostSolve();
      }
    }

    public override Vector3 GetIKPosition()
    {
      return (UnityEngine.Object) this.target != (UnityEngine.Object) null ? this.target.position : this.IKPosition;
    }

    protected override void OnInitiate()
    {
      if (this.firstInitiation || !Application.isPlaying)
        this.IKPosition = this.bones[this.bones.Length - 1].transform.position;
      for (int index = 0; index < this.bones.Length; ++index)
      {
        this.bones[index].solverPosition = this.bones[index].transform.position;
        this.bones[index].solverRotation = this.bones[index].transform.rotation;
      }
      this.limitedBones = new bool[this.bones.Length];
      this.solverLocalPositions = new Vector3[this.bones.Length];
      this.InitiateBones();
      for (int index = 0; index < this.bones.Length; ++index)
        this.solverLocalPositions[index] = Quaternion.Inverse(this.GetParentSolverRotation(index)) * (this.bones[index].transform.position - this.GetParentSolverPosition(index));
    }

    protected override void OnUpdate()
    {
      if ((double) this.IKPositionWeight <= 0.0)
        return;
      this.IKPositionWeight = Mathf.Clamp(this.IKPositionWeight, 0.0f, 1f);
      this.OnPreSolve();
      if ((UnityEngine.Object) this.target != (UnityEngine.Object) null)
        this.IKPosition = this.target.position;
      if (this.XY)
        this.IKPosition.z = this.bones[0].transform.position.z;
      Vector3 vector3 = this.maxIterations > 1 ? this.GetSingularityOffset() : Vector3.zero;
      for (int i = 0; i < this.maxIterations && (!(vector3 == Vector3.zero) || i < 1 || (double) this.tolerance <= 0.0 || (double) this.positionOffset >= (double) this.tolerance * (double) this.tolerance); ++i)
      {
        this.lastLocalDirection = this.localDirection;
        if (this.OnPreIteration != null)
          this.OnPreIteration(i);
        this.Solve(this.IKPosition + (i == 0 ? vector3 : Vector3.zero));
      }
      this.OnPostSolve();
    }

    protected override bool boneLengthCanBeZero => false;

    private Vector3 SolveJoint(Vector3 pos1, Vector3 pos2, float length)
    {
      if (this.XY)
        pos1.z = pos2.z;
      return pos2 + (pos1 - pos2).normalized * length;
    }

    private void OnPreSolve()
    {
      this.chainLength = 0.0f;
      for (int index = 0; index < this.bones.Length; ++index)
      {
        this.bones[index].solverPosition = this.bones[index].transform.position;
        this.bones[index].solverRotation = this.bones[index].transform.rotation;
        if (index < this.bones.Length - 1)
        {
          this.bones[index].length = (this.bones[index].transform.position - this.bones[index + 1].transform.position).magnitude;
          this.bones[index].axis = Quaternion.Inverse(this.bones[index].transform.rotation) * (this.bones[index + 1].transform.position - this.bones[index].transform.position);
          this.chainLength += this.bones[index].length;
        }
        if (this.useRotationLimits)
          this.solverLocalPositions[index] = Quaternion.Inverse(this.GetParentSolverRotation(index)) * (this.bones[index].transform.position - this.GetParentSolverPosition(index));
      }
    }

    private void OnPostSolve()
    {
      if (!this.useRotationLimits)
        this.MapToSolverPositions();
      else
        this.MapToSolverPositionsLimited();
      this.lastLocalDirection = this.localDirection;
    }

    private void Solve(Vector3 targetPosition)
    {
      this.ForwardReach(targetPosition);
      this.BackwardReach(this.bones[0].transform.position);
    }

    private void ForwardReach(Vector3 position)
    {
      this.bones[this.bones.Length - 1].solverPosition = Vector3.Lerp(this.bones[this.bones.Length - 1].solverPosition, position, this.IKPositionWeight);
      for (int index = 0; index < this.limitedBones.Length; ++index)
        this.limitedBones[index] = false;
      for (int rotateBone = this.bones.Length - 2; rotateBone > -1; --rotateBone)
      {
        this.bones[rotateBone].solverPosition = this.SolveJoint(this.bones[rotateBone].solverPosition, this.bones[rotateBone + 1].solverPosition, this.bones[rotateBone].length);
        this.LimitForward(rotateBone, rotateBone + 1);
      }
      this.LimitForward(0, 0);
    }

    private void SolverMove(int index, Vector3 offset)
    {
      for (int index1 = index; index1 < this.bones.Length; ++index1)
      {
        IKSolver.Bone bone = this.bones[index1];
        bone.solverPosition = bone.solverPosition + offset;
      }
    }

    private void SolverRotate(int index, Quaternion rotation, bool recursive)
    {
      for (int index1 = index; index1 < this.bones.Length; ++index1)
      {
        this.bones[index1].solverRotation = rotation * this.bones[index1].solverRotation;
        if (!recursive)
          break;
      }
    }

    private void SolverRotateChildren(int index, Quaternion rotation)
    {
      for (int index1 = index + 1; index1 < this.bones.Length; ++index1)
        this.bones[index1].solverRotation = rotation * this.bones[index1].solverRotation;
    }

    private void SolverMoveChildrenAroundPoint(int index, Quaternion rotation)
    {
      for (int index1 = index + 1; index1 < this.bones.Length; ++index1)
      {
        Vector3 vector3 = this.bones[index1].solverPosition - this.bones[index].solverPosition;
        this.bones[index1].solverPosition = this.bones[index].solverPosition + rotation * vector3;
      }
    }

    private Quaternion GetParentSolverRotation(int index)
    {
      if (index > 0)
        return this.bones[index - 1].solverRotation;
      return (UnityEngine.Object) this.bones[0].transform.parent == (UnityEngine.Object) null ? Quaternion.identity : this.bones[0].transform.parent.rotation;
    }

    private Vector3 GetParentSolverPosition(int index)
    {
      if (index > 0)
        return this.bones[index - 1].solverPosition;
      return (UnityEngine.Object) this.bones[0].transform.parent == (UnityEngine.Object) null ? Vector3.zero : this.bones[0].transform.parent.position;
    }

    private Quaternion GetLimitedRotation(int index, Quaternion q, out bool changed)
    {
      changed = false;
      Quaternion parentSolverRotation = this.GetParentSolverRotation(index);
      Quaternion localRotation = Quaternion.Inverse(parentSolverRotation) * q;
      Quaternion limitedLocalRotation = this.bones[index].rotationLimit.GetLimitedLocalRotation(localRotation, out changed);
      return !changed ? q : parentSolverRotation * limitedLocalRotation;
    }

    private void LimitForward(int rotateBone, int limitBone)
    {
      if (!this.useRotationLimits || (UnityEngine.Object) this.bones[limitBone].rotationLimit == (UnityEngine.Object) null)
        return;
      Vector3 solverPosition = this.bones[this.bones.Length - 1].solverPosition;
      for (int index = rotateBone; index < this.bones.Length - 1 && !this.limitedBones[index]; ++index)
      {
        Quaternion rotation = Quaternion.FromToRotation(this.bones[index].solverRotation * this.bones[index].axis, this.bones[index + 1].solverPosition - this.bones[index].solverPosition);
        this.SolverRotate(index, rotation, false);
      }
      bool changed = false;
      Quaternion limitedRotation = this.GetLimitedRotation(limitBone, this.bones[limitBone].solverRotation, out changed);
      if (changed)
      {
        if (limitBone < this.bones.Length - 1)
        {
          Quaternion rotation1 = QuaTools.FromToRotation(this.bones[limitBone].solverRotation, limitedRotation);
          this.bones[limitBone].solverRotation = limitedRotation;
          this.SolverRotateChildren(limitBone, rotation1);
          this.SolverMoveChildrenAroundPoint(limitBone, rotation1);
          Quaternion rotation2 = Quaternion.FromToRotation(this.bones[this.bones.Length - 1].solverPosition - this.bones[rotateBone].solverPosition, solverPosition - this.bones[rotateBone].solverPosition);
          this.SolverRotate(rotateBone, rotation2, true);
          this.SolverMoveChildrenAroundPoint(rotateBone, rotation2);
          this.SolverMove(rotateBone, solverPosition - this.bones[this.bones.Length - 1].solverPosition);
        }
        else
          this.bones[limitBone].solverRotation = limitedRotation;
      }
      this.limitedBones[limitBone] = true;
    }

    private void BackwardReach(Vector3 position)
    {
      if (this.useRotationLimits)
        this.BackwardReachLimited(position);
      else
        this.BackwardReachUnlimited(position);
    }

    private void BackwardReachUnlimited(Vector3 position)
    {
      this.bones[0].solverPosition = position;
      for (int index = 1; index < this.bones.Length; ++index)
        this.bones[index].solverPosition = this.SolveJoint(this.bones[index].solverPosition, this.bones[index - 1].solverPosition, this.bones[index - 1].length);
    }

    private void BackwardReachLimited(Vector3 position)
    {
      this.bones[0].solverPosition = position;
      for (int index = 0; index < this.bones.Length - 1; ++index)
      {
        Vector3 vector3 = this.SolveJoint(this.bones[index + 1].solverPosition, this.bones[index].solverPosition, this.bones[index].length);
        Quaternion quaternion = Quaternion.FromToRotation(this.bones[index].solverRotation * this.bones[index].axis, vector3 - this.bones[index].solverPosition) * this.bones[index].solverRotation;
        if ((UnityEngine.Object) this.bones[index].rotationLimit != (UnityEngine.Object) null)
        {
          bool changed = false;
          quaternion = this.GetLimitedRotation(index, quaternion, out changed);
        }
        Quaternion rotation = QuaTools.FromToRotation(this.bones[index].solverRotation, quaternion);
        this.bones[index].solverRotation = quaternion;
        this.SolverRotateChildren(index, rotation);
        this.bones[index + 1].solverPosition = this.bones[index].solverPosition + this.bones[index].solverRotation * this.solverLocalPositions[index + 1];
      }
      for (int index = 0; index < this.bones.Length; ++index)
        this.bones[index].solverRotation = Quaternion.LookRotation(this.bones[index].solverRotation * Vector3.forward, this.bones[index].solverRotation * Vector3.up);
    }

    private void MapToSolverPositions()
    {
      this.bones[0].transform.position = this.bones[0].solverPosition;
      for (int index = 0; index < this.bones.Length - 1; ++index)
      {
        if (this.XY)
          this.bones[index].Swing2D(this.bones[index + 1].solverPosition);
        else
          this.bones[index].Swing(this.bones[index + 1].solverPosition);
      }
    }

    private void MapToSolverPositionsLimited()
    {
      this.bones[0].transform.position = this.bones[0].solverPosition;
      for (int index = 0; index < this.bones.Length; ++index)
      {
        if (index < this.bones.Length - 1)
          this.bones[index].transform.rotation = this.bones[index].solverRotation;
      }
    }
  }
}
