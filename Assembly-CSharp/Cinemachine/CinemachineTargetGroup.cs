using System;
using Cinemachine.Utility;
using UnityEngine;

namespace Cinemachine;

[DocumentationSorting(19f, DocumentationSortingAttribute.Level.UserRef)]
[AddComponentMenu("Cinemachine/CinemachineTargetGroup")]
[SaveDuringPlay]
[ExecuteInEditMode]
public class CinemachineTargetGroup : MonoBehaviour {
	[Tooltip(
		"How the group's position is calculated.  Select GroupCenter for the center of the bounding box, and GroupAverage for a weighted average of the positions of the members.")]
	public PositionMode m_PositionMode = PositionMode.GroupCenter;

	[Tooltip(
		"How the group's rotation is calculated.  Select Manual to use the value in the group's transform, and GroupAverage for a weighted average of the orientations of the members.")]
	public RotationMode m_RotationMode = RotationMode.Manual;

	[Tooltip("When to update the group's transform based on the position of the group members")]
	public UpdateMethod m_UpdateMethod = UpdateMethod.LateUpdate;

	[NoSaveDuringPlay]
	[Tooltip(
		"The target objects, together with their weights and radii, that will contribute to the group's average position, orientation, and size.")]
	public Target[] m_Targets = new Target[0];

	private float m_lastRadius;

	public Bounds BoundingBox {
		get {
			float averageWeight;
			var averagePosition = CalculateAveragePosition(out averageWeight);
			var flag = false;
			var boundingBox = new Bounds(averagePosition,
				new Vector3(m_lastRadius * 2f, m_lastRadius * 2f, m_lastRadius * 2f));
			if (averageWeight > 9.9999997473787516E-05)
				for (var index = 0; index < m_Targets.Length; ++index)
					if (m_Targets[index].target != null) {
						var weight = m_Targets[index].weight;
						var t = weight >= averageWeight - 9.9999997473787516E-05 ? 1f : weight / averageWeight;
						var num = m_Targets[index].radius * 2f * t;
						var bounds = new Bounds(Vector3.Lerp(averagePosition, m_Targets[index].target.position, t),
							new Vector3(num, num, num));
						if (!flag)
							boundingBox = bounds;
						else
							boundingBox.Encapsulate(bounds);
						flag = true;
					}

			var extents = boundingBox.extents;
			m_lastRadius = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z));
			return boundingBox;
		}
	}

	public bool IsEmpty {
		get {
			for (var index = 0; index < m_Targets.Length; ++index)
				if (m_Targets[index].target != null && m_Targets[index].weight > 9.9999997473787516E-05)
					return false;
			return true;
		}
	}

	public Bounds GetViewSpaceBoundingBox(Matrix4x4 mView) {
		var inverse = mView.inverse;
		float averageWeight;
		var vector3 = inverse.MultiplyPoint3x4(CalculateAveragePosition(out averageWeight));
		var flag = false;
		var spaceBoundingBox =
			new Bounds(vector3, new Vector3(m_lastRadius * 2f, m_lastRadius * 2f, m_lastRadius * 2f));
		if (averageWeight > 9.9999997473787516E-05)
			for (var index = 0; index < m_Targets.Length; ++index)
				if (m_Targets[index].target != null) {
					var weight = m_Targets[index].weight;
					var t = weight >= averageWeight - 9.9999997473787516E-05 ? 1f : weight / averageWeight;
					var num = m_Targets[index].radius * 2f;
					Vector4 b = inverse.MultiplyPoint3x4(m_Targets[index].target.position);
					var bounds = new Bounds((Vector4)Vector3.Lerp(vector3, b, t), new Vector3(num, num, num));
					if (!flag)
						spaceBoundingBox = bounds;
					else
						spaceBoundingBox.Encapsulate(bounds);
					flag = true;
				}

		var extents = spaceBoundingBox.extents;
		m_lastRadius = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z));
		return spaceBoundingBox;
	}

	private Vector3 CalculateAveragePosition(out float averageWeight) {
		var zero = Vector3.zero;
		var num1 = 0.0f;
		var num2 = 0;
		for (var index = 0; index < m_Targets.Length; ++index)
			if (m_Targets[index].target != null && m_Targets[index].weight > 9.9999997473787516E-05) {
				++num2;
				num1 += m_Targets[index].weight;
				zero += m_Targets[index].target.position * m_Targets[index].weight;
			}

		if (num1 > 9.9999997473787516E-05)
			zero /= num1;
		if (num2 == 0) {
			averageWeight = 0.0f;
			return transform.position;
		}

		averageWeight = num1 / num2;
		return zero;
	}

	private Quaternion CalculateAverageOrientation() {
		var q = Quaternion.identity;
		for (var index = 0; index < m_Targets.Length; ++index)
			if (m_Targets[index].target != null) {
				var weight = m_Targets[index].weight;
				var rotation = m_Targets[index].target.rotation;
				q = new Quaternion(q.x + rotation.x * weight, q.y + rotation.y * weight, q.z + rotation.z * weight,
					q.w + rotation.w * weight);
			}

		return q.Normalized();
	}

	private void OnValidate() {
		for (var index = 0; index < m_Targets.Length; ++index) {
			if (m_Targets[index].weight < 0.0)
				m_Targets[index].weight = 0.0f;
			if (m_Targets[index].radius < 0.0)
				m_Targets[index].radius = 0.0f;
		}
	}

	private void FixedUpdate() {
		if (m_UpdateMethod != UpdateMethod.FixedUpdate)
			return;
		UpdateTransform();
	}

	private void Update() {
		if (Application.isPlaying && m_UpdateMethod != UpdateMethod.Update)
			return;
		UpdateTransform();
	}

	private void LateUpdate() {
		if (m_UpdateMethod != UpdateMethod.LateUpdate)
			return;
		UpdateTransform();
	}

	private void UpdateTransform() {
		if (IsEmpty)
			return;
		switch (m_PositionMode) {
			case PositionMode.GroupCenter:
				transform.position = BoundingBox.center;
				break;
			case PositionMode.GroupAverage:
				transform.position = CalculateAveragePosition(out var _);
				break;
		}

		switch (m_RotationMode) {
			case RotationMode.GroupAverage:
				transform.rotation = CalculateAverageOrientation();
				break;
		}
	}

	[DocumentationSorting(19.1f, DocumentationSortingAttribute.Level.UserRef)]
	[Serializable]
	public struct Target {
		[Tooltip(
			"The target objects.  This object's position and orientation will contribute to the group's average position and orientation, in accordance with its weight")]
		public Transform target;

		[Tooltip("How much weight to give the target when averaging.  Cannot be negative")]
		public float weight;

		[Tooltip("The radius of the target, used for calculating the bounding box.  Cannot be negative")]
		public float radius;
	}

	[DocumentationSorting(19.2f, DocumentationSortingAttribute.Level.UserRef)]
	public enum PositionMode {
		GroupCenter,
		GroupAverage
	}

	[DocumentationSorting(19.3f, DocumentationSortingAttribute.Level.UserRef)]
	public enum RotationMode {
		Manual,
		GroupAverage
	}

	public enum UpdateMethod {
		Update,
		FixedUpdate,
		LateUpdate
	}
}