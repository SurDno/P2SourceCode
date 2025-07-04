﻿using UnityEngine;

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
      if (!initiated)
        InitiateSolver();
      if (!initiated)
        return;
      UpdatePoser();
    }

    protected override void InitiateSolver()
    {
      if (initiated)
        return;
      InitiatePoser();
      initiated = true;
    }

    protected override void FixTransforms()
    {
      if (!initiated)
        return;
      FixPoserTransforms();
    }
  }
}
