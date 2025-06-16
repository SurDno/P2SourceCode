using System;
using UnityEngine;

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
    public Transform[] childBones = new Transform[0];
    public Transform planeBone1;
    public Transform planeBone2;
    public Transform planeBone3;
    public Quaternion planeRotationOffset = Quaternion.identity;
    private float posW;
    private float rotW;
    private Vector3[] localPositions = new Vector3[0];
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
    private int[] childChainIndexes = new int[0];
    private int[] childNodeIndexes = new int[0];

    public IKSolver.Node GetNode(IKSolverFullBody solver)
    {
      return solver.chain[this.chainIndex].nodes[this.nodeIndex];
    }

    public bool isEndEffector { get; private set; }

    public void PinToBone(float positionWeight, float rotationWeight)
    {
      this.position = this.bone.position;
      this.positionWeight = Mathf.Clamp(positionWeight, 0.0f, 1f);
      this.rotation = this.bone.rotation;
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
      if ((UnityEngine.Object) this.bone == (UnityEngine.Object) null)
      {
        message = "IK Effector bone is null.";
        return false;
      }
      if (solver.GetPoint(this.bone) == null)
      {
        message = "IK Effector is referencing to a bone '" + this.bone.name + "' that does not excist in the Node Chain.";
        return false;
      }
      foreach (UnityEngine.Object childBone in this.childBones)
      {
        if (childBone == (UnityEngine.Object) null)
        {
          message = "IK Effector contains a null reference.";
          return false;
        }
      }
      foreach (Transform childBone in this.childBones)
      {
        if (solver.GetPoint(childBone) == null)
        {
          message = "IK Effector is referencing to a bone '" + childBone.name + "' that does not excist in the Node Chain.";
          return false;
        }
      }
      if ((UnityEngine.Object) this.planeBone1 != (UnityEngine.Object) null && solver.GetPoint(this.planeBone1) == null)
      {
        message = "IK Effector is referencing to a bone '" + this.planeBone1.name + "' that does not excist in the Node Chain.";
        return false;
      }
      if ((UnityEngine.Object) this.planeBone2 != (UnityEngine.Object) null && solver.GetPoint(this.planeBone2) == null)
      {
        message = "IK Effector is referencing to a bone '" + this.planeBone2.name + "' that does not excist in the Node Chain.";
        return false;
      }
      if (!((UnityEngine.Object) this.planeBone3 != (UnityEngine.Object) null) || solver.GetPoint(this.planeBone3) != null)
        return true;
      message = "IK Effector is referencing to a bone '" + this.planeBone3.name + "' that does not excist in the Node Chain.";
      return false;
    }

    public void Initiate(IKSolverFullBody solver)
    {
      this.position = this.bone.position;
      this.rotation = this.bone.rotation;
      this.animatedPlaneRotation = Quaternion.identity;
      solver.GetChainAndNodeIndexes(this.bone, out this.chainIndex, out this.nodeIndex);
      this.childChainIndexes = new int[this.childBones.Length];
      this.childNodeIndexes = new int[this.childBones.Length];
      for (int index = 0; index < this.childBones.Length; ++index)
        solver.GetChainAndNodeIndexes(this.childBones[index], out this.childChainIndexes[index], out this.childNodeIndexes[index]);
      this.localPositions = new Vector3[this.childBones.Length];
      this.usePlaneNodes = false;
      if ((UnityEngine.Object) this.planeBone1 != (UnityEngine.Object) null)
      {
        solver.GetChainAndNodeIndexes(this.planeBone1, out this.plane1ChainIndex, out this.plane1NodeIndex);
        if ((UnityEngine.Object) this.planeBone2 != (UnityEngine.Object) null)
        {
          solver.GetChainAndNodeIndexes(this.planeBone2, out this.plane2ChainIndex, out this.plane2NodeIndex);
          if ((UnityEngine.Object) this.planeBone3 != (UnityEngine.Object) null)
          {
            solver.GetChainAndNodeIndexes(this.planeBone3, out this.plane3ChainIndex, out this.plane3NodeIndex);
            this.usePlaneNodes = true;
          }
        }
        this.isEndEffector = true;
      }
      else
        this.isEndEffector = false;
    }

    public void ResetOffset(IKSolverFullBody solver)
    {
      solver.GetNode(this.chainIndex, this.nodeIndex).offset = Vector3.zero;
      for (int index = 0; index < this.childChainIndexes.Length; ++index)
        solver.GetNode(this.childChainIndexes[index], this.childNodeIndexes[index]).offset = Vector3.zero;
    }

    public void SetToTarget()
    {
      if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
        return;
      this.position = this.target.position;
      this.rotation = this.target.rotation;
    }

    public void OnPreSolve(IKSolverFullBody solver)
    {
      this.positionWeight = Mathf.Clamp(this.positionWeight, 0.0f, 1f);
      this.rotationWeight = Mathf.Clamp(this.rotationWeight, 0.0f, 1f);
      this.maintainRelativePositionWeight = Mathf.Clamp(this.maintainRelativePositionWeight, 0.0f, 1f);
      this.posW = this.positionWeight * solver.IKPositionWeight;
      this.rotW = this.rotationWeight * solver.IKPositionWeight;
      solver.GetNode(this.chainIndex, this.nodeIndex).effectorPositionWeight = this.posW;
      solver.GetNode(this.chainIndex, this.nodeIndex).effectorRotationWeight = this.rotW;
      solver.GetNode(this.chainIndex, this.nodeIndex).solverRotation = this.rotation;
      if (float.IsInfinity(this.positionOffset.x) || float.IsInfinity(this.positionOffset.y) || float.IsInfinity(this.positionOffset.z))
        Debug.LogError((object) "Invalid IKEffector.positionOffset (contains Infinity)! Please make sure not to set IKEffector.positionOffset to infinite values.", (UnityEngine.Object) this.bone);
      if (float.IsNaN(this.positionOffset.x) || float.IsNaN(this.positionOffset.y) || float.IsNaN(this.positionOffset.z))
        Debug.LogError((object) "Invalid IKEffector.positionOffset (contains NaN)! Please make sure not to set IKEffector.positionOffset to NaN values.", (UnityEngine.Object) this.bone);
      if ((double) this.positionOffset.sqrMagnitude > 10000000000.0)
        Debug.LogError((object) "Additive effector positionOffset detected in Full Body IK (extremely large value). Make sure you are not circularily adding to effector positionOffset each frame.", (UnityEngine.Object) this.bone);
      if (float.IsInfinity(this.position.x) || float.IsInfinity(this.position.y) || float.IsInfinity(this.position.z))
        Debug.LogError((object) "Invalid IKEffector.position (contains Infinity)!");
      solver.GetNode(this.chainIndex, this.nodeIndex).offset += this.positionOffset * solver.IKPositionWeight;
      if (this.effectChildNodes && solver.iterations > 0)
      {
        for (int index = 0; index < this.childBones.Length; ++index)
        {
          this.localPositions[index] = this.childBones[index].transform.position - this.bone.transform.position;
          solver.GetNode(this.childChainIndexes[index], this.childNodeIndexes[index]).offset += this.positionOffset * solver.IKPositionWeight;
        }
      }
      if (this.usePlaneNodes && (double) this.maintainRelativePositionWeight > 0.0)
        this.animatedPlaneRotation = Quaternion.LookRotation(this.planeBone2.position - this.planeBone1.position, this.planeBone3.position - this.planeBone1.position);
      this.firstUpdate = true;
    }

    public void OnPostWrite() => this.positionOffset = Vector3.zero;

    private Quaternion GetPlaneRotation(IKSolverFullBody solver)
    {
      Vector3 solverPosition1 = solver.GetNode(this.plane1ChainIndex, this.plane1NodeIndex).solverPosition;
      Vector3 solverPosition2 = solver.GetNode(this.plane2ChainIndex, this.plane2NodeIndex).solverPosition;
      Vector3 solverPosition3 = solver.GetNode(this.plane3ChainIndex, this.plane3NodeIndex).solverPosition;
      Vector3 forward = solverPosition2 - solverPosition1;
      Vector3 upwards = solverPosition3 - solverPosition1;
      if (!(forward == Vector3.zero))
        return Quaternion.LookRotation(forward, upwards);
      Warning.Log("Make sure you are not placing 2 or more FBBIK effectors of the same chain to exactly the same position.", this.bone);
      return Quaternion.identity;
    }

    public void Update(IKSolverFullBody solver)
    {
      if (this.firstUpdate)
      {
        this.animatedPosition = this.bone.position + solver.GetNode(this.chainIndex, this.nodeIndex).offset;
        this.firstUpdate = false;
      }
      solver.GetNode(this.chainIndex, this.nodeIndex).solverPosition = Vector3.Lerp(this.GetPosition(solver, out this.planeRotationOffset), this.position, this.posW);
      if (!this.effectChildNodes)
        return;
      for (int index = 0; index < this.childBones.Length; ++index)
        solver.GetNode(this.childChainIndexes[index], this.childNodeIndexes[index]).solverPosition = Vector3.Lerp(solver.GetNode(this.childChainIndexes[index], this.childNodeIndexes[index]).solverPosition, solver.GetNode(this.chainIndex, this.nodeIndex).solverPosition + this.localPositions[index], this.posW);
    }

    private Vector3 GetPosition(IKSolverFullBody solver, out Quaternion planeRotationOffset)
    {
      planeRotationOffset = Quaternion.identity;
      if (!this.isEndEffector)
        return solver.GetNode(this.chainIndex, this.nodeIndex).solverPosition;
      if ((double) this.maintainRelativePositionWeight <= 0.0)
        return this.animatedPosition;
      Vector3 vector3_1 = this.bone.position - this.planeBone1.position;
      planeRotationOffset = this.GetPlaneRotation(solver) * Quaternion.Inverse(this.animatedPlaneRotation);
      Vector3 vector3_2 = solver.GetNode(this.plane1ChainIndex, this.plane1NodeIndex).solverPosition + planeRotationOffset * vector3_1;
      planeRotationOffset = Quaternion.Lerp(Quaternion.identity, planeRotationOffset, this.maintainRelativePositionWeight);
      return Vector3.Lerp(this.animatedPosition, vector3_2 + solver.GetNode(this.chainIndex, this.nodeIndex).offset, this.maintainRelativePositionWeight);
    }
  }
}
