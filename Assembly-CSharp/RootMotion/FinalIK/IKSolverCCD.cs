using System;
using UnityEngine;

namespace RootMotion.FinalIK;

[Serializable]
public class IKSolverCCD : IKSolverHeuristic {
	public IterationDelegate OnPreIteration;

	public void FadeOutBoneWeights() {
		if (bones.Length < 2)
			return;
		bones[0].weight = 1f;
		var num = 1f / (bones.Length - 1);
		for (var index = 1; index < bones.Length; ++index)
			bones[index].weight = num * (bones.Length - 1 - index);
	}

	protected override void OnInitiate() {
		if (firstInitiation || !Application.isPlaying)
			IKPosition = bones[bones.Length - 1].transform.position;
		InitiateBones();
	}

	protected override void OnUpdate() {
		if (IKPositionWeight <= 0.0)
			return;
		IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0.0f, 1f);
		if (target != null)
			IKPosition = target.position;
		if (XY)
			IKPosition.z = bones[0].transform.position.z;
		var vector3 = maxIterations > 1 ? GetSingularityOffset() : Vector3.zero;
		for (var i = 0;
		     i < maxIterations && (!(vector3 == Vector3.zero) || i < 1 || tolerance <= 0.0 ||
		                           positionOffset >= tolerance * (double)tolerance);
		     ++i) {
			lastLocalDirection = localDirection;
			if (OnPreIteration != null)
				OnPreIteration(i);
			Solve(IKPosition + (i == 0 ? vector3 : Vector3.zero));
		}

		lastLocalDirection = localDirection;
	}

	private void Solve(Vector3 targetPosition) {
		if (XY)
			for (var index = bones.Length - 2; index > -1; --index) {
				var num = bones[index].weight * IKPositionWeight;
				if (num > 0.0) {
					var vector3_1 = bones[bones.Length - 1].transform.position - bones[index].transform.position;
					var vector3_2 = targetPosition - bones[index].transform.position;
					var current = Mathf.Atan2(vector3_1.x, vector3_1.y) * 57.29578f;
					var target = Mathf.Atan2(vector3_2.x, vector3_2.y) * 57.29578f;
					bones[index].transform.rotation =
						Quaternion.AngleAxis(Mathf.DeltaAngle(current, target) * num, Vector3.back) *
						bones[index].transform.rotation;
				}

				if (useRotationLimits && bones[index].rotationLimit != null)
					bones[index].rotationLimit.Apply();
			}
		else
			for (var index = bones.Length - 2; index > -1; --index) {
				var t = bones[index].weight * IKPositionWeight;
				if (t > 0.0) {
					var b = Quaternion.FromToRotation(
						bones[bones.Length - 1].transform.position - bones[index].transform.position,
						targetPosition - bones[index].transform.position) * bones[index].transform.rotation;
					if (t >= 1.0)
						bones[index].transform.rotation = b;
					else
						bones[index].transform.rotation = Quaternion.Lerp(bones[index].transform.rotation, b, t);
				}

				if (useRotationLimits && bones[index].rotationLimit != null)
					bones[index].rotationLimit.Apply();
			}
	}
}