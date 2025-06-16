// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineBrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
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
    public bool m_ShowDebugText = false;
    [Tooltip("When enabled, the camera's frustum will be shown at all times in the scene view")]
    public bool m_ShowCameraFrustum = true;
    [Tooltip("When enabled, the cameras will always respond in real-time to user input and damping, even if the game is running in slow motion")]
    public bool m_IgnoreTimeScale = false;
    [Tooltip("If set, this object's Y axis will define the worldspace Up vector for all the virtual cameras.  This is useful for instance in top-down game environments.  If not set, Up is worldspace Y.  Setting this appropriately is important, because Virtual Cameras don't like looking straight up or straight down.")]
    public Transform m_WorldUpOverride;
    [Tooltip("Use FixedUpdate if all your targets are animated during FixedUpdate (e.g. RigidBodies), LateUpdate if all your targets are animated during the normal Update loop, and SmartUpdate if you want Cinemachine to do the appropriate thing on a per-target basis.  SmartUpdate is the recommended setting")]
    public CinemachineBrain.UpdateMethod m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
    [CinemachineBlendDefinitionProperty]
    [Tooltip("The blend that is used in cases where you haven't explicitly defined a blend between two Virtual Cameras")]
    public CinemachineBlendDefinition m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 2f);
    [Tooltip("This is the asset that contains custom settings for blends between specific virtual cameras in your scene")]
    public CinemachineBlenderSettings m_CustomBlends = (CinemachineBlenderSettings) null;
    private Camera m_OutputCamera = (Camera) null;
    [Tooltip("This event will fire whenever a virtual camera goes live and there is no blend")]
    public CinemachineBrain.BrainEvent m_CameraCutEvent = new CinemachineBrain.BrainEvent();
    [Tooltip("This event will fire whenever a virtual camera goes live.  If a blend is involved, then the event will fire on the first frame of the blend.")]
    public CinemachineBrain.VcamEvent m_CameraActivatedEvent = new CinemachineBrain.VcamEvent();
    internal static CinemachineBrain.BrainEvent sPostProcessingHandler = new CinemachineBrain.BrainEvent();
    private ICinemachineCamera mActiveCameraPreviousFrame;
    private ICinemachineCamera mOutgoingCameraPreviousFrame;
    private CinemachineBlend mActiveBlend = (CinemachineBlend) null;
    private bool mPreviousFrameWasOverride = false;
    private List<CinemachineBrain.OverrideStackFrame> mOverrideStack = new List<CinemachineBrain.OverrideStackFrame>();
    private int mNextOverrideId = 1;
    private CinemachineBrain.OverrideStackFrame mOverrideBlendFromNothing = new CinemachineBrain.OverrideStackFrame();
    private WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();
    private static int msCurrentFrame;
    private static int msFirstBrainObjectId;
    private static int msSubframes;

    public Camera OutputCamera
    {
      get
      {
        if ((UnityEngine.Object) this.m_OutputCamera == (UnityEngine.Object) null)
          this.m_OutputCamera = this.GetComponent<Camera>();
        return this.m_OutputCamera;
      }
    }

    public event Action CameraProcessedEvent;

    internal Component PostProcessingComponent { get; set; }

    public static ICinemachineCamera SoloCamera { get; set; }

    public static Color GetSoloGUIColor() => Color.Lerp(Color.red, Color.yellow, 0.8f);

    public Vector3 DefaultWorldUp
    {
      get
      {
        return (UnityEngine.Object) this.m_WorldUpOverride != (UnityEngine.Object) null ? this.m_WorldUpOverride.transform.up : Vector3.up;
      }
    }

    private CinemachineBrain.OverrideStackFrame GetOverrideFrame(int id)
    {
      int count = this.mOverrideStack.Count;
      for (int index = 0; index < count; ++index)
      {
        if (this.mOverrideStack[index].id == id)
          return this.mOverrideStack[index];
      }
      CinemachineBrain.OverrideStackFrame overrideFrame = new CinemachineBrain.OverrideStackFrame();
      overrideFrame.id = id;
      this.mOverrideStack.Insert(0, overrideFrame);
      return overrideFrame;
    }

    private CinemachineBrain.OverrideStackFrame GetNextActiveFrame(int overrideId)
    {
      bool flag = false;
      int count = this.mOverrideStack.Count;
      for (int index = 0; index < count; ++index)
      {
        if (this.mOverrideStack[index].id == overrideId)
          flag = true;
        else if (this.mOverrideStack[index].Active & flag)
          return this.mOverrideStack[index];
      }
      this.mOverrideBlendFromNothing.camera = this.TopCameraFromPriorityQueue();
      this.mOverrideBlendFromNothing.blend = this.mActiveBlend;
      return this.mOverrideBlendFromNothing;
    }

    private CinemachineBrain.OverrideStackFrame GetActiveOverride()
    {
      int count = this.mOverrideStack.Count;
      for (int index = 0; index < count; ++index)
      {
        if (this.mOverrideStack[index].Active)
          return this.mOverrideStack[index];
      }
      return (CinemachineBrain.OverrideStackFrame) null;
    }

    internal int SetCameraOverride(
      int overrideId,
      ICinemachineCamera camA,
      ICinemachineCamera camB,
      float weightB,
      float deltaTime)
    {
      if (overrideId < 0)
        overrideId = this.mNextOverrideId++;
      CinemachineBrain.OverrideStackFrame overrideFrame = this.GetOverrideFrame(overrideId);
      overrideFrame.camera = (ICinemachineCamera) null;
      overrideFrame.deltaTime = deltaTime;
      overrideFrame.timeOfOverride = Time.realtimeSinceStartup;
      if (camA != null || camB != null)
      {
        if ((double) weightB <= 9.9999997473787516E-05)
        {
          overrideFrame.blend = (CinemachineBlend) null;
          if (camA != null)
            overrideFrame.camera = camA;
        }
        else if ((double) weightB >= 0.99989998340606689)
        {
          overrideFrame.blend = (CinemachineBlend) null;
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
            CinemachineBrain.OverrideStackFrame nextActiveFrame = this.GetNextActiveFrame(overrideId);
            camA = nextActiveFrame.blend == null ? (nextActiveFrame.camera != null ? nextActiveFrame.camera : camB) : (ICinemachineCamera) new BlendSourceVirtualCamera(nextActiveFrame.blend, deltaTime);
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
      int count = this.mOverrideStack.Count;
      for (int index = 0; index < count; ++index)
      {
        if (this.mOverrideStack[index].id == overrideId)
        {
          this.mOverrideStack.RemoveAt(index);
          break;
        }
      }
    }

    private void OnEnable()
    {
      this.mActiveBlend = (CinemachineBlend) null;
      this.mActiveCameraPreviousFrame = (ICinemachineCamera) null;
      this.mOutgoingCameraPreviousFrame = (ICinemachineCamera) null;
      this.mPreviousFrameWasOverride = false;
      CinemachineCore.Instance.AddActiveBrain(this);
    }

    private void OnDisable()
    {
      CinemachineCore.Instance.RemoveActiveBrain(this);
      this.mActiveBlend = (CinemachineBlend) null;
      this.mActiveCameraPreviousFrame = (ICinemachineCamera) null;
      this.mOutgoingCameraPreviousFrame = (ICinemachineCamera) null;
      this.mPreviousFrameWasOverride = false;
      this.mOverrideStack.Clear();
    }

    private void Start()
    {
      this.UpdateVirtualCameras(CinemachineCore.UpdateFilter.Late, -1f);
      this.StartCoroutine(this.AfterPhysics());
    }

    private IEnumerator AfterPhysics()
    {
      while (true)
      {
        yield return (object) this.mWaitForFixedUpdate;
        if (this.m_UpdateMethod == CinemachineBrain.UpdateMethod.SmartUpdate)
        {
          this.AddSubframe();
          this.UpdateVirtualCameras(CinemachineCore.UpdateFilter.Fixed, this.GetEffectiveDeltaTime(true));
        }
        else if (this.m_UpdateMethod == CinemachineBrain.UpdateMethod.LateUpdate)
        {
          CinemachineBrain.msSubframes = 1;
        }
        else
        {
          this.AddSubframe();
          this.UpdateVirtualCameras(CinemachineCore.UpdateFilter.ForcedFixed, this.GetEffectiveDeltaTime(true));
        }
      }
    }

    private void LateUpdate()
    {
      float effectiveDeltaTime = this.GetEffectiveDeltaTime(false);
      if (this.m_UpdateMethod == CinemachineBrain.UpdateMethod.SmartUpdate)
        this.UpdateVirtualCameras(CinemachineCore.UpdateFilter.Late, effectiveDeltaTime);
      else if (this.m_UpdateMethod == CinemachineBrain.UpdateMethod.LateUpdate)
        this.UpdateVirtualCameras(CinemachineCore.UpdateFilter.ForcedLate, effectiveDeltaTime);
      this.ProcessActiveCamera(this.GetEffectiveDeltaTime(false));
    }

    private float GetEffectiveDeltaTime(bool fixedDelta)
    {
      if (CinemachineBrain.SoloCamera != null)
        return Time.unscaledDeltaTime;
      CinemachineBrain.OverrideStackFrame activeOverride = this.GetActiveOverride();
      if (activeOverride != null)
        return activeOverride.Expired ? -1f : activeOverride.deltaTime;
      if (!Application.isPlaying)
        return -1f;
      return this.m_IgnoreTimeScale ? (fixedDelta ? Time.fixedDeltaTime : Time.unscaledDeltaTime) : (fixedDelta ? Time.fixedDeltaTime * Time.timeScale : Time.deltaTime);
    }

    private void UpdateVirtualCameras(CinemachineCore.UpdateFilter updateFilter, float deltaTime)
    {
      CinemachineCore.Instance.CurrentUpdateFilter = updateFilter;
      CinemachineCore.Instance.UpdateAllActiveVirtualCameras(this.DefaultWorldUp, deltaTime);
      ICinemachineCamera activeVirtualCamera = this.ActiveVirtualCamera;
      if (activeVirtualCamera != null)
        CinemachineCore.Instance.UpdateVirtualCamera(activeVirtualCamera, this.DefaultWorldUp, deltaTime);
      this.ActiveBlend?.UpdateCameraState(this.DefaultWorldUp, deltaTime);
      CinemachineCore.Instance.CurrentUpdateFilter = CinemachineCore.UpdateFilter.Late;
    }

    private void ProcessActiveCamera(float deltaTime)
    {
      if (!this.isActiveAndEnabled)
      {
        this.mActiveCameraPreviousFrame = (ICinemachineCamera) null;
        this.mOutgoingCameraPreviousFrame = (ICinemachineCamera) null;
        this.mPreviousFrameWasOverride = false;
      }
      else
      {
        CinemachineBrain.OverrideStackFrame activeOverride = this.GetActiveOverride();
        ICinemachineCamera activeVirtualCamera = this.ActiveVirtualCamera;
        if (activeVirtualCamera == null)
        {
          this.mOutgoingCameraPreviousFrame = (ICinemachineCamera) null;
        }
        else
        {
          if (activeOverride != null)
            this.mActiveBlend = (CinemachineBlend) null;
          CinemachineBlend cinemachineBlend = this.ActiveBlend;
          if (this.mActiveCameraPreviousFrame != null && (UnityEngine.Object) this.mActiveCameraPreviousFrame.VirtualCameraGameObject == (UnityEngine.Object) null)
            this.mActiveCameraPreviousFrame = (ICinemachineCamera) null;
          if (this.mActiveCameraPreviousFrame != activeVirtualCamera)
          {
            if (this.mActiveCameraPreviousFrame != null && !this.mPreviousFrameWasOverride && activeOverride == null && (double) deltaTime >= 0.0)
            {
              float duration = 0.0f;
              AnimationCurve blendCurve = this.LookupBlendCurve(this.mActiveCameraPreviousFrame, activeVirtualCamera, out duration);
              cinemachineBlend = this.CreateBlend(this.mActiveCameraPreviousFrame, activeVirtualCamera, blendCurve, duration, this.mActiveBlend);
            }
            if (activeVirtualCamera != this.mOutgoingCameraPreviousFrame)
            {
              activeVirtualCamera.OnTransitionFromCamera(this.mActiveCameraPreviousFrame, this.DefaultWorldUp, deltaTime);
              if (!activeVirtualCamera.VirtualCameraGameObject.activeInHierarchy && (cinemachineBlend == null || !cinemachineBlend.Uses(activeVirtualCamera)))
                activeVirtualCamera.UpdateCameraState(this.DefaultWorldUp, -1f);
              if (this.m_CameraActivatedEvent != null)
                this.m_CameraActivatedEvent.Invoke(activeVirtualCamera);
            }
            if ((cinemachineBlend == null || cinemachineBlend.CamA != this.mActiveCameraPreviousFrame && cinemachineBlend.CamB != this.mActiveCameraPreviousFrame && cinemachineBlend.CamA != this.mOutgoingCameraPreviousFrame && cinemachineBlend.CamB != this.mOutgoingCameraPreviousFrame) && this.m_CameraCutEvent != null)
              this.m_CameraCutEvent.Invoke(this);
          }
          if (cinemachineBlend != null)
          {
            if (activeOverride == null)
              cinemachineBlend.TimeInBlend += (double) deltaTime >= 0.0 ? deltaTime : cinemachineBlend.Duration;
            if (cinemachineBlend.IsComplete)
              cinemachineBlend = (CinemachineBlend) null;
          }
          if (activeOverride == null)
            this.mActiveBlend = cinemachineBlend;
          CameraState state = activeVirtualCamera.State;
          if (cinemachineBlend != null)
            state = cinemachineBlend.State;
          this.PushStateToUnityCamera(state, activeVirtualCamera);
          this.mOutgoingCameraPreviousFrame = (ICinemachineCamera) null;
          if (cinemachineBlend != null)
            this.mOutgoingCameraPreviousFrame = cinemachineBlend.CamB;
        }
        this.mActiveCameraPreviousFrame = activeVirtualCamera;
        this.mPreviousFrameWasOverride = activeOverride != null;
        if (this.mPreviousFrameWasOverride && activeOverride.blend != null)
        {
          if ((double) activeOverride.blend.BlendWeight < 0.5)
          {
            this.mActiveCameraPreviousFrame = activeOverride.blend.CamA;
            this.mOutgoingCameraPreviousFrame = activeOverride.blend.CamB;
          }
          else
          {
            this.mActiveCameraPreviousFrame = activeOverride.blend.CamB;
            this.mOutgoingCameraPreviousFrame = activeOverride.blend.CamA;
          }
        }
        Action cameraProcessedEvent = this.CameraProcessedEvent;
        if (cameraProcessedEvent == null)
          return;
        cameraProcessedEvent();
      }
    }

    public bool IsBlending => this.ActiveBlend != null && this.ActiveBlend.IsValid;

    public CinemachineBlend ActiveBlend
    {
      get
      {
        if (CinemachineBrain.SoloCamera != null)
          return (CinemachineBlend) null;
        CinemachineBrain.OverrideStackFrame activeOverride = this.GetActiveOverride();
        return activeOverride == null || activeOverride.blend == null ? this.mActiveBlend : activeOverride.blend;
      }
    }

    public bool IsLive(ICinemachineCamera vcam)
    {
      if (this.IsLiveItself(vcam))
        return true;
      for (ICinemachineCamera parentCamera = vcam.ParentCamera; parentCamera != null && parentCamera.IsLiveChild(vcam); parentCamera = vcam.ParentCamera)
      {
        if (this.IsLiveItself(parentCamera))
          return true;
        vcam = parentCamera;
      }
      return false;
    }

    private bool IsLiveItself(ICinemachineCamera vcam)
    {
      return this.mActiveCameraPreviousFrame == vcam || this.ActiveVirtualCamera == vcam || this.IsBlending && this.ActiveBlend.Uses(vcam);
    }

    public ICinemachineCamera ActiveVirtualCamera
    {
      get
      {
        if (CinemachineBrain.SoloCamera != null)
          return CinemachineBrain.SoloCamera;
        CinemachineBrain.OverrideStackFrame activeOverride = this.GetActiveOverride();
        return activeOverride == null || activeOverride.camera == null ? this.TopCameraFromPriorityQueue() : activeOverride.camera;
      }
    }

    public CameraState CurrentCameraState { get; private set; }

    private ICinemachineCamera TopCameraFromPriorityQueue()
    {
      Camera outputCamera = this.OutputCamera;
      int num = (UnityEngine.Object) outputCamera == (UnityEngine.Object) null ? -1 : outputCamera.cullingMask;
      int virtualCameraCount = CinemachineCore.Instance.VirtualCameraCount;
      for (int index = 0; index < virtualCameraCount; ++index)
      {
        ICinemachineCamera virtualCamera = CinemachineCore.Instance.GetVirtualCamera(index);
        GameObject cameraGameObject = virtualCamera?.VirtualCameraGameObject;
        if ((UnityEngine.Object) cameraGameObject != (UnityEngine.Object) null && (num & 1 << cameraGameObject.layer) != 0)
          return virtualCamera;
      }
      return (ICinemachineCamera) null;
    }

    private AnimationCurve LookupBlendCurve(
      ICinemachineCamera fromKey,
      ICinemachineCamera toKey,
      out float duration)
    {
      AnimationCurve defaultCurve = this.m_DefaultBlend.BlendCurve;
      if ((UnityEngine.Object) this.m_CustomBlends != (UnityEngine.Object) null)
        defaultCurve = this.m_CustomBlends.GetBlendCurveForVirtualCameras(fromKey != null ? fromKey.Name : string.Empty, toKey != null ? toKey.Name : string.Empty, defaultCurve);
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
      if (blendCurve == null || (double) duration <= 0.0 || camA == null && camB == null)
        return (CinemachineBlend) null;
      if (camA == null || activeBlend != null)
      {
        CameraState state = CameraState.Default;
        if (activeBlend != null)
        {
          state = activeBlend.State;
        }
        else
        {
          state.RawPosition = this.transform.position;
          state.RawOrientation = this.transform.rotation;
          state.Lens = LensSettings.FromCamera(this.OutputCamera);
        }
        camA = (ICinemachineCamera) new StaticPointVirtualCamera(state, activeBlend == null ? "(none)" : "Mid-blend");
      }
      return new CinemachineBlend(camA, camB, blendCurve, duration, 0.0f);
    }

    private void PushStateToUnityCamera(CameraState state, ICinemachineCamera vcam)
    {
      this.CurrentCameraState = state;
      this.transform.position = state.FinalPosition;
      this.transform.rotation = state.FinalOrientation;
      Camera outputCamera = this.OutputCamera;
      if ((UnityEngine.Object) outputCamera != (UnityEngine.Object) null)
      {
        outputCamera.fieldOfView = state.Lens.FieldOfView;
        outputCamera.orthographicSize = state.Lens.OrthographicSize;
        outputCamera.nearClipPlane = state.Lens.NearClipPlane;
        outputCamera.farClipPlane = state.Lens.FarClipPlane;
      }
      if (CinemachineBrain.sPostProcessingHandler == null)
        return;
      CinemachineBrain.sPostProcessingHandler.Invoke(this);
    }

    private void AddSubframe()
    {
      int frameCount = Time.frameCount;
      if (frameCount == CinemachineBrain.msCurrentFrame)
      {
        if (CinemachineBrain.msFirstBrainObjectId != this.GetInstanceID())
          return;
        ++CinemachineBrain.msSubframes;
      }
      else
      {
        CinemachineBrain.msCurrentFrame = frameCount;
        CinemachineBrain.msFirstBrainObjectId = this.GetInstanceID();
        CinemachineBrain.msSubframes = 1;
      }
    }

    internal static int GetSubframeCount() => Math.Max(1, CinemachineBrain.msSubframes);

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

      public bool Active => this.camera != null;

      public bool Expired
      {
        get
        {
          return !Application.isPlaying && (double) Time.realtimeSinceStartup - (double) this.timeOfOverride > (double) Time.maximumDeltaTime;
        }
      }
    }
  }
}
