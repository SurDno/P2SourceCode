using UnityEngine;

namespace RootMotion;

public static class V3Tools {
	public static Vector3 Lerp(Vector3 fromVector, Vector3 toVector, float weight) {
		if (weight <= 0.0)
			return fromVector;
		return weight >= 1.0 ? toVector : Vector3.Lerp(fromVector, toVector, weight);
	}

	public static Vector3 Slerp(Vector3 fromVector, Vector3 toVector, float weight) {
		if (weight <= 0.0)
			return fromVector;
		return weight >= 1.0 ? toVector : Vector3.Slerp(fromVector, toVector, weight);
	}

	public static Vector3 ExtractVertical(Vector3 v, Vector3 verticalAxis, float weight) {
		return weight == 0.0 ? Vector3.zero : Vector3.Project(v, verticalAxis) * weight;
	}

	public static Vector3 ExtractHorizontal(Vector3 v, Vector3 normal, float weight) {
		if (weight == 0.0)
			return Vector3.zero;
		var tangent = v;
		Vector3.OrthoNormalize(ref normal, ref tangent);
		return Vector3.Project(v, tangent) * weight;
	}

	public static Vector3 ClampDirection(
		Vector3 direction,
		Vector3 normalDirection,
		float clampWeight,
		int clampSmoothing,
		out bool changed) {
		changed = false;
		if (clampWeight <= 0.0)
			return direction;
		if (clampWeight >= 1.0) {
			changed = true;
			return normalDirection;
		}

		var num1 = (float)(1.0 - Vector3.Angle(normalDirection, direction) / 180.0);
		if (num1 > (double)clampWeight)
			return direction;
		changed = true;
		var num2 = clampWeight > 0.0
			? Mathf.Clamp((float)(1.0 - (clampWeight - (double)num1) / (1.0 - num1)), 0.0f, 1f)
			: 1f;
		var num3 = clampWeight > 0.0 ? Mathf.Clamp(num1 / clampWeight, 0.0f, 1f) : 1f;
		for (var index = 0; index < clampSmoothing; ++index)
			num3 = Mathf.Sin((float)(num3 * 3.1415927410125732 * 0.5));
		return Vector3.Slerp(normalDirection, direction, num3 * num2);
	}

	public static Vector3 ClampDirection(
		Vector3 direction,
		Vector3 normalDirection,
		float clampWeight,
		int clampSmoothing,
		out float clampValue) {
		clampValue = 1f;
		if (clampWeight <= 0.0)
			return direction;
		if (clampWeight >= 1.0)
			return normalDirection;
		var num1 = (float)(1.0 - Vector3.Angle(normalDirection, direction) / 180.0);
		if (num1 > (double)clampWeight) {
			clampValue = 0.0f;
			return direction;
		}

		var num2 = clampWeight > 0.0
			? Mathf.Clamp((float)(1.0 - (clampWeight - (double)num1) / (1.0 - num1)), 0.0f, 1f)
			: 1f;
		var num3 = clampWeight > 0.0 ? Mathf.Clamp(num1 / clampWeight, 0.0f, 1f) : 1f;
		for (var index = 0; index < clampSmoothing; ++index)
			num3 = Mathf.Sin((float)(num3 * 3.1415927410125732 * 0.5));
		var t = num3 * num2;
		clampValue = 1f - t;
		return Vector3.Slerp(normalDirection, direction, t);
	}

	public static Vector3 LineToPlane(
		Vector3 origin,
		Vector3 direction,
		Vector3 planeNormal,
		Vector3 planePoint) {
		var num1 = Vector3.Dot(planePoint - origin, planeNormal);
		var num2 = Vector3.Dot(direction, planeNormal);
		if (num2 == 0.0)
			return Vector3.zero;
		var num3 = num1 / num2;
		return origin + direction.normalized * num3;
	}

	public static Vector3 PointToPlane(Vector3 point, Vector3 planePosition, Vector3 planeNormal) {
		if (planeNormal == Vector3.up)
			return new Vector3(point.x, planePosition.y, point.z);
		var tangent = point - planePosition;
		var normal = planeNormal;
		Vector3.OrthoNormalize(ref normal, ref tangent);
		return planePosition + Vector3.Project(point - planePosition, tangent);
	}
}