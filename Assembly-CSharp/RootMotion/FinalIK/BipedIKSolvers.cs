﻿using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class BipedIKSolvers
  {
    public IKSolverLimb leftFoot = new(AvatarIKGoal.LeftFoot);
    public IKSolverLimb rightFoot = new(AvatarIKGoal.RightFoot);
    public IKSolverLimb leftHand = new(AvatarIKGoal.LeftHand);
    public IKSolverLimb rightHand = new(AvatarIKGoal.RightHand);
    public IKSolverFABRIK spine = new();
    public IKSolverLookAt lookAt = new();
    public IKSolverAim aim = new();
    public Constraints pelvis = new();
    private IKSolverLimb[] _limbs;
    private IKSolver[] _ikSolvers;

    public IKSolverLimb[] limbs
    {
      get
      {
        if (_limbs == null || _limbs != null && _limbs.Length != 4)
          _limbs = [
            leftFoot,
            rightFoot,
            leftHand,
            rightHand
          ];
        return _limbs;
      }
    }

    public IKSolver[] ikSolvers
    {
      get
      {
        if (_ikSolvers == null || _ikSolvers != null && _ikSolvers.Length != 7)
          _ikSolvers = [
            leftFoot,
            rightFoot,
            leftHand,
            rightHand,
            spine,
            lookAt,
            aim
          ];
        return _ikSolvers;
      }
    }

    public void AssignReferences(BipedReferences references)
    {
      leftHand.SetChain(references.leftUpperArm, references.leftForearm, references.leftHand, references.root);
      rightHand.SetChain(references.rightUpperArm, references.rightForearm, references.rightHand, references.root);
      leftFoot.SetChain(references.leftThigh, references.leftCalf, references.leftFoot, references.root);
      rightFoot.SetChain(references.rightThigh, references.rightCalf, references.rightFoot, references.root);
      spine.SetChain(references.spine, references.root);
      lookAt.SetChain(references.spine, references.head, references.eyes, references.root);
      aim.SetChain(references.spine, references.root);
      leftFoot.goal = AvatarIKGoal.LeftFoot;
      rightFoot.goal = AvatarIKGoal.RightFoot;
      leftHand.goal = AvatarIKGoal.LeftHand;
      rightHand.goal = AvatarIKGoal.RightHand;
    }
  }
}
