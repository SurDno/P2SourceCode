﻿using UnityEngine;

namespace RootMotion
{
  public class SolverManager : MonoBehaviour
  {
    [Tooltip("If true, will fix all the Transforms used by the solver to their initial state in each Update. This prevents potential problems with unanimated bones and animator culling with a small cost of performance. Not recommended for CCD and FABRIK solvers.")]
    public bool fixTransforms = true;
    private Animator animator;
    private Animation legacy;
    private bool updateFrame;
    private bool componentInitiated;
    private bool skipSolverUpdate;

    public void Disable()
    {
      Debug.Log("IK.Disable() is deprecated. Use enabled = false instead", transform);
      enabled = false;
    }

    protected virtual void InitiateSolver()
    {
    }

    protected virtual void UpdateSolver()
    {
    }

    protected virtual void FixTransforms()
    {
    }

    private void OnDisable()
    {
      if (!Application.isPlaying)
        return;
      Initiate();
    }

    private void Start() => Initiate();

    private bool animatePhysics
    {
      get
      {
        if (animator != null)
          return animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
        return legacy != null && legacy.animatePhysics;
      }
    }

    private void Initiate()
    {
      if (componentInitiated)
        return;
      FindAnimatorRecursive(transform, true);
      InitiateSolver();
      componentInitiated = true;
    }

    private void Update()
    {
      if (skipSolverUpdate || animatePhysics || !fixTransforms)
        return;
      FixTransforms();
    }

    private void FindAnimatorRecursive(Transform t, bool findInChildren)
    {
      if (isAnimated)
        return;
      animator = t.GetComponent<Animator>();
      legacy = t.GetComponent<Animation>();
      if (isAnimated)
        return;
      if (animator == null & findInChildren)
        animator = t.GetComponentInChildren<Animator>();
      if (legacy == null & findInChildren)
        legacy = t.GetComponentInChildren<Animation>();
      if (isAnimated || !(t.parent != null))
        return;
      FindAnimatorRecursive(t.parent, false);
    }

    private bool isAnimated => animator != null || legacy != null;

    private void FixedUpdate()
    {
      if (skipSolverUpdate)
        skipSolverUpdate = false;
      updateFrame = true;
      if (!animatePhysics || !fixTransforms)
        return;
      FixTransforms();
    }

    private void LateUpdate()
    {
      if (skipSolverUpdate)
        return;
      if (!animatePhysics)
        updateFrame = true;
      if (!updateFrame)
        return;
      updateFrame = false;
      UpdateSolver();
    }

    public void UpdateSolverExternal()
    {
      if (!enabled)
        return;
      skipSolverUpdate = true;
      UpdateSolver();
    }
  }
}
