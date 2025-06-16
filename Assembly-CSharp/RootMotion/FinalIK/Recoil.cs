using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class Recoil : OffsetModifier
  {
    [Tooltip("Reference to the AimIK component. Optional, only used to getting the aiming direction.")]
    public AimIK aimIK;
    [Tooltip("Set this true if you are using IKExecutionOrder.cs or a custom script to force AimIK solve after FBBIK.")]
    public bool aimIKSolvedLast;
    [Tooltip("Which hand is holding the weapon?")]
    public Recoil.Handedness handedness;
    [Tooltip("Check for 2-handed weapons.")]
    public bool twoHanded = true;
    [Tooltip("Weight curve for the recoil offsets. Recoil procedure is as long as this curve.")]
    public AnimationCurve recoilWeight;
    [Tooltip("How much is the magnitude randomized each time Recoil is called?")]
    public float magnitudeRandom = 0.1f;
    [Tooltip("How much is the rotation randomized each time Recoil is called?")]
    public Vector3 rotationRandom;
    [Tooltip("Rotating the primary hand bone for the recoil (in local space).")]
    public Vector3 handRotationOffset;
    [Tooltip("Time of blending in another recoil when doing automatic fire.")]
    public float blendTime;
    [Space(10f)]
    [Tooltip("FBBIK effector position offsets for the recoil (in aiming direction space).")]
    public Recoil.RecoilOffset[] offsets;
    [HideInInspector]
    public Quaternion rotationOffset = Quaternion.identity;
    private float magnitudeMlp = 1f;
    private float endTime = -1f;
    private Quaternion handRotation;
    private Quaternion secondaryHandRelativeRotation;
    private Quaternion randomRotation;
    private float length = 1f;
    private bool initiated;
    private float blendWeight;
    private float w;
    private Quaternion primaryHandRotation = Quaternion.identity;
    private bool handRotationsSet;
    private Vector3 aimIKAxis;

    public bool isFinished => (double) Time.time > (double) this.endTime;

    public void SetHandRotations(Quaternion leftHandRotation, Quaternion rightHandRotation)
    {
      this.primaryHandRotation = this.handedness != Recoil.Handedness.Left ? rightHandRotation : leftHandRotation;
      this.handRotationsSet = true;
    }

    public void Fire(float magnitude)
    {
      float num = magnitude * UnityEngine.Random.value * this.magnitudeRandom;
      this.magnitudeMlp = magnitude + num;
      this.randomRotation = Quaternion.Euler(this.rotationRandom * UnityEngine.Random.value);
      foreach (Recoil.RecoilOffset offset in this.offsets)
        offset.Start();
      this.blendWeight = (double) Time.time >= (double) this.endTime ? 1f : 0.0f;
      Keyframe[] keys = this.recoilWeight.keys;
      this.length = keys[keys.Length - 1].time;
      this.endTime = Time.time + this.length;
    }

    protected override void OnModifyOffset()
    {
      if ((UnityEngine.Object) this.aimIK != (UnityEngine.Object) null)
        this.aimIKAxis = this.aimIK.solver.axis;
      if ((double) Time.time >= (double) this.endTime)
      {
        this.rotationOffset = Quaternion.identity;
      }
      else
      {
        if (!this.initiated && (UnityEngine.Object) this.ik != (UnityEngine.Object) null)
        {
          this.initiated = true;
          IKSolverFullBodyBiped solver1 = this.ik.solver;
          solver1.OnPostUpdate = solver1.OnPostUpdate + new IKSolver.UpdateDelegate(this.AfterFBBIK);
          if ((UnityEngine.Object) this.aimIK != (UnityEngine.Object) null)
          {
            IKSolverAim solver2 = this.aimIK.solver;
            solver2.OnPostUpdate = solver2.OnPostUpdate + new IKSolver.UpdateDelegate(this.AfterAimIK);
          }
        }
        this.blendTime = Mathf.Max(this.blendTime, 0.0f);
        this.blendWeight = (double) this.blendTime <= 0.0 ? 1f : Mathf.Min(this.blendWeight + Time.deltaTime * (1f / this.blendTime), 1f);
        this.w = Mathf.Lerp(this.w, this.recoilWeight.Evaluate(this.length - (this.endTime - Time.time)) * this.magnitudeMlp, this.blendWeight);
        Quaternion rotation = this.randomRotation * (!((UnityEngine.Object) this.aimIK != (UnityEngine.Object) null) || !((UnityEngine.Object) this.aimIK.solver.transform != (UnityEngine.Object) null) || this.aimIKSolvedLast ? this.ik.references.root.rotation : Quaternion.LookRotation(this.aimIK.solver.IKPosition - this.aimIK.solver.transform.position, this.ik.references.root.up));
        foreach (Recoil.RecoilOffset offset in this.offsets)
          offset.Apply(this.ik.solver, rotation, this.w, this.length, this.endTime - Time.time);
        if (!this.handRotationsSet)
          this.primaryHandRotation = this.primaryHand.rotation;
        this.handRotationsSet = false;
        this.rotationOffset = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(this.randomRotation * this.primaryHandRotation * this.handRotationOffset), this.w);
        this.handRotation = this.rotationOffset * this.primaryHandRotation;
        if (this.twoHanded)
        {
          Vector3 vector3 = Quaternion.Inverse(this.primaryHand.rotation) * (this.secondaryHand.position - this.primaryHand.position);
          this.secondaryHandRelativeRotation = Quaternion.Inverse(this.primaryHand.rotation) * this.secondaryHand.rotation;
          this.secondaryHandEffector.positionOffset += this.primaryHand.position + this.primaryHandEffector.positionOffset + this.handRotation * vector3 - (this.secondaryHand.position + this.secondaryHandEffector.positionOffset);
        }
        if (!((UnityEngine.Object) this.aimIK != (UnityEngine.Object) null) || !this.aimIKSolvedLast)
          return;
        this.aimIK.solver.axis = Quaternion.Inverse(this.ik.references.root.rotation) * Quaternion.Inverse(this.rotationOffset) * this.aimIKAxis;
      }
    }

    private void AfterFBBIK()
    {
      if ((double) Time.time >= (double) this.endTime)
        return;
      this.primaryHand.rotation = this.handRotation;
      if (!this.twoHanded)
        return;
      this.secondaryHand.rotation = this.primaryHand.rotation * this.secondaryHandRelativeRotation;
    }

    private void AfterAimIK()
    {
      if (!this.aimIKSolvedLast)
        return;
      this.aimIK.solver.axis = this.aimIKAxis;
    }

    private IKEffector primaryHandEffector
    {
      get
      {
        return this.handedness == Recoil.Handedness.Right ? this.ik.solver.rightHandEffector : this.ik.solver.leftHandEffector;
      }
    }

    private IKEffector secondaryHandEffector
    {
      get
      {
        return this.handedness == Recoil.Handedness.Right ? this.ik.solver.leftHandEffector : this.ik.solver.rightHandEffector;
      }
    }

    private Transform primaryHand => this.primaryHandEffector.bone;

    private Transform secondaryHand => this.secondaryHandEffector.bone;

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!((UnityEngine.Object) this.ik != (UnityEngine.Object) null) || !this.initiated)
        return;
      IKSolverFullBodyBiped solver1 = this.ik.solver;
      solver1.OnPostUpdate = solver1.OnPostUpdate - new IKSolver.UpdateDelegate(this.AfterFBBIK);
      if ((UnityEngine.Object) this.aimIK != (UnityEngine.Object) null)
      {
        IKSolverAim solver2 = this.aimIK.solver;
        solver2.OnPostUpdate = solver2.OnPostUpdate - new IKSolver.UpdateDelegate(this.AfterAimIK);
      }
    }

    [Serializable]
    public class RecoilOffset
    {
      [Tooltip("Offset vector for the associated effector when doing recoil.")]
      public Vector3 offset;
      [Tooltip("When firing before the last recoil has faded, how much of the current recoil offset will be maintained?")]
      [Range(0.0f, 1f)]
      public float additivity = 1f;
      [Tooltip("Max additive recoil for automatic fire.")]
      public float maxAdditiveOffsetMag = 0.2f;
      [Tooltip("Linking this recoil offset to FBBIK effectors.")]
      public Recoil.RecoilOffset.EffectorLink[] effectorLinks;
      private Vector3 additiveOffset;
      private Vector3 lastOffset;

      public void Start()
      {
        if ((double) this.additivity <= 0.0)
          return;
        this.additiveOffset = Vector3.ClampMagnitude(this.lastOffset * this.additivity, this.maxAdditiveOffsetMag);
      }

      public void Apply(
        IKSolverFullBodyBiped solver,
        Quaternion rotation,
        float masterWeight,
        float length,
        float timeLeft)
      {
        this.additiveOffset = Vector3.Lerp(Vector3.zero, this.additiveOffset, timeLeft / length);
        this.lastOffset = rotation * (this.offset * masterWeight) + rotation * this.additiveOffset;
        foreach (Recoil.RecoilOffset.EffectorLink effectorLink in this.effectorLinks)
          solver.GetEffector(effectorLink.effector).positionOffset += this.lastOffset * effectorLink.weight;
      }

      [Serializable]
      public class EffectorLink
      {
        [Tooltip("Type of the FBBIK effector to use")]
        public FullBodyBipedEffector effector;
        [Tooltip("Weight of using this effector")]
        public float weight;
      }
    }

    [Serializable]
    public enum Handedness
    {
      Right,
      Left,
    }
  }
}
