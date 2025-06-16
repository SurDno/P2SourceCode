using System;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine;

[DocumentationSorting(15f, DocumentationSortingAttribute.Level.UserRef)]
[ExecuteInEditMode]
[AddComponentMenu("")]
[SaveDuringPlay]
public class CinemachineCollider : CinemachineExtension {
	[Header("Obstacle Detection")] [Tooltip("The Unity layer mask against which the collider will raycast")]
	public LayerMask m_CollideAgainst = 1;

	[TagField]
	[Tooltip("Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
	public string m_IgnoreTag = string.Empty;

	[Tooltip("Obstacles closer to the target than this will be ignored")]
	public float m_MinimumDistanceFromTarget = 0.1f;

	[Space]
	[Tooltip(
		"When enabled, will attempt to resolve situations where the line of sight to the target is blocked by an obstacle")]
	[FormerlySerializedAs("m_PreserveLineOfSight")]
	public bool m_AvoidObstacles = true;

	[Tooltip(
		"The maximum raycast distance when checking if the line of sight to this camera's target is clear.  If the setting is 0 or less, the current actual distance to target will be used.")]
	[FormerlySerializedAs("m_LineOfSightFeelerDistance")]
	public float m_DistanceLimit;

	[Tooltip(
		"Camera will try to maintain this distance from any obstacle.  Try to keep this value small.  Increase it if you are seeing inside obstacles due to a large FOV on the camera.")]
	public float m_CameraRadius = 0.1f;

	[Tooltip("The way in which the Collider will attempt to preserve sight of the target.")]
	public ResolutionStrategy m_Strategy = ResolutionStrategy.PreserveCameraHeight;

	[Range(1f, 10f)]
	[Tooltip(
		"Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.  In most environments, 4 is enough.")]
	public int m_MaximumEffort = 4;

	[Range(0.0f, 10f)]
	[Tooltip(
		"The gradualness of collision resolution.  Higher numbers will move the camera more gradually away from obstructions.")]
	[FormerlySerializedAs("m_Smoothing")]
	public float m_Damping;

	[Header("Shot Evaluation")]
	[Tooltip(
		"If greater than zero, a higher score will be given to shots when the target is closer to this distance.  Set this to zero to disable this feature.")]
	public float m_OptimalTargetDistance;

	private const float PrecisionSlush = 0.001f;
	private RaycastHit[] m_CornerBuffer = new RaycastHit[4];
	private const float AngleThreshold = 0.1f;
	private Collider[] mColliderBuffer = new Collider[5];
	private SphereCollider mCameraCollider;
	private GameObject mCameraColliderGameObject;

	public bool IsTargetObscured(ICinemachineCamera vcam) {
		return GetExtraState<VcamExtraState>(vcam).targetObscured;
	}

	public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam) {
		return GetExtraState<VcamExtraState>(vcam).colliderDisplacement > 0.0;
	}

	private void OnValidate() {
		m_DistanceLimit = Mathf.Max(0.0f, m_DistanceLimit);
		m_CameraRadius = Mathf.Max(0.0f, m_CameraRadius);
		m_MinimumDistanceFromTarget = Mathf.Max(0.01f, m_MinimumDistanceFromTarget);
		m_OptimalTargetDistance = Mathf.Max(0.0f, m_OptimalTargetDistance);
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		CleanupCameraCollider();
	}

	public List<List<Vector3>> DebugPaths {
		get {
			var debugPaths = new List<List<Vector3>>();
			foreach (var allExtraState in GetAllExtraStates<VcamExtraState>())
				if (allExtraState.debugResolutionPath != null)
					debugPaths.Add(allExtraState.debugResolutionPath);
			return debugPaths;
		}
	}

	protected override void PostPipelineStageCallback(
		CinemachineVirtualCameraBase vcam,
		CinemachineCore.Stage stage,
		ref CameraState state,
		float deltaTime) {
		VcamExtraState extra = null;
		if (stage == CinemachineCore.Stage.Body) {
			extra = GetExtraState<VcamExtraState>(vcam);
			extra.targetObscured = false;
			extra.colliderDisplacement = 0.0f;
			extra.debugResolutionPath = null;
		}

		if (stage == CinemachineCore.Stage.Body && m_AvoidObstacles) {
			var vector3_1 = PreserveLignOfSight(ref state, ref extra);
			if (m_Damping > 0.0 && deltaTime >= 0.0) {
				var vector3_2 = Damper.Damp(vector3_1 - extra.m_previousDisplacement, m_Damping, deltaTime);
				vector3_1 = extra.m_previousDisplacement + vector3_2;
			}

			extra.m_previousDisplacement = vector3_1;
			state.PositionCorrection += vector3_1;
			extra.colliderDisplacement += vector3_1.magnitude;
		}

		if (stage != CinemachineCore.Stage.Aim)
			return;
		var extraState = GetExtraState<VcamExtraState>(vcam);
		extraState.targetObscured = CheckForTargetObstructions(state);
		if (extraState.targetObscured)
			state.ShotQuality *= 0.2f;
		if (extraState.colliderDisplacement > 0.0)
			state.ShotQuality *= 0.8f;
		var num1 = 0.0f;
		if (m_OptimalTargetDistance > 0.0 && state.HasLookAt) {
			var num2 = Vector3.Magnitude(state.ReferenceLookAt - state.FinalPosition);
			if (num2 <= (double)m_OptimalTargetDistance) {
				var num3 = m_OptimalTargetDistance / 2f;
				if (num2 >= (double)num3)
					num1 = (float)(0.20000000298023224 * (num2 - (double)num3) /
					               (m_OptimalTargetDistance - (double)num3));
			} else {
				var num4 = num2 - m_OptimalTargetDistance;
				var num5 = m_OptimalTargetDistance * 3f;
				if (num4 < (double)num5)
					num1 = (float)(0.20000000298023224 * (1.0 - num4 / (double)num5));
			}

			state.ShotQuality *= 1f + num1;
		}
	}

	private Vector3 PreserveLignOfSight(
		ref CameraState state,
		ref VcamExtraState extra) {
		var vector3_1 = Vector3.zero;
		if (state.HasLookAt) {
			var correctedPosition = state.CorrectedPosition;
			var referenceLookAt = state.ReferenceLookAt;
			var vector3_2 = correctedPosition;
			var vector3_3 = vector3_2 - referenceLookAt;
			var magnitude = vector3_3.magnitude;
			var num = Mathf.Max(m_MinimumDistanceFromTarget, 0.0001f);
			if (magnitude > (double)num) {
				vector3_3.Normalize();
				var b = magnitude - num;
				if (m_DistanceLimit > 9.9999997473787516E-05)
					b = Mathf.Min(m_DistanceLimit, b);
				var ray = new Ray(vector3_2 - b * vector3_3, vector3_3);
				var rayLength = b + 1f / 1000f;
				RaycastHit hitInfo;
				if (rayLength > 9.9999997473787516E-05 && RaycastIgnoreTag(ray, out hitInfo, rayLength)) {
					var distance = Mathf.Max(0.0f, hitInfo.distance - 1f / 1000f);
					vector3_2 = ray.GetPoint(distance);
					extra.AddPointToDebugPath(vector3_2);
					if (m_Strategy != 0)
						vector3_2 = PushCameraBack(vector3_2, vector3_3, hitInfo, referenceLookAt,
							new Plane(state.ReferenceUp, correctedPosition), magnitude, m_MaximumEffort, ref extra);
				}
			}

			if (m_CameraRadius > 9.9999997473787516E-05)
				vector3_2 += RespectCameraRadius(vector3_2, state.ReferenceLookAt);
			else if (mCameraColliderGameObject != null)
				CleanupCameraCollider();
			vector3_1 = vector3_2 - correctedPosition;
		}

		return vector3_1;
	}

	private bool RaycastIgnoreTag(Ray ray, out RaycastHit hitInfo, float rayLength) {
		while (Physics.Raycast(ray, out hitInfo, rayLength, m_CollideAgainst.value, QueryTriggerInteraction.Ignore)) {
			if (m_IgnoreTag.Length == 0 || !hitInfo.collider.CompareTag(m_IgnoreTag))
				return true;
			var ray1 = new Ray(ray.GetPoint(rayLength), -ray.direction);
			if (hitInfo.collider.Raycast(ray1, out hitInfo, rayLength)) {
				rayLength = hitInfo.distance - 1f / 1000f;
				if (rayLength >= 9.9999997473787516E-05)
					ray.origin = ray1.GetPoint(rayLength);
				else
					break;
			} else
				break;
		}

		return false;
	}

	private Vector3 PushCameraBack(
		Vector3 currentPos,
		Vector3 pushDir,
		RaycastHit obstacle,
		Vector3 lookAtPos,
		Plane startPlane,
		float targetDistance,
		int iterations,
		ref VcamExtraState extra) {
		var vector3_1 = currentPos;
		var zero = Vector3.zero;
		if (!GetWalkingDirection(vector3_1, pushDir, obstacle, ref zero))
			return vector3_1;
		var ray = new Ray(vector3_1, zero);
		var pushBackDistance1 = GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
		if (pushBackDistance1 <= 9.9999997473787516E-05)
			return vector3_1;
		var bounds = ClampRayToBounds(ray, pushBackDistance1, obstacle.collider.bounds);
		var num = Mathf.Min(pushBackDistance1, bounds + 1f / 1000f);
		RaycastHit hitInfo;
		if (RaycastIgnoreTag(ray, out hitInfo, num)) {
			var distance = hitInfo.distance - 1f / 1000f;
			var vector3_2 = ray.GetPoint(distance);
			extra.AddPointToDebugPath(vector3_2);
			if (iterations > 1)
				vector3_2 = PushCameraBack(vector3_2, zero, hitInfo, lookAtPos, startPlane, targetDistance,
					iterations - 1, ref extra);
			return vector3_2;
		}

		var vector3_3 = ray.GetPoint(num);
		var vector3_4 = vector3_3 - lookAtPos;
		var magnitude = vector3_4.magnitude;
		if (magnitude < 9.9999997473787516E-05 ||
		    RaycastIgnoreTag(new Ray(lookAtPos, vector3_4), out var _, magnitude - 1f / 1000f))
			return currentPos;
		ray = new Ray(vector3_3, vector3_4);
		extra.AddPointToDebugPath(vector3_3);
		var pushBackDistance2 = GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
		if (pushBackDistance2 > 9.9999997473787516E-05) {
			if (!RaycastIgnoreTag(ray, out hitInfo, pushBackDistance2)) {
				vector3_3 = ray.GetPoint(pushBackDistance2);
				extra.AddPointToDebugPath(vector3_3);
			} else {
				var distance = hitInfo.distance - 1f / 1000f;
				vector3_3 = ray.GetPoint(distance);
				extra.AddPointToDebugPath(vector3_3);
				if (iterations > 1)
					vector3_3 = PushCameraBack(vector3_3, vector3_4, hitInfo, lookAtPos, startPlane, targetDistance,
						iterations - 1, ref extra);
			}
		}

		return vector3_3;
	}

	private bool GetWalkingDirection(
		Vector3 pos,
		Vector3 pushDir,
		RaycastHit obstacle,
		ref Vector3 outDir) {
		var normal = obstacle.normal;
		var num1 = 0.00500000035f;
		var num2 = Physics.SphereCastNonAlloc(pos, num1, pushDir.normalized, m_CornerBuffer, 0.0f,
			m_CollideAgainst.value, QueryTriggerInteraction.Ignore);
		if (num2 > 1)
			for (var index = 0; index < num2; ++index)
				if (m_IgnoreTag.Length <= 0 || !m_CornerBuffer[index].collider.CompareTag(m_IgnoreTag)) {
					var type = m_CornerBuffer[index].collider.GetType();
					if (type == typeof(BoxCollider) || type == typeof(SphereCollider) ||
					    type == typeof(CapsuleCollider)) {
						var direction = m_CornerBuffer[index].collider.ClosestPoint(pos) - pos;
						if (direction.magnitude > 9.9999997473787516E-06 && m_CornerBuffer[index].collider
							    .Raycast(new Ray(pos, direction), out m_CornerBuffer[index], num1)) {
							if (!(m_CornerBuffer[index].normal - obstacle.normal).AlmostZero())
								normal = m_CornerBuffer[index].normal;
							break;
						}
					}
				}

		var vector3 = Vector3.Cross(obstacle.normal, normal);
		if (vector3.AlmostZero())
			vector3 = Vector3.ProjectOnPlane(pushDir, obstacle.normal);
		else {
			var f = Vector3.Dot(vector3, pushDir);
			if (Mathf.Abs(f) < 9.9999997473787516E-05)
				return false;
			if (f < 0.0)
				vector3 = -vector3;
		}

		if (vector3.AlmostZero())
			return false;
		outDir = vector3.normalized;
		return true;
	}

	private float GetPushBackDistance(
		Ray ray,
		Plane startPlane,
		float targetDistance,
		Vector3 lookAtPos) {
		var a = targetDistance - (ray.origin - lookAtPos).magnitude;
		if (a < 9.9999997473787516E-05)
			return 0.0f;
		if (m_Strategy == ResolutionStrategy.PreserveCameraDistance)
			return a;
		float enter;
		if (!startPlane.Raycast(ray, out enter))
			enter = 0.0f;
		var b = Mathf.Min(a, enter);
		if (b < 9.9999997473787516E-05)
			return 0.0f;
		var num = Mathf.Abs(Vector3.Angle(startPlane.normal, ray.direction) - 90f);
		if (num < 0.10000000149011612)
			b = Mathf.Lerp(0.0f, b, num / 0.1f);
		return b;
	}

	private float ClampRayToBounds(Ray ray, float distance, Bounds bounds) {
		float enter;
		if (Vector3.Dot(ray.direction, Vector3.up) > 0.0) {
			if (new Plane(Vector3.down, bounds.max).Raycast(ray, out enter) && enter > 9.9999997473787516E-05)
				distance = Mathf.Min(distance, enter);
		} else if (Vector3.Dot(ray.direction, Vector3.down) > 0.0 &&
		           new Plane(Vector3.up, bounds.min).Raycast(ray, out enter) && enter > 9.9999997473787516E-05)
			distance = Mathf.Min(distance, enter);

		if (Vector3.Dot(ray.direction, Vector3.right) > 0.0) {
			if (new Plane(Vector3.left, bounds.max).Raycast(ray, out enter) && enter > 9.9999997473787516E-05)
				distance = Mathf.Min(distance, enter);
		} else if (Vector3.Dot(ray.direction, Vector3.left) > 0.0 &&
		           new Plane(Vector3.right, bounds.min).Raycast(ray, out enter) && enter > 9.9999997473787516E-05)
			distance = Mathf.Min(distance, enter);

		if (Vector3.Dot(ray.direction, Vector3.forward) > 0.0) {
			if (new Plane(Vector3.back, bounds.max).Raycast(ray, out enter) && enter > 9.9999997473787516E-05)
				distance = Mathf.Min(distance, enter);
		} else if (Vector3.Dot(ray.direction, Vector3.back) > 0.0 &&
		           new Plane(Vector3.forward, bounds.min).Raycast(ray, out enter) && enter > 9.9999997473787516E-05)
			distance = Mathf.Min(distance, enter);

		return distance;
	}

	private Vector3 RespectCameraRadius(Vector3 cameraPos, Vector3 lookAtPos) {
		var zero = Vector3.zero;
		var num = Physics.OverlapSphereNonAlloc(cameraPos, m_CameraRadius, mColliderBuffer, m_CollideAgainst,
			QueryTriggerInteraction.Ignore);
		if (num > 0) {
			if (mCameraColliderGameObject == null) {
				mCameraColliderGameObject = new GameObject("Cinemachine Collider Collider");
				mCameraColliderGameObject.hideFlags = HideFlags.HideAndDontSave;
				mCameraColliderGameObject.transform.position =
					new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
				mCameraColliderGameObject.SetActive(true);
				mCameraCollider = mCameraColliderGameObject.AddComponent<SphereCollider>();
			}

			mCameraCollider.radius = m_CameraRadius;
			for (var index = 0; index < num; ++index) {
				var colliderB = mColliderBuffer[index];
				Vector3 direction;
				float distance;
				if ((m_IgnoreTag.Length <= 0 || !colliderB.CompareTag(m_IgnoreTag)) && Physics.ComputePenetration(
					    mCameraCollider, cameraPos, Quaternion.identity, colliderB, colliderB.transform.position,
					    colliderB.transform.rotation, out direction, out distance))
					zero += direction * distance;
			}
		}

		return zero;
	}

	private void CleanupCameraCollider() {
		if (mCameraColliderGameObject != null)
			DestroyImmediate(mCameraColliderGameObject);
		mCameraColliderGameObject = null;
		mCameraCollider = null;
	}

	private bool CheckForTargetObstructions(CameraState state) {
		if (state.HasLookAt) {
			var referenceLookAt = state.ReferenceLookAt;
			var correctedPosition = state.CorrectedPosition;
			var vector3 = referenceLookAt - correctedPosition;
			var magnitude = vector3.magnitude;
			if (magnitude < (double)Mathf.Max(m_MinimumDistanceFromTarget, 0.0001f) || RaycastIgnoreTag(
				    new Ray(correctedPosition, vector3.normalized), out var _, magnitude - m_MinimumDistanceFromTarget))
				return true;
		}

		return false;
	}

	public enum ResolutionStrategy {
		PullCameraForward,
		PreserveCameraHeight,
		PreserveCameraDistance
	}

	private class VcamExtraState {
		public Vector3 m_previousDisplacement;
		public float colliderDisplacement;
		public bool targetObscured;
		public List<Vector3> debugResolutionPath;

		public void AddPointToDebugPath(Vector3 p) { }
	}
}