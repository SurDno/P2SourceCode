using System;
using Cinemachine.Utility;
using UnityEngine;

namespace Cinemachine;

[DocumentationSorting(3f, DocumentationSortingAttribute.Level.UserRef)]
[ExecuteInEditMode]
[AddComponentMenu("")]
[RequireComponent(typeof(CinemachinePipeline))]
[SaveDuringPlay]
public class CinemachineComposer : CinemachineComponentBase {
	[NoSaveDuringPlay] [HideInInspector] public Action OnGUICallback = null;

	[Tooltip(
		"Target offset from the target object's center in target-local space. Use this to fine-tune the tracking target position when the desired area is not the tracked object's center.")]
	public Vector3 m_TrackedObjectOffset = Vector3.zero;

	[Tooltip(
		"This setting will instruct the composer to adjust its target offset based on the motion of the target.  The composer will look at a point where it estimates the target will be this many seconds into the future.  Note that this setting is sensitive to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly.")]
	[Range(0.0f, 1f)]
	public float m_LookaheadTime;

	[Tooltip(
		"Controls the smoothness of the lookahead algorithm.  Larger values smooth out jittery predictions and also increase prediction lag")]
	[Range(3f, 30f)]
	public float m_LookaheadSmoothing = 10f;

	[Space]
	[Range(0.0f, 20f)]
	[Tooltip(
		"How aggressively the camera tries to follow the target in the screen-horizontal direction. Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical and horizontal settings can yield a wide range of camera behaviors.")]
	public float m_HorizontalDamping = 0.5f;

	[Range(0.0f, 20f)]
	[Tooltip(
		"How aggressively the camera tries to follow the target in the screen-vertical direction. Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical and horizontal settings can yield a wide range of camera behaviors.")]
	public float m_VerticalDamping = 0.5f;

	[Space]
	[Range(0.0f, 1f)]
	[Tooltip("Horizontal screen position for target. The camera will rotate to position the tracked object here.")]
	public float m_ScreenX = 0.5f;

	[Range(0.0f, 1f)]
	[Tooltip("Vertical screen position for target, The camera will rotate to position the tracked object here.")]
	public float m_ScreenY = 0.5f;

	[Range(0.0f, 1f)]
	[Tooltip("Camera will not rotate horizontally if the target is within this range of the position.")]
	public float m_DeadZoneWidth = 0.1f;

	[Range(0.0f, 1f)] [Tooltip("Camera will not rotate vertically if the target is within this range of the position.")]
	public float m_DeadZoneHeight = 0.1f;

	[Range(0.0f, 2f)]
	[Tooltip(
		"When target is within this region, camera will gradually rotate horizontally to re-align towards the desired position, depending on the damping speed.")]
	public float m_SoftZoneWidth = 0.8f;

	[Range(0.0f, 2f)]
	[Tooltip(
		"When target is within this region, camera will gradually rotate vertically to re-align towards the desired position, depending on the damping speed.")]
	public float m_SoftZoneHeight = 0.8f;

	[Range(-0.5f, 0.5f)]
	[Tooltip("A non-zero bias will move the target position horizontally away from the center of the soft zone.")]
	public float m_BiasX;

	[Range(-0.5f, 0.5f)]
	[Tooltip("A non-zero bias will move the target position vertically away from the center of the soft zone.")]
	public float m_BiasY;

	private Vector3 m_CameraPosPrevFrame = Vector3.zero;
	private Vector3 m_LookAtPrevFrame = Vector3.zero;
	private Vector2 m_ScreenOffsetPrevFrame = Vector2.zero;
	private Quaternion m_CameraOrientationPrevFrame = Quaternion.identity;
	private PositionPredictor m_Predictor = new();

	public override bool IsValid => enabled && LookAtTarget != null;

	public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

	public Vector3 TrackedPoint { get; private set; }

	protected virtual Vector3 GetLookAtPointAndSetTrackedPoint(Vector3 lookAt) {
		var pos = lookAt;
		if (LookAtTarget != null)
			pos += LookAtTarget.transform.rotation * m_TrackedObjectOffset;
		m_Predictor.Smoothing = m_LookaheadSmoothing;
		m_Predictor.AddPosition(pos);
		TrackedPoint = m_LookaheadTime > 0.0 ? m_Predictor.PredictPosition(m_LookaheadTime) : pos;
		return pos;
	}

	public override void PrePipelineMutateCameraState(ref CameraState curState) {
		if (!IsValid || !curState.HasLookAt)
			return;
		curState.ReferenceLookAt = GetLookAtPointAndSetTrackedPoint(curState.ReferenceLookAt);
	}

