using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cinemachine
{
  [DocumentationSorting(0.0f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachineBrain")]
  [SaveDuringPlay]
  public class CinemachineBrain : MonoBehaviour
  {
    [Tooltip("When enabled, the current camera and blend will be indicated in the game window, for debugging")]
    public bool m_ShowDebugText;
    [Tooltip("When enabled, the camera's frustum will be shown at all times in the scene view")]
    public bool m_ShowCameraFrustum = true;
    [Tooltip("When enabled, the cameras will always respond in real-time to user input and damping, even if the game is running in slow motion")]
    public bool m_IgnoreTimeScale;
    [Tooltip("If set, this object's Y axis will define the worldspace Up vector for all the virtual cameras.  This is useful for instance in top-down game environments.  If not set, Up is worldspace Y.  Setting this appropriately is important, because Virtual Cameras don't like looking straight up or straight down.")]
    public Transform m_WorldUpOverride;
    [Tooltip("Use FixedUpdate if all your targets are animated during FixedUpdate (e.g. RigidBodies), LateUpdate if all your targets are animated during the normal Update loop, and SmartUpdate if you want Cinemachine to do the appropriate thing on a per-target basis.  SmartUpdate is the recommended setting")]
    public UpdateMethod m_UpdateMethod = UpdateMethod.SmartUpdate;
    [CinemachineBlendDefinitionProperty]
    [Tooltip("The blend that is used in cases where you haven't explicitly defined a blend between two Virtual Cameras")]
    public CinemachineBlendDefinition m_DefaultBlend = new(CinemachineBlendDefinition.Style.EaseInOut, 2f);
    [Tooltip("This is the asset that contains custom settings for blends between specific virtual cameras in your scene")]
    public CinemachineBlenderSettings m_CustomBlends;
    private Camera m_OutputCamera;
    [Tooltip("This event will fire whenever a virtual camera goes live and there is no blend")]
    public BrainEvent m_CameraCutEvent = new();
    [Tooltip("This event will fire whenever a virtual camera goes live.  If a blend is involved, then the event will fire on the first frame of the blend.")]
    public VcamEvent m_CameraActivatedEvent = new();
    internal static BrainEvent sPostProcessingHandler = new();
    private ICinemachineCamera mActiveCameraPreviousFrame;
    private ICinemachineCamera mOutgoingCameraPreviousFrame;
    private CinemachineBlend mActiveBlend;
    private bool mPreviousFrameWasOverride;
    private List<OverrideStackFrame> mOverrideStack = [];
    private int mNextOverrideId = 1;
    private OverrideStackFrame mOverrideBlendFromNothing = new();
    private WaitForFixedUpdate mWaitForFixedUpdate = new();
    private static int msCurrentFrame;
    private static int msFirstBrainObjectId;
    private static int msSubframes;

    public Camera OutputCamera
    {
      get
      {
        if (m_OutputCamera == null)
          m_OutputCamera = GetComponent<Camera>();
        return m_OutputCamera;
      }
    }

    public event Action CameraProcessedEvent;

    internal Component PostProcessingComponent { get; set; }

    public static ICinemachineCamera SoloCamera { get; set; }

    public static Color GetSoloGUIColor() => Color.Lerp(Color.red, Color.yellow, 0.8f);

    public Vector3 DefaultWorldUp => m_WorldUpOverride != null ? m_WorldUpOverride.transform.up : Vector3.up;

    private OverrideStackFrame GetOverrideFrame(int id)
    {
      int count = mOverrideStack.Count;
      for (int index = 0; index < count; ++index)
      {
        if (mOverrideStack[index].id == id)
          return mOverrideStack[index];
      }
      OverrideStackFrame overrideFrame = new OverrideStackFrame();
      overrideFrame.id = id;
      mOverrideStack.Insert(0, overrideFrame);
      return overrideFrame;
    }

    private OverrideStackFrame GetNextActiveFrame(int overrideId)
    {
      bool flag = false;
      int count = mOverrideStack.Count;
      for (int index = 0; index < count; ++index)
      {
        if (mOverrideStack[index].id == overrideId)
          flag = true;
        else if (mOverrideStack[index].Active & flag)
          return mOverrideStack[index];
      }
      mOverrideBlendFromNothing.camera = TopCameraFromPriorityQueue();
      mOverrideBlendFromNothing.blend = mActiveBlend;
      return mOverrideBlendFromNothing;
    }

    private OverrideStackFrame GetActiveOverride()
    {
      int count = mOverrideStack.Count;
      for (int index = 0; index < count; ++index)
      {
        if (mOverrideStack[index].Active)
          return mOverrideStack[index];
      }
      return null;
    }

    internal int SetCameraOverride(
      int overrideId,
      ICinemachineCamera camA,
      ICinemachineCamera camB,
      float weightB,
      float deltaTime)
    {
      if (overrideId < 0)
        overrideId = mNextOverrideId++;
      OverrideStackFrame overrideFrame = GetOverrideFrame(overrideId);
      overrideFrame.camera = null;
      overrideFrame.deltaTime = deltaTime;
      overrideFrame.timeOfOverride = Time.realtimeSinceStartup;
      if (camA != null || camB != null)
      {
        if (weightB <= 9.9999997473787516E-05)
        {
          overrideFrame.blend = null;
          if (camA != null)
            overrideFrame.camera = camA;
        }
        else if (weightB >= 0.99989998340606689)
        {
          overrideFrame.blend = null;
          if (camB != null)
            overrideFrame.camera = camB;
        }
        else
        {
          if (camB == null)
          {
            ICinemachineCamera cinemachineCamera = camB;
            camB = camA;
            camA = cinemachineCamera;
            weightB = 1f - weightB;
          }
          if (camA == null)
          {
            OverrideStackFrame nextActiveFrame = GetNextActiveFrame(overrideId);
            camA = nextActiveFrame.blend == null ? (nextActiveFrame.camera != null ? nextActiveFrame.camera : camB) : new BlendSourceVirtualCamera(nextActiveFrame.blend, deltaTime);
          }
          if (overrideFrame.blend == null)
            overrideFrame.blend = new CinemachineBlend(camA, camB, AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f), 1f, weightB);
          overrideFrame.blend.CamA = camA;
          overrideFrame.blend.CamB = camB;
          overrideFrame.blend.TimeInBlend = weightB;
          overrideFrame.camera = camB;
        }
      }
      return overrideId;
    }

    internal void ReleaseCameraOverride(int overrideId)
    {
      int count = mOverrideStack.Count;
      for (int index = 0; index < count; ++index)
      {
        if (mOverrideStack[index].id == overrideId)
        {
          mOverrideStack.RemoveAt(index);
          break;
        }
      }
    }

    private void OnEnable()
    {
      mActiveBlend = null;
      mActiveCameraPreviousFrame = null;
      mOutgoingCameraPreviousFrame = null;
      mPreviousFrameWasOverride = false;
      CinemachineCore.Instance.AddActiveBrain(this);
    }

    private void OnDisable()
    {
      CinemachineCore.Instance.RemoveActiveBrain(this);
      mActiveBlend = null;
      mActiveCameraPreviousFrame = null;
      mOutgoingCameraPreviousFrame = null;
      mPreviousFrameWasOverride = false;
      mOverrideStack.Clear();
    }

    private void Start()
    {
      UpdateVirtualCameras(CinemachineCore.UpdateFilter.Late, -1f);
      StartCoroutine(AfterPhysics());
    }

    private IEnumerator AfterPhysics()
    {
      while (true)
      {
        yield return mWaitForFixedUpdate;
        if (m_UpdateMethod == UpdateMethod.SmartUpdate)
        {
          AddSubframe();
          UpdateVirtualCameras(CinemachineCore.UpdateFilter.Fixed, GetEffectiveDeltaTime(true));
        }
        else if (m_UpdateMethod == UpdateMethod.LateUpdate)
        {
          msSubframes = 1;
        }
        else
        {
          AddSubframe();
          UpdateVirtualCameras(CinemachineCore.UpdateFilter.ForcedFixed, GetEffectiveDeltaTime(true));
        }
      }
    }

    private void LateUpdate()
    {
      float effectiveDeltaTime = GetEffectiveDeltaTime(false);
      if (m_UpdateMethod == UpdateMethod.SmartUpdate)
        UpdateVirtualCameras(CinemachineCore.UpdateFilter.Late, effectiveDeltaTime);
      else if (m_UpdateMethod == UpdateMethod.LateUpdate)
        UpdateVirtualCameras(CinemachineCore.UpdateFilter.ForcedLate, effectiveDeltaTime);
      ProcessActiveCamera(GetEffectiveDeltaTime(false));
    }

    private float GetEffectiveDeltaTime(bool fixedDelta)
    {
      if (SoloCamera != null)
        return Time.unscaledDeltaTime;
      OverrideStackFrame activeOverride = GetActiveOverride();
      if (activeOverride != null)
        return activeOverride.Expired ? -1f : activeOverride.deltaTime;
      if (!Application.isPlaying)
        return -1f;
      return m_IgnoreTimeScale ? (fixedDelta ? Time.fixedDeltaTime : Time.unscaledDeltaTime) : (fixedDelta ? Time.fixedDeltaTime * Time.timeScale : Time.deltaTime);
    }

    private void UpdateVirtualCameras(CinemachineCore.UpdateFilter updateFilter, float deltaTime)
    {
      CinemachineCore.Instance.CurrentUpdateFilter = updateFilter;
      CinemachineCore.Instance.UpdateAllActiveVirtualCameras(DefaultWorldUp, deltaTime);
      ICinemachineCamera activeVirtualCamera = ActiveVirtualCamera;
      if (activeVirtualCamera != null)
        CinemachineCore.Instance.UpdateVirtualCamera(activeVirtualCamera, DefaultWorldUp, deltaTime);
      ActiveBlend?.UpdateCameraState(DefaultWorldUp, deltaTime);
      CinemachineCore.Instance.CurrentUpdateFilter = CinemachineCore.UpdateFilter.Late;
    }

    private void ProcessActiveCamera(float deltaTime)
    {
      if (!isActiveAndEnabled)
      {
        mActiveCameraPreviousFrame = null;
        mOutgoingCameraPreviousFrame = null;
        mPreviousFrameWasOverride = false;
      }
      else
      {
        OverrideStackFrame activeOverride = GetActiveOverride();
        ICinemachineCamera activeVirtualCamera = ActiveVirtualCamera;
        if (activeVirtualCamera == null)
        {
          mOutgoingCameraPreviousFrame = null;
        }
        else
        {
          if (activeOverride != null)
            mActiveBlend = null;
          CinemachineBlend cinemachineBlend = ActiveBlend;
          if (mActiveCameraPreviousFrame != null && mActiveCameraPreviousFrame.VirtualCameraGameObject == null)
            mActiveCameraPreviousFrame = null;
          if (mActiveCameraPreviousFrame != activeVirtualCamera)
          {
            if (mActiveCameraPreviousFrame != null && !mPreviousFrameWasOverride && activeOverride == null && deltaTime >= 0.0)
            {
              AnimationCurve blendCurve = LookupBlendCurve(mActiveCameraPreviousFrame, activeVirtualCamera, out float duration);
              cinemachineBlend = CreateBlend(mActiveCameraPreviousFrame, activeVirtualCamera, blendCurve, duration, mActiveBlend);
            }
            if (activeVirtualCamera != mOutgoingCameraPreviousFrame)
            {
              activeVirtualCamera.OnTransitionFromCamera(mActiveCameraPreviousFrame, DefaultWorldUp, deltaTime);
              if (!activeVirtualCamera.VirtualCameraGameObject.activeInHierarchy && (cinemachineBlend == null || !cinemachineBlend.Uses(activeVirtualCamera)))
                activeVirtualCamera.UpdateCameraState(DefaultWorldUp, -1f);
              if (m_CameraActivatedEvent != null)
                m_CameraActivatedEvent.Invoke(activeVirtualCamera);
            }
            if ((cinemachineBlend == null || cinemachineBlend.CamA != mActiveCameraPreviousFrame && cinemachineBlend.CamB != mActiveCameraPreviousFrame && cinemachineBlend.CamA != mOutgoingCameraPreviousFrame && cinemachineBlend.CamB != mOutgoingCameraPreviousFrame) && m_CameraCutEvent != null)
              m_CameraCutEvent.Invoke(this);
          }
          if (cinemachineBlend != null)
          {
            if (activeOverride == null)
              cinemachineBlend.TimeInBlend += deltaTime >= 0.0 ? deltaTime : cinemachineBlend.Duration;
            if (cinemachineBlend.IsComplete)
              cinemachineBlend = null;
          }
          if (activeOverride == null)
            mActiveBlend = cinemachineBlend;
          CameraState state = activeVirtualCamera.State;
          if (cinemachineBlend != null)
            state = cinemachineBlend.State;
          PushStateToUnityCamera(state, activeVirtualCamera);
          mOutgoingCameraPreviousFrame = null;
          if (cinemachineBlend != null)
            mOutgoingCameraPreviousFrame = cinemachineBlend.CamB;
        }
        mActiveCameraPreviousFrame = activeVirtualCamera;
        mPreviousFrameWasOverride = activeOverride != null;
        if (mPreviousFrameWasOverride && activeOverride.blend != null)
        {
          if (activeOverride.blend.BlendWeight < 0.5)
          {
            mActiveCameraPreviousFrame = activeOverride.blend.CamA;
            mOutgoingCameraPreviousFrame = activeOverride.blend.CamB;
          }
          else
          {
            mActiveCameraPreviousFrame = activeOverride.blend.CamB;
            mOutgoingCameraPreviousFrame = activeOverride.blend.CamA;
          }
        }
        Action cameraProcessedEvent = CameraProcessedEvent;
        if (cameraProcessedEvent == null)
          return;
        cameraProcessedEvent();
      }
    }

    public bool IsBlending => ActiveBlend != null && ActiveBlend.IsValid;

    public CinemachineBlend ActiveBlend
    {
      get
      {
        if (SoloCamera != null)
          return null;
        OverrideStackFrame activeOverride = GetActiveOverride();
        return activeOverride == null || activeOverride.blend == null ? mActiveBlend : activeOverride.blend;
      }
    }

    public bool IsLive(ICinemachineCamera vcam)
    {
      if (IsLiveItself(vcam))
        return true;
      for (ICinemachineCamera parentCamera = vcam.ParentCamera; parentCamera != null && parentCamera.IsLiveChild(vcam); parentCamera = vcam.ParentCamera)
      {
        if (IsLiveItself(parentCamera))
          return true;
        vcam = parentCamera;
      }
      return false;
    }

    private bool IsLiveItself(ICinemachineCamera vcam)
    {
      return mActiveCameraPreviousFrame == vcam || ActiveVirtualCamera == vcam || IsBlending && ActiveBlend.Uses(vcam);
    }

    public ICinemachineCamera ActiveVirtualCamera
    {
      get
      {
        if (SoloCamera != null)
          return SoloCamera;
        OverrideStackFrame activeOverride = GetActiveOverride();
        return activeOverride == null || activeOverride.camera == null ? TopCameraFromPriorityQueue() : activeOverride.camera;
      }
    }

    public CameraState CurrentCameraState { get; private set; }

    private ICinemachineCamera TopCameraFromPriorityQueue()
    {
      Camera outputCamera = OutputCamera;
      int num = outputCamera == null ? -1 : outputCamera.cullingMask;
      int virtualCameraCount = CinemachineCore.Instance.VirtualCameraCount;
      for (int index = 0; index < virtualCameraCount; ++index)
      {
        ICinemachineCamera virtualCamera = CinemachineCore.Instance.GetVirtualCamera(index);
        GameObject cameraGameObject = virtualCamera?.VirtualCameraGameObject;
        if (cameraGameObject != null && (num & 1 << cameraGameObject.layer) != 0)
          return virtualCamera;
      }
      return null;
    }

    private AnimationCurve LookupBlendCurve(
      ICinemachineCamera fromKey,
      ICinemachineCamera toKey,
      out float duration)
    {
      AnimationCurve defaultCurve = m_DefaultBlend.BlendCurve;
      if (m_CustomBlends != null)
        defaultCurve = m_CustomBlends.GetBlendCurveForVirtualCameras(fromKey != null ? fromKey.Name : string.Empty, toKey != null ? toKey.Name : string.Empty, defaultCurve);
      Keyframe[] keys = defaultCurve.keys;
      duration = keys == null || keys.Length == 0 ? 0.0f : keys[keys.Length - 1].time;
      return defaultCurve;
    }

    private CinemachineBlend CreateBlend(
      ICinemachineCamera camA,
      ICinemachineCamera camB,
      AnimationCurve blendCurve,
      float duration,
      CinemachineBlend activeBlend)
    {
      if (blendCurve == null || duration <= 0.0 || camA == null && camB == null)
        return null;
      if (camA == null || activeBlend != null)
      {
        CameraState state = CameraState.Default;
        if (activeBlend != null)
        {
          state = activeBlend.State;
        }
        else
        {
          state.RawPosition = transform.position;
          state.RawOrientation = transform.rotation;
          state.Lens = LensSettings.FromCamera(OutputCamera);
        }
        camA = new StaticPointVirtualCamera(state, activeBlend == null ? "(none)" : "Mid-blend");
      }
      return new CinemachineBlend(camA, camB, blendCurve, duration, 0.0f);
    }

    private void PushStateToUnityCamera(CameraState state, ICinemachineCamera vcam)
    {
      CurrentCameraState = state;
      transform.position = state.FinalPosition;
      transform.rotation = state.FinalOrientation;
      Camera outputCamera = OutputCamera;
      if (outputCamera != null)
      {
        outputCamera.fieldOfView = state.Lens.FieldOfView;
        outputCamera.orthographicSize = state.Lens.OrthographicSize;
        outputCamera.nearClipPlane = state.Lens.NearClipPlane;
        outputCamera.farClipPlane = state.Lens.FarClipPlane;
      }
      if (sPostProcessingHandler == null)
        return;
      sPostProcessingHandler.Invoke(this);
    }

    private void AddSubframe()
    {
      int frameCount = Time.frameCount;
      if (frameCount == msCurrentFrame)
      {
        if (msFirstBrainObjectId != GetInstanceID())
          return;
        ++msSubframes;
      }
      else
      {
        msCurrentFrame = frameCount;
        msFirstBrainObjectId = GetInstanceID();
        msSubframes = 1;
      }
    }

    internal static int GetSubframeCount() => Math.Max(1, msSubframes);

    [DocumentationSorting(0.1f, DocumentationSortingAttribute.Level.UserRef)]
    public enum UpdateMethod
    {
      FixedUpdate,
      LateUpdate,
      SmartUpdate,
    }

    [Serializable]
    public class BrainEvent : UnityEvent<CinemachineBrain>
    {
    }

    [Serializable]
    public class VcamEvent : UnityEvent<ICinemachineCamera>
    {
    }

    private class OverrideStackFrame
    {
      public int id;
      public ICinemachineCamera camera;
      public CinemachineBlend blend;
      public float deltaTime;
      public float timeOfOverride;

      public bool Active => camera != null;

      public bool Expired => !Application.isPlaying && Time.realtimeSinceStartup - (double) timeOfOverride > Time.maximumDeltaTime;
    }
  }
}
