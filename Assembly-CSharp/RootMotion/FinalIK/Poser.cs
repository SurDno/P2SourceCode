// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.Poser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public abstract class Poser : SolverManager
  {
    public Transform poseRoot;
    [Range(0.0f, 1f)]
    public float weight = 1f;
    [Range(0.0f, 1f)]
    public float localRotationWeight = 1f;
    [Range(0.0f, 1f)]
    public float localPositionWeight;
    private bool initiated;

    public abstract void AutoMapping();

    protected abstract void InitiatePoser();

    protected abstract void UpdatePoser();

    protected abstract void FixPoserTransforms();

    protected override void UpdateSolver()
    {
      if (!this.initiated)
        this.InitiateSolver();
      if (!this.initiated)
        return;
      this.UpdatePoser();
    }

    protected override void InitiateSolver()
    {
      if (this.initiated)
        return;
      this.InitiatePoser();
      this.initiated = true;
    }

    protected override void FixTransforms()
    {
      if (!this.initiated)
        return;
      this.FixPoserTransforms();
    }
  }
}
