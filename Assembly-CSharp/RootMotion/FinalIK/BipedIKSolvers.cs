// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.BipedIKSolvers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class BipedIKSolvers
  {
    public IKSolverLimb leftFoot = new IKSolverLimb(AvatarIKGoal.LeftFoot);
    public IKSolverLimb rightFoot = new IKSolverLimb(AvatarIKGoal.RightFoot);
    public IKSolverLimb leftHand = new IKSolverLimb(AvatarIKGoal.LeftHand);
    public IKSolverLimb rightHand = new IKSolverLimb(AvatarIKGoal.RightHand);
    public IKSolverFABRIK spine = new IKSolverFABRIK();
    public IKSolverLookAt lookAt = new IKSolverLookAt();
    public IKSolverAim aim = new IKSolverAim();
    public Constraints pelvis = new Constraints();
    private IKSolverLimb[] _limbs;
    private IKSolver[] _ikSolvers;

    public IKSolverLimb[] limbs
    {
      get
      {
        if (this._limbs == null || this._limbs != null && this._limbs.Length != 4)
          this._limbs = new IKSolverLimb[4]
          {
            this.leftFoot,
            this.rightFoot,
            this.leftHand,
            this.rightHand
          };
        return this._limbs;
      }
    }

    public IKSolver[] ikSolvers
    {
      get
      {
        if (this._ikSolvers == null || this._ikSolvers != null && this._ikSolvers.Length != 7)
          this._ikSolvers = new IKSolver[7]
          {
            (IKSolver) this.leftFoot,
            (IKSolver) this.rightFoot,
            (IKSolver) this.leftHand,
            (IKSolver) this.rightHand,
            (IKSolver) this.spine,
            (IKSolver) this.lookAt,
            (IKSolver) this.aim
          };
        return this._ikSolvers;
      }
    }

    public void AssignReferences(BipedReferences references)
    {
      this.leftHand.SetChain(references.leftUpperArm, references.leftForearm, references.leftHand, references.root);
      this.rightHand.SetChain(references.rightUpperArm, references.rightForearm, references.rightHand, references.root);
      this.leftFoot.SetChain(references.leftThigh, references.leftCalf, references.leftFoot, references.root);
      this.rightFoot.SetChain(references.rightThigh, references.rightCalf, references.rightFoot, references.root);
      this.spine.SetChain(references.spine, references.root);
      this.lookAt.SetChain(references.spine, references.head, references.eyes, references.root);
      this.aim.SetChain(references.spine, references.root);
      this.leftFoot.goal = AvatarIKGoal.LeftFoot;
      this.rightFoot.goal = AvatarIKGoal.RightFoot;
      this.leftHand.goal = AvatarIKGoal.LeftHand;
      this.rightHand.goal = AvatarIKGoal.RightHand;
    }
  }
}
