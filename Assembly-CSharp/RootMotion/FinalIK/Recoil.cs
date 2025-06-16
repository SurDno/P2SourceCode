using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RootMotion.FinalIK;

public class Recoil : OffsetModifier {
	[Tooltip("Reference to the AimIK component. Optional, only used to getting the aiming direction.")]
	public AimIK aimIK;

	[Tooltip("Set this true if you are using IKExecutionOrder.cs or a custom script to force AimIK solve after FBBIK.")]
	public bool aimIKSolvedLast;

	[Tooltip("Which hand is holding the weapon?")]
	public Handedness handedness;

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

	[Space(10f)] [Tooltip("FBBIK effector position offsets for the recoil (in aiming direction space).")]
	public RecoilOffset[] offsets;

	[HideInInspector] public Quaternion rotationOffset = Quaternion.identity;
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

	public bool isFinished => Time.time > (double)endTime;

	public void SetHandRotations(Quaternion leftHandRotation, Quaternion rightHandRotation) {
		primaryHandRotation = handedness != Handedness.Left ? rightHandRotation : leftHandRotation;
		handRotationsSet = true;
	}

	public void Fire(float magnitude) {
		var num = magnitude * Random.value * magnitudeRandom;
		magnitudeMlp = magnitude + num;
		randomRotation = Quaternion.Euler(rotationRandom * Random.value);
		foreach (var offset in offsets)
			offset.Start();
		blendWeight = Time.time >= (double)endTime ? 1f : 0.0f;
		var keys = recoilWeight.keys;
		length = keys[keys.Length - 1].time;
		endTime = Time.time + length;
	}

	protected override void OnModifyOffset() {
		if (aimIK != null)
			aimIKAxis = aimIK.solver.axis;
		if (Time.time >= (double)endTime)
			rotationOffset = Quaternion.identity;
		else {
			if (!initiated && ik != null) {
				initiated = true;
				var solver1 = ik.solver;
				solver1.OnPostUpdate = solver1.OnPostUpdate + AfterFBBIK;
				if (aimIK != null) {
					var solver2 = aimIK.solver;
					solver2.OnPostUpdate = solver2.OnPostUpdate + AfterAimIK;
				}
			}

			blendTime = Mathf.Max(blendTime, 0.0f);
			blendWeight = blendTime <= 0.0 ? 1f : Mathf.Min(blendWeight + Time.deltaTime * (1f / blendTime), 1f);
			w = Mathf.Lerp(w, recoilWeight.Evaluate(length - (endTime - Time.time)) * magnitudeMlp, blendWeight);
			var rotation = randomRotation * (!(aimIK != null) || !(aimIK.solver.transform != null) || aimIKSolvedLast
				? ik.references.root.rotation
				: Quaternion.LookRotation(aimIK.solver.IKPosition - aimIK.solver.transform.position,
					ik.references.root.up));
			foreach (var offset in offsets)
				offset.Apply(ik.solver, rotation, w, length, endTime - Time.time);
			if (!handRotationsSet)
				primaryHandRotation = primaryHand.rotation;
			handRotationsSet = false;
			rotationOffset = Quaternion.Lerp(Quaternion.identity,
				Quaternion.Euler(randomRotation * primaryHandRotation * handRotationOffset), w);
			handRotation = rotationOffset * primaryHandRotation;
			if (twoHanded) {
				var vector3 = Quaternion.Inverse(primaryHand.rotation) *
				              (secondaryHand.position - primaryHand.position);
				secondaryHandRelativeRotation = Quaternion.Inverse(primaryHand.rotation) * secondaryHand.rotation;
				secondaryHandEffector.positionOffset += primaryHand.position + primaryHandEffector.positionOffset +
					handRotation * vector3 - (secondaryHand.position + secondaryHandEffector.positionOffset);
			}

			if (!(aimIK != null) || !aimIKSolvedLast)
				return;
			aimIK.solver.axis = Quaternion.Inverse(ik.references.root.rotation) * Quaternion.Inverse(rotationOffset) *
			                    aimIKAxis;
		}
	}

	private void AfterFBBIK() {
		if (Time.time >= (double)endTime)
			return;
		primaryHand.rotation = handRotation;
		if (!twoHanded)
			return;
		secondaryHand.rotation = primaryHand.rotation * secondaryHandRelativeRotation;
	}

	private void AfterAimIK() {
		if (!aimIKSolvedLast)
			return;
		aimIK.solver.axis = aimIKAxis;
	}

	private IKEffector primaryHandEffector =>
		handedness == Handedness.Right ? ik.solver.rightHandEffector : ik.solver.leftHandEffector;

	private IKEffector secondaryHandEffector =>
		handedness == Handedness.Right ? ik.solver.leftHandEffector : ik.solver.rightHandEffector;

	private Transform primaryHand => primaryHandEffector.bone;

	private Transform secondaryHand => secondaryHandEffector.bone;

	protected override void OnDestroy() {
		base.OnDestroy();
		if (!(ik != null) || !initiated)
			return;
		var solver1 = ik.solver;
		solver1.OnPostUpdate = solver1.OnPostUpdate - AfterFBBIK;
		if (aimIK != null) {
			var solver2 = aimIK.solver;
			solver2.OnPostUpdate = solver2.OnPostUpdate - AfterAimIK;
		}
	}

	[Serializable]
	public class RecoilOffset {
		[Tooltip("Offset vector for the associated effector when doing recoil.")]
		public Vector3 offset;

		[Tooltip(
			"When firing before the last recoil has faded, how much of the current recoil offset will be maintained?")]
		[Range(0.0f, 1f)]
		public float additivity = 1f;

		[Tooltip("Max additive recoil for automatic fire.")]
		public float maxAdditiveOffsetMag = 0.2f;

		[Tooltip("Linking this recoil offset to FBBIK effectors.")]
		public EffectorLink[] effectorLinks;

		private Vector3 additiveOffset;
		private Vector3 lastOffset;

		public void Start() {
			if (additivity <= 0.0)
				return;
			additiveOffset = Vector3.ClampMagnitude(lastOffset * additivity, maxAdditiveOffsetMag);
		}

		public void Apply(
			IKSolverFullBodyBiped solver,
			Quaternion rotation,
			float masterWeight,
			float length,
			float timeLeft) {
			additiveOffset = Vector3.Lerp(Vector3.zero, additiveOffset, timeLeft / length);
			lastOffset = rotation * (offset * masterWeight) + rotation * additiveOffset;
			foreach (var effectorLink in effectorLinks)
				solver.GetEffector(effectorLink.effector).positionOffset += lastOffset * effectorLink.weight;
		}

		[Serializable]
		public class EffectorLink {
			[Tooltip("Type of the FBBIK effector to use")]
			public FullBodyBipedEffector effector;

			[Tooltip("Weight of using this effector")]
			public float weight;
		}
	}

	[Serializable]
	public enum Handedness {
		Right,
		Left
	}
}