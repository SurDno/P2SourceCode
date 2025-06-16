// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineTrackedDolly
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(7f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineTrackedDolly : CinemachineComponentBase
  {
    [Tooltip("The path to which the camera will be constrained.  This must be non-null.")]
    public CinemachinePathBase m_Path;
    [Tooltip("The position along the path at which the camera will be placed.  This can be animated directly, or set automatically by the Auto-Dolly feature to get as close as possible to the Follow target.  The value is interpreted according to the Position Units setting.")]
    public float m_PathPosition;
    [Tooltip("How to interpret Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
    public CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;
    [Tooltip("Where to put the camera relative to the path position.  X is perpendicular to the path, Y is up, and Z is parallel to the path.  This allows the camera to be offset from the path itself (as if on a tripod, for example).")]
    public Vector3 m_PathOffset = Vector3.zero;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain its position in a direction perpendicular to the path.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_XDamping = 0.0f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain its position in the path-local up direction.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_YDamping = 0.0f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain its position in a direction parallel to the path.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_ZDamping = 1f;
    [Tooltip("How to set the virtual camera's Up vector.  This will affect the screen composition, because the camera Aim behaviours will always try to respect the Up direction.")]
    public CinemachineTrackedDolly.CameraUpMode m_CameraUp = CinemachineTrackedDolly.CameraUpMode.Default;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to track the target rotation's X angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_PitchDamping = 0.0f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to track the target rotation's Y angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_YawDamping = 0.0f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to track the target rotation's Z angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_RollDamping = 0.0f;
    [Tooltip("Controls how automatic dollying occurs.  A Follow target is necessary to use this feature.")]
    public CinemachineTrackedDolly.AutoDolly m_AutoDolly = new CinemachineTrackedDolly.AutoDolly(false, 0.0f, 2, 5);
    private float m_PreviousPathPosition = 0.0f;
    private Quaternion m_PreviousOrientation = Quaternion.identity;
    private Vector3 m_PreviousCameraPosition = Vector3.zero;

    public override bool IsValid => this.enabled && (UnityEngine.Object) this.m_Path != (UnityEngine.Object) null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Body;

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      if ((double) deltaTime < 0.0)
      {
        this.m_PreviousPathPosition = this.m_PathPosition;
        this.m_PreviousCameraPosition = curState.RawPosition;
      }
      if (!this.IsValid)
        return;
      if (this.m_AutoDolly.m_Enabled && (UnityEngine.Object) this.FollowTarget != (UnityEngine.Object) null)
      {
        float num = this.m_PreviousPathPosition;
        if (this.m_PositionUnits == CinemachinePathBase.PositionUnits.Distance)
          num = this.m_Path.GetPathPositionFromDistance(num);
        this.m_PathPosition = this.m_Path.FindClosestPoint(this.FollowTarget.transform.position, Mathf.FloorToInt(num), (double) deltaTime < 0.0 || this.m_AutoDolly.m_SearchRadius <= 0 ? -1 : this.m_AutoDolly.m_SearchRadius, this.m_AutoDolly.m_SearchResolution);
        if (this.m_PositionUnits == CinemachinePathBase.PositionUnits.Distance)
          this.m_PathPosition = this.m_Path.GetPathDistanceFromPosition(this.m_PathPosition);
        this.m_PathPosition += this.m_AutoDolly.m_PositionOffset;
      }
      float pos = this.m_PathPosition;
      if ((double) deltaTime >= 0.0)
      {
        float num1 = this.m_Path.MaxUnit(this.m_PositionUnits);
        if ((double) num1 > 0.0)
        {
          float num2 = this.m_Path.NormalizeUnit(this.m_PreviousPathPosition, this.m_PositionUnits);
          float num3 = this.m_Path.NormalizeUnit(pos, this.m_PositionUnits);
          if (this.m_Path.Looped && (double) Mathf.Abs(num3 - num2) > (double) num1 / 2.0)
          {
            if ((double) num3 > (double) num2)
              num2 += num1;
            else
              num2 -= num1;
          }
          this.m_PreviousPathPosition = num2;
          pos = num3;
        }
        pos = this.m_PreviousPathPosition - Damper.Damp(this.m_PreviousPathPosition - pos, this.m_ZDamping, deltaTime);
      }
      this.m_PreviousPathPosition = pos;
      Quaternion orientationAtUnit = this.m_Path.EvaluateOrientationAtUnit(pos, this.m_PositionUnits);
      Vector3 positionAtUnit = this.m_Path.EvaluatePositionAtUnit(pos, this.m_PositionUnits);
      Vector3 vector3_1 = orientationAtUnit * Vector3.right;
      Vector3 rhs = orientationAtUnit * Vector3.up;
      Vector3 vector3_2 = orientationAtUnit * Vector3.forward;
      Vector3 vector3_3 = positionAtUnit + this.m_PathOffset.x * vector3_1 + this.m_PathOffset.y * rhs + this.m_PathOffset.z * vector3_2;
      if ((double) deltaTime >= 0.0)
      {
        Vector3 previousCameraPosition = this.m_PreviousCameraPosition;
        Vector3 lhs = previousCameraPosition - vector3_3;
        Vector3 initial = Vector3.Dot(lhs, rhs) * rhs;
        Vector3 vector3_4 = Damper.Damp(lhs - initial, this.m_XDamping, deltaTime);
        Vector3 vector3_5 = Damper.Damp(initial, this.m_YDamping, deltaTime);
        vector3_3 = previousCameraPosition - (vector3_4 + vector3_5);
      }
      curState.RawPosition = this.m_PreviousCameraPosition = vector3_3;
      Quaternion quaternion = this.GetTargetOrientationAtPathPoint(orientationAtUnit, curState.ReferenceUp);
      if ((double) deltaTime < 0.0)
      {
        this.m_PreviousOrientation = quaternion;
      }
      else
      {
        if ((double) deltaTime >= 0.0)
        {
          Vector3 vector3_6 = (Quaternion.Inverse(this.m_PreviousOrientation) * quaternion).eulerAngles;
          for (int index = 0; index < 3; ++index)
          {
            if ((double) vector3_6[index] > 180.0)
              vector3_6[index] -= 360f;
          }
          vector3_6 = Damper.Damp(vector3_6, this.AngularDamping, deltaTime);
          quaternion = this.m_PreviousOrientation * Quaternion.Euler(vector3_6);
        }
        this.m_PreviousOrientation = quaternion;
      }
      curState.RawOrientation = quaternion;
      curState.ReferenceUp = curState.RawOrientation * Vector3.up;
    }

    public override void OnPositionDragged(Vector3 delta)
    {
      this.m_PathOffset += Quaternion.Inverse(this.m_Path.EvaluateOrientationAtUnit(this.m_PathPosition, this.m_PositionUnits)) * delta;
    }

    private Quaternion GetTargetOrientationAtPathPoint(Quaternion pathOrientation, Vector3 up)
    {
      switch (this.m_CameraUp)
      {
        case CinemachineTrackedDolly.CameraUpMode.Path:
          return pathOrientation;
        case CinemachineTrackedDolly.CameraUpMode.PathNoRoll:
          return Quaternion.LookRotation(pathOrientation * Vector3.forward, up);
        case CinemachineTrackedDolly.CameraUpMode.FollowTarget:
          if ((UnityEngine.Object) this.FollowTarget != (UnityEngine.Object) null)
            return this.FollowTarget.rotation;
          break;
        case CinemachineTrackedDolly.CameraUpMode.FollowTargetNoRoll:
          if ((UnityEngine.Object) this.FollowTarget != (UnityEngine.Object) null)
            return Quaternion.LookRotation(this.FollowTarget.rotation * Vector3.forward, up);
          break;
      }
      return Quaternion.LookRotation(this.transform.rotation * Vector3.forward, up);
    }

    private Vector3 AngularDamping
    {
      get
      {
        switch (this.m_CameraUp)
        {
          case CinemachineTrackedDolly.CameraUpMode.Default:
            return Vector3.zero;
          case CinemachineTrackedDolly.CameraUpMode.PathNoRoll:
          case CinemachineTrackedDolly.CameraUpMode.FollowTargetNoRoll:
            return new Vector3(this.m_PitchDamping, this.m_YawDamping, 0.0f);
          default:
            return new Vector3(this.m_PitchDamping, this.m_YawDamping, this.m_RollDamping);
        }
      }
    }

    [DocumentationSorting(7.1f, DocumentationSortingAttribute.Level.UserRef)]
    public enum CameraUpMode
    {
      Default,
      Path,
      PathNoRoll,
      FollowTarget,
      FollowTargetNoRoll,
    }

    [DocumentationSorting(7.2f, DocumentationSortingAttribute.Level.UserRef)]
    [Serializable]
    public struct AutoDolly
    {
      [Tooltip("If checked, will enable automatic dolly, which chooses a path position that is as close as possible to the Follow target.  Note: this can have significant performance impact")]
      public bool m_Enabled;
      [Tooltip("Offset, in current position units, from the closest point on the path to the follow target")]
      public float m_PositionOffset;
      [Tooltip("Search up to how many waypoints on either side of the current position.  Use 0 for Entire path.")]
      public int m_SearchRadius;
      [FormerlySerializedAs("m_StepsPerSegment")]
      [Tooltip("We search between waypoints by dividing the segment into this many straight pieces.  The higher the number, the more accurate the result, but performance is proportionally slower for higher numbers")]
      public int m_SearchResolution;

      public AutoDolly(bool enabled, float positionOffset, int searchRadius, int stepsPerSegment)
      {
        this.m_Enabled = enabled;
        this.m_PositionOffset = positionOffset;
        this.m_SearchRadius = searchRadius;
        this.m_SearchResolution = stepsPerSegment;
      }
    }
  }
}
