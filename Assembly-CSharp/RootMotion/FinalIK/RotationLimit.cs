using UnityEngine;

namespace RootMotion.FinalIK;

public abstract class RotationLimit : MonoBehaviour {
	public Vector3 axis = Vector3.forward;
	[HideInInspector] public Quaternion defaultLocalRotation;
	private bool initiated;
	private bool applicationQuit;
	private bool defaultLocalRotationSet;

	public void SetDefaultLocalRotation() {
		defaultLocalRotation = transform.localRotation;
		defaultLocalRotationSet = true;
	}

	public Quaternion GetLimitedLocalRotation(Quaternion localRotation, out bool changed) {
		if (!initiated)
			Awake();
		var rotation = Quaternion.Inverse(defaultLocalRotation) * localRotation;
		var quaternion = LimitRotation(rotation);
		changed = quaternion != rotation;
		return !changed ? localRotation : defaultLocalRotation * quaternion;
	}

	public bool Apply() {
		var changed = false;
		transform.localRotation = GetLimitedLocalRotation(transform.localRotation, out changed);
		return changed;
	}

	public void Disable() {
		if (initiated)
			enabled = false;
		else {
			Awake();
			enabled = false;
		}
	}

	public Vector3 secondaryAxis => new(axis.y, axis.z, axis.x);

	public Vector3 crossAxis => Vector3.Cross(axis, secondaryAxis);

	protected abstract Quaternion LimitRotation(Quaternion rotation);

	private void Awake() {
		if (!defaultLocalRotationSet)
			SetDefaultLocalRotation();
		if (axis == Vector3.zero)
			Debug.LogError("Axis is Vector3.zero.");
		initiated = true;
	}

	private void LateUpdate() {
		Apply();
	}

	public void LogWarning(string message) {
		Warning.Log(message, transform);
	}

	protected static Quaternion Limit1DOF(Quaternion rotation, Vector3 axis) {
		return Quaternion.FromToRotation(rotation * axis, axis) * rotation;
	}

	protected static Quaternion LimitTwist(
		Quaternion rotation,
		Vector3 axis,
		Vector3 orthoAxis,
		float twistLimit) {
		twistLimit = Mathf.Clamp(twistLimit, 0.0f, 180f);
		if (twistLimit >= 180.0)
			return rotation;
		var normal = rotation * axis;
		var tangent1 = orthoAxis;
		Vector3.OrthoNormalize(ref normal, ref tangent1);
		var tangent2 = rotation * orthoAxis;
		Vector3.OrthoNormalize(ref normal, ref tangent2);
		var from = Quaternion.FromToRotation(tangent2, tangent1) * rotation;
		return twistLimit <= 0.0 ? from : Quaternion.RotateTowards(from, rotation, twistLimit);
	}

	protected static float GetOrthogonalAngle(Vector3 v1, Vector3 v2, Vector3 normal) {
		Vector3.OrthoNormalize(ref normal, ref v1);
		Vector3.OrthoNormalize(ref normal, ref v2);
		return Vector3.Angle(v1, v2);
	}
}