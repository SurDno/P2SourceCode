using System;
using UnityEngine;

namespace RootMotion.FinalIK;

[Serializable]
public class IKSolverAim : IKSolverHeuristic {
	public Transform transform;
	public Vector3 axis = Vector3.forward;
	public Vector3 poleAxis = Vector3.up;
	public Vector3 polePosition;
	[Range(0.0f, 1f)] public float poleWeight;
	public Transform poleTarget;
	[Range(0.0f, 1f)] public float clampWeight = 0.1f;
	[Range(0.0f, 2f)] public int clampSmoothing = 2;
	public IterationDelegate OnPreIteration;
	private float step;
	private Vector3 clampedIKPosition;
	private RotationLimit transformLimit;
	private Transform lastTransform;

	public float GetAngle() {
		return Vector3.Angle(transformAxis, IKPosition - transform.position);
	}

	public Vector3 transformAxis => transform.rotation * axis;

	public Vector3 transformPoleAxis => transform.rotation * poleAxis;

	protected override void OnInitiate() {
		if ((firstInitiation || !Application.isPlaying) && transform != null) {
			IKPosition = transform.position + transformAxis * 3f;
			polePosition = transform.position + transformPoleAxis * 3f;
		}

		for (var index = 0; index < bones.Length; ++index)
			if (bones[index].rotationLimit != null)
				bones[index].rotationLimit.Disable();
		step = 1f / bones.Length;
		if (!Application.isPlaying)
			return;
		axis = axis.normalized;
	}

	protected override void OnUpdate() {
		if (axis == Vector3.zero) {
			if (Warning.logged)
				return;
			LogWarning("IKSolverAim axis is Vector3.zero.");
		} else if (poleAxis == Vector3.zero && poleWeight > 0.0) {
			if (Warning.logged)
				return;
			LogWarning("IKSolverAim poleAxis is Vector3.zero.");
		} else {
			if (target != null)
				IKPosition = target.position;
			if (poleTarget != null)
				polePosition = poleTarget.position;
			if (XY)
				IKPosition.z = bones[0].transform.position.z;
			if (IKPositionWeight <= 0.0)
				return;
			IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0.0f, 1f);
			if (transform != lastTransform) {
				transformLimit = transform.GetComponent<RotationLimit>();
				if (transformLimit != null)
					transformLimit.enabled = false;
				lastTransform = transform;
			}

			if (transformLimit != null)
				transformLimit.Apply();
			if (transform == null) {
				if (Warning.logged)
					return;
				LogWarning(
					"Aim Transform unassigned in Aim IK solver. Please Assign a Transform (lineal descendant to the last bone in the spine) that you want to be aimed at IKPosition");
			} else {
				clampWeight = Mathf.Clamp(clampWeight, 0.0f, 1f);
				clampedIKPosition = GetClampedIKPosition();
				var b = clampedIKPosition - transform.position;
				clampedIKPosition =
					transform.position + Vector3.Slerp(transformAxis * b.magnitude, b, IKPositionWeight);
				for (var i = 0;
				     i < maxIterations && (i < 1 || tolerance <= 0.0 || GetAngle() >= (double)tolerance);
				     ++i) {
					lastLocalDirection = localDirection;
					if (OnPreIteration != null)
						OnPreIteration(i);
					Solve();
				}

				lastLocalDirection = localDirection;
			}
		}
	}

	protected override int minBones => 1;

	private void Solve() {
		for (var index = 0; index < bones.Length - 1; ++index)
			RotateToTarget(clampedIKPosition, bones[index],
				step * (index + 1) * IKPositionWeight * bones[index].weight);
		RotateToTarget(clampedIKPosition, bones[bones.Length - 1], IKPositionWeight * bones[bones.Length - 1].weight);
	}

	private Vector3 GetClampedIKPosition() {
		if (clampWeight <= 0.0)
			return IKPosition;
		if (clampWeight >= 1.0)
			return transform.position + transformAxis * (IKPosition - transform.position).magnitude;
		var num1 = (float)(1.0 - Vector3.Angle(transformAxis, IKPosition - transform.position) / 180.0);
		var num2 = clampWeight > 0.0
			? Mathf.Clamp((float)(1.0 - (clampWeight - (double)num1) / (1.0 - num1)), 0.0f, 1f)
			: 1f;
		var num3 = clampWeight > 0.0 ? Mathf.Clamp(num1 / clampWeight, 0.0f, 1f) : 1f;
		for (var index = 0; index < clampSmoothing; ++index)
			num3 = Mathf.Sin((float)(num3 * 3.1415927410125732 * 0.5));
		return transform.position + Vector3.Slerp(transformAxis * 10f, IKPosition - transform.position, num3 * num2);
	}

	private void RotateToTarget(Vector3 targetPosition, Bone bone, float weight) {
		if (XY) {
			if (weight >= 0.0) {
				var transformAxis = this.transformAxis;
				var vector3 = targetPosition - transform.position;
				var current = Mathf.Atan2(transformAxis.x, transformAxis.y) * 57.29578f;
				var target = Mathf.Atan2(vector3.x, vector3.y) * 57.29578f;
				bone.transform.rotation = Quaternion.AngleAxis(Mathf.DeltaAngle(current, target), Vector3.back) *
				                          bone.transform.rotation;
			}
		} else {
			if (weight >= 0.0) {
				var rotation = Quaternion.FromToRotation(transformAxis, targetPosition - transform.position);
				if (weight >= 1.0)
					bone.transform.rotation = rotation * bone.transform.rotation;
				else
					bone.transform.rotation =
						Quaternion.Lerp(Quaternion.identity, rotation, weight) * bone.transform.rotation;
			}

			if (poleWeight > 0.0) {
				var tangent = polePosition - transform.position;
				var transformAxis = this.transformAxis;
				Vector3.OrthoNormalize(ref transformAxis, ref tangent);
				var rotation = Quaternion.FromToRotation(transformPoleAxis, tangent);
				bone.transform.rotation = Quaternion.Lerp(Quaternion.identity, rotation, weight * poleWeight) *
				                          bone.transform.rotation;
			}
		}

		if (!useRotationLimits || !(bone.rotationLimit != null))
			return;
		bone.rotationLimit.Apply();
	}

	protected override Vector3 localDirection =>
		bones[0].transform.InverseTransformDirection(bones[bones.Length - 1].transform.forward);
}