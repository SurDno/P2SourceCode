using System;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKMappingSpine : IKMapping
  {
    public Transform[] spineBones;
    public Transform leftUpperArmBone;
    public Transform rightUpperArmBone;
    public Transform leftThighBone;
    public Transform rightThighBone;
    [Range(1f, 3f)]
    public int iterations = 3;
    [Range(0.0f, 1f)]
    public float twistWeight = 1f;
    private int rootNodeIndex;
    private BoneMap[] spine = new BoneMap[0];
    private BoneMap leftUpperArm = new BoneMap();
    private BoneMap rightUpperArm = new BoneMap();
    private BoneMap leftThigh = new BoneMap();
    private BoneMap rightThigh = new BoneMap();
    private bool useFABRIK;

    public override bool IsValid(IKSolver solver, ref string message)
    {
      if (!base.IsValid(solver, ref message))
        return false;
      foreach (UnityEngine.Object spineBone in spineBones)
      {
        if (spineBone == (UnityEngine.Object) null)
        {
          message = "Spine bones contains a null reference.";
          return false;
        }
      }
      int num = 0;
      for (int index = 0; index < spineBones.Length; ++index)
      {
        if (solver.GetPoint(spineBones[index]) != null)
          ++num;
      }
      if (num == 0)
      {
        message = "IKMappingSpine does not contain any nodes.";
        return false;
      }
      if ((UnityEngine.Object) leftUpperArmBone == (UnityEngine.Object) null)
      {
        message = "IKMappingSpine is missing the left upper arm bone.";
        return false;
      }
      if ((UnityEngine.Object) rightUpperArmBone == (UnityEngine.Object) null)
      {
        message = "IKMappingSpine is missing the right upper arm bone.";
        return false;
      }
      if ((UnityEngine.Object) leftThighBone == (UnityEngine.Object) null)
      {
        message = "IKMappingSpine is missing the left thigh bone.";
        return false;
      }
      if ((UnityEngine.Object) rightThighBone == (UnityEngine.Object) null)
      {
        message = "IKMappingSpine is missing the right thigh bone.";
        return false;
      }
      if (solver.GetPoint(leftUpperArmBone) == null)
      {
        message = "Full Body IK is missing the left upper arm node.";
        return false;
      }
      if (solver.GetPoint(rightUpperArmBone) == null)
      {
        message = "Full Body IK is missing the right upper arm node.";
        return false;
      }
      if (solver.GetPoint(leftThighBone) == null)
      {
        message = "Full Body IK is missing the left thigh node.";
        return false;
      }
      if (solver.GetPoint(rightThighBone) != null)
        return true;
      message = "Full Body IK is missing the right thigh node.";
      return false;
    }

    public IKMappingSpine()
    {
    }

    public IKMappingSpine(
      Transform[] spineBones,
      Transform leftUpperArmBone,
      Transform rightUpperArmBone,
      Transform leftThighBone,
      Transform rightThighBone)
    {
      SetBones(spineBones, leftUpperArmBone, rightUpperArmBone, leftThighBone, rightThighBone);
    }

    public void SetBones(
      Transform[] spineBones,
      Transform leftUpperArmBone,
      Transform rightUpperArmBone,
      Transform leftThighBone,
      Transform rightThighBone)
    {
      this.spineBones = spineBones;
      this.leftUpperArmBone = leftUpperArmBone;
      this.rightUpperArmBone = rightUpperArmBone;
      this.leftThighBone = leftThighBone;
      this.rightThighBone = rightThighBone;
    }

    public void StoreDefaultLocalState()
    {
      for (int index = 0; index < spine.Length; ++index)
        spine[index].StoreDefaultLocalState();
    }

    public void FixTransforms()
    {
      for (int index = 0; index < spine.Length; ++index)
        spine[index].FixTransform(index == 0 || index == spine.Length - 1);
    }

    public override void Initiate(IKSolverFullBody solver)
    {
      if (iterations <= 0)
        iterations = 3;
      if (spine == null || spine.Length != spineBones.Length)
        spine = new BoneMap[spineBones.Length];
      rootNodeIndex = -1;
      for (int index = 0; index < spineBones.Length; ++index)
      {
        if (spine[index] == null)
          spine[index] = new BoneMap();
        spine[index].Initiate(spineBones[index], solver);
        if (spine[index].isNodeBone)
          rootNodeIndex = index;
      }
      if (leftUpperArm == null)
        leftUpperArm = new BoneMap();
      if (rightUpperArm == null)
        rightUpperArm = new BoneMap();
      if (leftThigh == null)
        leftThigh = new BoneMap();
      if (rightThigh == null)
        rightThigh = new BoneMap();
      leftUpperArm.Initiate(leftUpperArmBone, solver);
      rightUpperArm.Initiate(rightUpperArmBone, solver);
      leftThigh.Initiate(leftThighBone, solver);
      rightThigh.Initiate(rightThighBone, solver);
      for (int index = 0; index < spine.Length; ++index)
        spine[index].SetIKPosition();
      spine[0].SetPlane(solver, spine[rootNodeIndex].transform, leftThigh.transform, rightThigh.transform);
      for (int index = 0; index < spine.Length - 1; ++index)
      {
        spine[index].SetLength(spine[index + 1]);
        spine[index].SetLocalSwingAxis(spine[index + 1]);
        spine[index].SetLocalTwistAxis(leftUpperArm.transform.position - rightUpperArm.transform.position, spine[index + 1].transform.position - spine[index].transform.position);
      }
      spine[spine.Length - 1].SetPlane(solver, spine[rootNodeIndex].transform, leftUpperArm.transform, rightUpperArm.transform);
      spine[spine.Length - 1].SetLocalSwingAxis(leftUpperArm, rightUpperArm);
      useFABRIK = UseFABRIK();
    }

    private bool UseFABRIK() => spine.Length > 3 || rootNodeIndex != 1;

    public void ReadPose()
    {
      spine[0].UpdatePlane(true, true);
      for (int index = 0; index < spine.Length - 1; ++index)
      {
        spine[index].SetLength(spine[index + 1]);
        spine[index].SetLocalSwingAxis(spine[index + 1]);
        spine[index].SetLocalTwistAxis(leftUpperArm.transform.position - rightUpperArm.transform.position, spine[index + 1].transform.position - spine[index].transform.position);
      }
      spine[spine.Length - 1].UpdatePlane(true, true);
      spine[spine.Length - 1].SetLocalSwingAxis(leftUpperArm, rightUpperArm);
    }

    public void WritePose(IKSolverFullBody solver)
    {
      Vector3 planePosition1 = spine[0].GetPlanePosition(solver);
      Vector3 solverPosition = solver.GetNode(spine[rootNodeIndex].chainIndex, spine[rootNodeIndex].nodeIndex).solverPosition;
      Vector3 planePosition2 = spine[spine.Length - 1].GetPlanePosition(solver);
      if (useFABRIK)
      {
        Vector3 vector3 = solver.GetNode(spine[rootNodeIndex].chainIndex, spine[rootNodeIndex].nodeIndex).solverPosition - spine[rootNodeIndex].transform.position;
        for (int index = 0; index < spine.Length; ++index)
          spine[index].ikPosition = spine[index].transform.position + vector3;
        for (int index = 0; index < iterations; ++index)
        {
          ForwardReach(planePosition2);
          BackwardReach(planePosition1);
          spine[rootNodeIndex].ikPosition = solverPosition;
        }
      }
      else
      {
        spine[0].ikPosition = planePosition1;
        spine[rootNodeIndex].ikPosition = solverPosition;
      }
      spine[spine.Length - 1].ikPosition = planePosition2;
      MapToSolverPositions(solver);
    }

    public void ForwardReach(Vector3 position)
    {
      spine[spineBones.Length - 1].ikPosition = position;
      for (int index = spine.Length - 2; index > -1; --index)
        spine[index].ikPosition = SolveFABRIKJoint(spine[index].ikPosition, spine[index + 1].ikPosition, spine[index].length);
    }

    private void BackwardReach(Vector3 position)
    {
      spine[0].ikPosition = position;
      for (int index = 1; index < spine.Length; ++index)
        spine[index].ikPosition = SolveFABRIKJoint(spine[index].ikPosition, spine[index - 1].ikPosition, spine[index - 1].length);
    }

    private void MapToSolverPositions(IKSolverFullBody solver)
    {
      spine[0].SetToIKPosition();
      spine[0].RotateToPlane(solver, 1f);
      for (int index = 1; index < spine.Length - 1; ++index)
      {
        spine[index].Swing(spine[index + 1].ikPosition, 1f);
        if (twistWeight > 0.0)
        {
          float num = index / (spine.Length - 2f);
          Vector3 solverPosition1 = solver.GetNode(leftUpperArm.chainIndex, leftUpperArm.nodeIndex).solverPosition;
          Vector3 solverPosition2 = solver.GetNode(rightUpperArm.chainIndex, rightUpperArm.nodeIndex).solverPosition;
          spine[index].Twist(solverPosition1 - solverPosition2, spine[index + 1].ikPosition - spine[index].transform.position, num * twistWeight);
        }
      }
      spine[spine.Length - 1].SetToIKPosition();
      spine[spine.Length - 1].RotateToPlane(solver, 1f);
    }
  }
}
