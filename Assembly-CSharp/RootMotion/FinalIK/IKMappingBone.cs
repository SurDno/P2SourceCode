using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKMappingBone : IKMapping
  {
    public Transform bone;
    [Range(0.0f, 1f)]
    public float maintainRotationWeight = 1f;
    private IKMapping.BoneMap boneMap = new IKMapping.BoneMap();

    public override bool IsValid(IKSolver solver, ref string message)
    {
      if (!base.IsValid(solver, ref message))
        return false;
      if (!((UnityEngine.Object) this.bone == (UnityEngine.Object) null))
        return true;
      message = "IKMappingBone's bone is null.";
      return false;
    }

    public IKMappingBone()
    {
    }

    public IKMappingBone(Transform bone) => this.bone = bone;

    public void StoreDefaultLocalState() => this.boneMap.StoreDefaultLocalState();

    public void FixTransforms() => this.boneMap.FixTransform(false);

    public override void Initiate(IKSolverFullBody solver)
    {
      if (this.boneMap == null)
        this.boneMap = new IKMapping.BoneMap();
      this.boneMap.Initiate(this.bone, solver);
    }

    public void ReadPose() => this.boneMap.MaintainRotation();

    public void WritePose(float solverWeight)
    {
      this.boneMap.RotateToMaintain(solverWeight * this.maintainRotationWeight);
    }
  }
}
