using Cinemachine.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{
  [DocumentationSorting(15f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [AddComponentMenu("")]
  [SaveDuringPlay]
  public class CinemachineCollider : CinemachineExtension
  {
    [Header("Obstacle Detection")]
    [Tooltip("The Unity layer mask against which the collider will raycast")]
    public LayerMask m_CollideAgainst = (LayerMask) 1;
    [TagField]
    [Tooltip("Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
    public string m_IgnoreTag = string.Empty;
    [Tooltip("Obstacles closer to the target than this will be ignored")]
    public float m_MinimumDistanceFromTarget = 0.1f;
    [Space]
    [Tooltip("When enabled, will attempt to resolve situations where the line of sight to the target is blocked by an obstacle")]
    [FormerlySerializedAs("m_PreserveLineOfSight")]
    public bool m_AvoidObstacles = true;
    [Tooltip("The maximum raycast distance when checking if the line of sight to this camera's target is clear.  If the setting is 0 or less, the current actual distance to target will be used.")]
    [FormerlySerializedAs("m_LineOfSightFeelerDistance")]
    public float m_DistanceLimit = 0.0f;
    [Tooltip("Camera will try to maintain this distance from any obstacle.  Try to keep this value small.  Increase it if you are seeing inside obstacles due to a large FOV on the camera.")]
    public float m_CameraRadius = 0.1f;
    [Tooltip("The way in which the Collider will attempt to preserve sight of the target.")]
    public CinemachineCollider.ResolutionStrategy m_Strategy = CinemachineCollider.ResolutionStrategy.PreserveCameraHeight;
    [Range(1f, 10f)]
    [Tooltip("Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.  In most environments, 4 is enough.")]
    public int m_MaximumEffort = 4;
    [Range(0.0f, 10f)]
    [Tooltip("The gradualness of collision resolution.  Higher numbers will move the camera more gradually away from obstructions.")]
    [FormerlySerializedAs("m_Smoothing")]
    public float m_Damping = 0.0f;
    [Header("Shot Evaluation")]
    [Tooltip("If greater than zero, a higher score will be given to shots when the target is closer to this distance.  Set this to zero to disable this feature.")]
    public float m_OptimalTargetDistance = 0.0f;
    private const float PrecisionSlush = 0.001f;
    private RaycastHit[] m_CornerBuffer = new RaycastHit[4];
    private const float AngleThreshold = 0.1f;
    private Collider[] mColliderBuffer = new Collider[5];
    private SphereCollider mCameraCollider;
    private GameObject mCameraColliderGameObject;

    public bool IsTargetObscured(ICinemachineCamera vcam)
    {
      return this.GetExtraState<CinemachineCollider.VcamExtraState>(vcam).targetObscured;
    }

    public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
    {
      return (double) this.GetExtraState<CinemachineCollider.VcamExtraState>((ICinemachineCamera) vcam).colliderDisplacement > 0.0;
    }

    private void OnValidate()
    {
      this.m_DistanceLimit = Mathf.Max(0.0f, this.m_DistanceLimit);
      this.m_CameraRadius = Mathf.Max(0.0f, this.m_CameraRadius);
      this.m_MinimumDistanceFromTarget = Mathf.Max(0.01f, this.m_MinimumDistanceFromTarget);
      this.m_OptimalTargetDistance = Mathf.Max(0.0f, this.m_OptimalTargetDistance);
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      this.CleanupCameraCollider();
    }

    public List<List<Vector3>> DebugPaths
    {
      get
      {
        List<List<Vector3>> debugPaths = new List<List<Vector3>>();
        foreach (CinemachineCollider.VcamExtraState allExtraState in this.GetAllExtraStates<CinemachineCollider.VcamExtraState>())
        {
          if (allExtraState.debugResolutionPath != null)
            debugPaths.Add(allExtraState.debugResolutionPath);
        }
        return debugPaths;
      }
    }

    protected override void PostPipelineStageCallback(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState state,
      float deltaTime)
    {
      CinemachineCollider.VcamExtraState extra = (CinemachineCollider.VcamExtraState) null;
      if (stage == CinemachineCore.Stage.Body)
      {
        extra = this.GetExtraState<CinemachineCollider.VcamExtraState>((ICinemachineCamera) vcam);
        extra.targetObscured = false;
        extra.colliderDisplacement = 0.0f;
        extra.debugResolutionPath = (List<Vector3>) null;
      }
      if (stage == CinemachineCore.Stage.Body && this.m_AvoidObstacles)
      {
        Vector3 vector3_1 = this.PreserveLignOfSight(ref state, ref extra);
        if ((double) this.m_Damping > 0.0 && (double) deltaTime >= 0.0)
        {
          Vector3 vector3_2 = Damper.Damp(vector3_1 - extra.m_previousDisplacement, this.m_Damping, deltaTime);
          vector3_1 = extra.m_previousDisplacement + vector3_2;
        }
        extra.m_previousDisplacement = vector3_1;
        state.PositionCorrection += vector3_1;
        extra.colliderDisplacement += vector3_1.magnitude;
      }
      if (stage != CinemachineCore.Stage.Aim)
        return;
      CinemachineCollider.VcamExtraState extraState = this.GetExtraState<CinemachineCollider.VcamExtraState>((ICinemachineCamera) vcam);
      extraState.targetObscured = this.CheckForTargetObstructions(state);
      if (extraState.targetObscured)
        state.ShotQuality *= 0.2f;
      if ((double) extraState.colliderDisplacement > 0.0)
        state.ShotQuality *= 0.8f;
      float num1 = 0.0f;
      if ((double) this.m_OptimalTargetDistance > 0.0 && state.HasLookAt)
      {
        float num2 = Vector3.Magnitude(state.ReferenceLookAt - state.FinalPosition);
        if ((double) num2 <= (double) this.m_OptimalTargetDistance)
        {
          float num3 = this.m_OptimalTargetDistance / 2f;
          if ((double) num2 >= (double) num3)
            num1 = (float) (0.20000000298023224 * ((double) num2 - (double) num3) / ((double) this.m_OptimalTargetDistance - (double) num3));
        }
        else
        {
          float num4 = num2 - this.m_OptimalTargetDistance;
          float num5 = this.m_OptimalTargetDistance * 3f;
          if ((double) num4 < (double) num5)
            num1 = (float) (0.20000000298023224 * (1.0 - (double) num4 / (double) num5));
        }
        state.ShotQuality *= 1f + num1;
      }
    }

    private Vector3 PreserveLignOfSight(
      ref CameraState state,
      ref CinemachineCollider.VcamExtraState extra)
    {
      Vector3 vector3_1 = Vector3.zero;
      if (state.HasLookAt)
      {
        Vector3 correctedPosition = state.CorrectedPosition;
        Vector3 referenceLookAt = state.ReferenceLookAt;
        Vector3 vector3_2 = correctedPosition;
        Vector3 vector3_3 = vector3_2 - referenceLookAt;
        float magnitude = vector3_3.magnitude;
        float num = Mathf.Max(this.m_MinimumDistanceFromTarget, 0.0001f);
        if ((double) magnitude > (double) num)
        {
          vector3_3.Normalize();
          float b = magnitude - num;
          if ((double) this.m_DistanceLimit > 9.9999997473787516E-05)
            b = Mathf.Min(this.m_DistanceLimit, b);
          Ray ray = new Ray(vector3_2 - b * vector3_3, vector3_3);
          float rayLength = b + 1f / 1000f;
          RaycastHit hitInfo;
          if ((double) rayLength > 9.9999997473787516E-05 && this.RaycastIgnoreTag(ray, out hitInfo, rayLength))
          {
            float distance = Mathf.Max(0.0f, hitInfo.distance - 1f / 1000f);
            vector3_2 = ray.GetPoint(distance);
            extra.AddPointToDebugPath(vector3_2);
            if (this.m_Strategy != 0)
              vector3_2 = this.PushCameraBack(vector3_2, vector3_3, hitInfo, referenceLookAt, new Plane(state.ReferenceUp, correctedPosition), magnitude, this.m_MaximumEffort, ref extra);
          }
        }
        if ((double) this.m_CameraRadius > 9.9999997473787516E-05)
          vector3_2 += this.RespectCameraRadius(vector3_2, state.ReferenceLookAt);
        else if ((UnityEngine.Object) this.mCameraColliderGameObject != (UnityEngine.Object) null)
          this.CleanupCameraCollider();
        vector3_1 = vector3_2 - correctedPosition;
      }
      return vector3_1;
    }

    private bool RaycastIgnoreTag(Ray ray, out RaycastHit hitInfo, float rayLength)
    {
      while (Physics.Raycast(ray, out hitInfo, rayLength, this.m_CollideAgainst.value, QueryTriggerInteraction.Ignore))
      {
        if (this.m_IgnoreTag.Length == 0 || !hitInfo.collider.CompareTag(this.m_IgnoreTag))
          return true;
        Ray ray1 = new Ray(ray.GetPoint(rayLength), -ray.direction);
        if (hitInfo.collider.Raycast(ray1, out hitInfo, rayLength))
        {
          rayLength = hitInfo.distance - 1f / 1000f;
          if ((double) rayLength >= 9.9999997473787516E-05)
            ray.origin = ray1.GetPoint(rayLength);
          else
            break;
        }
        else
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
      ref CinemachineCollider.VcamExtraState extra)
    {
      Vector3 vector3_1 = currentPos;
      Vector3 zero = Vector3.zero;
      if (!this.GetWalkingDirection(vector3_1, pushDir, obstacle, ref zero))
        return vector3_1;
      Ray ray = new Ray(vector3_1, zero);
      float pushBackDistance1 = this.GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
      if ((double) pushBackDistance1 <= 9.9999997473787516E-05)
        return vector3_1;
      float bounds = this.ClampRayToBounds(ray, pushBackDistance1, obstacle.collider.bounds);
      float num = Mathf.Min(pushBackDistance1, bounds + 1f / 1000f);
      RaycastHit hitInfo;
      if (this.RaycastIgnoreTag(ray, out hitInfo, num))
      {
        float distance = hitInfo.distance - 1f / 1000f;
        Vector3 vector3_2 = ray.GetPoint(distance);
        extra.AddPointToDebugPath(vector3_2);
        if (iterations > 1)
          vector3_2 = this.PushCameraBack(vector3_2, zero, hitInfo, lookAtPos, startPlane, targetDistance, iterations - 1, ref extra);
        return vector3_2;
      }
      Vector3 vector3_3 = ray.GetPoint(num);
      Vector3 vector3_4 = vector3_3 - lookAtPos;
      float magnitude = vector3_4.magnitude;
      if ((double) magnitude < 9.9999997473787516E-05 || this.RaycastIgnoreTag(new Ray(lookAtPos, vector3_4), out RaycastHit _, magnitude - 1f / 1000f))
        return currentPos;
      ray = new Ray(vector3_3, vector3_4);
      extra.AddPointToDebugPath(vector3_3);
      float pushBackDistance2 = this.GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
      if ((double) pushBackDistance2 > 9.9999997473787516E-05)
      {
        if (!this.RaycastIgnoreTag(ray, out hitInfo, pushBackDistance2))
        {
          vector3_3 = ray.GetPoint(pushBackDistance2);
          extra.AddPointToDebugPath(vector3_3);
        }
        else
        {
          float distance = hitInfo.distance - 1f / 1000f;
          vector3_3 = ray.GetPoint(distance);
          extra.AddPointToDebugPath(vector3_3);
          if (iterations > 1)
            vector3_3 = this.PushCameraBack(vector3_3, vector3_4, hitInfo, lookAtPos, startPlane, targetDistance, iterations - 1, ref extra);
        }
      }
      return vector3_3;
    }

    private bool GetWalkingDirection(
      Vector3 pos,
      Vector3 pushDir,
      RaycastHit obstacle,
      ref Vector3 outDir)
    {
      Vector3 normal = obstacle.normal;
      float num1 = 0.00500000035f;
      int num2 = Physics.SphereCastNonAlloc(pos, num1, pushDir.normalized, this.m_CornerBuffer, 0.0f, this.m_CollideAgainst.value, QueryTriggerInteraction.Ignore);
      if (num2 > 1)
      {
        for (int index = 0; index < num2; ++index)
        {
          if (this.m_IgnoreTag.Length <= 0 || !this.m_CornerBuffer[index].collider.CompareTag(this.m_IgnoreTag))
          {
            System.Type type = ((object) this.m_CornerBuffer[index].collider).GetType();
            if (type == typeof (BoxCollider) || type == typeof (SphereCollider) || type == typeof (CapsuleCollider))
            {
              Vector3 direction = this.m_CornerBuffer[index].collider.ClosestPoint(pos) - pos;
              if ((double) direction.magnitude > 9.9999997473787516E-06 && this.m_CornerBuffer[index].collider.Raycast(new Ray(pos, direction), out this.m_CornerBuffer[index], num1))
              {
                if (!(this.m_CornerBuffer[index].normal - obstacle.normal).AlmostZero())
                {
                  normal = this.m_CornerBuffer[index].normal;
                  break;
                }
                break;
              }
            }
          }
        }
      }
      Vector3 vector3 = Vector3.Cross(obstacle.normal, normal);
      if (vector3.AlmostZero())
      {
        vector3 = Vector3.ProjectOnPlane(pushDir, obstacle.normal);
      }
      else
      {
        float f = Vector3.Dot(vector3, pushDir);
        if ((double) Mathf.Abs(f) < 9.9999997473787516E-05)
          return false;
        if ((double) f < 0.0)
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
      Vector3 lookAtPos)
    {
      float a = targetDistance - (ray.origin - lookAtPos).magnitude;
      if ((double) a < 9.9999997473787516E-05)
        return 0.0f;
      if (this.m_Strategy == CinemachineCollider.ResolutionStrategy.PreserveCameraDistance)
        return a;
      float enter;
      if (!startPlane.Raycast(ray, out enter))
        enter = 0.0f;
      float b = Mathf.Min(a, enter);
      if ((double) b < 9.9999997473787516E-05)
        return 0.0f;
      float num = Mathf.Abs(Vector3.Angle(startPlane.normal, ray.direction) - 90f);
      if ((double) num < 0.10000000149011612)
        b = Mathf.Lerp(0.0f, b, num / 0.1f);
      return b;
    }

    private float ClampRayToBounds(Ray ray, float distance, Bounds bounds)
    {
      float enter;
      if ((double) Vector3.Dot(ray.direction, Vector3.up) > 0.0)
      {
        if (new Plane(Vector3.down, bounds.max).Raycast(ray, out enter) && (double) enter > 9.9999997473787516E-05)
          distance = Mathf.Min(distance, enter);
      }
      else if ((double) Vector3.Dot(ray.direction, Vector3.down) > 0.0 && new Plane(Vector3.up, bounds.min).Raycast(ray, out enter) && (double) enter > 9.9999997473787516E-05)
        distance = Mathf.Min(distance, enter);
      if ((double) Vector3.Dot(ray.direction, Vector3.right) > 0.0)
      {
        if (new Plane(Vector3.left, bounds.max).Raycast(ray, out enter) && (double) enter > 9.9999997473787516E-05)
          distance = Mathf.Min(distance, enter);
      }
      else if ((double) Vector3.Dot(ray.direction, Vector3.left) > 0.0 && new Plane(Vector3.right, bounds.min).Raycast(ray, out enter) && (double) enter > 9.9999997473787516E-05)
        distance = Mathf.Min(distance, enter);
      if ((double) Vector3.Dot(ray.direction, Vector3.forward) > 0.0)
      {
        if (new Plane(Vector3.back, bounds.max).Raycast(ray, out enter) && (double) enter > 9.9999997473787516E-05)
          distance = Mathf.Min(distance, enter);
      }
      else if ((double) Vector3.Dot(ray.direction, Vector3.back) > 0.0 && new Plane(Vector3.forward, bounds.min).Raycast(ray, out enter) && (double) enter > 9.9999997473787516E-05)
        distance = Mathf.Min(distance, enter);
      return distance;
    }

    private Vector3 RespectCameraRadius(Vector3 cameraPos, Vector3 lookAtPos)
    {
      Vector3 zero = Vector3.zero;
      int num = Physics.OverlapSphereNonAlloc(cameraPos, this.m_CameraRadius, this.mColliderBuffer, (int) this.m_CollideAgainst, QueryTriggerInteraction.Ignore);
      if (num > 0)
      {
        if ((UnityEngine.Object) this.mCameraColliderGameObject == (UnityEngine.Object) null)
        {
          this.mCameraColliderGameObject = new GameObject("Cinemachine Collider Collider");
          this.mCameraColliderGameObject.hideFlags = HideFlags.HideAndDontSave;
          this.mCameraColliderGameObject.transform.position = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
          this.mCameraColliderGameObject.SetActive(true);
          this.mCameraCollider = this.mCameraColliderGameObject.AddComponent<SphereCollider>();
        }
        this.mCameraCollider.radius = this.m_CameraRadius;
        for (int index = 0; index < num; ++index)
        {
          Collider colliderB = this.mColliderBuffer[index];
          Vector3 direction;
          float distance;
          if ((this.m_IgnoreTag.Length <= 0 || !colliderB.CompareTag(this.m_IgnoreTag)) && Physics.ComputePenetration((Collider) this.mCameraCollider, cameraPos, Quaternion.identity, colliderB, colliderB.transform.position, colliderB.transform.rotation, out direction, out distance))
            zero += direction * distance;
        }
      }
      return zero;
    }

    private void CleanupCameraCollider()
    {
      if ((UnityEngine.Object) this.mCameraColliderGameObject != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.mCameraColliderGameObject);
      this.mCameraColliderGameObject = (GameObject) null;
      this.mCameraCollider = (SphereCollider) null;
    }

    private bool CheckForTargetObstructions(CameraState state)
    {
      if (state.HasLookAt)
      {
        Vector3 referenceLookAt = state.ReferenceLookAt;
        Vector3 correctedPosition = state.CorrectedPosition;
        Vector3 vector3 = referenceLookAt - correctedPosition;
        float magnitude = vector3.magnitude;
        if ((double) magnitude < (double) Mathf.Max(this.m_MinimumDistanceFromTarget, 0.0001f) || this.RaycastIgnoreTag(new Ray(correctedPosition, vector3.normalized), out RaycastHit _, magnitude - this.m_MinimumDistanceFromTarget))
          return true;
      }
      return false;
    }

    public enum ResolutionStrategy
    {
      PullCameraForward,
      PreserveCameraHeight,
      PreserveCameraDistance,
    }

    private class VcamExtraState
    {
      public Vector3 m_previousDisplacement;
      public float colliderDisplacement;
      public bool targetObscured;
      public List<Vector3> debugResolutionPath;

      public void AddPointToDebugPath(Vector3 p)
      {
      }
    }
  }
}
