using UnityEngine;

namespace RootMotion;

public static class QuaTools {
	public static Quaternion Lerp(Quaternion fromRotation, Quaternion toRotation, float weight) {
		if (weight <= 0.0)
			return fromRotation;
		return weight >= 1.0 ? toRotation : Quaternion.Lerp(fromRotation, toRotation, weight);
	}

	public static Quaternion Slerp(Quaternion fromRotation, Quaternion toRotation, float weight) {
		if (weight <= 0.0)
			return fromRotation;
		return weight >= 1.0 ? toRotation : Quaternion.Slerp(fromRotation, toRotation, weight);
	}

	public static Quaternion LinearBlend(Quaternion q, float weight) {
		if (weight <= 0.0)
			return Quaternion.identity;
		return weight >= 1.0 ? q : Quaternion.Lerp(Quaternion.identity, q, weight);
	}

	public static Quaternion SphericalBlend(Quaternion q, float weight) {
		if (weight <= 0.0)
			return Quaternion.identity;
		return weight >= 1.0 ? q : Quaternion.Slerp(Quaternion.identity, q, weight);
	}

	public static Quaternion FromToAroundAxis(
		Vector3 fromDirection,
		Vector3 toDirection,
		Vector3 axis) {
		var rotation = Quaternion.FromToRotation(fromDirection, toDirection);
		var angle = 0.0f;
		var axis1 = Vector3.zero;
		rotation.ToAngleAxis(out angle, out axis1);
		if (Vector3.Dot(axis1, axis) < 0.0)
			angle = -angle;
		return Quaternion.AngleAxis(angle, axis);
	}

	public static Quaternion RotationToLocalSpace(Quaternion space, Quaternion rotation) {
		return Quaternion.Inverse(Quaternion.Inverse(space) * rotation);
	}

	public static Quaternion FromToRotation(Quaternion from, Quaternion to) {
		return to == from ? Quaternion.identity : to * Quaternion.Inverse(from);
	}

	public static Vector3 GetAxis(Vector3 v) {
		var axis = Vector3.right;
		var flag = false;
		var f1 = Vector3.Dot(v, Vector3.right);
		var num1 = Mathf.Abs(f1);
		if (f1 < 0.0)
			flag = true;
		var f2 = Vector3.Dot(v, Vector3.up);
		var num2 = Mathf.Abs(f2);
		if (num2 > (double)num1) {
			num1 = num2;
			axis = Vector3.up;
			flag = f2 < 0.0;
		}

		var f3 = Vector3.Dot(v, Vector3.forward);
		if (Mathf.Abs(f3) > (double)num1) {
			axis = Vector3.forward;
			flag = f3 < 0.0;
		}

		if (flag)
			axis = -axis;
		return axis;
	}

	public static Quaternion ClampRotation(
		Quaternion rotation,
		float clampWeight,
		int clampSmoothing) {
		if (clampWeight >= 1.0)
			return Quaternion.identity;
		if (clampWeight <= 0.0)
			return rotation;
		var num1 = (float)(1.0 - Quaternion.Angle(Quaternion.identity, rotation) / 180.0);
		var num2 = Mathf.Clamp((float)(1.0 - (clampWeight - (double)num1) / (1.0 - num1)), 0.0f, 1f);
		var num3 = Mathf.Clamp(num1 / clampWeight, 0.0f, 1f);
		for (var index = 0; index < clampSmoothing; ++index)
			num3 = Mathf.Sin((float)(num3 * 3.1415927410125732 * 0.5));
		return Quaternion.Slerp(Quaternion.identity, rotation, num3 * num2);
	}

	public static float ClampAngle(float angle, float clampWeight, int clampSmoothing) {
		if (clampWeight >= 1.0)
			return 0.0f;
		if (clampWeight <= 0.0)
			return angle;
		var num1 = (float)(1.0 - Mathf.Abs(angle) / 180.0);
		var num2 = Mathf.Clamp((float)(1.0 - (clampWeight - (double)num1) / (1.0 - num1)), 0.0f, 1f);
		var num3 = Mathf.Clamp(num1 / clampWeight, 0.0f, 1f);
		for (var index = 0; index < clampSmoothing; ++index)
			num3 = Mathf.Sin((float)(num3 * 3.1415927410125732 * 0.5));
		return Mathf.Lerp(0.0f, angle, num3 * num2);
	}

	public static Quaternion MatchRotation(
		Quaternion targetRotation,
		Vector3 targetforwardAxis,
		Vector3 targetUpAxis,
		Vector3 forwardAxis,
		Vector3 upAxis) {
		var rotation = Quaternion.LookRotation(forwardAxis, upAxis);
		var quaternion = Quaternion.LookRotation(targetforwardAxis, targetUpAxis);
		return targetRotation * quaternion * Quaternion.Inverse(rotation);
	}
}