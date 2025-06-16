using System;
using UnityEngine;

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
    public Smoothing reachSmoothing = Smoothing.Exponential;
    public Smoothing pushSmoothing = Smoothing.Exponential;
    public IKSolver.Node[] nodes = new IKSolver.Node[0];
    public int[] children = new int[0];
    public ChildConstraint[] childConstraints = new ChildConstraint[0];
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
      SetNodes(nodeTransforms);
      children = new int[0];
    }

    public void SetNodes(params Transform[] boneTransforms)
    {
      nodes = new IKSolver.Node[boneTransforms.Length];
      for (int index = 0; index < boneTransforms.Length; ++index)
        nodes[index] = new IKSolver.Node(boneTransforms[index]);
    }

    public int GetNodeIndex(Transform boneTransform)
    {
      for (int nodeIndex = 0; nodeIndex < nodes.Length; ++nodeIndex)
      {
        if (nodes[nodeIndex].transform == boneTransform)
          return nodeIndex;
      }
      return -1;
    }

    public bool IsValid(ref string message)
    {
      if (nodes.Length == 0)
      {
        message = "FBIK chain contains no nodes.";
        return false;
      }
      foreach (IKSolver.Point node in nodes)
      {
        if (node.transform == null)
        {
          message = "Node transform is null in FBIK chain.";
          return false;
        }
      }
      return true;
    }

    public void Initiate(IKSolverFullBody solver)
    {
      initiated = false;
      foreach (IKSolver.Node node in nodes)
        node.solverPosition = node.transform.position;
      CalculateBoneLengths(solver);
      foreach (ChildConstraint childConstraint in childConstraints)
        childConstraint.Initiate(solver);
      if (nodes.Length == 3)
      {
        bendConstraint.SetBones(nodes[0].transform, nodes[1].transform, nodes[2].transform);
        bendConstraint.Initiate(solver);
      }
      crossFades = new float[children.Length];
      initiated = true;
    }

    public void ReadPose(IKSolverFullBody solver, bool fullBody)
    {
      if (!initiated)
        return;
      for (int index = 0; index < nodes.Length; ++index)
        nodes[index].solverPosition = nodes[index].transform.position + nodes[index].offset;
      CalculateBoneLengths(solver);
      if (!fullBody)
        return;
      for (int index = 0; index < childConstraints.Length; ++index)
        childConstraints[index].OnPreSolve(solver);
      if (children.Length != 0)
      {
        float effectorPositionWeight = nodes[nodes.Length - 1].effectorPositionWeight;
        for (int index = 0; index < children.Length; ++index)
          effectorPositionWeight += solver.chain[children[index]].nodes[0].effectorPositionWeight * solver.chain[children[index]].pull;
        float num = Mathf.Clamp(effectorPositionWeight, 1f, float.PositiveInfinity);
        for (int index = 0; index < children.Length; ++index)
          crossFades[index] = solver.chain[children[index]].nodes[0].effectorPositionWeight * solver.chain[children[index]].pull / num;
      }
      pullParentSum = 0.0f;
      for (int index = 0; index < children.Length; ++index)
        pullParentSum += solver.chain[children[index]].pull;
      pullParentSum = Mathf.Clamp(pullParentSum, 1f, float.PositiveInfinity);
      reachForce = nodes.Length != 3 ? 0.0f : reach * Mathf.Clamp(nodes[2].effectorPositionWeight, 0.0f, 1f);
      if (push > 0.0 && nodes.Length > 1)
        distance = Vector3.Distance(nodes[0].transform.position, nodes[nodes.Length - 1].transform.position);
    }

    private void CalculateBoneLengths(IKSolverFullBody solver)
    {
      length = 0.0f;
      for (int index = 0; index < nodes.Length - 1; ++index)
      {
        nodes[index].length = Vector3.Distance(nodes[index].transform.position, nodes[index + 1].transform.position);
        length += nodes[index].length;
        if (nodes[index].length == 0.0)
        {
          Warning.Log("Bone " + nodes[index].transform.name + " - " + nodes[index + 1].transform.name + " length is zero, can not solve.", nodes[index].transform);
          return;
        }
      }
      for (int index = 0; index < children.Length; ++index)
      {
        solver.chain[children[index]].rootLength = (solver.chain[children[index]].nodes[0].transform.position - nodes[nodes.Length - 1].transform.position).magnitude;
        if (solver.chain[children[index]].rootLength == 0.0)
          return;
      }
      if (nodes.Length != 3)
        return;
      sqrMag1 = nodes[0].length * nodes[0].length;
      sqrMag2 = nodes[1].length * nodes[1].length;
      sqrMagDif = sqrMag1 - sqrMag2;
    }

    public void Reach(IKSolverFullBody solver)
    {
      if (!initiated)
        return;
      for (int index = 0; index < children.Length; ++index)
        solver.chain[children[index]].Reach(solver);
      if (reachForce <= 0.0)
        return;
      Vector3 vector3_1 = nodes[2].solverPosition - nodes[0].solverPosition;
      if (vector3_1 == Vector3.zero)
        return;
      float magnitude = vector3_1.magnitude;
      Vector3 vector3_2 = vector3_1 / magnitude * length;
      float num = Mathf.Clamp(Mathf.Clamp(magnitude / length, 1f - reachForce, 1f + reachForce) - 1f + reachForce, -1f, 1f);
      switch (reachSmoothing)
      {
        case Smoothing.Exponential:
          num *= num;
          break;
        case Smoothing.Cubic:
          num *= num * num;
          break;
      }
      Vector3 vector3_3 = vector3_2 * Mathf.Clamp(num, 0.0f, magnitude);
      IKSolver.Node node1 = nodes[0];
      node1.solverPosition = node1.solverPosition + vector3_3 * (1f - nodes[0].effectorPositionWeight);
      IKSolver.Node node2 = nodes[2];
      node2.solverPosition = node2.solverPosition + vector3_3;
    }

    public Vector3 Push(IKSolverFullBody solver)
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < children.Length; ++index)
        zero += solver.chain[children[index]].Push(solver) * solver.chain[children[index]].pushParent;
      IKSolver.Node node1 = nodes[nodes.Length - 1];
      node1.solverPosition = node1.solverPosition + zero;
      if (nodes.Length < 2 || push <= 0.0)
        return Vector3.zero;
      Vector3 vector3_1 = nodes[2].solverPosition - nodes[0].solverPosition;
      float magnitude = vector3_1.magnitude;
      if (magnitude == 0.0)
        return Vector3.zero;
      float num = (float) (1.0 - magnitude / (double) distance);
      if (num <= 0.0)
        return Vector3.zero;
      switch (pushSmoothing)
      {
        case Smoothing.Exponential:
          num *= num;
          break;
        case Smoothing.Cubic:
          num *= num * num;
          break;
      }
      Vector3 vector3_2 = -vector3_1 * num * push;
      IKSolver.Node node2 = nodes[0];
      node2.solverPosition = node2.solverPosition + vector3_2;
      return vector3_2;
    }

    public void SolveTrigonometric(IKSolverFullBody solver, bool calculateBendDirection = false)
    {
      if (!initiated)
        return;
      for (int index = 0; index < children.Length; ++index)
        solver.chain[children[index]].SolveTrigonometric(solver, calculateBendDirection);
      if (nodes.Length != 3)
        return;
      Vector3 vector3 = nodes[2].solverPosition - nodes[0].solverPosition;
      float magnitude = vector3.magnitude;
      if (magnitude == 0.0)
        return;
      float directionMagnitude = Mathf.Clamp(magnitude, 0.0f, length * 0.99999f);
      nodes[1].solverPosition = nodes[0].solverPosition + GetDirToBendPoint(vector3 / magnitude * directionMagnitude, !calculateBendDirection || !bendConstraint.initiated ? nodes[1].solverPosition - nodes[0].solverPosition : bendConstraint.GetDir(solver), directionMagnitude);
    }

    public void Stage1(IKSolverFullBody solver)
    {
      for (int index = 0; index < children.Length; ++index)
        solver.chain[children[index]].Stage1(solver);
      if (children.Length == 0)
      {
        ForwardReach(nodes[nodes.Length - 1].solverPosition);
      }
      else
      {
        Vector3 solverPosition = nodes[nodes.Length - 1].solverPosition;
        SolveChildConstraints(solver);
        for (int index = 0; index < children.Length; ++index)
        {
          Vector3 vector3 = solver.chain[children[index]].nodes[0].solverPosition;
          if (solver.chain[children[index]].rootLength > 0.0)
            vector3 = SolveFABRIKJoint(nodes[nodes.Length - 1].solverPosition, solver.chain[children[index]].nodes[0].solverPosition, solver.chain[children[index]].rootLength);
          if (pullParentSum > 0.0)
            solverPosition += (vector3 - nodes[nodes.Length - 1].solverPosition) * (solver.chain[children[index]].pull / pullParentSum);
        }
        ForwardReach(Vector3.Lerp(solverPosition, nodes[nodes.Length - 1].solverPosition, pin));
      }
    }

    public void Stage2(IKSolverFullBody solver, Vector3 position)
    {
      BackwardReach(position);
      int num = Mathf.Clamp(solver.iterations, 2, 4);
      if (childConstraints.Length != 0)
      {
        for (int index = 0; index < num; ++index)
          SolveConstraintSystems(solver);
      }
      for (int index = 0; index < children.Length; ++index)
        solver.chain[children[index]].Stage2(solver, nodes[nodes.Length - 1].solverPosition);
    }

    public void SolveConstraintSystems(IKSolverFullBody solver)
    {
      SolveChildConstraints(solver);
      for (int index = 0; index < children.Length; ++index)
        SolveLinearConstraint(nodes[nodes.Length - 1], solver.chain[children[index]].nodes[0], crossFades[index], solver.chain[children[index]].rootLength);
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
      float z = (float) ((directionMagnitude * (double) directionMagnitude + sqrMagDif) / 2.0) / directionMagnitude;
      float y = (float) Math.Sqrt(Mathf.Clamp(sqrMag1 - z * z, 0.0f, float.PositiveInfinity));
      return direction == Vector3.zero ? Vector3.zero : Quaternion.LookRotation(direction, bendDirection) * new Vector3(0.0f, y, z);
    }

    private void SolveChildConstraints(IKSolverFullBody solver)
    {
      for (int index = 0; index < childConstraints.Length; ++index)
        childConstraints[index].Solve(solver);
    }

    private void SolveLinearConstraint(
      IKSolver.Node node1,
      IKSolver.Node node2,
      float crossFade,
      float distance)
    {
      Vector3 vector3_1 = node2.solverPosition - node1.solverPosition;
      float magnitude = vector3_1.magnitude;
      if (distance == (double) magnitude || magnitude == 0.0)
        return;
      Vector3 vector3_2 = vector3_1 * (float) (1.0 - distance / (double) magnitude);
      IKSolver.Node node3 = node1;
      node3.solverPosition = node3.solverPosition + vector3_2 * crossFade;
      IKSolver.Node node4 = node2;
      node4.solverPosition = node4.solverPosition - vector3_2 * (1f - crossFade);
    }

    public void ForwardReach(Vector3 position)
    {
      nodes[nodes.Length - 1].solverPosition = position;
      for (int index = nodes.Length - 2; index > -1; --index)
        nodes[index].solverPosition = SolveFABRIKJoint(nodes[index].solverPosition, nodes[index + 1].solverPosition, nodes[index].length);
    }

    private void BackwardReach(Vector3 position)
    {
      if (rootLength > 0.0)
        position = SolveFABRIKJoint(nodes[0].solverPosition, position, rootLength);
      nodes[0].solverPosition = position;
      for (int index = 1; index < nodes.Length; ++index)
        nodes[index].solverPosition = SolveFABRIKJoint(nodes[index].solverPosition, nodes[index - 1].solverPosition, nodes[index - 1].length);
    }

    [Serializable]
    public class ChildConstraint
    {
      public float pushElasticity;
      public float pullElasticity;
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
        chain1Index = solver.GetChainIndex(bone1);
        chain2Index = solver.GetChainIndex(bone2);
        OnPreSolve(solver);
      }

      public void OnPreSolve(IKSolverFullBody solver)
      {
        nominalDistance = Vector3.Distance(solver.chain[chain1Index].nodes[0].transform.position, solver.chain[chain2Index].nodes[0].transform.position);
        isRigid = pushElasticity <= 0.0 && pullElasticity <= 0.0;
        crossFade = !isRigid ? 0.5f : (float) (1.0 - (0.5 + (solver.chain[chain1Index].pull - solver.chain[chain2Index].pull) * 0.5));
        inverseCrossFade = 1f - crossFade;
      }

      public void Solve(IKSolverFullBody solver)
      {
        if (pushElasticity >= 1.0 && pullElasticity >= 1.0)
          return;
        Vector3 vector3_1 = solver.chain[chain2Index].nodes[0].solverPosition - solver.chain[chain1Index].nodes[0].solverPosition;
        float magnitude = vector3_1.magnitude;
        if (magnitude == (double) nominalDistance || magnitude == 0.0)
          return;
        float num1 = 1f;
        if (!isRigid)
          num1 = 1f - (magnitude > (double) nominalDistance ? pullElasticity : pushElasticity);
        float num2 = num1 * (float) (1.0 - nominalDistance / (double) magnitude);
        Vector3 vector3_2 = vector3_1 * num2;
        IKSolver.Node node1 = solver.chain[chain1Index].nodes[0];
        node1.solverPosition = node1.solverPosition + vector3_2 * crossFade;
        IKSolver.Node node2 = solver.chain[chain2Index].nodes[0];
        node2.solverPosition = node2.solverPosition - vector3_2 * inverseCrossFade;
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
