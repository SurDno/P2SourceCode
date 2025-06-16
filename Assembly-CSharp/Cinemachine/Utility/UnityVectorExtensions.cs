using UnityEngine;

namespace Cinemachine.Utility;

public static class UnityVectorExtensions {
	public const float Epsilon = 0.0001f;

	public static float ClosestPointOnSegment(this Vector3 p, Vector3 s0, Vector3 s1) {
		var vector3 = s1 - s0;
		var num = Vector3.SqrMagnitude(vector3);
		return num < 9.9999997473787516E-05 ? 0.0f : Mathf.Clamp01(Vector3.Dot(p - s0, vector3) / num);
	}

	public static float ClosestPointOnSegment(this Vector2 p, Vector2 s0, Vector2 s1) {
		var vector2 = s1 - s0;
		var num = Vector2.SqrMagnitude(vector2);
		return num < 9.9999997473787516E-05 ? 0.0f : Mathf.Clamp01(Vector2.Dot(p - s0, vector2) / num);
	}

	public static Vector3 ProjectOntoPlane(this Vector3 vector, Vector3 planeNormal) {
		return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
	}

	public static bool AlmostZero(this Vector3 v) {
		return v.sqrMagnitude < 9.99999905104687E-09;
	}

	public static float SignedAngle(Vector3 from, Vector3 to, Vector3 refNormal) {
		from.Normalize();
		to.Normalize();
		var f = Vector3.Dot(Vector3.Cross(from, to), refNormal);
		if (Mathf.Abs(f) < -9.9999997473787516E-05)
			return Vector3.Dot(from, to) < 0.0 ? 180f : 0.0f;
		var num = Vector3.Angle(from, to);
		return f < 0.0 ? -num : num;
	}

	public static Vector3 SlerpWithReferenceUp(Vector3 vA, Vector3 vB, float t, Vector3 up) {
		var magnitude1 = vA.magnitude;
		var magnitude2 = vB.magnitude;
		if (magnitude1 < 9.9999997473787516E-05 || magnitude2 < 9.9999997473787516E-05)
			return Vector3.Lerp(vA, vB, t);
		var forward1 = vA / magnitude1;
		var forward2 = vB / magnitude2;
		return UnityQuaternionExtensions.SlerpWithReferenceUp(Quaternion.LookRotation(forward1, up),
			Quaternion.LookRotation(forward2, up), t, up) * Vector3.forward * Mathf.Lerp(magnitude1, magnitude2, t);
	}
}