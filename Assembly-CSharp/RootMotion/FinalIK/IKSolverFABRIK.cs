using System;
using UnityEngine;

namespace RootMotion.FinalIK;

[Serializable]
public class IKSolverFABRIK : IKSolverHeuristic {
	public IterationDelegate OnPreIteration;
	private bool[] limitedBones = new bool[0];
	private Vector3[] solverLocalPositions = new Vector3[0];

	public void SolveForward(Vector3 position) {
		if (!initiated) {
			if (Warning.logged)
				return;
			LogWarning("Trying to solve uninitiated FABRIK chain.");
		} else {
			OnPreSolve();
			ForwardReach(position);
		}
	}

	public void SolveBackward(Vector3 position) {
		if (!initiated) {
			if (Warning.logged)
				return;
			LogWarning("Trying to solve uninitiated FABRIK chain.");
		} else {
			BackwardReach(position);
			OnPostSolve();
		}
	}

	public override Vector3 GetIKPosition() {
		return target != null ? target.position : IKPosition;
	}

	protected override void OnInitiate() {
		if (firstInitiation || !Application.isPlaying)
			IKPosition = bones[bones.Length - 1].transform.position;
		for (var index = 0; index < bones.Length; ++index) {
			bones[index].solverPosition = bones[index].transform.position;
			bones[index].solverRotation = bones[index].transform.rotation;
		}

		limitedBones = new bool[bones.Length];
		solverLocalPositions = new Vector3[bones.Length];
		InitiateBones();
		for (var index = 0; index < bones.Length; ++index)
			solverLocalPositions[index] = Quaternion.Inverse(GetParentSolverRotation(index)) *
			                              (bones[index].transform.position - GetParentSolverPosition(index));
	}

	protected override void OnUpdate() {
		if (IKPositionWeight <= 0.0)
			return;
		IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0.0f, 1f);
		OnPreSolve();
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

