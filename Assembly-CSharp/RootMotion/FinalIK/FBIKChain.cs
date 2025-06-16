// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.FBIKChain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class FBIKChain
  {
    [Range(0.0f, 1f)]
    public float pin;
    [Range(0.0f, 1f)]
    public float pull = 1f;
    [Range(0.0f, 1f)]
    public float push;
    [Range(-1f, 1f)]
    public float pushParent;
    [Range(0.0f, 1f)]
    public float reach = 0.1f;
    public FBIKChain.Smoothing reachSmoothing = FBIKChain.Smoothing.Exponential;
    public FBIKChain.Smoothing pushSmoothing = FBIKChain.Smoothing.Exponential;
    public IKSolver.Node[] nodes = new IKSolver.Node[0];
    public int[] children = new int[0];
    public FBIKChain.ChildConstraint[] childConstraints = new FBIKChain.ChildConstraint[0];
    public IKConstraintBend bendConstraint = new IKConstraintBend();
    private float rootLength;
    private bool initiated;
    private float length;
    private float distance;
    private IKSolver.Point p;
    private float reachForce;
    private float pullParentSum;
    private float[] crossFades;
    private float sqrMag1;
    private float sqrMag2;
    private float sqrMagDif;
    private const float maxLimbLength = 0.99999f;

    public FBIKChain()
    {
    }

    public FBIKChain(float pin, float pull, params Transform[] nodeTransforms)
    {
      this.pin = pin;
      this.pull = pull;
      this.SetNodes(nodeTransforms);
      this.children = new int[0];
    }

    public void SetNodes(params Transform[] boneTransforms)
    {
      this.nodes = new IKSolver.Node[boneTransforms.Length];
      for (int index = 0; index < boneTransforms.Length; ++index)
        this.nodes[index] = new IKSolver.Node(boneTransforms[index]);
    }

    public int GetNodeIndex(Transform boneTransform)
    {
      for (int nodeIndex = 0; nodeIndex < this.nodes.Length; ++nodeIndex)
      {
        if ((UnityEngine.Object) this.nodes[nodeIndex].transform == (UnityEngine.Object) boneTransform)
          return nodeIndex;
      }
      return -1;
    }

    public bool IsValid(ref string message)
    {
      if (this.nodes.Length == 0)
      {
        message = "FBIK chain contains no nodes.";
        return false;
      }
      foreach (IKSolver.Point node in this.nodes)
      {
        if ((UnityEngine.Object) node.transform == (UnityEngine.Object) null)
        {
          message = "Node transform is null in FBIK chain.";
          return false;
        }
      }
      return true;
    }

    public void Initiate(IKSolverFullBody solver)
    {
      this.initiated = false;
      foreach (IKSolver.Node node in this.nodes)
        node.solverPosition = node.transform.position;
      this.CalculateBoneLengths(solver);
      foreach (FBIKChain.ChildConstraint childConstraint in this.childConstraints)
        childConstraint.Initiate(solver);
      if (this.nodes.Length == 3)
      {
        this.bendConstraint.SetBones(this.nodes[0].transform, this.nodes[1].transform, this.nodes[2].transform);
        this.bendConstraint.Initiate(solver);
      }
      this.crossFades = new float[this.children.Length];
      this.initiated = true;
    }

    public void ReadPose(IKSolverFullBody solver, bool fullBody)
    {
      if (!this.initiated)
        return;
      for (int index = 0; index < this.nodes.Length; ++index)
        this.nodes[index].solverPosition = this.nodes[index].transform.position + this.nodes[index].offset;
      this.CalculateBoneLengths(solver);
      if (!fullBody)
        return;
      for (int index = 0; index < this.childConstraints.Length; ++index)
        this.childConstraints[index].OnPreSolve(solver);
      if (this.children.Length != 0)
      {
        float effectorPositionWeight = this.nodes[this.nodes.Length - 1].effectorPositionWeight;
        for (int index = 0; index < this.children.Length; ++index)
          effectorPositionWeight += solver.chain[this.children[index]].nodes[0].effectorPositionWeight * solver.chain[this.children[index]].pull;
        float num = Mathf.Clamp(effectorPositionWeight, 1f, float.PositiveInfinity);
        for (int index = 0; index < this.children.Length; ++index)
          this.crossFades[index] = solver.chain[this.children[index]].nodes[0].effectorPositionWeight * solver.chain[this.children[index]].pull / num;
      }
      this.pullParentSum = 0.0f;
      for (int index = 0; index < this.children.Length; ++index)
        this.pullParentSum += solver.chain[this.children[index]].pull;
      this.pullParentSum = Mathf.Clamp(this.pullParentSum, 1f, float.PositiveInfinity);
      this.reachForce = this.nodes.Length != 3 ? 0.0f : this.reach * Mathf.Clamp(this.nodes[2].effectorPositionWeight, 0.0f, 1f);
      if ((double) this.push > 0.0 && this.nodes.Length > 1)
        this.distance = Vector3.Distance(this.nodes[0].transform.position, this.nodes[this.nodes.Length - 1].transform.position);
    }

    private void CalculateBoneLengths(IKSolverFullBody solver)
    {
      this.length = 0.0f;
      for (int index = 0; index < this.nodes.Length - 1; ++index)
      {
        this.nodes[index].length = Vector3.Distance(this.nodes[index].transform.position, this.nodes[index + 1].transform.position);
        this.length += this.nodes[index].length;
        if ((double) this.nodes[index].length == 0.0)
        {
          Warning.Log("Bone " + this.nodes[index].transform.name + " - " + this.nodes[index + 1].transform.name + " length is zero, can not solve.", this.nodes[index].transform);
          return;
        }
      }
      for (int index = 0; index < this.children.Length; ++index)
      {
        solver.chain[this.children[index]].rootLength = (solver.chain[this.children[index]].nodes[0].transform.position - this.nodes[this.nodes.Length - 1].transform.position).magnitude;
        if ((double) solver.chain[this.children[index]].rootLength == 0.0)
          return;
      }
      if (this.nodes.Length != 3)
        return;
      this.sqrMag1 = this.nodes[0].length * this.nodes[0].length;
      this.sqrMag2 = this.nodes[1].length * this.nodes[1].length;
      this.sqrMagDif = this.sqrMag1 - this.sqrMag2;
    }

    public void Reach(IKSolverFullBody solver)
    {
      if (!this.initiated)
        return;
      for (int index = 0; index < this.children.Length; ++index)
        solver.chain[this.children[index]].Reach(solver);
      if ((double) this.reachForce <= 0.0)
        return;
      Vector3 vector3_1 = this.nodes[2].solverPosition - this.nodes[0].solverPosition;
      if (vector3_1 == Vector3.zero)
        return;
      float magnitude = vector3_1.magnitude;
      Vector3 vector3_2 = vector3_1 / magnitude * this.length;
      float num = Mathf.Clamp(Mathf.Clamp(magnitude / this.length, 1f - this.reachForce, 1f + this.reachForce) - 1f + this.reachForce, -1f, 1f);
      switch (this.reachSmoothing)
      {
        case FBIKChain.Smoothing.Exponential:
          num *= num;
          break;
        case FBIKChain.Smoothing.Cubic:
          num *= num * num;
          break;
      }
      Vector3 vector3_3 = vector3_2 * Mathf.Clamp(num, 0.0f, magnitude);
      IKSolver.Node node1 = this.nodes[0];
      node1.solverPosition = node1.solverPosition + vector3_3 * (1f - this.nodes[0].effectorPositionWeight);
      IKSolver.Node node2 = this.nodes[2];
      node2.solverPosition = node2.solverPosition + vector3_3;
    }

    public Vector3 Push(IKSolverFullBody solver)
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < this.children.Length; ++index)
        zero += solver.chain[this.children[index]].Push(solver) * solver.chain[this.children[index]].pushParent;
      IKSolver.Node node1 = this.nodes[this.nodes.Length - 1];
      node1.solverPosition = node1.solverPosition + zero;
      if (this.nodes.Length < 2 || (double) this.push <= 0.0)
        return Vector3.zero;
      Vector3 vector3_1 = this.nodes[2].solverPosition - this.nodes[0].solverPosition;
      float magnitude = vector3_1.magnitude;
      if ((double) magnitude == 0.0)
        return Vector3.zero;
      float num = (float) (1.0 - (double) magnitude / (double) this.distance);
      if ((double) num <= 0.0)
        return Vector3.zero;
      switch (this.pushSmoothing)
      {
        case FBIKChain.Smoothing.Exponential:
          num *= num;
          break;
        case FBIKChain.Smoothing.Cubic:
          num *= num * num;
          break;
      }
      Vector3 vector3_2 = -vector3_1 * num * this.push;
      IKSolver.Node node2 = this.nodes[0];
      node2.solverPosition = node2.solverPosition + vector3_2;
      return vector3_2;
    }

    public void SolveTrigonometric(IKSolverFullBody solver, bool calculateBendDirection = false)
    {
      if (!this.initiated)
        return;
      for (int index = 0; index < this.children.Length; ++index)
        solver.chain[this.children[index]].SolveTrigonometric(solver, calculateBendDirection);
      if (this.nodes.Length != 3)
        return;
      Vector3 vector3 = this.nodes[2].solverPosition - this.nodes[0].solverPosition;
      float magnitude = vector3.magnitude;
      if ((double) magnitude == 0.0)
        return;
      float directionMagnitude = Mathf.Clamp(magnitude, 0.0f, this.length * 0.99999f);
      this.nodes[1].solverPosition = this.nodes[0].solverPosition + this.GetDirToBendPoint(vector3 / magnitude * directionMagnitude, !calculateBendDirection || !this.bendConstraint.initiated ? this.nodes[1].solverPosition - this.nodes[0].solverPosition : this.bendConstraint.GetDir(solver), directionMagnitude);
    }

    public void Stage1(IKSolverFullBody solver)
    {
      for (int index = 0; index < this.children.Length; ++index)
        solver.chain[this.children[index]].Stage1(solver);
      if (this.children.Length == 0)
      {
        this.ForwardReach(this.nodes[this.nodes.Length - 1].solverPosition);
      }
      else
      {
        Vector3 solverPosition = this.nodes[this.nodes.Length - 1].solverPosition;
        this.SolveChildConstraints(solver);
        for (int index = 0; index < this.children.Length; ++index)
        {
          Vector3 vector3 = solver.chain[this.children[index]].nodes[0].solverPosition;
          if ((double) solver.chain[this.children[index]].rootLength > 0.0)
            vector3 = this.SolveFABRIKJoint(this.nodes[this.nodes.Length - 1].solverPosition, solver.chain[this.children[index]].nodes[0].solverPosition, solver.chain[this.children[index]].rootLength);
          if ((double) this.pullParentSum > 0.0)
            solverPosition += (vector3 - this.nodes[this.nodes.Length - 1].solverPosition) * (solver.chain[this.children[index]].pull / this.pullParentSum);
        }
        this.ForwardReach(Vector3.Lerp(solverPosition, this.nodes[this.nodes.Length - 1].solverPosition, this.pin));
      }
    }

    public void Stage2(IKSolverFullBody solver, Vector3 position)
    {
      this.BackwardReach(position);
      int num = Mathf.Clamp(solver.iterations, 2, 4);
      if (this.childConstraints.Length != 0)
      {
        for (int index = 0; index < num; ++index)
          this.SolveConstraintSystems(solver);
      }
      for (int index = 0; index < this.children.Length; ++index)
        solver.chain[this.children[index]].Stage2(solver, this.nodes[this.nodes.Length - 1].solverPosition);
    }

    public void SolveConstraintSystems(IKSolverFullBody solver)
    {
      this.SolveChildConstraints(solver);
      for (int index = 0; index < this.children.Length; ++index)
        this.SolveLinearConstraint(this.nodes[this.nodes.Length - 1], solver.chain[this.children[index]].nodes[0], this.crossFades[index], solver.chain[this.children[index]].rootLength);
    }

    private Vector3 SolveFABRIKJoint(Vector3 pos1, Vector3 pos2, float length)
    {
      return pos2 + (pos1 - pos2).normalized * length;
    }

    protected Vector3 GetDirToBendPoint(
      Vector3 direction,
      Vector3 bendDirection,
      float directionMagnitude)
    {
      float z = (float) (((double) directionMagnitude * (double) directionMagnitude + (double) this.sqrMagDif) / 2.0) / directionMagnitude;
      float y = (float) Math.Sqrt((double) Mathf.Clamp(this.sqrMag1 - z * z, 0.0f, float.PositiveInfinity));
      return direction == Vector3.zero ? Vector3.zero : Quaternion.LookRotation(direction, bendDirection) * new Vector3(0.0f, y, z);
    }

    private void SolveChildConstraints(IKSolverFullBody solver)
    {
      for (int index = 0; index < this.childConstraints.Length; ++index)
        this.childConstraints[index].Solve(solver);
    }

    private void SolveLinearConstraint(
      IKSolver.Node node1,
      IKSolver.Node node2,
      float crossFade,
      float distance)
    {
      Vector3 vector3_1 = node2.solverPosition - node1.solverPosition;
      float magnitude = vector3_1.magnitude;
      if ((double) distance == (double) magnitude || (double) magnitude == 0.0)
        return;
      Vector3 vector3_2 = vector3_1 * (float) (1.0 - (double) distance / (double) magnitude);
      IKSolver.Node node3 = node1;
      node3.solverPosition = node3.solverPosition + vector3_2 * crossFade;
      IKSolver.Node node4 = node2;
      node4.solverPosition = node4.solverPosition - vector3_2 * (1f - crossFade);
    }

    public void ForwardReach(Vector3 position)
    {
      this.nodes[this.nodes.Length - 1].solverPosition = position;
      for (int index = this.nodes.Length - 2; index > -1; --index)
        this.nodes[index].solverPosition = this.SolveFABRIKJoint(this.nodes[index].solverPosition, this.nodes[index + 1].solverPosition, this.nodes[index].length);
    }

    private void BackwardReach(Vector3 position)
    {
      if ((double) this.rootLength > 0.0)
        position = this.SolveFABRIKJoint(this.nodes[0].solverPosition, position, this.rootLength);
      this.nodes[0].solverPosition = position;
      for (int index = 1; index < this.nodes.Length; ++index)
        this.nodes[index].solverPosition = this.SolveFABRIKJoint(this.nodes[index].solverPosition, this.nodes[index - 1].solverPosition, this.nodes[index - 1].length);
    }

    [Serializable]
    public class ChildConstraint
    {
      public float pushElasticity = 0.0f;
      public float pullElasticity = 0.0f;
      [SerializeField]
      private Transform bone1;
      [SerializeField]
      private Transform bone2;
      private float crossFade;
      private float inverseCrossFade;
      private int chain1Index;
      private int chain2Index;

      public float nominalDistance { get; private set; }

      public bool isRigid { get; private set; }

      public ChildConstraint(
        Transform bone1,
        Transform bone2,
        float pushElasticity = 0.0f,
        float pullElasticity = 0.0f)
      {
        this.bone1 = bone1;
        this.bone2 = bone2;
        this.pushElasticity = pushElasticity;
        this.pullElasticity = pullElasticity;
      }

      public void Initiate(IKSolverFullBody solver)
      {
        this.chain1Index = solver.GetChainIndex(this.bone1);
        this.chain2Index = solver.GetChainIndex(this.bone2);
        this.OnPreSolve(solver);
      }

      public void OnPreSolve(IKSolverFullBody solver)
      {
        this.nominalDistance = Vector3.Distance(solver.chain[this.chain1Index].nodes[0].transform.position, solver.chain[this.chain2Index].nodes[0].transform.position);
        this.isRigid = (double) this.pushElasticity <= 0.0 && (double) this.pullElasticity <= 0.0;
        this.crossFade = !this.isRigid ? 0.5f : (float) (1.0 - (0.5 + (double) (solver.chain[this.chain1Index].pull - solver.chain[this.chain2Index].pull) * 0.5));
        this.inverseCrossFade = 1f - this.crossFade;
      }

      public void Solve(IKSolverFullBody solver)
      {
        if ((double) this.pushElasticity >= 1.0 && (double) this.pullElasticity >= 1.0)
          return;
        Vector3 vector3_1 = solver.chain[this.chain2Index].nodes[0].solverPosition - solver.chain[this.chain1Index].nodes[0].solverPosition;
        float magnitude = vector3_1.magnitude;
        if ((double) magnitude == (double) this.nominalDistance || (double) magnitude == 0.0)
          return;
        float num1 = 1f;
        if (!this.isRigid)
          num1 = 1f - ((double) magnitude > (double) this.nominalDistance ? this.pullElasticity : this.pushElasticity);
        float num2 = num1 * (float) (1.0 - (double) this.nominalDistance / (double) magnitude);
        Vector3 vector3_2 = vector3_1 * num2;
        IKSolver.Node node1 = solver.chain[this.chain1Index].nodes[0];
        node1.solverPosition = node1.solverPosition + vector3_2 * this.crossFade;
        IKSolver.Node node2 = solver.chain[this.chain2Index].nodes[0];
        node2.solverPosition = node2.solverPosition - vector3_2 * this.inverseCrossFade;
      }
    }

    [Serializable]
    public enum Smoothing
    {
      None,
      Exponential,
      Cubic,
    }
  }
}
