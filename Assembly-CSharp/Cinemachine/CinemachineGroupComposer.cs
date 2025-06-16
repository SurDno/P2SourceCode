// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineGroupComposer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using System;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(4f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineGroupComposer : CinemachineComposer
  {
    [Space]
    [Tooltip("The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc.")]
    public float m_GroupFramingSize = 0.8f;
    [Tooltip("What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both")]
    public CinemachineGroupComposer.FramingMode m_FramingMode = CinemachineGroupComposer.FramingMode.HorizontalAndVertical;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to frame the group. Small numbers are more responsive, rapidly adjusting the camera to keep the group in the frame.  Larger numbers give a more heavy slowly responding camera.")]
    public float m_FrameDamping = 2f;
    [Tooltip("How to adjust the camera to get the desired framing.  You can zoom, dolly in/out, or do both.")]
    public CinemachineGroupComposer.AdjustmentMode m_AdjustmentMode = CinemachineGroupComposer.AdjustmentMode.DollyThenZoom;
    [Tooltip("The maximum distance toward the target that this behaviour is allowed to move the camera.")]
    public float m_MaxDollyIn = 5000f;
    [Tooltip("The maximum distance away the target that this behaviour is allowed to move the camera.")]
    public float m_MaxDollyOut = 5000f;
    [Tooltip("Set this to limit how close to the target the camera can get.")]
    public float m_MinimumDistance = 1f;
    [Tooltip("Set this to limit how far from the target the camera can get.")]
    public float m_MaximumDistance = 5000f;
    [Range(1f, 179f)]
    [Tooltip("If adjusting FOV, will not set the FOV lower than this.")]
    public float m_MinimumFOV = 3f;
    [Range(1f, 179f)]
    [Tooltip("If adjusting FOV, will not set the FOV higher than this.")]
    public float m_MaximumFOV = 60f;
    [Tooltip("If adjusting Orthographic Size, will not set it lower than this.")]
    public float m_MinimumOrthoSize = 1f;
    [Tooltip("If adjusting Orthographic Size, will not set it higher than this.")]
    public float m_MaximumOrthoSize = 100f;
    private float m_prevTargetHeight;

    private void OnValidate()
    {
      this.m_GroupFramingSize = Mathf.Max(0.0001f, this.m_GroupFramingSize);
      this.m_MaxDollyIn = Mathf.Max(0.0f, this.m_MaxDollyIn);
      this.m_MaxDollyOut = Mathf.Max(0.0f, this.m_MaxDollyOut);
      this.m_MinimumDistance = Mathf.Max(0.0f, this.m_MinimumDistance);
      this.m_MaximumDistance = Mathf.Max(this.m_MinimumDistance, this.m_MaximumDistance);
      this.m_MinimumFOV = Mathf.Max(1f, this.m_MinimumFOV);
      this.m_MaximumFOV = Mathf.Clamp(this.m_MaximumFOV, this.m_MinimumFOV, 179f);
      this.m_MinimumOrthoSize = Mathf.Max(0.01f, this.m_MinimumOrthoSize);
      this.m_MaximumOrthoSize = Mathf.Max(this.m_MinimumOrthoSize, this.m_MaximumOrthoSize);
    }

    public CinemachineTargetGroup TargetGroup
    {
      get
      {
        Transform lookAtTarget = this.LookAtTarget;
        return (UnityEngine.Object) lookAtTarget != (UnityEngine.Object) null ? lookAtTarget.GetComponent<CinemachineTargetGroup>() : (CinemachineTargetGroup) null;
      }
    }

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      CinemachineTargetGroup targetGroup = this.TargetGroup;
      if ((UnityEngine.Object) targetGroup == (UnityEngine.Object) null)
        base.MutateCameraState(ref curState, deltaTime);
      else if (!this.IsValid || !curState.HasLookAt)
      {
        this.m_prevTargetHeight = 0.0f;
      }
      else
      {
        curState.ReferenceLookAt = this.GetLookAtPointAndSetTrackedPoint(targetGroup.transform.position);
        Vector3 v = this.TrackedPoint - curState.RawPosition;
        float magnitude1 = v.magnitude;
        if ((double) magnitude1 < 9.9999997473787516E-05)
          return;
        Vector3 forward = v.AlmostZero() ? Vector3.forward : v.normalized;
        Bounds boundingBox = targetGroup.BoundingBox;
        this.m_lastBoundsMatrix = Matrix4x4.TRS(boundingBox.center - forward * boundingBox.extents.magnitude, Quaternion.LookRotation(forward, curState.ReferenceUp), Vector3.one);
        this.m_LastBounds = targetGroup.GetViewSpaceBoundingBox(this.m_lastBoundsMatrix);
        float num1 = this.GetTargetHeight(this.m_LastBounds);
        Vector3 vector3 = this.m_lastBoundsMatrix.MultiplyPoint3x4(this.m_LastBounds.center);
        if ((double) deltaTime >= 0.0)
          num1 = this.m_prevTargetHeight + Damper.Damp(num1 - this.m_prevTargetHeight, this.m_FrameDamping, deltaTime);
        this.m_prevTargetHeight = num1;
        Bounds mLastBounds;
        if (!curState.Lens.Orthographic && this.m_AdjustmentMode != 0)
        {
          float fieldOfView = curState.Lens.FieldOfView;
          double num2 = (double) (num1 / (2f * Mathf.Tan((float) ((double) fieldOfView * (Math.PI / 180.0) / 2.0))));
          mLastBounds = this.m_LastBounds;
          double z = (double) mLastBounds.extents.z;
          float num3 = Mathf.Clamp(Mathf.Clamp((float) (num2 + z), magnitude1 - this.m_MaxDollyIn, magnitude1 + this.m_MaxDollyOut), this.m_MinimumDistance, this.m_MaximumDistance);
          curState.PositionCorrection += vector3 - forward * num3 - curState.RawPosition;
        }
        if (curState.Lens.Orthographic || this.m_AdjustmentMode != CinemachineGroupComposer.AdjustmentMode.DollyOnly)
        {
          double magnitude2 = (double) (this.TrackedPoint - curState.CorrectedPosition).magnitude;
          mLastBounds = this.m_LastBounds;
          double z = (double) mLastBounds.extents.z;
          float num4 = (float) (magnitude2 - z);
          float num5 = 179f;
          if ((double) num4 > 9.9999997473787516E-05)
            num5 = (float) (2.0 * (double) Mathf.Atan(num1 / (2f * num4)) * 57.295780181884766);
          LensSettings lens = curState.Lens with
          {
            FieldOfView = Mathf.Clamp(num5, this.m_MinimumFOV, this.m_MaximumFOV),
            OrthographicSize = Mathf.Clamp(num1 / 2f, this.m_MinimumOrthoSize, this.m_MaximumOrthoSize)
          };
          curState.Lens = lens;
        }
        base.MutateCameraState(ref curState, deltaTime);
      }
    }

    public Bounds m_LastBounds { get; private set; }

    public Matrix4x4 m_lastBoundsMatrix { get; private set; }

    private float GetTargetHeight(Bounds b)
    {
      float num = Mathf.Max(0.0001f, this.m_GroupFramingSize);
      switch (this.m_FramingMode)
      {
        case CinemachineGroupComposer.FramingMode.Horizontal:
          return Mathf.Max(0.0001f, b.size.x) / (num * this.VcamState.Lens.Aspect);
        case CinemachineGroupComposer.FramingMode.Vertical:
          return Mathf.Max(0.0001f, b.size.y) / num;
        default:
          return Mathf.Max(Mathf.Max(0.0001f, b.size.x) / (num * this.VcamState.Lens.Aspect), Mathf.Max(0.0001f, b.size.y) / num);
      }
    }

    [DocumentationSorting(4.01f, DocumentationSortingAttribute.Level.UserRef)]
    public enum FramingMode
    {
      Horizontal,
      Vertical,
      HorizontalAndVertical,
    }

    public enum AdjustmentMode
    {
      ZoomOnly,
      DollyOnly,
      DollyThenZoom,
    }
  }
}
