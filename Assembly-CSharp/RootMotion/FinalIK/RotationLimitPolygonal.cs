using System;
using UnityEngine;

namespace RootMotion.FinalIK;

[HelpURL("http://www.root-motion.com/finalikdox/html/page12.html")]
[AddComponentMenu("Scripts/RootMotion.FinalIK/Rotation Limits/Rotation Limit Polygonal")]
public class RotationLimitPolygonal : RotationLimit {
	[Range(0.0f, 180f)] public float twistLimit = 180f;
	[Range(0.0f, 3f)] public int smoothIterations;
	[SerializeField] [HideInInspector] public LimitPoint[] points;
	[SerializeField] [HideInInspector] public Vector3[] P;
	[SerializeField] [HideInInspector] public ReachCone[] reachCones = new ReachCone[0];

	[ContextMenu("User Manual")]
	private void OpenUserManual() {
		Application.OpenURL("http://www.root-motion.com/finalikdox/html/page12.html");
	}

	[ContextMenu("Scrpt Reference")]
	private void OpenScriptReference() {
		Application.OpenURL(
			"http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_rotation_limit_polygonal.html");
	}

	[ContextMenu("Support Group")]
	private void SupportGroup() {
		Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
	}

	[ContextMenu("Asset Store Thread")]
	private void ASThread() {
		Application.OpenURL(
			"http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
	}

	public void SetLimitPoints(LimitPoint[] points) {
		if (points.Length < 3)
			LogWarning("The polygon must have at least 3 Limit Points.");
		else {
			this.points = points;
			BuildReachCones();
		}
	}

	protected override Quaternion LimitRotation(Quaternion rotation) {
		if (reachCones.Length == 0)
			Start();
		return LimitTwist(LimitSwing(rotation), axis, secondaryAxis, twistLimit);
	}

	private void Start() {
		if (points.Length < 3)
			ResetToDefault();
		for (var index = 0; index < reachCones.Length; ++index)
			if (!reachCones[index].isValid) {
				if (smoothIterations <= 0) {
					var num = index >= reachCones.Length - 1 ? 0 : index + 1;
					LogWarning("Reach Cone {point " + index + ", point " + num +
					           ", Origin} has negative volume. Make sure Axis vector is in the reachable area and the polygon is convex.");
				} else
					LogWarning(
						"One of the Reach Cones in the polygon has negative volume. Make sure Axis vector is in the reachable area and the polygon is convex.");
			}

		axis = axis.normalized;
	}

	public void ResetToDefault() {
		points = new LimitPoint[4];
		for (var index = 0; index < points.Length; ++index)
			points[index] = new LimitPoint();
		var rotation1 = Quaternion.AngleAxis(45f, Vector3.right);
		var rotation2 = Quaternion.AngleAxis(45f, Vector3.up);
		points[0].point = rotation1 * rotation2 * axis;
		points[1].point = Quaternion.Inverse(rotation1) * rotation2 * axis;
		points[2].point = Quaternion.Inverse(rotation1) * Quaternion.Inverse(rotation2) * axis;
		points[3].point = rotation1 * Quaternion.Inverse(rotation2) * axis;
		BuildReachCones();
	}

	public void BuildReachCones() {
		smoothIterations = Mathf.Clamp(smoothIterations, 0, 3);
		P = new Vector3[points.Length];
		for (var index = 0; index < points.Length; ++index)
			P[index] = points[index].point.normalized;
		for (var index = 0; index < smoothIterations; ++index)
			P = SmoothPoints();
		reachCones = new ReachCone[P.Length];
		for (var index = 0; index < reachCones.Length - 1; ++index)
			reachCones[index] = new ReachCone(Vector3.zero, axis.normalized, P[index], P[index + 1]);
		reachCones[P.Length - 1] = new ReachCone(Vector3.zero, axis.normalized, P[P.Length - 1], P[0]);
		for (var index = 0; index < reachCones.Length; ++index)
			reachCones[index].Calculate();
	}

	private Vector3[] SmoothPoints() {
		var vector3Array = new Vector3[P.Length * 2];
		var scalar = GetScalar(P.Length);
		for (var index = 0; index < vector3Array.Length; index += 2)
			vector3Array[index] = PointToTangentPlane(P[index / 2], 1f);
		for (var index = 1; index < vector3Array.Length; index += 2) {
			var zero1 = Vector3.zero;
			var zero2 = Vector3.zero;
			var zero3 = Vector3.zero;
			if (index > 1 && index < vector3Array.Length - 2) {
				zero1 = vector3Array[index - 2];
				zero3 = vector3Array[index + 1];
			} else if (index == 1) {
				zero1 = vector3Array[vector3Array.Length - 2];
				zero3 = vector3Array[index + 1];
			} else if (index == vector3Array.Length - 1) {
				zero1 = vector3Array[index - 2];
				zero3 = vector3Array[0];
			}

			var vector3 = index >= vector3Array.Length - 1 ? vector3Array[0] : vector3Array[index + 1];
			var num = vector3Array.Length / points.Length;
			vector3Array[index] = 0.5f * (vector3Array[index - 1] + vector3) +
			                      scalar * points[index / num].tangentWeight * (vector3 - zero1) + scalar *
			                      points[index / num].tangentWeight * (vector3Array[index - 1] - zero3);
		}

		for (var index = 0; index < vector3Array.Length; ++index)
			vector3Array[index] = TangentPointToSphere(vector3Array[index], 1f);
		return vector3Array;
	}

	private float GetScalar(int k) {
		if (k <= 3)
			return 0.1667f;
		if (k == 4)
			return 0.1036f;
		if (k == 5)
			return 0.085f;
		if (k == 6)
			return 0.0773f;
		return k == 7 ? 0.07f : 1f / 16f;
	}

	private Vector3 PointToTangentPlane(Vector3 p, float r) {
		var num1 = Vector3.Dot(axis, p);
		var num2 = (float)(2.0 * r * r / (r * (double)r + num1));
		return num2 * p + (1f - num2) * -axis;
	}

	private Vector3 TangentPointToSphere(Vector3 q, float r) {
		var num1 = Vector3.Dot(q - axis, q - axis);
		var num2 = (float)(4.0 * r * r / (4.0 * r * r + num1));
		return num2 * q + (1f - num2) * -axis;
	}

	private Quaternion LimitSwing(Quaternion rotation) {
		if (rotation == Quaternion.identity)
			return rotation;
		var vector3 = rotation * axis;
		var reachCone = GetReachCone(vector3);
		if (reachCone == -1) {
			if (!Warning.logged)
				LogWarning("RotationLimitPolygonal reach cones are invalid.");
			return rotation;
		}

		if (Vector3.Dot(reachCones[reachCone].B, vector3) > 0.0)
			return rotation;
		var rhs = Vector3.Cross(axis, vector3);
		var toDirection = Vector3.Cross(-reachCones[reachCone].B, rhs);
		return Quaternion.FromToRotation(rotation * axis, toDirection) * rotation;
	}

	private int GetReachCone(Vector3 L) {
		var num1 = Vector3.Dot(reachCones[0].S, L);
		for (var reachCone = 0; reachCone < reachCones.Length; ++reachCone) {
			var num2 = num1;
			num1 = reachCone >= reachCones.Length - 1
				? Vector3.Dot(reachCones[0].S, L)
				: Vector3.Dot(reachCones[reachCone + 1].S, L);
			if (num2 >= 0.0 && num1 < 0.0)
				return reachCone;
		}

		return -1;
	}

	[Serializable]
	public class ReachCone {
		public Vector3[] tetrahedron;
		public float volume;
		public Vector3 S;
		public Vector3 B;

		public Vector3 o => tetrahedron[0];

		public Vector3 a => tetrahedron[1];

		public Vector3 b => tetrahedron[2];

		public Vector3 c => tetrahedron[3];

		public ReachCone(Vector3 _o, Vector3 _a, Vector3 _b, Vector3 _c) {
			tetrahedron = new Vector3[4];
			tetrahedron[0] = _o;
			tetrahedron[1] = _a;
			tetrahedron[2] = _b;
			tetrahedron[3] = _c;
			volume = 0.0f;
			S = Vector3.zero;
			B = Vector3.zero;
		}

		public bool isValid => volume > 0.0;

		public void Calculate() {
			volume = Vector3.Dot(Vector3.Cross(a, b), c) / 6f;
			S = Vector3.Cross(a, b).normalized;
			B = Vector3.Cross(b, c).normalized;
		}
	}

	[Serializable]
	public class LimitPoint {
		public Vector3 point;
		public float tangentWeight;

		public LimitPoint() {
			point = Vector3.forward;
			tangentWeight = 1f;
		}
	}
}