	public override void MutateCameraState(ref CameraState curState, float deltaTime) {
		if (deltaTime < 0.0)
			m_Predictor.Reset();
		if (!IsValid || !curState.HasLookAt)
			return;
		var magnitude = (TrackedPoint - curState.CorrectedPosition).magnitude;
		if (magnitude < 9.9999997473787516E-05) {
			if (deltaTime < 0.0)
				return;
			curState.RawOrientation = m_CameraOrientationPrevFrame;
		} else {
			float fov1;
			LensSettings lens;
			float fovH1;
			if (curState.Lens.Orthographic) {
				fov1 = 114.59156f * Mathf.Atan(curState.Lens.OrthographicSize / magnitude);
				lens = curState.Lens;
				fovH1 = (float)(114.59156036376953 *
				                Mathf.Atan(lens.Aspect * curState.Lens.OrthographicSize / magnitude));
			} else {
				fov1 = curState.Lens.FieldOfView;
				fovH1 = (float)(57.295780181884766 *
				                (2.0 * Math.Atan(Math.Tan(fov1 * (Math.PI / 180.0) / 2.0) * curState.Lens.Aspect)));
			}

			var rigOrientation = curState.RawOrientation;
			var softGuideRect = SoftGuideRect;
			double fov2 = fov1;
			double fovH2 = fovH1;
			lens = curState.Lens;
			double aspect1 = lens.Aspect;
			var fov3 = ScreenToFOV(softGuideRect, (float)fov2, (float)fovH2, (float)aspect1);
			if (deltaTime < 0.0) {
				var screenRect = new Rect(fov3.center, Vector2.zero);
				RotateToScreenBounds(ref curState, screenRect, ref rigOrientation, fov1, fovH1, -1f);
			} else {
				var vector3 = m_LookAtPrevFrame - (m_CameraPosPrevFrame + curState.PositionDampingBypass);
				rigOrientation = !vector3.AlmostZero()
					? Quaternion.LookRotation(vector3, curState.ReferenceUp)
						.ApplyCameraRotation(-m_ScreenOffsetPrevFrame, curState.ReferenceUp)
					: Quaternion.LookRotation(m_CameraOrientationPrevFrame * Vector3.forward, curState.ReferenceUp);
				var hardGuideRect = HardGuideRect;
				double fov4 = fov1;
				double fovH3 = fovH1;
				lens = curState.Lens;
				double aspect2 = lens.Aspect;
				var fov5 = ScreenToFOV(hardGuideRect, (float)fov4, (float)fovH3, (float)aspect2);
				if (!RotateToScreenBounds(ref curState, fov5, ref rigOrientation, fov1, fovH1, -1f))
					RotateToScreenBounds(ref curState, fov3, ref rigOrientation, fov1, fovH1, deltaTime);
			}

			m_CameraPosPrevFrame = curState.CorrectedPosition;
			m_LookAtPrevFrame = TrackedPoint;
			m_CameraOrientationPrevFrame = rigOrientation.Normalized();
			m_ScreenOffsetPrevFrame =
				m_CameraOrientationPrevFrame.GetCameraRotationToTarget(m_LookAtPrevFrame - curState.CorrectedPosition,
					curState.ReferenceUp);
			curState.RawOrientation = m_CameraOrientationPrevFrame;
		}
	}

	public Rect SoftGuideRect {
		get => new(m_ScreenX - m_DeadZoneWidth / 2f, m_ScreenY - m_DeadZoneHeight / 2f, m_DeadZoneWidth,
			m_DeadZoneHeight);
		set {
			m_DeadZoneWidth = Mathf.Clamp01(value.width);
			m_DeadZoneHeight = Mathf.Clamp01(value.height);
			m_ScreenX = Mathf.Clamp01(value.x + m_DeadZoneWidth / 2f);
			m_ScreenY = Mathf.Clamp01(value.y + m_DeadZoneHeight / 2f);
			m_SoftZoneWidth = Mathf.Max(m_SoftZoneWidth, m_DeadZoneWidth);
			m_SoftZoneHeight = Mathf.Max(m_SoftZoneHeight, m_DeadZoneHeight);
		}
	}

	public Rect HardGuideRect {
		get {
			var hardGuideRect = new Rect(m_ScreenX - m_SoftZoneWidth / 2f, m_ScreenY - m_SoftZoneHeight / 2f,
				m_SoftZoneWidth, m_SoftZoneHeight);
			hardGuideRect.position += new Vector2(m_BiasX * (m_SoftZoneWidth - m_DeadZoneWidth),
				m_BiasY * (m_SoftZoneHeight - m_DeadZoneHeight));
			return hardGuideRect;
		}
		set {
			m_SoftZoneWidth = Mathf.Clamp(value.width, 0.0f, 2f);
			m_SoftZoneHeight = Mathf.Clamp(value.height, 0.0f, 2f);
			m_DeadZoneWidth = Mathf.Min(m_DeadZoneWidth, m_SoftZoneWidth);
			m_DeadZoneHeight = Mathf.Min(m_DeadZoneHeight, m_SoftZoneHeight);
			var vector2 = value.center - new Vector2(m_ScreenX, m_ScreenY);
			var num1 = Mathf.Max(0.0f, m_SoftZoneWidth - m_DeadZoneWidth);
			var num2 = Mathf.Max(0.0f, m_SoftZoneHeight - m_DeadZoneHeight);
			m_BiasX = num1 < 9.9999997473787516E-05 ? 0.0f : Mathf.Clamp(vector2.x / num1, -0.5f, 0.5f);
			m_BiasY = num2 < 9.9999997473787516E-05 ? 0.0f : Mathf.Clamp(vector2.y / num2, -0.5f, 0.5f);
		}
	}

