using System;
using UnityEngine;

namespace RootMotion.FinalIK;

[Serializable]
public class IKSolverHeuristic : IKSolver {
	public Transform target;
	public float tolerance;
	public int maxIterations = 4;
	public bool useRotationLimits = true;
	public bool XY;
	public Bone[] bones = new Bone[0];
	protected Vector3 lastLocalDirection;
	protected float chainLength;

	public bool SetChain(Transform[] hierarchy, Transform root) {
		if (bones == null || bones.Length != hierarchy.Length)
			bones = new Bone[hierarchy.Length];
		for (var index = 0; index < hierarchy.Length; ++index) {
			if (bones[index] == null)
				bones[index] = new Bone();
			bones[index].transform = hierarchy[index];
		}

		Initiate(root);
		return initiated;
	}

	public void AddBone(Transform bone) {
		var hierarchy = new Transform[bones.Length + 1];
		for (var index = 0; index < bones.Length; ++index)
			hierarchy[index] = bones[index].transform;
		hierarchy[hierarchy.Length - 1] = bone;
		SetChain(hierarchy, root);
	}

	public override void StoreDefaultLocalState() {
		for (var index = 0; index < bones.Length; ++index)
			bones[index].StoreDefaultLocalState();
	}

	public override void FixTransforms() {
		if (!initiated || IKPositionWeight <= 0.0)
			return;
		for (var index = 0; index < bones.Length; ++index)
			bones[index].FixTransform();
	}

	public override bool IsValid(ref string message) {
		if (bones.Length == 0) {
			message = "IK chain has no Bones.";
			return false;
		}

		if (bones.Length < minBones) {
			message = "IK chain has less than " + minBones + " Bones.";
			return false;
		}

		foreach (Point bone in bones)
			if (bone.transform == null) {
				message = "One of the Bones is null.";
				return false;
			}

		var transform = ContainsDuplicateBone(bones);
		if (transform != null) {
			message = transform.name + " is represented multiple times in the Bones.";
			return false;
		}

		if (!allowCommonParent && !HierarchyIsValid(bones)) {
			message =
				"Invalid bone hierarchy detected. IK requires for it's bones to be parented to each other in descending order.";
			return false;
		}

		if (!boneLengthCanBeZero)
			for (var index = 0; index < bones.Length - 1; ++index)
				if ((bones[index].transform.position - bones[index + 1].transform.position).magnitude == 0.0) {
					message = "Bone " + index + " length is zero.";
					return false;
				}

		return true;
	}

	public override Point[] GetPoints() {
		return bones;
	}

	public override Point GetPoint(Transform transform) {
		for (var index = 0; index < bones.Length; ++index)
			if (bones[index].transform == transform)
				return bones[index];
		return null;
	}

	protected virtual int minBones => 2;

	protected virtual bool boneLengthCanBeZero => true;

	protected virtual bool allowCommonParent => false;

	protected override void OnInitiate() { }

	protected override void OnUpdate() { }

	protected void InitiateBones() {
		chainLength = 0.0f;
		for (var index = 0; index < bones.Length; ++index)
			if (index < bones.Length - 1) {
				bones[index].length = (bones[index].transform.position - bones[index + 1].transform.position).magnitude;
				chainLength += bones[index].length;
				var position = bones[index + 1].transform.position;
				bones[index].axis = Quaternion.Inverse(bones[index].transform.rotation) *
				                    (position - bones[index].transform.position);
				if (bones[index].rotationLimit != null) {
					if (XY && !(bones[index].rotationLimit is RotationLimitHinge))
						Warning.Log("Only Hinge Rotation Limits should be used on 2D IK solvers.",
							bones[index].transform);
					bones[index].rotationLimit.Disable();
				}
			} else
				bones[index].axis = Quaternion.Inverse(bones[index].transform.rotation) *
				                    (bones[bones.Length - 1].transform.position - bones[0].transform.position);
	}

	protected virtual Vector3 localDirection => bones[0].transform
		.InverseTransformDirection(bones[bones.Length - 1].transform.position - bones[0].transform.position);

	protected float positionOffset => Vector3.SqrMagnitude(localDirection - lastLocalDirection);

	protected Vector3 GetSingularityOffset() {
		if (!SingularityDetected())
			return Vector3.zero;
		var normalized = (IKPosition - bones[0].transform.position).normalized;
		var rhs = new Vector3(normalized.y, normalized.z, normalized.x);
		if (useRotationLimits && bones[bones.Length - 2].rotationLimit != null &&
		    bones[bones.Length - 2].rotationLimit is RotationLimitHinge)
			rhs = bones[bones.Length - 2].transform.rotation * bones[bones.Length - 2].rotationLimit.axis;
		return Vector3.Cross(normalized, rhs) * bones[bones.Length - 2].length * 0.5f;
	}

	private bool SingularityDetected() {
		if (!initiated)
			return false;
		var vector3_1 = bones[bones.Length - 1].transform.position - bones[0].transform.position;
		var vector3_2 = IKPosition - bones[0].transform.position;
		var magnitude1 = vector3_1.magnitude;
		var magnitude2 = vector3_2.magnitude;
		return magnitude1 >= (double)magnitude2 &&
		       magnitude1 >= chainLength - bones[bones.Length - 2].length * 0.10000000149011612 && magnitude1 != 0.0 &&
		       magnitude2 != 0.0 && magnitude2 <= (double)magnitude1 &&
		       Vector3.Dot(vector3_1 / magnitude1, vector3_2 / magnitude2) >= 0.99900001287460327;
	}
}