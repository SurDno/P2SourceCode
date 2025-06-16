using System;
using Cinemachine.Utility;
using UnityEngine;

namespace Cinemachine;

[DocumentationSorting(4f, DocumentationSortingAttribute.Level.UserRef)]
[ExecuteInEditMode]
[AddComponentMenu("")]
[RequireComponent(typeof(CinemachinePipeline))]
[SaveDuringPlay]
public class CinemachineGroupComposer : CinemachineComposer {
	[Space]
	[Tooltip(
		"The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc.")]
	public float m_GroupFramingSize = 0.8f;

	[Tooltip("What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both")]
	public FramingMode m_FramingMode = FramingMode.HorizontalAndVertical;

	[Range(0.0f, 20f)]
	[Tooltip(
		"How aggressively the camera tries to frame the group. Small numbers are more responsive, rapidly adjusting the camera to keep the group in the frame.  Larger numbers give a more heavy slowly responding camera.")]
	public float m_FrameDamping = 2f;

	[Tooltip("How to adjust the camera to get the desired framing.  You can zoom, dolly in/out, or do both.")]
	public AdjustmentMode m_AdjustmentMode = AdjustmentMode.DollyThenZoom;

	[Tooltip("The maximum distance toward the target that this behaviour is allowed to move the camera.")]
	public float m_MaxDollyIn = 5000f;

	[Tooltip("The maximum distance away the target that this behaviour is allowed to move the camera.")]
	public float m_MaxDollyOut = 5000f;

	[Tooltip("Set this to limit how close to the target the camera can get.")]
	public float m_MinimumDistance = 1f;

	[Tooltip("Set this to limit how far from the target the camera can get.")]
	public float m_MaximumDistance = 5000f;

	[Range(1f, 179f)] [Tooltip("If adjusting FOV, will not set the FOV lower than this.")]
	public float m_MinimumFOV = 3f;

	[Range(1f, 179f)] [Tooltip("If adjusting FOV, will not set the FOV higher than this.")]
	public float m_MaximumFOV = 60f;

	[Tooltip("If adjusting Orthographic Size, will not set it lower than this.")]
	public float m_MinimumOrthoSize = 1f;

	[Tooltip("If adjusting Orthographic Size, will not set it higher than this.")]
	public float m_MaximumOrthoSize = 100f;

	private float m_prevTargetHeight;

	private void OnValidate() {
		m_GroupFramingSize = Mathf.Max(0.0001f, m_GroupFramingSize);
		m_MaxDollyIn = Mathf.Max(0.0f, m_MaxDollyIn);
		m_MaxDollyOut = Mathf.Max(0.0f, m_MaxDollyOut);
		m_MinimumDistance = Mathf.Max(0.0f, m_MinimumDistance);
		m_MaximumDistance = Mathf.Max(m_MinimumDistance, m_MaximumDistance);
		m_MinimumFOV = Mathf.Max(1f, m_MinimumFOV);
		m_MaximumFOV = Mathf.Clamp(m_MaximumFOV, m_MinimumFOV, 179f);
		m_MinimumOrthoSize = Mathf.Max(0.01f, m_MinimumOrthoSize);
		m_MaximumOrthoSize = Mathf.Max(m_MinimumOrthoSize, m_MaximumOrthoSize);
	}

	public CinemachineTargetGroup TargetGroup {
		get {
			var lookAtTarget = LookAtTarget;
			return lookAtTarget != null ? lookAtTarget.GetComponent<CinemachineTargetGroup>() : null;
		}
	}

	public override void MutateCameraState(ref CameraState curState, float deltaTime) {
		var targetGroup = TargetGroup;
		if (targetGroup == null)
			base.MutateCameraState(ref curState, deltaTime);
		else if (!IsValid || !curState.HasLookAt)
			m_prevTargetHeight = 0.0f;
		else {
			curState.ReferenceLookAt = GetLookAtPointAndSetTrackedPoint(targetGroup.transform.position);
			var v = TrackedPoint - curState.RawPosition;
			var magnitude1 = v.magnitude;
			if (magnitude1 < 9.9999997473787516E-05)
				return;
			var forward = v.AlmostZero() ? Vector3.forward : v.normalized;
			var boundingBox = targetGroup.BoundingBox;
			m_lastBoundsMatrix = Matrix4x4.TRS(boundingBox.center - forward * boundingBox.extents.magnitude,
				Quaternion.LookRotation(forward, curState.ReferenceUp), Vector3.one);
			m_LastBounds = targetGroup.GetViewSpaceBoundingBox(m_lastBoundsMatrix);
			var num1 = GetTargetHeight(m_LastBounds);
			var vector3 = m_lastBoundsMatrix.MultiplyPoint3x4(m_LastBounds.center);
			if (deltaTime >= 0.0)
				num1 = m_prevTargetHeight + Damper.Damp(num1 - m_prevTargetHeight, m_FrameDamping, deltaTime);
			m_prevTargetHeight = num1;
			Bounds mLastBounds;
			if (!curState.Lens.Orthographic && m_AdjustmentMode != 0) {
				var fieldOfView = curState.Lens.FieldOfView;
				double num2 = num1 / (2f * Mathf.Tan((float)(fieldOfView * (Math.PI / 180.0) / 2.0)));
				mLastBounds = m_LastBounds;
				double z = mLastBounds.extents.z;
				var num3 = Mathf.Clamp(
					Mathf.Clamp((float)(num2 + z), magnitude1 - m_MaxDollyIn, magnitude1 + m_MaxDollyOut),
					m_MinimumDistance, m_MaximumDistance);
				curState.PositionCorrection += vector3 - forward * num3 - curState.RawPosition;
			}

			if (curState.Lens.Orthographic || m_AdjustmentMode != AdjustmentMode.DollyOnly) {
				double magnitude2 = (TrackedPoint - curState.CorrectedPosition).magnitude;
				mLastBounds = m_LastBounds;
				double z = mLastBounds.extents.z;
				var num4 = (float)(magnitude2 - z);
				var num5 = 179f;
				if (num4 > 9.9999997473787516E-05)
					num5 = (float)(2.0 * Mathf.Atan(num1 / (2f * num4)) * 57.295780181884766);
				var lens = curState.Lens with {
					FieldOfView = Mathf.Clamp(num5, m_MinimumFOV, m_MaximumFOV),
					OrthographicSize = Mathf.Clamp(num1 / 2f, m_MinimumOrthoSize, m_MaximumOrthoSize)
				};
				curState.Lens = lens;
			}

			base.MutateCameraState(ref curState, deltaTime);
		}
	}

	public Bounds m_LastBounds { get; private set; }

	public Matrix4x4 m_lastBoundsMatrix { get; private set; }

	private float GetTargetHeight(Bounds b) {
		var num = Mathf.Max(0.0001f, m_GroupFramingSize);
		switch (m_FramingMode) {
			case FramingMode.Horizontal:
				return Mathf.Max(0.0001f, b.size.x) / (num * VcamState.Lens.Aspect);
			case FramingMode.Vertical:
				return Mathf.Max(0.0001f, b.size.y) / num;
			default:
				return Mathf.Max(Mathf.Max(0.0001f, b.size.x) / (num * VcamState.Lens.Aspect),
					Mathf.Max(0.0001f, b.size.y) / num);
		}
	}

	[DocumentationSorting(4.01f, DocumentationSortingAttribute.Level.UserRef)]
	public enum FramingMode {
		Horizontal,
		Vertical,
		HorizontalAndVertical
	}

	public enum AdjustmentMode {
		ZoomOnly,
		DollyOnly,
		DollyThenZoom
	}
}