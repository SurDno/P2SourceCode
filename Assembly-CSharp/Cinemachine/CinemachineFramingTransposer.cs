using System;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{
  [DocumentationSorting(5.5f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineFramingTransposer : CinemachineComponentBase
  {
    [NoSaveDuringPlay]
    [HideInInspector]
    public Action OnGUICallback = null;
    [Tooltip("This setting will instruct the composer to adjust its target offset based on the motion of the target.  The composer will look at a point where it estimates the target will be this many seconds into the future.  Note that this setting is sensitive to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly.")]
    [Range(0.0f, 1f)]
    public float m_LookaheadTime;
    [Tooltip("Controls the smoothness of the lookahead algorithm.  Larger values smooth out jittery predictions and also increase prediction lag")]
    [Range(3f, 30f)]
    public float m_LookaheadSmoothing = 10f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain the offset in the X-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_XDamping = 1f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain the offset in the Y-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_YDamping = 1f;
    [Range(0.0f, 20f)]
    [Tooltip("How aggressively the camera tries to maintain the offset in the Z-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
    public float m_ZDamping = 1f;
    [Space]
    [Range(0.0f, 1f)]
    [Tooltip("Horizontal screen position for target. The camera will move to position the tracked object here.")]
    public float m_ScreenX = 0.5f;
    [Range(0.0f, 1f)]
    [Tooltip("Vertical screen position for target, The camera will move to position the tracked object here.")]
    public float m_ScreenY = 0.5f;
    [Tooltip("The distance along the camera axis that will be maintained from the Follow target")]
    public float m_CameraDistance = 10f;
    [Space]
    [Range(0.0f, 1f)]
    [Tooltip("Camera will not move horizontally if the target is within this range of the position.")]
    public float m_DeadZoneWidth = 0.1f;
    [Range(0.0f, 1f)]
    [Tooltip("Camera will not move vertically if the target is within this range of the position.")]
    public float m_DeadZoneHeight = 0.1f;
    [Tooltip("The camera will not move along its z-axis if the Follow target is within this distance of the specified camera distance")]
    [FormerlySerializedAs("m_DistanceDeadZoneSize")]
    public float m_DeadZoneDepth;
    [Space]
    [Tooltip("If checked, then then soft zone will be unlimited in size.")]
    public bool m_UnlimitedSoftZone;
    [Range(0.0f, 2f)]
    [Tooltip("When target is within this region, camera will gradually move horizontally to re-align towards the desired position, depending on the damping speed.")]
    public float m_SoftZoneWidth = 0.8f;
    [Range(0.0f, 2f)]
    [Tooltip("When target is within this region, camera will gradually move vertically to re-align towards the desired position, depending on the damping speed.")]
    public float m_SoftZoneHeight = 0.8f;
    [Range(-0.5f, 0.5f)]
    [Tooltip("A non-zero bias will move the target position horizontally away from the center of the soft zone.")]
    public float m_BiasX;
    [Range(-0.5f, 0.5f)]
    [Tooltip("A non-zero bias will move the target position vertically away from the center of the soft zone.")]
    public float m_BiasY;
    [Space]
    [Tooltip("What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both")]
    [FormerlySerializedAs("m_FramingMode")]
    public FramingMode m_GroupFramingMode = FramingMode.HorizontalAndVertical;
    [Tooltip("How to adjust the camera to get the desired framing.  You can zoom, dolly in/out, or do both.")]
    public AdjustmentMode m_AdjustmentMode = AdjustmentMode.DollyThenZoom;
    [Tooltip("The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc.")]
    public float m_GroupFramingSize = 0.8f;
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
    private const float kMinimumCameraDistance = 0.01f;
    private Vector3 m_PreviousCameraPosition = Vector3.zero;
    private PositionPredictor m_Predictor = new();
    private float m_prevTargetHeight;

    public Rect SoftGuideRect
    {
      get => new(m_ScreenX - m_DeadZoneWidth / 2f, m_ScreenY - m_DeadZoneHeight / 2f, m_DeadZoneWidth, m_DeadZoneHeight);
      set
      {
        m_DeadZoneWidth = Mathf.Clamp01(value.width);
        m_DeadZoneHeight = Mathf.Clamp01(value.height);
        m_ScreenX = Mathf.Clamp01(value.x + m_DeadZoneWidth / 2f);
        m_ScreenY = Mathf.Clamp01(value.y + m_DeadZoneHeight / 2f);
        m_SoftZoneWidth = Mathf.Max(m_SoftZoneWidth, m_DeadZoneWidth);
        m_SoftZoneHeight = Mathf.Max(m_SoftZoneHeight, m_DeadZoneHeight);
      }
    }

    public Rect HardGuideRect
    {
      get
      {
        Rect hardGuideRect = new Rect(m_ScreenX - m_SoftZoneWidth / 2f, m_ScreenY - m_SoftZoneHeight / 2f, m_SoftZoneWidth, m_SoftZoneHeight);
        hardGuideRect.position += new Vector2(m_BiasX * (m_SoftZoneWidth - m_DeadZoneWidth), m_BiasY * (m_SoftZoneHeight - m_DeadZoneHeight));
        return hardGuideRect;
      }
      set
      {
        m_SoftZoneWidth = Mathf.Clamp(value.width, 0.0f, 2f);
        m_SoftZoneHeight = Mathf.Clamp(value.height, 0.0f, 2f);
        m_DeadZoneWidth = Mathf.Min(m_DeadZoneWidth, m_SoftZoneWidth);
        m_DeadZoneHeight = Mathf.Min(m_DeadZoneHeight, m_SoftZoneHeight);
        Vector2 vector2 = value.center - new Vector2(m_ScreenX, m_ScreenY);
        float num1 = Mathf.Max(0.0f, m_SoftZoneWidth - m_DeadZoneWidth);
        float num2 = Mathf.Max(0.0f, m_SoftZoneHeight - m_DeadZoneHeight);
        m_BiasX = num1 < 9.9999997473787516E-05 ? 0.0f : Mathf.Clamp(vector2.x / num1, -0.5f, 0.5f);
        m_BiasY = num2 < 9.9999997473787516E-05 ? 0.0f : Mathf.Clamp(vector2.y / num2, -0.5f, 0.5f);
      }
    }

    private void OnValidate()
    {
      m_CameraDistance = Mathf.Max(m_CameraDistance, 0.01f);
      m_DeadZoneDepth = Mathf.Max(m_DeadZoneDepth, 0.0f);
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

    public override bool IsValid => enabled && FollowTarget != null && LookAtTarget == null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Body;

    public Vector3 TrackedPoint { get; private set; }

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      if (deltaTime < 0.0)
      {
        m_Predictor.Reset();
        m_PreviousCameraPosition = curState.RawPosition + curState.RawOrientation * Vector3.back * m_CameraDistance;
      }
      if (!IsValid)
        return;
      Vector3 previousCameraPosition = m_PreviousCameraPosition;
      curState.ReferenceLookAt = FollowTarget.position;
      m_Predictor.Smoothing = m_LookaheadSmoothing;
      m_Predictor.AddPosition(curState.ReferenceLookAt);
      TrackedPoint = m_LookaheadTime > 0.0 ? m_Predictor.PredictPosition(m_LookaheadTime) : curState.ReferenceLookAt;
      Quaternion rawOrientation = curState.RawOrientation;
      Quaternion quaternion = Quaternion.Inverse(rawOrientation);
      Vector3 vector3_1 = quaternion * previousCameraPosition;
      Vector3 targetPos2D = quaternion * TrackedPoint - vector3_1;
      Vector3 vector3_2 = Vector3.zero;
      float a = Mathf.Max(0.01f, m_CameraDistance - m_DeadZoneDepth / 2f);
      float num1 = Mathf.Max(a, m_CameraDistance + m_DeadZoneDepth / 2f);
      if (targetPos2D.z < (double) a)
        vector3_2.z = targetPos2D.z - a;
      if (targetPos2D.z > (double) num1)
        vector3_2.z = targetPos2D.z - num1;
      CinemachineTargetGroup targetGroup = TargetGroup;
      if (targetGroup != null && m_GroupFramingMode != FramingMode.None)
        vector3_2.z += AdjustCameraDepthAndLensForGroupFraming(targetGroup, targetPos2D.z - vector3_2.z, ref curState, deltaTime);
      targetPos2D.z -= vector3_2.z;
      LensSettings lens = curState.Lens;
      float num2 = lens.Orthographic ? curState.Lens.OrthographicSize : Mathf.Tan((float) (0.5 * curState.Lens.FieldOfView * (Math.PI / 180.0))) * targetPos2D.z;
      Rect softGuideRect = SoftGuideRect;
      double orthoSize1 = num2;
      lens = curState.Lens;
      double aspect1 = lens.Aspect;
      Rect ortho1 = ScreenToOrtho(softGuideRect, (float) orthoSize1, (float) aspect1);
      if (deltaTime < 0.0)
      {
        Rect screenRect = new Rect(ortho1.center, Vector2.zero);
        vector3_2 += OrthoOffsetToScreenBounds(targetPos2D, screenRect);
      }
      else
      {
        vector3_2 += OrthoOffsetToScreenBounds(targetPos2D, ortho1);
        Vector3 vector3_3 = Vector3.zero;
        if (!m_UnlimitedSoftZone)
        {
          Rect hardGuideRect = HardGuideRect;
          double orthoSize2 = num2;
          lens = curState.Lens;
          double aspect2 = lens.Aspect;
          Rect ortho2 = ScreenToOrtho(hardGuideRect, (float) orthoSize2, (float) aspect2);
          Vector3 screenBounds = OrthoOffsetToScreenBounds(targetPos2D, ortho2);
          float num3 = Mathf.Max(screenBounds.x / (vector3_2.x + 0.0001f), screenBounds.y / (vector3_2.y + 0.0001f));
          vector3_3 = vector3_2 * num3;
        }
        vector3_2 = vector3_3 + Damper.Damp(vector3_2 - vector3_3, new Vector3(m_XDamping, m_YDamping, m_ZDamping), deltaTime);
      }
      curState.RawPosition = m_PreviousCameraPosition = rawOrientation * (vector3_1 + vector3_2);
    }

    private Rect ScreenToOrtho(Rect rScreen, float orthoSize, float aspect)
    {
      return new Rect {
        yMax = (float) (2.0 * orthoSize * (1.0 - rScreen.yMin - 0.5)),
        yMin = (float) (2.0 * orthoSize * (1.0 - rScreen.yMax - 0.5)),
        xMin = (float) (2.0 * orthoSize * aspect * (rScreen.xMin - 0.5)),
        xMax = (float) (2.0 * orthoSize * aspect * (rScreen.xMax - 0.5))
      };
    }

    private Vector3 OrthoOffsetToScreenBounds(Vector3 targetPos2D, Rect screenRect)
    {
      Vector3 zero = Vector3.zero;
      if (targetPos2D.x < (double) screenRect.xMin)
        zero.x += targetPos2D.x - screenRect.xMin;
      if (targetPos2D.x > (double) screenRect.xMax)
        zero.x += targetPos2D.x - screenRect.xMax;
      if (targetPos2D.y < (double) screenRect.yMin)
        zero.y += targetPos2D.y - screenRect.yMin;
      if (targetPos2D.y > (double) screenRect.yMax)
        zero.y += targetPos2D.y - screenRect.yMax;
      return zero;
    }

    public Bounds m_LastBounds { get; private set; }

    public Matrix4x4 m_lastBoundsMatrix { get; private set; }

    public CinemachineTargetGroup TargetGroup
    {
      get
      {
        Transform followTarget = FollowTarget;
        return followTarget != null ? followTarget.GetComponent<CinemachineTargetGroup>() : null;
      }
    }

    private float AdjustCameraDepthAndLensForGroupFraming(
      CinemachineTargetGroup group,
      float targetZ,
      ref CameraState curState,
      float deltaTime)
    {
      float num1 = 0.0f;
      Bounds boundingBox = group.BoundingBox;
      Vector3 vector3 = curState.RawOrientation * Vector3.forward;
      m_lastBoundsMatrix = Matrix4x4.TRS(boundingBox.center - vector3 * boundingBox.extents.magnitude, curState.RawOrientation, Vector3.one);
      m_LastBounds = group.GetViewSpaceBoundingBox(m_lastBoundsMatrix);
      float num2 = GetTargetHeight(m_LastBounds);
      if (deltaTime >= 0.0)
        num2 = m_prevTargetHeight + Damper.Damp(num2 - m_prevTargetHeight, m_ZDamping, deltaTime);
      m_prevTargetHeight = num2;
      LensSettings lens1 = curState.Lens;
      if (!lens1.Orthographic && m_AdjustmentMode != 0)
      {
        float num3 = Mathf.Clamp(Mathf.Clamp(num2 / (2f * Mathf.Tan((float) (curState.Lens.FieldOfView * (Math.PI / 180.0) / 2.0))) + m_LastBounds.extents.z, targetZ - m_MaxDollyIn, targetZ + m_MaxDollyOut), m_MinimumDistance, m_MaximumDistance);
        num1 += num3 - targetZ;
      }
      lens1 = curState.Lens;
      if (lens1.Orthographic || m_AdjustmentMode != AdjustmentMode.DollyOnly)
      {
        float num4 = targetZ + num1 - m_LastBounds.extents.z;
        float num5 = 179f;
        if (num4 > 9.9999997473787516E-05)
          num5 = (float) (2.0 * Mathf.Atan(num2 / (2f * num4)) * 57.295780181884766);
        LensSettings lens2 = curState.Lens with
        {
          FieldOfView = Mathf.Clamp(num5, m_MinimumFOV, m_MaximumFOV),
          OrthographicSize = Mathf.Clamp(num2 / 2f, m_MinimumOrthoSize, m_MaximumOrthoSize)
        };
        curState.Lens = lens2;
      }
      return -num1;
    }

    private float GetTargetHeight(Bounds b)
    {
      float num = Mathf.Max(0.0001f, m_GroupFramingSize);
      switch (m_GroupFramingMode)
      {
        case FramingMode.Horizontal:
          return b.size.x / (num * VcamState.Lens.Aspect);
        case FramingMode.Vertical:
          return b.size.y / num;
        default:
          return Mathf.Max(b.size.x / (num * VcamState.Lens.Aspect), b.size.y / num);
      }
    }

    [DocumentationSorting(4.01f, DocumentationSortingAttribute.Level.UserRef)]
    public enum FramingMode
    {
      Horizontal,
      Vertical,
      HorizontalAndVertical,
      None,
    }

    public enum AdjustmentMode
    {
      ZoomOnly,
      DollyOnly,
      DollyThenZoom,
    }
  }
}