	private Rect ScreenToFOV(Rect rScreen, float fov, float fovH, float aspect) {
		var fov1 = new Rect(rScreen);
		var inverse = Matrix4x4.Perspective(fov, aspect, 0.01f, 10000f).inverse;
		var to1 = inverse.MultiplyPoint(new Vector3(0.0f, (float)(fov1.yMin * 2.0 - 1.0), 0.1f));
		to1.z = -to1.z;
		var num1 = UnityVectorExtensions.SignedAngle(Vector3.forward, to1, Vector3.left);
		fov1.yMin = (fov / 2f + num1) / fov;
		var to2 = inverse.MultiplyPoint(new Vector3(0.0f, (float)(fov1.yMax * 2.0 - 1.0), 0.1f));
		to2.z = -to2.z;
		var num2 = UnityVectorExtensions.SignedAngle(Vector3.forward, to2, Vector3.left);
		fov1.yMax = (fov / 2f + num2) / fov;
		var to3 = inverse.MultiplyPoint(new Vector3((float)(fov1.xMin * 2.0 - 1.0), 0.0f, 0.1f));
		to3.z = -to3.z;
		var num3 = UnityVectorExtensions.SignedAngle(Vector3.forward, to3, Vector3.up);
		fov1.xMin = (fovH / 2f + num3) / fovH;
		var to4 = inverse.MultiplyPoint(new Vector3((float)(fov1.xMax * 2.0 - 1.0), 0.0f, 0.1f));
		to4.z = -to4.z;
		var num4 = UnityVectorExtensions.SignedAngle(Vector3.forward, to4, Vector3.up);
		fov1.xMax = (fovH / 2f + num4) / fovH;
		return fov1;
	}

	private bool RotateToScreenBounds(
		ref CameraState state,
		Rect screenRect,
		ref Quaternion rigOrientation,
		float fov,
		float fovH,
		float deltaTime) {
		var vector3 = TrackedPoint - state.CorrectedPosition;
		var rotationToTarget = rigOrientation.GetCameraRotationToTarget(vector3, state.ReferenceUp);
		ClampVerticalBounds(ref screenRect, vector3, state.ReferenceUp, fov);
		var num1 = (screenRect.yMin - 0.5f) * fov;
		var num2 = (screenRect.yMax - 0.5f) * fov;
		if (rotationToTarget.x < (double)num1)
			rotationToTarget.x -= num1;
		else if (rotationToTarget.x > (double)num2)
			rotationToTarget.x -= num2;
		else
			rotationToTarget.x = 0.0f;
		var num3 = (screenRect.xMin - 0.5f) * fovH;
		var num4 = (screenRect.xMax - 0.5f) * fovH;
		if (rotationToTarget.y < (double)num3)
			rotationToTarget.y -= num3;
		else if (rotationToTarget.y > (double)num4)
			rotationToTarget.y -= num4;
		else
			rotationToTarget.y = 0.0f;
		if (deltaTime >= 0.0) {
			rotationToTarget.x = Damper.Damp(rotationToTarget.x, m_VerticalDamping, deltaTime);
			rotationToTarget.y = Damper.Damp(rotationToTarget.y, m_HorizontalDamping, deltaTime);
		}

		rigOrientation = rigOrientation.ApplyCameraRotation(rotationToTarget, state.ReferenceUp);
		return false;
	}

	private bool ClampVerticalBounds(ref Rect r, Vector3 dir, Vector3 up, float fov) {
		var num1 = Vector3.Angle(dir, up);
		var num2 = (float)(fov / 2.0 + 1.0);
		if (num1 < (double)num2) {
			var b = (float)(1.0 - (num2 - (double)num1) / fov);
			if (r.yMax > (double)b) {
				r.yMin = Mathf.Min(r.yMin, b);
				r.yMax = Mathf.Min(r.yMax, b);
				return true;
			}
		}

		if (num1 > 180.0 - num2) {
			var b = (num1 - (180f - num2)) / fov;
			if (b > (double)r.yMin) {
				r.yMin = Mathf.Max(r.yMin, b);
				r.yMax = Mathf.Max(r.yMax, b);
				return true;
			}
		}

		return false;
	}
}