using System;
using Cinemachine.Utility;
using UnityEngine;

namespace Cinemachine;

[DocumentationSorting(18f, DocumentationSortingAttribute.Level.UserRef)]
[AddComponentMenu("Cinemachine/CinemachinePath")]
[SaveDuringPlay]
public class CinemachinePath : CinemachinePathBase {
	[Tooltip("If checked, then the path ends are joined to form a continuous loop.")]
	public bool m_Looped;

	[Tooltip("The waypoints that define the path.  They will be interpolated using a bezier curve.")]
	public Waypoint[] m_Waypoints = new Waypoint[0];

	public override float MinPos => 0.0f;

	public override float MaxPos {
		get {
			var num = m_Waypoints.Length - 1;
			return num < 1 ? 0.0f : m_Looped ? num + 1 : (float)num;
		}
	}

	public override bool Looped => m_Looped;

	public override int DistanceCacheSampleStepsPerSegment => m_Resolution;

	private float GetBoundingIndices(float pos, out int indexA, out int indexB) {
		pos = NormalizePos(pos);
		var num = Mathf.RoundToInt(pos);
		if (Mathf.Abs(pos - num) < 9.9999997473787516E-05)
			indexA = indexB = num == m_Waypoints.Length ? 0 : num;
		else {
			indexA = Mathf.FloorToInt(pos);
			if (indexA >= m_Waypoints.Length) {
				pos -= MaxPos;
				indexA = 0;
			}

			indexB = Mathf.CeilToInt(pos);
			if (indexB >= m_Waypoints.Length)
				indexB = 0;
		}

		return pos;
	}

	public override Vector3 EvaluatePosition(float pos) {
		var vector3 = new Vector3();
		Vector3 position;
		if (m_Waypoints.Length == 0)
			position = transform.position;
		else {
			int indexA;
			int indexB;
			pos = GetBoundingIndices(pos, out indexA, out indexB);
			if (indexA == indexB)
				position = m_Waypoints[indexA].position;
			else {
				var waypoint1 = m_Waypoints[indexA];
				var waypoint2 = m_Waypoints[indexB];
				position = SplineHelpers.Bezier3(pos - indexA, m_Waypoints[indexA].position,
					waypoint1.position + waypoint1.tangent, waypoint2.position - waypoint2.tangent, waypoint2.position);
			}
		}

		return transform.TransformPoint(position);
	}

	public override Vector3 EvaluateTangent(float pos) {
		var vector3 = new Vector3();
		Vector3 direction;
		if (m_Waypoints.Length == 0)
			direction = transform.rotation * Vector3.forward;
		else {
			int indexA;
			int indexB;
			pos = GetBoundingIndices(pos, out indexA, out indexB);
			if (indexA == indexB)
				direction = m_Waypoints[indexA].tangent;
			else {
				var waypoint1 = m_Waypoints[indexA];
				var waypoint2 = m_Waypoints[indexB];
				direction = SplineHelpers.BezierTangent3(pos - indexA, m_Waypoints[indexA].position,
					waypoint1.position + waypoint1.tangent, waypoint2.position - waypoint2.tangent, waypoint2.position);
			}
		}

		return transform.TransformDirection(direction);
	}

	public override Quaternion EvaluateOrientation(float pos) {
		var orientation = transform.rotation;
		if (m_Waypoints.Length != 0) {
			int indexA;
			int indexB;
			pos = GetBoundingIndices(pos, out indexA, out indexB);
			float angle;
			if (indexA == indexB)
				angle = m_Waypoints[indexA].roll;
			else {
				var roll1 = m_Waypoints[indexA].roll;
				var roll2 = m_Waypoints[indexB].roll;
				if (indexB == 0) {
					roll1 %= 360f;
					roll2 %= 360f;
				}

				angle = Mathf.Lerp(roll1, roll2, pos - indexA);
			}

			var tangent = EvaluateTangent(pos);
			if (!tangent.AlmostZero()) {
				var upwards = transform.rotation * Vector3.up;
				orientation = Quaternion.LookRotation(tangent, upwards) * Quaternion.AngleAxis(angle, Vector3.forward);
			}
		}

		return orientation;
	}

	private void OnValidate() {
		InvalidateDistanceCache();
	}

	[DocumentationSorting(18.2f, DocumentationSortingAttribute.Level.UserRef)]
	[Serializable]
	public struct Waypoint {
		[Tooltip("Position in path-local space")]
		public Vector3 position;

		[Tooltip(
			"Offset from the position, which defines the tangent of the curve at the waypoint.  The length of the tangent encodes the strength of the bezier handle.  The same handle is used symmetrically on both sides of the waypoint, to ensure smoothness.")]
		public Vector3 tangent;

		[Tooltip(
			"Defines the roll of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up.")]
		public float roll;
	}
}