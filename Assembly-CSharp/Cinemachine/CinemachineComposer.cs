// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineComposer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using System;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(3f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineComposer : CinemachineComponentBase
  {
    [NoSaveDuringPlay]
    [HideInInspector]
    public Action OnGUICallback = (Action) null;
    [Tooltip("Target offset from the target object's center in target-local space. Use this to fine-tune the tracking target position when the desired area is not the tracked object's center.")]
    public Vector3 m_TrackedObjectOffset = Vector3.zero;
    [Tooltip("This setting will instruct the composer to adjust its target offset based on the motion of the target.  The composer will look at a point where it estimates the target will be this many seconds into the future.  Note that this setting is sensitive to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly.")]
    [Range(0.0f, 1f)]
    public float m_LookaheadTime = 0.0f;
    [Tooltip("Controls the smoothness of the lookahead algorithm.  Larger values smooth out jittery predictions and also increase prediction lag")]
    [Range(3f, 30f)]
    public float m_LookaheadSmoothing = 10f;
    [Space]
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to follow the target in the screen-horizontal direction. Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical and horizontal settings can yield a wide range of camera behaviors.")]
    public float m_HorizontalDamping = 0.5f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to follow the target in the screen-vertical direction. Small numbers are more responsive, rapidly orienting the camera to keep the target in the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical and horizontal settings can yield a wide range of camera behaviors.")]
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
    [Range(0.0f, 1f)]
    [Tooltip("Camera will not rotate vertically if the target is within this range of the position.")]
    public float m_DeadZoneHeight = 0.1f;
    [Range(0.0f, 2f)]
    [Tooltip("When target is within this region, camera will gradually rotate horizontally to re-align towards the desired position, depending on the damping speed.")]
    public float m_SoftZoneWidth = 0.8f;
    [Range(0.0f, 2f)]
    [Tooltip("When target is within this region, camera will gradually rotate vertically to re-align towards the desired position, depending on the damping speed.")]
    public float m_SoftZoneHeight = 0.8f;
    [Range(-0.5f, 0.5f)]
    [Tooltip("A non-zero bias will move the target position horizontally away from the center of the soft zone.")]
    public float m_BiasX = 0.0f;
    [Range(-0.5f, 0.5f)]
    [Tooltip("A non-zero bias will move the target position vertically away from the center of the soft zone.")]
    public float m_BiasY = 0.0f;
    private Vector3 m_CameraPosPrevFrame = Vector3.zero;
    private Vector3 m_LookAtPrevFrame = Vector3.zero;
    private Vector2 m_ScreenOffsetPrevFrame = Vector2.zero;
    private Quaternion m_CameraOrientationPrevFrame = Quaternion.identity;
    private PositionPredictor m_Predictor = new PositionPredictor();

    public override bool IsValid => this.enabled && (UnityEngine.Object) this.LookAtTarget != (UnityEngine.Object) null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

    public Vector3 TrackedPoint { get; private set; }

    protected virtual Vector3 GetLookAtPointAndSetTrackedPoint(Vector3 lookAt)
    {
      Vector3 pos = lookAt;
      if ((UnityEngine.Object) this.LookAtTarget != (UnityEngine.Object) null)
        pos += this.LookAtTarget.transform.rotation * this.m_TrackedObjectOffset;
      this.m_Predictor.Smoothing = this.m_LookaheadSmoothing;
      this.m_Predictor.AddPosition(pos);
      this.TrackedPoint = (double) this.m_LookaheadTime > 0.0 ? this.m_Predictor.PredictPosition(this.m_LookaheadTime) : pos;
      return pos;
    }

    public override void PrePipelineMutateCameraState(ref CameraState curState)
    {
      if (!this.IsValid || !curState.HasLookAt)
        return;
      curState.ReferenceLookAt = this.GetLookAtPointAndSetTrackedPoint(curState.ReferenceLookAt);
    }

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      if ((double) deltaTime < 0.0)
        this.m_Predictor.Reset();
      if (!this.IsValid || !curState.HasLookAt)
        return;
      float magnitude = (this.TrackedPoint - curState.CorrectedPosition).magnitude;
      if ((double) magnitude < 9.9999997473787516E-05)
      {
        if ((double) deltaTime < 0.0)
          return;
        curState.RawOrientation = this.m_CameraOrientationPrevFrame;
      }
      else
      {
        float fov1;
        LensSettings lens;
        float fovH1;
        if (curState.Lens.Orthographic)
        {
          fov1 = 114.59156f * Mathf.Atan(curState.Lens.OrthographicSize / magnitude);
          lens = curState.Lens;
          fovH1 = (float) (114.59156036376953 * (double) Mathf.Atan(lens.Aspect * curState.Lens.OrthographicSize / magnitude));
        }
        else
        {
          fov1 = curState.Lens.FieldOfView;
          fovH1 = (float) (57.295780181884766 * (2.0 * Math.Atan(Math.Tan((double) fov1 * (Math.PI / 180.0) / 2.0) * (double) curState.Lens.Aspect)));
        }
        Quaternion rigOrientation = curState.RawOrientation;
        Rect softGuideRect = this.SoftGuideRect;
        double fov2 = (double) fov1;
        double fovH2 = (double) fovH1;
        lens = curState.Lens;
        double aspect1 = (double) lens.Aspect;
        Rect fov3 = this.ScreenToFOV(softGuideRect, (float) fov2, (float) fovH2, (float) aspect1);
        if ((double) deltaTime < 0.0)
        {
          Rect screenRect = new Rect(fov3.center, Vector2.zero);
          this.RotateToScreenBounds(ref curState, screenRect, ref rigOrientation, fov1, fovH1, -1f);
        }
        else
        {
          Vector3 vector3 = this.m_LookAtPrevFrame - (this.m_CameraPosPrevFrame + curState.PositionDampingBypass);
          rigOrientation = !vector3.AlmostZero() ? Quaternion.LookRotation(vector3, curState.ReferenceUp).ApplyCameraRotation(-this.m_ScreenOffsetPrevFrame, curState.ReferenceUp) : Quaternion.LookRotation(this.m_CameraOrientationPrevFrame * Vector3.forward, curState.ReferenceUp);
          Rect hardGuideRect = this.HardGuideRect;
          double fov4 = (double) fov1;
          double fovH3 = (double) fovH1;
          lens = curState.Lens;
          double aspect2 = (double) lens.Aspect;
          Rect fov5 = this.ScreenToFOV(hardGuideRect, (float) fov4, (float) fovH3, (float) aspect2);
          if (!this.RotateToScreenBounds(ref curState, fov5, ref rigOrientation, fov1, fovH1, -1f))
            this.RotateToScreenBounds(ref curState, fov3, ref rigOrientation, fov1, fovH1, deltaTime);
        }
        this.m_CameraPosPrevFrame = curState.CorrectedPosition;
        this.m_LookAtPrevFrame = this.TrackedPoint;
        this.m_CameraOrientationPrevFrame = rigOrientation.Normalized();
        this.m_ScreenOffsetPrevFrame = this.m_CameraOrientationPrevFrame.GetCameraRotationToTarget(this.m_LookAtPrevFrame - curState.CorrectedPosition, curState.ReferenceUp);
        curState.RawOrientation = this.m_CameraOrientationPrevFrame;
      }
    }

    public Rect SoftGuideRect
    {
      get
      {
        return new Rect(this.m_ScreenX - this.m_DeadZoneWidth / 2f, this.m_ScreenY - this.m_DeadZoneHeight / 2f, this.m_DeadZoneWidth, this.m_DeadZoneHeight);
      }
      set
      {
        this.m_DeadZoneWidth = Mathf.Clamp01(value.width);
        this.m_DeadZoneHeight = Mathf.Clamp01(value.height);
        this.m_ScreenX = Mathf.Clamp01(value.x + this.m_DeadZoneWidth / 2f);
        this.m_ScreenY = Mathf.Clamp01(value.y + this.m_DeadZoneHeight / 2f);
        this.m_SoftZoneWidth = Mathf.Max(this.m_SoftZoneWidth, this.m_DeadZoneWidth);
        this.m_SoftZoneHeight = Mathf.Max(this.m_SoftZoneHeight, this.m_DeadZoneHeight);
      }
    }

    public Rect HardGuideRect
    {
      get
      {
        Rect hardGuideRect = new Rect(this.m_ScreenX - this.m_SoftZoneWidth / 2f, this.m_ScreenY - this.m_SoftZoneHeight / 2f, this.m_SoftZoneWidth, this.m_SoftZoneHeight);
        hardGuideRect.position += new Vector2(this.m_BiasX * (this.m_SoftZoneWidth - this.m_DeadZoneWidth), this.m_BiasY * (this.m_SoftZoneHeight - this.m_DeadZoneHeight));
        return hardGuideRect;
      }
      set
      {
        this.m_SoftZoneWidth = Mathf.Clamp(value.width, 0.0f, 2f);
        this.m_SoftZoneHeight = Mathf.Clamp(value.height, 0.0f, 2f);
        this.m_DeadZoneWidth = Mathf.Min(this.m_DeadZoneWidth, this.m_SoftZoneWidth);
        this.m_DeadZoneHeight = Mathf.Min(this.m_DeadZoneHeight, this.m_SoftZoneHeight);
        Vector2 vector2 = value.center - new Vector2(this.m_ScreenX, this.m_ScreenY);
        float num1 = Mathf.Max(0.0f, this.m_SoftZoneWidth - this.m_DeadZoneWidth);
        float num2 = Mathf.Max(0.0f, this.m_SoftZoneHeight - this.m_DeadZoneHeight);
        this.m_BiasX = (double) num1 < 9.9999997473787516E-05 ? 0.0f : Mathf.Clamp(vector2.x / num1, -0.5f, 0.5f);
        this.m_BiasY = (double) num2 < 9.9999997473787516E-05 ? 0.0f : Mathf.Clamp(vector2.y / num2, -0.5f, 0.5f);
      }
    }

    private Rect ScreenToFOV(Rect rScreen, float fov, float fovH, float aspect)
    {
      Rect fov1 = new Rect(rScreen);
      Matrix4x4 inverse = Matrix4x4.Perspective(fov, aspect, 0.01f, 10000f).inverse;
      Vector3 to1 = inverse.MultiplyPoint(new Vector3(0.0f, (float) ((double) fov1.yMin * 2.0 - 1.0), 0.1f));
      to1.z = -to1.z;
      float num1 = UnityVectorExtensions.SignedAngle(Vector3.forward, to1, Vector3.left);
      fov1.yMin = (fov / 2f + num1) / fov;
      Vector3 to2 = inverse.MultiplyPoint(new Vector3(0.0f, (float) ((double) fov1.yMax * 2.0 - 1.0), 0.1f));
      to2.z = -to2.z;
      float num2 = UnityVectorExtensions.SignedAngle(Vector3.forward, to2, Vector3.left);
      fov1.yMax = (fov / 2f + num2) / fov;
      Vector3 to3 = inverse.MultiplyPoint(new Vector3((float) ((double) fov1.xMin * 2.0 - 1.0), 0.0f, 0.1f));
      to3.z = -to3.z;
      float num3 = UnityVectorExtensions.SignedAngle(Vector3.forward, to3, Vector3.up);
      fov1.xMin = (fovH / 2f + num3) / fovH;
      Vector3 to4 = inverse.MultiplyPoint(new Vector3((float) ((double) fov1.xMax * 2.0 - 1.0), 0.0f, 0.1f));
      to4.z = -to4.z;
      float num4 = UnityVectorExtensions.SignedAngle(Vector3.forward, to4, Vector3.up);
      fov1.xMax = (fovH / 2f + num4) / fovH;
      return fov1;
    }

    private bool RotateToScreenBounds(
      ref CameraState state,
      Rect screenRect,
      ref Quaternion rigOrientation,
      float fov,
      float fovH,
      float deltaTime)
    {
      Vector3 vector3 = this.TrackedPoint - state.CorrectedPosition;
      Vector2 rotationToTarget = rigOrientation.GetCameraRotationToTarget(vector3, state.ReferenceUp);
      this.ClampVerticalBounds(ref screenRect, vector3, state.ReferenceUp, fov);
      float num1 = (screenRect.yMin - 0.5f) * fov;
      float num2 = (screenRect.yMax - 0.5f) * fov;
      if ((double) rotationToTarget.x < (double) num1)
        rotationToTarget.x -= num1;
      else if ((double) rotationToTarget.x > (double) num2)
        rotationToTarget.x -= num2;
      else
        rotationToTarget.x = 0.0f;
      float num3 = (screenRect.xMin - 0.5f) * fovH;
      float num4 = (screenRect.xMax - 0.5f) * fovH;
      if ((double) rotationToTarget.y < (double) num3)
        rotationToTarget.y -= num3;
      else if ((double) rotationToTarget.y > (double) num4)
        rotationToTarget.y -= num4;
      else
        rotationToTarget.y = 0.0f;
      if ((double) deltaTime >= 0.0)
      {
        rotationToTarget.x = Damper.Damp(rotationToTarget.x, this.m_VerticalDamping, deltaTime);
        rotationToTarget.y = Damper.Damp(rotationToTarget.y, this.m_HorizontalDamping, deltaTime);
      }
      rigOrientation = rigOrientation.ApplyCameraRotation(rotationToTarget, state.ReferenceUp);
      return false;
    }

    private bool ClampVerticalBounds(ref Rect r, Vector3 dir, Vector3 up, float fov)
    {
      float num1 = Vector3.Angle(dir, up);
      float num2 = (float) ((double) fov / 2.0 + 1.0);
      if ((double) num1 < (double) num2)
      {
        float b = (float) (1.0 - ((double) num2 - (double) num1) / (double) fov);
        if ((double) r.yMax > (double) b)
        {
          r.yMin = Mathf.Min(r.yMin, b);
          r.yMax = Mathf.Min(r.yMax, b);
          return true;
        }
      }
      if ((double) num1 > 180.0 - (double) num2)
      {
        float b = (num1 - (180f - num2)) / fov;
        if ((double) b > (double) r.yMin)
        {
          r.yMin = Mathf.Max(r.yMin, b);
          r.yMax = Mathf.Max(r.yMax, b);
          return true;
        }
      }
      return false;
    }
  }
}
