using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKEffector
  {
    public Transform bone;
    public Transform target;
    [Range(0.0f, 1f)]
    public float positionWeight;
    [Range(0.0f, 1f)]
    public float rotationWeight;
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 positionOffset;
    public bool effectChildNodes = true;
    [Range(0.0f, 1f)]
    public float maintainRelativePositionWeight;
    public Transform[] childBones = [];
    public Transform planeBone1;
    public Transform planeBone2;
    public Transform planeBone3;
    public Quaternion planeRotationOffset = Quaternion.identity;
    private float posW;
    private float rotW;
    private Vector3[] localPositions = [];
    private bool usePlaneNodes;
    private Quaternion animatedPlaneRotation = Quaternion.identity;
    private Vector3 animatedPosition;
    private bool firstUpdate;
    private int chainIndex = -1;
    private int nodeIndex = -1;
    private int plane1ChainIndex;
    private int plane1NodeIndex = -1;
    private int plane2ChainIndex = -1;
    private int plane2NodeIndex = -1;
    private int plane3ChainIndex = -1;
    private int plane3NodeIndex = -1;
    private int[] childChainIndexes = [];
    private int[] childNodeIndexes = [];

    public IKSolver.Node GetNode(IKSolverFullBody solver)
    {
      return solver.chain[chainIndex].nodes[nodeIndex];
    }

    public bool isEndEffector { get; private set; }

    public void PinToBone(float positionWeight, float rotationWeight)
    {
      position = bone.position;
      this.positionWeight = Mathf.Clamp(positionWeight, 0.0f, 1f);
      rotation = bone.rotation;
      this.rotationWeight = Mathf.Clamp(rotationWeight, 0.0f, 1f);
    }

    public IKEffector()
    {
    }

    public IKEffector(Transform bone, Transform[] childBones)
    {
      this.bone = bone;
      this.childBones = childBones;
    }

    public bool IsValid(IKSolver solver, ref string message)
    {
      if (bone == null)
      {
        message = "IK Effector bone is null.";
        return false;
      }
      if (solver.GetPoint(bone) == null)
      {
        message = "IK Effector is referencing to a bone '" + bone.name + "' that does not excist in the Node Chain.";
        return false;
      }
      foreach (Object childBone in childBones)
      {
        if (childBone == null)
        {
          message = "IK Effector contains a null reference.";
          return false;
        }
      }
      foreach (Transform childBone in childBones)
      {
        if (solver.GetPoint(childBone) == null)
        {
          message = "IK Effector is referencing to a bone '" + childBone.name + "' that does not excist in the Node Chain.";
          return false;
        }
      }
      if (planeBone1 != null && solver.GetPoint(planeBone1) == null)
      {
        message = "IK Effector is referencing to a bone '" + planeBone1.name + "' that does not excist in the Node Chain.";
        return false;
      }
      if (planeBone2 != null && solver.GetPoint(planeBone2) == null)
      {
        message = "IK Effector is referencing to a bone '" + planeBone2.name + "' that does not excist in the Node Chain.";
        return false;
      }
      if (!(planeBone3 != null) || solver.GetPoint(planeBone3) != null)
        return true;
      message = "IK Effector is referencing to a bone '" + planeBone3.name + "' that does not excist in the Node Chain.";
      return false;
    }

    public void Initiate(IKSolverFullBody solver)
    {
      position = bone.position;
      rotation = bone.rotation;
      animatedPlaneRotation = Quaternion.identity;
      solver.GetChainAndNodeIndexes(bone, out chainIndex, out nodeIndex);
      childChainIndexes = new int[childBones.Length];
      childNodeIndexes = new int[childBones.Length];
      for (int index = 0; index < childBones.Length; ++index)
        solver.GetChainAndNodeIndexes(childBones[index], out childChainIndexes[index], out childNodeIndexes[index]);
      localPositions = new Vector3[childBones.Length];
      usePlaneNodes = false;
      if (planeBone1 != null)
      {
        solver.GetChainAndNodeIndexes(planeBone1, out plane1ChainIndex, out plane1NodeIndex);
        if (planeBone2 != null)
        {
          solver.GetChainAndNodeIndexes(planeBone2, out plane2ChainIndex, out plane2NodeIndex);
          if (planeBone3 != null)
          {
            solver.GetChainAndNodeIndexes(planeBone3, out plane3ChainIndex, out plane3NodeIndex);
            usePlaneNodes = true;
          }
        }
        isEndEffector = true;
      }
      else
        isEndEffector = false;
    }

    public void ResetOffset(IKSolverFullBody solver)
    {
      solver.GetNode(chainIndex, nodeIndex).offset = Vector3.zero;
      for (int index = 0; index < childChainIndexes.Length; ++index)
        solver.GetNode(childChainIndexes[index], childNodeIndexes[index]).offset = Vector3.zero;
    }

    public void SetToTarget()
    {
      if (target == null)
        return;
      position = target.position;
      rotation = target.rotation;
    }

    public void OnPreSolve(IKSolverFullBody solver)
    {
      positionWeight = Mathf.Clamp(positionWeight, 0.0f, 1f);
      rotationWeight = Mathf.Clamp(rotationWeight, 0.0f, 1f);
      maintainRelativePositionWeight = Mathf.Clamp(maintainRelativePositionWeight, 0.0f, 1f);
      posW = positionWeight * solver.IKPositionWeight;
      rotW = rotationWeight * solver.IKPositionWeight;
      solver.GetNode(chainIndex, nodeIndex).effectorPositionWeight = posW;
      solver.GetNode(chainIndex, nodeIndex).effectorRotationWeight = rotW;
      solver.GetNode(chainIndex, nodeIndex).solverRotation = rotation;
      if (float.IsInfinity(positionOffset.x) || float.IsInfinity(positionOffset.y) || float.IsInfinity(positionOffset.z))
        Debug.LogError("Invalid IKEffector.positionOffset (contains Infinity)! Please make sure not to set IKEffector.positionOffset to infinite values.", bone);
      if (float.IsNaN(positionOffset.x) || float.IsNaN(positionOffset.y) || float.IsNaN(positionOffset.z))
        Debug.LogError("Invalid IKEffector.positionOffset (contains NaN)! Please make sure not to set IKEffector.positionOffset to NaN values.", bone);
      if (positionOffset.sqrMagnitude > 10000000000.0)
        Debug.LogError("Additive effector positionOffset detected in Full Body IK (extremely large value). Make sure you are not circularily adding to effector positionOffset each frame.", bone);
      if (float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z))
        Debug.LogError("Invalid IKEffector.position (contains Infinity)!");
      solver.GetNode(chainIndex, nodeIndex).offset += positionOffset * solver.IKPositionWeight;
      if (effectChildNodes && solver.iterations > 0)
      {
        for (int index = 0; index < childBones.Length; ++index)
        {
          localPositions[index] = childBones[index].transform.position - bone.transform.position;
          solver.GetNode(childChainIndexes[index], childNodeIndexes[index]).offset += positionOffset * solver.IKPositionWeight;
        }
      }
      if (usePlaneNodes && maintainRelativePositionWeight > 0.0)
        animatedPlaneRotation = Quaternion.LookRotation(planeBone2.position - planeBone1.position, planeBone3.position - planeBone1.position);
      firstUpdate = true;
    }

    public void OnPostWrite() => positionOffset = Vector3.zero;

    private Quaternion GetPlaneRotation(IKSolverFullBody solver)
    {
      Vector3 solverPosition1 = solver.GetNode(plane1ChainIndex, plane1NodeIndex).solverPosition;
      Vector3 solverPosition2 = solver.GetNode(plane2ChainIndex, plane2NodeIndex).solverPosition;
      Vector3 solverPosition3 = solver.GetNode(plane3ChainIndex, plane3NodeIndex).solverPosition;
      Vector3 forward = solverPosition2 - solverPosition1;
      Vector3 upwards = solverPosition3 - solverPosition1;
      if (!(forward == Vector3.zero))
        return Quaternion.LookRotation(forward, upwards);
      Warning.Log("Make sure you are not placing 2 or more FBBIK effectors of the same chain to exactly the same position.", bone);
      return Quaternion.identity;
    }

    public void Update(IKSolverFullBody solver)
    {
      if (firstUpdate)
      {
        animatedPosition = bone.position + solver.GetNode(chainIndex, nodeIndex).offset;
        firstUpdate = false;
      }
      solver.GetNode(chainIndex, nodeIndex).solverPosition = Vector3.Lerp(GetPosition(solver, out planeRotationOffset), position, posW);
      if (!effectChildNodes)
        return;
      for (int index = 0; index < childBones.Length; ++index)
        solver.GetNode(childChainIndexes[index], childNodeIndexes[index]).solverPosition = Vector3.Lerp(solver.GetNode(childChainIndexes[index], childNodeIndexes[index]).solverPosition, solver.GetNode(chainIndex, nodeIndex).solverPosition + localPositions[index], posW);
    }

    private Vector3 GetPosition(IKSolverFullBody solver, out Quaternion planeRotationOffset)
    {
      planeRotationOffset = Quaternion.identity;
      if (!isEndEffector)
        return solver.GetNode(chainIndex, nodeIndex).solverPosition;
      if (maintainRelativePositionWeight <= 0.0)
        return animatedPosition;
      Vector3 vector3_1 = bone.position - planeBone1.position;
      planeRotationOffset = GetPlaneRotation(solver) * Quaternion.Inverse(animatedPlaneRotation);
      Vector3 vector3_2 = solver.GetNode(plane1ChainIndex, plane1NodeIndex).solverPosition + planeRotationOffset * vector3_1;
      planeRotationOffset = Quaternion.Lerp(Quaternion.identity, planeRotationOffset, maintainRelativePositionWeight);
      return Vector3.Lerp(animatedPosition, vector3_2 + solver.GetNode(chainIndex, nodeIndex).offset, maintainRelativePositionWeight);
    }
  }
}
