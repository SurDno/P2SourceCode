using System;

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
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null)
        return;
      if (ik.solver.IKPositionWeight <= 0.0)
        ik.solver.IKPosition = ik.solver.GetRoot().position + ik.solver.GetRoot().forward * 3f;
      lookAtTarget = target;
      stopLookTime = time;
    }

    public void OnFixTransforms()
    {
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null || !ik.fixTransforms)
        return;
      ik.solver.FixTransforms();
    }

    public void Update()
    {
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null)
        return;
      if (ik.enabled)
        ik.enabled = false;
      if ((UnityEngine.Object) lookAtTarget == (UnityEngine.Object) null)
        return;
      if (isPaused)
        stopLookTime += Time.deltaTime;
      weight = Mathf.Clamp(weight + ((double) Time.time < stopLookTime ? weightSpeed : -weightSpeed) * Time.deltaTime, 0.0f, 1f);
      ik.solver.IKPositionWeight = Interp.Float(weight, InterpolationMode.InOutQuintic);
      ik.solver.IKPosition = Vector3.Lerp(ik.solver.IKPosition, lookAtTarget.position, lerpSpeed * Time.deltaTime);
      if (weight <= 0.0)
        lookAtTarget = (Transform) null;
      firstFBBIKSolve = true;
    }

    public void SolveSpine()
    {
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null || !firstFBBIKSolve)
        return;
      float headWeight = ik.solver.headWeight;
      float eyesWeight = ik.solver.eyesWeight;
      ik.solver.headWeight = 0.0f;
      ik.solver.eyesWeight = 0.0f;
      ik.solver.Update();
      ik.solver.headWeight = headWeight;
      ik.solver.eyesWeight = eyesWeight;
    }

    public void SolveHead()
    {
      if ((UnityEngine.Object) ik == (UnityEngine.Object) null || !firstFBBIKSolve)
        return;
      float bodyWeight = ik.solver.bodyWeight;
      ik.solver.bodyWeight = 0.0f;
      ik.solver.Update();
      ik.solver.bodyWeight = bodyWeight;
      firstFBBIKSolve = false;
    }
  }
}