		OnPostSolve();
	}

	protected override bool boneLengthCanBeZero => false;

	private Vector3 SolveJoint(Vector3 pos1, Vector3 pos2, float length) {
		if (XY)
			pos1.z = pos2.z;
		return pos2 + (pos1 - pos2).normalized * length;
	}

	private void OnPreSolve() {
		chainLength = 0.0f;
		for (var index = 0; index < bones.Length; ++index) {
			bones[index].solverPosition = bones[index].transform.position;
			bones[index].solverRotation = bones[index].transform.rotation;
			if (index < bones.Length - 1) {
				bones[index].length = (bones[index].transform.position - bones[index + 1].transform.position).magnitude;
				bones[index].axis = Quaternion.Inverse(bones[index].transform.rotation) *
				                    (bones[index + 1].transform.position - bones[index].transform.position);
				chainLength += bones[index].length;
			}

			if (useRotationLimits)
				solverLocalPositions[index] = Quaternion.Inverse(GetParentSolverRotation(index)) *
				                              (bones[index].transform.position - GetParentSolverPosition(index));
		}
	}

	private void OnPostSolve() {
		if (!useRotationLimits)
			MapToSolverPositions();
		else
			MapToSolverPositionsLimited();
		lastLocalDirection = localDirection;
	}

	private void Solve(Vector3 targetPosition) {
		ForwardReach(targetPosition);
		BackwardReach(bones[0].transform.position);
	}

	private void ForwardReach(Vector3 position) {
		bones[bones.Length - 1].solverPosition =
			Vector3.Lerp(bones[bones.Length - 1].solverPosition, position, IKPositionWeight);
		for (var index = 0; index < limitedBones.Length; ++index)
			limitedBones[index] = false;
		for (var rotateBone = bones.Length - 2; rotateBone > -1; --rotateBone) {
			bones[rotateBone].solverPosition = SolveJoint(bones[rotateBone].solverPosition,
				bones[rotateBone + 1].solverPosition, bones[rotateBone].length);
			LimitForward(rotateBone, rotateBone + 1);
		}

		LimitForward(0, 0);
	}

	private void SolverMove(int index, Vector3 offset) {
		for (var index1 = index; index1 < bones.Length; ++index1) {
			var bone = bones[index1];
			bone.solverPosition = bone.solverPosition + offset;
		}
	}

	private void SolverRotate(int index, Quaternion rotation, bool recursive) {
		for (var index1 = index; index1 < bones.Length; ++index1) {
			bones[index1].solverRotation = rotation * bones[index1].solverRotation;
			if (!recursive)
				break;
		}
	}

	private void SolverRotateChildren(int index, Quaternion rotation) {
		for (var index1 = index + 1; index1 < bones.Length; ++index1)
			bones[index1].solverRotation = rotation * bones[index1].solverRotation;
	}

	private void SolverMoveChildrenAroundPoint(int index, Quaternion rotation) {
		for (var index1 = index + 1; index1 < bones.Length; ++index1) {
			var vector3 = bones[index1].solverPosition - bones[index].solverPosition;
			bones[index1].solverPosition = bones[index].solverPosition + rotation * vector3;
		}
	}

	private Quaternion GetParentSolverRotation(int index) {
		if (index > 0)
			return bones[index - 1].solverRotation;
		return bones[0].transform.parent == null ? Quaternion.identity : bones[0].transform.parent.rotation;
	}

	private Vector3 GetParentSolverPosition(int index) {
		if (index > 0)
			return bones[index - 1].solverPosition;
		return bones[0].transform.parent == null ? Vector3.zero : bones[0].transform.parent.position;
	}

	private Quaternion GetLimitedRotation(int index, Quaternion q, out bool changed) {
		changed = false;
		var parentSolverRotation = GetParentSolverRotation(index);
		var localRotation = Quaternion.Inverse(parentSolverRotation) * q;
		var limitedLocalRotation = bones[index].rotationLimit.GetLimitedLocalRotation(localRotation, out changed);
		return !changed ? q : parentSolverRotation * limitedLocalRotation;
	}

	private void LimitForward(int rotateBone, int limitBone) {
		if (!useRotationLimits || bones[limitBone].rotationLimit == null)
			return;
		var solverPosition = bones[bones.Length - 1].solverPosition;
		for (var index = rotateBone; index < bones.Length - 1 && !limitedBones[index]; ++index) {
			var rotation = Quaternion.FromToRotation(bones[index].solverRotation * bones[index].axis,
				bones[index + 1].solverPosition - bones[index].solverPosition);
			SolverRotate(index, rotation, false);
		}

		var changed = false;
		var limitedRotation = GetLimitedRotation(limitBone, bones[limitBone].solverRotation, out changed);
		if (changed) {
			if (limitBone < bones.Length - 1) {
				var rotation1 = QuaTools.FromToRotation(bones[limitBone].solverRotation, limitedRotation);
				bones[limitBone].solverRotation = limitedRotation;
				SolverRotateChildren(limitBone, rotation1);
				SolverMoveChildrenAroundPoint(limitBone, rotation1);
				var rotation2 = Quaternion.FromToRotation(
					bones[bones.Length - 1].solverPosition - bones[rotateBone].solverPosition,
					solverPosition - bones[rotateBone].solverPosition);
				SolverRotate(rotateBone, rotation2, true);
				SolverMoveChildrenAroundPoint(rotateBone, rotation2);
				SolverMove(rotateBone, solverPosition - bones[bones.Length - 1].solverPosition);
			} else
				bones[limitBone].solverRotation = limitedRotation;
		}

		limitedBones[limitBone] = true;
	}

	private void BackwardReach(Vector3 position) {
		if (useRotationLimits)
			BackwardReachLimited(position);
		else
			BackwardReachUnlimited(position);
	}

	private void BackwardReachUnlimited(Vector3 position) {
		bones[0].solverPosition = position;
		for (var index = 1; index < bones.Length; ++index)
			bones[index].solverPosition = SolveJoint(bones[index].solverPosition, bones[index - 1].solverPosition,
				bones[index - 1].length);
	}

	private void BackwardReachLimited(Vector3 position) {
		bones[0].solverPosition = position;
		for (var index = 0; index < bones.Length - 1; ++index) {
			var vector3 = SolveJoint(bones[index + 1].solverPosition, bones[index].solverPosition, bones[index].length);
			var quaternion =
				Quaternion.FromToRotation(bones[index].solverRotation * bones[index].axis,
					vector3 - bones[index].solverPosition) * bones[index].solverRotation;
			if (bones[index].rotationLimit != null) {
				var changed = false;
				quaternion = GetLimitedRotation(index, quaternion, out changed);
			}

			var rotation = QuaTools.FromToRotation(bones[index].solverRotation, quaternion);
			bones[index].solverRotation = quaternion;
			SolverRotateChildren(index, rotation);
			bones[index + 1].solverPosition = bones[index].solverPosition +
			                                  bones[index].solverRotation * solverLocalPositions[index + 1];
		}

		for (var index = 0; index < bones.Length; ++index)
			bones[index].solverRotation = Quaternion.LookRotation(bones[index].solverRotation * Vector3.forward,
				bones[index].solverRotation * Vector3.up);
	}

	private void MapToSolverPositions() {
		bones[0].transform.position = bones[0].solverPosition;
		for (var index = 0; index < bones.Length - 1; ++index)
			if (XY)
				bones[index].Swing2D(bones[index + 1].solverPosition);
			else
				bones[index].Swing(bones[index + 1].solverPosition);
	}

	private void MapToSolverPositionsLimited() {
		bones[0].transform.position = bones[0].solverPosition;
		for (var index = 0; index < bones.Length; ++index)
			if (index < bones.Length - 1)
				bones[index].transform.rotation = bones[index].solverRotation;
	}
}