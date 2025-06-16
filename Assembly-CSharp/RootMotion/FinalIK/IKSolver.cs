using System;
using UnityEngine;

namespace RootMotion.FinalIK;

[Serializable]
public abstract class IKSolver {
	[HideInInspector] public Vector3 IKPosition;

	[Tooltip("The positional or the master weight of the solver.")] [Range(0.0f, 1f)]
	public float IKPositionWeight = 1f;

	public UpdateDelegate OnPreInitiate;
	public UpdateDelegate OnPostInitiate;
	public UpdateDelegate OnPreUpdate;
	public UpdateDelegate OnPostUpdate;
	protected bool firstInitiation = true;
	[SerializeField] [HideInInspector] protected Transform root;

	public bool IsValid() {
		var empty = string.Empty;
		return IsValid(ref empty);
	}

	public abstract bool IsValid(ref string message);

	public void Initiate(Transform root) {
		if (OnPreInitiate != null)
			OnPreInitiate();
		if (root == null)
			Debug.LogError("Initiating IKSolver with null root Transform.");
		this.root = root;
		initiated = false;
		var empty = string.Empty;
		if (!IsValid(ref empty))
			Warning.Log(empty, root);
		else {
			OnInitiate();
			StoreDefaultLocalState();
			initiated = true;
			firstInitiation = false;
			if (OnPostInitiate == null)
				return;
			OnPostInitiate();
		}
	}

	public void Update() {
		if (OnPreUpdate != null)
			OnPreUpdate();
		if (firstInitiation)
			Initiate(root);
		if (!initiated)
			return;
		OnUpdate();
		if (OnPostUpdate == null)
			return;
		OnPostUpdate();
	}

	public virtual Vector3 GetIKPosition() {
		return IKPosition;
	}

	public void SetIKPosition(Vector3 position) {
		IKPosition = position;
	}

	public float GetIKPositionWeight() {
		return IKPositionWeight;
	}

	public void SetIKPositionWeight(float weight) {
		IKPositionWeight = Mathf.Clamp(weight, 0.0f, 1f);
	}

	public Transform GetRoot() {
		return root;
	}

	public bool initiated { get; private set; }

	public abstract Point[] GetPoints();

	public abstract Point GetPoint(Transform transform);

	public abstract void FixTransforms();

	public abstract void StoreDefaultLocalState();

	protected abstract void OnInitiate();

	protected abstract void OnUpdate();

	protected void LogWarning(string message) {
		Warning.Log(message, root, true);
	}

	public static Transform ContainsDuplicateBone(Bone[] bones) {
		for (var index1 = 0; index1 < bones.Length; ++index1) {
			for (var index2 = 0; index2 < bones.Length; ++index2)
				if (index1 != index2 && bones[index1].transform == bones[index2].transform)
					return bones[index1].transform;
		}

		return null;
	}

	public static bool HierarchyIsValid(Bone[] bones) {
		for (var index = 1; index < bones.Length; ++index)
			if (!Hierarchy.IsAncestor(bones[index].transform, bones[index - 1].transform))
				return false;
		return true;
	}

	protected static float PreSolveBones(ref Bone[] bones) {
		var num = 0.0f;
		for (var index = 0; index < bones.Length; ++index) {
			bones[index].solverPosition = bones[index].transform.position;
			bones[index].solverRotation = bones[index].transform.rotation;
		}

		for (var index = 0; index < bones.Length; ++index)
			if (index < bones.Length - 1) {
				bones[index].sqrMag = (bones[index + 1].solverPosition - bones[index].solverPosition).sqrMagnitude;
				bones[index].length = Mathf.Sqrt(bones[index].sqrMag);
				num += bones[index].length;
				bones[index].axis = Quaternion.Inverse(bones[index].solverRotation) *
				                    (bones[index + 1].solverPosition - bones[index].solverPosition);
			} else {
				bones[index].sqrMag = 0.0f;
				bones[index].length = 0.0f;
			}

		return num;
	}

