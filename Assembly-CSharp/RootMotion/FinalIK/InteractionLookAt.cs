using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class InteractionLookAt
  {
    [Tooltip("(Optional) reference to the LookAtIK component that will be used to make the character look at the objects that it is interacting with.")]
    public LookAtIK ik;
    [Tooltip("Interpolation speed of the LookAtIK target.")]
    public float lerpSpeed = 5f;
    [Tooltip("Interpolation speed of the LookAtIK weight.")]
    public float weightSpeed = 1f;
    [HideInInspector]
    public bool isPaused;
    private Transform lookAtTarget;
    private float stopLookTime;
    private float weight;
    private bool firstFBBIKSolve;

    public void Look(Transform target, float time)
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
        return;
      if ((double) this.ik.solver.IKPositionWeight <= 0.0)
        this.ik.solver.IKPosition = this.ik.solver.GetRoot().position + this.ik.solver.GetRoot().forward * 3f;
      this.lookAtTarget = target;
      this.stopLookTime = time;
    }

    public void OnFixTransforms()
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null || !this.ik.fixTransforms)
        return;
      this.ik.solver.FixTransforms();
    }

    public void Update()
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
        return;
      if (this.ik.enabled)
        this.ik.enabled = false;
      if ((UnityEngine.Object) this.lookAtTarget == (UnityEngine.Object) null)
        return;
      if (this.isPaused)
        this.stopLookTime += Time.deltaTime;
      this.weight = Mathf.Clamp(this.weight + ((double) Time.time < (double) this.stopLookTime ? this.weightSpeed : -this.weightSpeed) * Time.deltaTime, 0.0f, 1f);
      this.ik.solver.IKPositionWeight = Interp.Float(this.weight, InterpolationMode.InOutQuintic);
      this.ik.solver.IKPosition = Vector3.Lerp(this.ik.solver.IKPosition, this.lookAtTarget.position, this.lerpSpeed * Time.deltaTime);
      if ((double) this.weight <= 0.0)
        this.lookAtTarget = (Transform) null;
      this.firstFBBIKSolve = true;
    }

    public void SolveSpine()
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null || !this.firstFBBIKSolve)
        return;
      float headWeight = this.ik.solver.headWeight;
      float eyesWeight = this.ik.solver.eyesWeight;
      this.ik.solver.headWeight = 0.0f;
      this.ik.solver.eyesWeight = 0.0f;
      this.ik.solver.Update();
      this.ik.solver.headWeight = headWeight;
      this.ik.solver.eyesWeight = eyesWeight;
    }

    public void SolveHead()
    {
      if ((UnityEngine.Object) this.ik == (UnityEngine.Object) null || !this.firstFBBIKSolve)
        return;
      float bodyWeight = this.ik.solver.bodyWeight;
      this.ik.solver.bodyWeight = 0.0f;
      this.ik.solver.Update();
      this.ik.solver.bodyWeight = bodyWeight;
      this.firstFBBIKSolve = false;
    }
  }
}
