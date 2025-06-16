// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKMappingSpine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
    private IKMapping.BoneMap[] spine = new IKMapping.BoneMap[0];
    private IKMapping.BoneMap leftUpperArm = new IKMapping.BoneMap();
    private IKMapping.BoneMap rightUpperArm = new IKMapping.BoneMap();
    private IKMapping.BoneMap leftThigh = new IKMapping.BoneMap();
    private IKMapping.BoneMap rightThigh = new IKMapping.BoneMap();
    private bool useFABRIK;

    public override bool IsValid(IKSolver solver, ref string message)
    {
      if (!base.IsValid(solver, ref message))
        return false;
      foreach (UnityEngine.Object spineBone in this.spineBones)
      {
        if (spineBone == (UnityEngine.Object) null)
        {
          message = "Spine bones contains a null reference.";
          return false;
        }
      }
      int num = 0;
      for (int index = 0; index < this.spineBones.Length; ++index)
      {
        if (solver.GetPoint(this.spineBones[index]) != null)
          ++num;
      }
      if (num == 0)
      {
        message = "IKMappingSpine does not contain any nodes.";
        return false;
      }
      if ((UnityEngine.Object) this.leftUpperArmBone == (UnityEngine.Object) null)
      {
        message = "IKMappingSpine is missing the left upper arm bone.";
        return false;
      }
      if ((UnityEngine.Object) this.rightUpperArmBone == (UnityEngine.Object) null)
      {
        message = "IKMappingSpine is missing the right upper arm bone.";
        return false;
      }
      if ((UnityEngine.Object) this.leftThighBone == (UnityEngine.Object) null)
      {
        message = "IKMappingSpine is missing the left thigh bone.";
        return false;
      }
      if ((UnityEngine.Object) this.rightThighBone == (UnityEngine.Object) null)
      {
        message = "IKMappingSpine is missing the right thigh bone.";
        return false;
      }
      if (solver.GetPoint(this.leftUpperArmBone) == null)
      {
        message = "Full Body IK is missing the left upper arm node.";
        return false;
      }
      if (solver.GetPoint(this.rightUpperArmBone) == null)
      {
        message = "Full Body IK is missing the right upper arm node.";
        return false;
      }
      if (solver.GetPoint(this.leftThighBone) == null)
      {
        message = "Full Body IK is missing the left thigh node.";
        return false;
      }
      if (solver.GetPoint(this.rightThighBone) != null)
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
      this.SetBones(spineBones, leftUpperArmBone, rightUpperArmBone, leftThighBone, rightThighBone);
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
      for (int index = 0; index < this.spine.Length; ++index)
        this.spine[index].StoreDefaultLocalState();
    }

    public void FixTransforms()
    {
      for (int index = 0; index < this.spine.Length; ++index)
        this.spine[index].FixTransform(index == 0 || index == this.spine.Length - 1);
    }

    public override void Initiate(IKSolverFullBody solver)
    {
      if (this.iterations <= 0)
        this.iterations = 3;
      if (this.spine == null || this.spine.Length != this.spineBones.Length)
        this.spine = new IKMapping.BoneMap[this.spineBones.Length];
      this.rootNodeIndex = -1;
      for (int index = 0; index < this.spineBones.Length; ++index)
      {
        if (this.spine[index] == null)
          this.spine[index] = new IKMapping.BoneMap();
        this.spine[index].Initiate(this.spineBones[index], solver);
        if (this.spine[index].isNodeBone)
          this.rootNodeIndex = index;
      }
      if (this.leftUpperArm == null)
        this.leftUpperArm = new IKMapping.BoneMap();
      if (this.rightUpperArm == null)
        this.rightUpperArm = new IKMapping.BoneMap();
      if (this.leftThigh == null)
        this.leftThigh = new IKMapping.BoneMap();
      if (this.rightThigh == null)
        this.rightThigh = new IKMapping.BoneMap();
      this.leftUpperArm.Initiate(this.leftUpperArmBone, solver);
      this.rightUpperArm.Initiate(this.rightUpperArmBone, solver);
      this.leftThigh.Initiate(this.leftThighBone, solver);
      this.rightThigh.Initiate(this.rightThighBone, solver);
      for (int index = 0; index < this.spine.Length; ++index)
        this.spine[index].SetIKPosition();
      this.spine[0].SetPlane(solver, this.spine[this.rootNodeIndex].transform, this.leftThigh.transform, this.rightThigh.transform);
      for (int index = 0; index < this.spine.Length - 1; ++index)
      {
        this.spine[index].SetLength(this.spine[index + 1]);
        this.spine[index].SetLocalSwingAxis(this.spine[index + 1]);
        this.spine[index].SetLocalTwistAxis(this.leftUpperArm.transform.position - this.rightUpperArm.transform.position, this.spine[index + 1].transform.position - this.spine[index].transform.position);
      }
      this.spine[this.spine.Length - 1].SetPlane(solver, this.spine[this.rootNodeIndex].transform, this.leftUpperArm.transform, this.rightUpperArm.transform);
      this.spine[this.spine.Length - 1].SetLocalSwingAxis(this.leftUpperArm, this.rightUpperArm);
      this.useFABRIK = this.UseFABRIK();
    }

    private bool UseFABRIK() => this.spine.Length > 3 || this.rootNodeIndex != 1;

    public void ReadPose()
    {
      this.spine[0].UpdatePlane(true, true);
      for (int index = 0; index < this.spine.Length - 1; ++index)
      {
        this.spine[index].SetLength(this.spine[index + 1]);
        this.spine[index].SetLocalSwingAxis(this.spine[index + 1]);
        this.spine[index].SetLocalTwistAxis(this.leftUpperArm.transform.position - this.rightUpperArm.transform.position, this.spine[index + 1].transform.position - this.spine[index].transform.position);
      }
      this.spine[this.spine.Length - 1].UpdatePlane(true, true);
      this.spine[this.spine.Length - 1].SetLocalSwingAxis(this.leftUpperArm, this.rightUpperArm);
    }

    public void WritePose(IKSolverFullBody solver)
    {
      Vector3 planePosition1 = this.spine[0].GetPlanePosition(solver);
      Vector3 solverPosition = solver.GetNode(this.spine[this.rootNodeIndex].chainIndex, this.spine[this.rootNodeIndex].nodeIndex).solverPosition;
      Vector3 planePosition2 = this.spine[this.spine.Length - 1].GetPlanePosition(solver);
      if (this.useFABRIK)
      {
        Vector3 vector3 = solver.GetNode(this.spine[this.rootNodeIndex].chainIndex, this.spine[this.rootNodeIndex].nodeIndex).solverPosition - this.spine[this.rootNodeIndex].transform.position;
        for (int index = 0; index < this.spine.Length; ++index)
          this.spine[index].ikPosition = this.spine[index].transform.position + vector3;
        for (int index = 0; index < this.iterations; ++index)
        {
          this.ForwardReach(planePosition2);
          this.BackwardReach(planePosition1);
          this.spine[this.rootNodeIndex].ikPosition = solverPosition;
        }
      }
      else
      {
        this.spine[0].ikPosition = planePosition1;
        this.spine[this.rootNodeIndex].ikPosition = solverPosition;
      }
      this.spine[this.spine.Length - 1].ikPosition = planePosition2;
      this.MapToSolverPositions(solver);
    }

    public void ForwardReach(Vector3 position)
    {
      this.spine[this.spineBones.Length - 1].ikPosition = position;
      for (int index = this.spine.Length - 2; index > -1; --index)
        this.spine[index].ikPosition = this.SolveFABRIKJoint(this.spine[index].ikPosition, this.spine[index + 1].ikPosition, this.spine[index].length);
    }

    private void BackwardReach(Vector3 position)
    {
      this.spine[0].ikPosition = position;
      for (int index = 1; index < this.spine.Length; ++index)
        this.spine[index].ikPosition = this.SolveFABRIKJoint(this.spine[index].ikPosition, this.spine[index - 1].ikPosition, this.spine[index - 1].length);
    }

    private void MapToSolverPositions(IKSolverFullBody solver)
    {
      this.spine[0].SetToIKPosition();
      this.spine[0].RotateToPlane(solver, 1f);
      for (int index = 1; index < this.spine.Length - 1; ++index)
      {
        this.spine[index].Swing(this.spine[index + 1].ikPosition, 1f);
        if ((double) this.twistWeight > 0.0)
        {
          float num = (float) index / ((float) this.spine.Length - 2f);
          Vector3 solverPosition1 = solver.GetNode(this.leftUpperArm.chainIndex, this.leftUpperArm.nodeIndex).solverPosition;
          Vector3 solverPosition2 = solver.GetNode(this.rightUpperArm.chainIndex, this.rightUpperArm.nodeIndex).solverPosition;
          this.spine[index].Twist(solverPosition1 - solverPosition2, this.spine[index + 1].ikPosition - this.spine[index].transform.position, num * this.twistWeight);
        }
      }
      this.spine[this.spine.Length - 1].SetToIKPosition();
      this.spine[this.spine.Length - 1].RotateToPlane(solver, 1f);
    }
  }
}
