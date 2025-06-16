// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKMappingBone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
