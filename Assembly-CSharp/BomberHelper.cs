using UnityEngine;

public static class BomberHelper {
	public static bool CalcThrowAngles(
		float v,
		float x,
		float h,
		out float angle1,
		out float angle2) {
		var num = 10f;
		var f = (float)(v * (double)v * v * v - num * (num * (double)x * x - 2.0 * h * v * v));
		if (f < 0.0) {
			angle1 = angle2 = 0.0f;
			return false;
		}

		angle1 = Mathf.Atan((float)((v * (double)v - Mathf.Sqrt(f)) / (num * (double)x)));
		angle2 = Mathf.Atan((float)((v * (double)v + Mathf.Sqrt(f)) / (num * (double)x)));
		return true;
	}

	public static void DrawParabola(
		float angle,
		float v,
		float h,
		Vector3 startPosition,
		Vector3 forward) {
		var num1 = 0.0f;
		var num2 = 10f;
		float num3;
		do {
			var num4 = (float)(v * (double)Mathf.Sin(angle) * num1 - num2 * (double)num1 * num1 / 2.0);
			var num5 = v * Mathf.Cos(angle) * num1;
			num1 += 0.25f;
			num3 = (float)(v * (double)Mathf.Sin(angle) * num1 - num2 * (double)num1 * num1 / 2.0);
			var num6 = v * Mathf.Cos(angle) * num1;
			Gizmos.DrawLine(startPosition + forward * num5 + Vector3.up * num4,
				startPosition + forward * num6 + Vector3.up * num3);
		} while (num3 >= -(double)h);
	}

	public static bool SphereCastParabola(
		float angle,
		float v,
		float h,
		Vector3 startPosition,
		Vector3 forward) {
		var radius = 0.2f;
		var num1 = 0.0f;
		var num2 = 10f;
		Vector3 origin;
		Vector3 vector3;
		float magnitude;
		do {
			var num3 = (float)(v * (double)Mathf.Sin(angle) * num1 - num2 * (double)num1 * num1 / 2.0);
			var num4 = v * Mathf.Cos(angle) * num1;
			num1 += 0.25f;
			var num5 = (float)(v * (double)Mathf.Sin(angle) * num1 - num2 * (double)num1 * num1 / 2.0);
			var num6 = v * Mathf.Cos(angle) * num1;
			origin = startPosition + forward * num4 + Vector3.up * num3;
			vector3 = startPosition + forward * num6 + Vector3.up * num5;
			magnitude = (vector3 - origin).magnitude;
			if (num5 < -(double)h)
				goto label_4;
		} while (!Physics.SphereCast(new Ray(origin, (vector3 - origin).normalized), radius, magnitude,
			         -1 ^ LayerMask.NameToLayer("Enemy")));

		return false;
		label_4:
		return true;
	}
}