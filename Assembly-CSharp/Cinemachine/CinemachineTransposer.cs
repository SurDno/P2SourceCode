using Cinemachine.Utility;
using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(5f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineTransposer : CinemachineComponentBase
  {
    [Tooltip("The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.")]
    public BindingMode m_BindingMode = BindingMode.LockToTargetWithWorldUp;
    [Tooltip("The distance vector that the transposer will attempt to maintain from the Follow target")]
    public Vector3 m_FollowOffset = Vector3.back * 10f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain the offset in the X-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_XDamping = 1f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain the offset in the Y-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_YDamping = 1f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain the offset in the Z-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_ZDamping = 1f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to track the target rotation's X angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_PitchDamping;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to track the target rotation's Y angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_YawDamping;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to track the target rotation's Z angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_RollDamping;
    private Vector3 m_PreviousTargetPosition = Vector3.zero;
    private Quaternion m_PreviousReferenceOrientation = Quaternion.identity;
    private Quaternion m_targetOrientationOnAssign = Quaternion.identity;
    private Transform m_previousTarget;

    protected virtual void OnValidate() => m_FollowOffset = EffectiveOffset;

    protected Vector3 EffectiveOffset
    {
      get
      {
        Vector3 followOffset = m_FollowOffset;
        if (m_BindingMode == BindingMode.SimpleFollowWithWorldUp)
        {
          followOffset.x = 0.0f;
          followOffset.z = -Mathf.Abs(followOffset.z);
        }
        return followOffset;
      }
    }

    public override bool IsValid => enabled && FollowTarget != null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Body;

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      InitPrevFrameStateInfo(ref curState, deltaTime);
      if (!IsValid)
        return;
      Vector3 effectiveOffset = EffectiveOffset;
      TrackTarget(deltaTime, curState.ReferenceUp, effectiveOffset, out Vector3 outTargetPosition, out Quaternion outTargetOrient);
      curState.RawPosition = outTargetPosition + outTargetOrient * effectiveOffset;
      curState.ReferenceUp = outTargetOrient * Vector3.up;
    }

    public override void OnPositionDragged(Vector3 delta)
    {
      m_FollowOffset += Quaternion.Inverse(GetReferenceOrientation(VcamState.ReferenceUp)) * delta;
      m_FollowOffset = EffectiveOffset;
    }

    protected void InitPrevFrameStateInfo(ref CameraState curState, float deltaTime)
    {
      if (m_previousTarget != FollowTarget || deltaTime < 0.0)
      {
        m_previousTarget = FollowTarget;
        m_targetOrientationOnAssign = m_previousTarget == null ? Quaternion.identity : FollowTarget.rotation;
      }
      if (deltaTime >= 0.0)
        return;
      m_PreviousTargetPosition = curState.RawPosition;
      m_PreviousReferenceOrientation = GetReferenceOrientation(curState.ReferenceUp);
    }

    protected void TrackTarget(
      float deltaTime,
      Vector3 up,
      Vector3 desiredCameraOffset,
      out Vector3 outTargetPosition,
      out Quaternion outTargetOrient)
    {
      Quaternion referenceOrientation = GetReferenceOrientation(up);
      Quaternion quaternion = referenceOrientation;
      if (deltaTime >= 0.0)
      {
        Vector3 vector3 = (Quaternion.Inverse(m_PreviousReferenceOrientation) * referenceOrientation).eulerAngles;
        for (int index = 0; index < 3; ++index)
        {
          if (vector3[index] > 180.0)
            vector3[index] -= 360f;
        }
        vector3 = Damper.Damp(vector3, AngularDamping, deltaTime);
        quaternion = m_PreviousReferenceOrientation * Quaternion.Euler(vector3);
      }
      m_PreviousReferenceOrientation = quaternion;
      Vector3 position = FollowTarget.position;
      Vector3 previousTargetPosition = m_PreviousTargetPosition;
      Vector3 vector3_1 = position - previousTargetPosition;
      if (deltaTime >= 0.0)
      {
        Quaternion rotation = !desiredCameraOffset.AlmostZero() ? Quaternion.LookRotation(quaternion * desiredCameraOffset.normalized, up) : VcamState.RawOrientation;
        Vector3 vector3_2 = Damper.Damp(Quaternion.Inverse(rotation) * vector3_1, Damping, deltaTime);
        vector3_1 = rotation * vector3_2;
      }
      outTargetPosition = m_PreviousTargetPosition = previousTargetPosition + vector3_1;
      outTargetOrient = quaternion;
    }

    protected Vector3 Damping => m_BindingMode == BindingMode.SimpleFollowWithWorldUp ? new Vector3(0.0f, m_YDamping, m_ZDamping) : new Vector3(m_XDamping, m_YDamping, m_ZDamping);

    protected Vector3 AngularDamping
    {
      get
      {
        switch (m_BindingMode)
        {
          case BindingMode.LockToTargetOnAssign:
          case BindingMode.WorldSpace:
          case BindingMode.SimpleFollowWithWorldUp:
            return Vector3.zero;
          case BindingMode.LockToTargetWithWorldUp:
            return new Vector3(0.0f, m_YawDamping, 0.0f);
          case BindingMode.LockToTargetNoRoll:
            return new Vector3(m_PitchDamping, m_YawDamping, 0.0f);
          default:
            return new Vector3(m_PitchDamping, m_YawDamping, m_RollDamping);
        }
      }
    }

    public Vector3 GeTargetCameraPosition(Vector3 worldUp)
    {
      return !IsValid ? Vector3.zero : FollowTarget.position + GetReferenceOrientation(worldUp) * EffectiveOffset;
    }

    public Quaternion GetReferenceOrientation(Vector3 worldUp)
    {
      if (FollowTarget != null)
      {
        Quaternion rotation = FollowTarget.rotation;
        switch (m_BindingMode)
        {
          case BindingMode.LockToTargetOnAssign:
            return m_targetOrientationOnAssign;
          case BindingMode.LockToTargetWithWorldUp:
            return Uppify(rotation, worldUp);
          case BindingMode.LockToTargetNoRoll:
            return Quaternion.LookRotation(rotation * Vector3.forward, worldUp);
          case BindingMode.LockToTarget:
            return rotation;
          case BindingMode.SimpleFollowWithWorldUp:
            Vector3 vector3 = FollowTarget.position - VcamState.RawPosition;
            if (!vector3.AlmostZero())
              return Uppify(Quaternion.LookRotation(vector3, worldUp), worldUp);
            break;
        }
      }
      return Quaternion.identity;
    }

    private static Quaternion Uppify(Quaternion q, Vector3 up)
    {
      return Quaternion.FromToRotation(q * Vector3.up, up) * q;
    }

    [DocumentationSorting(5.01f, DocumentationSortingAttribute.Level.UserRef)]
    public enum BindingMode
    {
      LockToTargetOnAssign,
      LockToTargetWithWorldUp,
      LockToTargetNoRoll,
      LockToTarget,
      WorldSpace,
      SimpleFollowWithWorldUp,
    }
  }
}
