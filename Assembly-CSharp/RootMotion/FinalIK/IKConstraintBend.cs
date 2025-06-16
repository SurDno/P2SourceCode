using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKConstraintBend
  {
    public Transform bone1;
    public Transform bone2;
    public Transform bone3;
    public Transform bendGoal;
    public Vector3 direction = Vector3.right;
    public Quaternion rotationOffset;
    [Range(0.0f, 1f)]
    public float weight = 0.0f;
    public Vector3 defaultLocalDirection;
    public Vector3 defaultChildDirection;
    [NonSerialized]
    public float clampF = 0.505f;
    private int chainIndex1;
    private int nodeIndex1;
    private int chainIndex2;
    private int nodeIndex2;
    private int chainIndex3;
    private int nodeIndex3;

    public bool IsValid(IKSolverFullBody solver, Warning.Logger logger)
    {
      if ((UnityEngine.Object) this.bone1 == (UnityEngine.Object) null || (UnityEngine.Object) this.bone2 == (UnityEngine.Object) null || (UnityEngine.Object) this.bone3 == (UnityEngine.Object) null)
      {
        if (logger != null)
          logger("Bend Constraint contains a null reference.");
        return false;
      }
      if (solver.GetPoint(this.bone1) == null)
      {
        if (logger != null)
          logger("Bend Constraint is referencing to a bone '" + this.bone1.name + "' that does not excist in the Node Chain.");
        return false;
      }
      if (solver.GetPoint(this.bone2) == null)
      {
        if (logger != null)
          logger("Bend Constraint is referencing to a bone '" + this.bone2.name + "' that does not excist in the Node Chain.");
        return false;
      }
      if (solver.GetPoint(this.bone3) != null)
        return true;
      if (logger != null)
        logger("Bend Constraint is referencing to a bone '" + this.bone3.name + "' that does not excist in the Node Chain.");
      return false;
    }

    public bool initiated { get; private set; }

    public IKConstraintBend()
    {
    }

    public IKConstraintBend(Transform bone1, Transform bone2, Transform bone3)
    {
      this.SetBones(bone1, bone2, bone3);
    }

    public void SetBones(Transform bone1, Transform bone2, Transform bone3)
    {
      this.bone1 = bone1;
      this.bone2 = bone2;
      this.bone3 = bone3;
    }

    public void Initiate(IKSolverFullBody solver)
    {
      solver.GetChainAndNodeIndexes(this.bone1, out this.chainIndex1, out this.nodeIndex1);
      solver.GetChainAndNodeIndexes(this.bone2, out this.chainIndex2, out this.nodeIndex2);
      solver.GetChainAndNodeIndexes(this.bone3, out this.chainIndex3, out this.nodeIndex3);
      this.direction = this.OrthoToBone1(solver, this.OrthoToLimb(solver, this.bone2.position - this.bone1.position));
      this.defaultLocalDirection = Quaternion.Inverse(this.bone1.rotation) * this.direction;
      Vector3 vector3 = Vector3.Cross((this.bone3.position - this.bone1.position).normalized, this.direction);
      this.defaultChildDirection = Quaternion.Inverse(this.bone3.rotation) * vector3;
      this.initiated = true;
    }

    public void SetLimbOrientation(Vector3 upper, Vector3 lower, Vector3 last)
    {
      if (upper == Vector3.zero)
        Debug.LogError((object) "Attempting to set limb orientation to Vector3.zero axis");
      if (lower == Vector3.zero)
        Debug.LogError((object) "Attempting to set limb orientation to Vector3.zero axis");
      if (last == Vector3.zero)
        Debug.LogError((object) "Attempting to set limb orientation to Vector3.zero axis");
      this.defaultLocalDirection = upper.normalized;
      this.defaultChildDirection = last.normalized;
    }

    public void LimitBend(float solverWeight, float positionWeight)
    {
      if (!this.initiated)
        return;
      Vector3 vector3_1 = this.bone1.rotation * -this.defaultLocalDirection;
      Vector3 vector3_2 = this.bone3.position - this.bone2.position;
      bool changed = false;
      Vector3 toDirection = V3Tools.ClampDirection(vector3_2, vector3_1, this.clampF * solverWeight, 0, out changed);
      Quaternion rotation = this.bone3.rotation;
      if (changed)
        this.bone2.rotation = Quaternion.FromToRotation(vector3_2, toDirection) * this.bone2.rotation;
      if ((double) positionWeight > 0.0)
      {
        Vector3 normal = this.bone2.position - this.bone1.position;
        Vector3 tangent = this.bone3.position - this.bone2.position;
        Vector3.OrthoNormalize(ref normal, ref tangent);
        this.bone2.rotation = Quaternion.Lerp(this.bone2.rotation, Quaternion.FromToRotation(tangent, vector3_1) * this.bone2.rotation, positionWeight * solverWeight);
      }
      if (!changed && (double) positionWeight <= 0.0)
        return;
      this.bone3.rotation = rotation;
    }

    public Vector3 GetDir(IKSolverFullBody solver)
    {
      if (!this.initiated)
        return Vector3.zero;
      float t = this.weight * solver.IKPositionWeight;
      if ((UnityEngine.Object) this.bendGoal != (UnityEngine.Object) null)
      {
        Vector3 vector3 = this.bendGoal.position - solver.GetNode(this.chainIndex1, this.nodeIndex1).solverPosition;
        if (vector3 != Vector3.zero)
          this.direction = vector3;
      }
      if ((double) t >= 1.0)
        return this.direction.normalized;
      Vector3 vector3_1 = solver.GetNode(this.chainIndex3, this.nodeIndex3).solverPosition - solver.GetNode(this.chainIndex1, this.nodeIndex1).solverPosition;
      Vector3 a = Quaternion.FromToRotation(this.bone3.position - this.bone1.position, vector3_1) * (this.bone2.position - this.bone1.position);
      if ((double) solver.GetNode(this.chainIndex3, this.nodeIndex3).effectorRotationWeight > 0.0)
      {
        Vector3 b = -Vector3.Cross(vector3_1, solver.GetNode(this.chainIndex3, this.nodeIndex3).solverRotation * this.defaultChildDirection);
        a = Vector3.Lerp(a, b, solver.GetNode(this.chainIndex3, this.nodeIndex3).effectorRotationWeight);
      }
      if (this.rotationOffset != Quaternion.identity)
        a = Quaternion.FromToRotation(this.rotationOffset * vector3_1, vector3_1) * this.rotationOffset * a;
      return (double) t <= 0.0 ? a : Vector3.Lerp(a, this.direction.normalized, t);
    }

    private Vector3 OrthoToLimb(IKSolverFullBody solver, Vector3 tangent)
    {
      Vector3 normal = solver.GetNode(this.chainIndex3, this.nodeIndex3).solverPosition - solver.GetNode(this.chainIndex1, this.nodeIndex1).solverPosition;
      Vector3.OrthoNormalize(ref normal, ref tangent);
      return tangent;
    }

    private Vector3 OrthoToBone1(IKSolverFullBody solver, Vector3 tangent)
    {
      Vector3 normal = solver.GetNode(this.chainIndex2, this.nodeIndex2).solverPosition - solver.GetNode(this.chainIndex1, this.nodeIndex1).solverPosition;
      Vector3.OrthoNormalize(ref normal, ref tangent);
      return tangent;
    }
  }
}