	[Serializable]
	public class Point {
		public Transform transform;
		[Range(0.0f, 1f)] public float weight = 1f;
		public Vector3 solverPosition;
		public Quaternion solverRotation = Quaternion.identity;
		public Vector3 defaultLocalPosition;
		public Quaternion defaultLocalRotation;

		public void StoreDefaultLocalState() {
			defaultLocalPosition = transform.localPosition;
			defaultLocalRotation = transform.localRotation;
		}

		public void FixTransform() {
			if (transform.localPosition != defaultLocalPosition)
				transform.localPosition = defaultLocalPosition;
			if (!(transform.localRotation != defaultLocalRotation))
				return;
			transform.localRotation = defaultLocalRotation;
		}

		public void UpdateSolverPosition() {
			solverPosition = transform.position;
		}

		public void UpdateSolverLocalPosition() {
			solverPosition = transform.localPosition;
		}

		public void UpdateSolverState() {
			solverPosition = transform.position;
			solverRotation = transform.rotation;
		}

		public void UpdateSolverLocalState() {
			solverPosition = transform.localPosition;
			solverRotation = transform.localRotation;
		}
	}

	[Serializable]
	public class Bone : Point {
		public float length;
		public float sqrMag;
		public Vector3 axis = -Vector3.right;
		private RotationLimit _rotationLimit;
		private bool isLimited = true;

		public RotationLimit rotationLimit {
			get {
				if (!isLimited)
					return null;
				if (_rotationLimit == null)
					_rotationLimit = transform.GetComponent<RotationLimit>();
				isLimited = _rotationLimit != null;
				return _rotationLimit;
			}
			set {
				_rotationLimit = value;
				isLimited = value != null;
			}
		}

		public void Swing(Vector3 swingTarget, float weight = 1f) {
			if (weight <= 0.0)
				return;
			var rotation = Quaternion.FromToRotation(transform.rotation * axis, swingTarget - transform.position);
			if (weight >= 1.0)
				transform.rotation = rotation * transform.rotation;
			else
				transform.rotation = Quaternion.Lerp(Quaternion.identity, rotation, weight) * transform.rotation;
		}

		public static void SolverSwing(
			Bone[] bones,
			int index,
			Vector3 swingTarget,
			float weight = 1f) {
			if (weight <= 0.0)
				return;
			var rotation = Quaternion.FromToRotation(bones[index].solverRotation * bones[index].axis,
				swingTarget - bones[index].solverPosition);
			if (weight >= 1.0)
				for (var index1 = index; index1 < bones.Length; ++index1)
					bones[index1].solverRotation = rotation * bones[index1].solverRotation;
			else
				for (var index2 = index; index2 < bones.Length; ++index2)
					bones[index2].solverRotation = Quaternion.Lerp(Quaternion.identity, rotation, weight) *
					                               bones[index2].solverRotation;
		}

		public void Swing2D(Vector3 swingTarget, float weight = 1f) {
			if (weight <= 0.0)
				return;
			var vector3_1 = transform.rotation * axis;
			var vector3_2 = swingTarget - transform.position;
			transform.rotation =
				Quaternion.AngleAxis(
					Mathf.DeltaAngle(Mathf.Atan2(vector3_1.x, vector3_1.y) * 57.29578f,
						Mathf.Atan2(vector3_2.x, vector3_2.y) * 57.29578f) * weight, Vector3.back) * transform.rotation;
		}

		public void SetToSolverPosition() {
			transform.position = solverPosition;
		}

		public Bone() { }

		public Bone(Transform transform) {
			this.transform = transform;
		}

		public Bone(Transform transform, float weight) {
			this.transform = transform;
			this.weight = weight;
		}
	}

	[Serializable]
	public class Node : Point {
		public float length;
		public float effectorPositionWeight;
		public float effectorRotationWeight;
		public Vector3 offset;

		public Node() { }

		public Node(Transform transform) {
			this.transform = transform;
		}

		public Node(Transform transform, float weight) {
			this.transform = transform;
			this.weight = weight;
		}
	}

	public delegate void UpdateDelegate();

	public delegate void IterationDelegate(int i);
}