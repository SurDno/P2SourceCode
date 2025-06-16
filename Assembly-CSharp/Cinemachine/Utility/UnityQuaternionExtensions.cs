using UnityEngine;

namespace Cinemachine.Utility;

public static class UnityQuaternionExtensions {
	public static Quaternion SlerpWithReferenceUp(
		Quaternion qA,
		Quaternion qB,
		float t,
		Vector3 up) {
		var vector3 = (qA * Vector3.forward).ProjectOntoPlane(up);
		var v = (qB * Vector3.forward).ProjectOntoPlane(up);
		if (vector3.AlmostZero() || v.AlmostZero())
			return Quaternion.Slerp(qA, qB, t);
		var rotation = Quaternion.LookRotation(vector3, up);
		var quaternion1 = Quaternion.Inverse(rotation) * qA;
		var quaternion2 = Quaternion.Inverse(rotation) * qB;
		var eulerAngles1 = quaternion1.eulerAngles;
		var eulerAngles2 = quaternion2.eulerAngles;
		return rotation * Quaternion.Euler(Mathf.LerpAngle(eulerAngles1.x, eulerAngles2.x, t),
			Mathf.LerpAngle(eulerAngles1.y, eulerAngles2.y, t), Mathf.LerpAngle(eulerAngles1.z, eulerAngles2.z, t));
	}

	public static Quaternion Normalized(this Quaternion q) {
		var normalized = new Vector4(q.x, q.y, q.z, q.w).normalized;
		return new Quaternion(normalized.x, normalized.y, normalized.z, normalized.w);
	}

	public static Vector2 GetCameraRotationToTarget(
		this Quaternion orient,
		Vector3 lookAtDir,
		Vector3 worldUp) {
		if (lookAtDir.AlmostZero())
			return Vector2.zero;
		var quaternion1 = Quaternion.Inverse(orient);
		var vector3_1 = quaternion1 * worldUp;
		lookAtDir = quaternion1 * lookAtDir;
		var num = 0.0f;
		var vector3_2 = lookAtDir.ProjectOntoPlane(vector3_1);
		if (!vector3_2.AlmostZero()) {
			var vector3_3 = Vector3.forward.ProjectOntoPlane(vector3_1);
			if (vector3_3.AlmostZero())
				vector3_3 = Vector3.Dot(vector3_3, vector3_1) <= 0.0
					? Vector3.up.ProjectOntoPlane(vector3_1)
					: Vector3.down.ProjectOntoPlane(vector3_1);
			num = UnityVectorExtensions.SignedAngle(vector3_3, vector3_2, vector3_1);
		}

		var quaternion2 = Quaternion.AngleAxis(num, vector3_1);
		return new Vector2(
			UnityVectorExtensions.SignedAngle(quaternion2 * Vector3.forward, lookAtDir, quaternion2 * Vector3.right),
			num);
	}

	public static Quaternion ApplyCameraRotation(
		this Quaternion orient,
		Vector2 rot,
		Vector3 worldUp) {
		var quaternion = Quaternion.AngleAxis(rot.x, Vector3.right);
		return Quaternion.AngleAxis(rot.y, worldUp) * orient * quaternion;
	}
}