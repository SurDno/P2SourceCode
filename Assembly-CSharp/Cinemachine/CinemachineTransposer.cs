// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineTransposer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(5f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineTransposer : CinemachineComponentBase
  {
    [Tooltip("The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.")]
    public CinemachineTransposer.BindingMode m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
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
    public float m_PitchDamping = 0.0f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to track the target rotation's Y angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_YawDamping = 0.0f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to track the target rotation's Z angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_RollDamping = 0.0f;
    private Vector3 m_PreviousTargetPosition = Vector3.zero;
    private Quaternion m_PreviousReferenceOrientation = Quaternion.identity;
    private Quaternion m_targetOrientationOnAssign = Quaternion.identity;
    private Transform m_previousTarget = (Transform) null;

    protected virtual void OnValidate() => this.m_FollowOffset = this.EffectiveOffset;

    protected Vector3 EffectiveOffset
    {
      get
      {
        Vector3 followOffset = this.m_FollowOffset;
        if (this.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
        {
          followOffset.x = 0.0f;
          followOffset.z = -Mathf.Abs(followOffset.z);
        }
        return followOffset;
      }
    }

    public override bool IsValid => this.enabled && (Object) this.FollowTarget != (Object) null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Body;

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      this.InitPrevFrameStateInfo(ref curState, deltaTime);
      if (!this.IsValid)
        return;
      Vector3 effectiveOffset = this.EffectiveOffset;
      Vector3 outTargetPosition;
      Quaternion outTargetOrient;
      this.TrackTarget(deltaTime, curState.ReferenceUp, effectiveOffset, out outTargetPosition, out outTargetOrient);
      curState.RawPosition = outTargetPosition + outTargetOrient * effectiveOffset;
      curState.ReferenceUp = outTargetOrient * Vector3.up;
    }

    public override void OnPositionDragged(Vector3 delta)
    {
      this.m_FollowOffset += Quaternion.Inverse(this.GetReferenceOrientation(this.VcamState.ReferenceUp)) * delta;
      this.m_FollowOffset = this.EffectiveOffset;
    }

    protected void InitPrevFrameStateInfo(ref CameraState curState, float deltaTime)
    {
      if ((Object) this.m_previousTarget != (Object) this.FollowTarget || (double) deltaTime < 0.0)
      {
        this.m_previousTarget = this.FollowTarget;
        this.m_targetOrientationOnAssign = (Object) this.m_previousTarget == (Object) null ? Quaternion.identity : this.FollowTarget.rotation;
      }
      if ((double) deltaTime >= 0.0)
        return;
      this.m_PreviousTargetPosition = curState.RawPosition;
      this.m_PreviousReferenceOrientation = this.GetReferenceOrientation(curState.ReferenceUp);
    }

    protected void TrackTarget(
      float deltaTime,
      Vector3 up,
      Vector3 desiredCameraOffset,
      out Vector3 outTargetPosition,
      out Quaternion outTargetOrient)
    {
      Quaternion referenceOrientation = this.GetReferenceOrientation(up);
      Quaternion quaternion = referenceOrientation;
      if ((double) deltaTime >= 0.0)
      {
        Vector3 vector3 = (Quaternion.Inverse(this.m_PreviousReferenceOrientation) * referenceOrientation).eulerAngles;
        for (int index = 0; index < 3; ++index)
        {
          if ((double) vector3[index] > 180.0)
            vector3[index] -= 360f;
        }
        vector3 = Damper.Damp(vector3, this.AngularDamping, deltaTime);
        quaternion = this.m_PreviousReferenceOrientation * Quaternion.Euler(vector3);
      }
      this.m_PreviousReferenceOrientation = quaternion;
      Vector3 position = this.FollowTarget.position;
      Vector3 previousTargetPosition = this.m_PreviousTargetPosition;
      Vector3 vector3_1 = position - previousTargetPosition;
      if ((double) deltaTime >= 0.0)
      {
        Quaternion rotation = !desiredCameraOffset.AlmostZero() ? Quaternion.LookRotation(quaternion * desiredCameraOffset.normalized, up) : this.VcamState.RawOrientation;
        Vector3 vector3_2 = Damper.Damp(Quaternion.Inverse(rotation) * vector3_1, this.Damping, deltaTime);
        vector3_1 = rotation * vector3_2;
      }
      outTargetPosition = this.m_PreviousTargetPosition = previousTargetPosition + vector3_1;
      outTargetOrient = quaternion;
    }

    protected Vector3 Damping
    {
      get
      {
        return this.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp ? new Vector3(0.0f, this.m_YDamping, this.m_ZDamping) : new Vector3(this.m_XDamping, this.m_YDamping, this.m_ZDamping);
      }
    }

    protected Vector3 AngularDamping
    {
      get
      {
        switch (this.m_BindingMode)
        {
          case CinemachineTransposer.BindingMode.LockToTargetOnAssign:
          case CinemachineTransposer.BindingMode.WorldSpace:
          case CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp:
            return Vector3.zero;
          case CinemachineTransposer.BindingMode.LockToTargetWithWorldUp:
            return new Vector3(0.0f, this.m_YawDamping, 0.0f);
          case CinemachineTransposer.BindingMode.LockToTargetNoRoll:
            return new Vector3(this.m_PitchDamping, this.m_YawDamping, 0.0f);
          default:
            return new Vector3(this.m_PitchDamping, this.m_YawDamping, this.m_RollDamping);
        }
      }
    }

    public Vector3 GeTargetCameraPosition(Vector3 worldUp)
    {
      return !this.IsValid ? Vector3.zero : this.FollowTarget.position + this.GetReferenceOrientation(worldUp) * this.EffectiveOffset;
    }

    public Quaternion GetReferenceOrientation(Vector3 worldUp)
    {
      if ((Object) this.FollowTarget != (Object) null)
      {
        Quaternion rotation = this.FollowTarget.rotation;
        switch (this.m_BindingMode)
        {
          case CinemachineTransposer.BindingMode.LockToTargetOnAssign:
            return this.m_targetOrientationOnAssign;
          case CinemachineTransposer.BindingMode.LockToTargetWithWorldUp:
            return CinemachineTransposer.Uppify(rotation, worldUp);
          case CinemachineTransposer.BindingMode.LockToTargetNoRoll:
            return Quaternion.LookRotation(rotation * Vector3.forward, worldUp);
          case CinemachineTransposer.BindingMode.LockToTarget:
            return rotation;
          case CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp:
            Vector3 vector3 = this.FollowTarget.position - this.VcamState.RawPosition;
            if (!vector3.AlmostZero())
              return CinemachineTransposer.Uppify(Quaternion.LookRotation(vector3, worldUp), worldUp);
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
