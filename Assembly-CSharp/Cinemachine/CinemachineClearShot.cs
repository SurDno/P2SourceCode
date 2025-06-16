using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(12f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachineClearShot")]
  public class CinemachineClearShot : CinemachineVirtualCameraBase
  {
    [Tooltip("Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty if all children specify targets of their own.")]
    [NoSaveDuringPlay]
    public Transform m_LookAt = (Transform) null;
    [Tooltip("Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all children specify targets of their own.")]
    [NoSaveDuringPlay]
    public Transform m_Follow = (Transform) null;
    [Tooltip("When enabled, the current child camera and blend will be indicated in the game window, for debugging")]
    [NoSaveDuringPlay]
    public bool m_ShowDebugText = false;
    [SerializeField]
    [HideInInspector]
    [NoSaveDuringPlay]
    public CinemachineVirtualCameraBase[] m_ChildCameras = (CinemachineVirtualCameraBase[]) null;
    [Tooltip("Wait this many seconds before activating a new child camera")]
    public float m_ActivateAfter;
    [Tooltip("An active camera must be active for at least this many seconds")]
    public float m_MinDuration;
    [Tooltip("If checked, camera choice will be randomized if multiple cameras are equally desirable.  Otherwise, child list order and child camera priority will be used.")]
    public bool m_RandomizeChoice = false;
    [CinemachineBlendDefinitionProperty]
    [Tooltip("The blend which is used if you don't explicitly define a blend between two Virtual Cameras")]
    public CinemachineBlendDefinition m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0.0f);
    [HideInInspector]
    public CinemachineBlenderSettings m_CustomBlends = (CinemachineBlenderSettings) null;
    private CameraState m_State = CameraState.Default;
    private float mActivationTime = 0.0f;
    private float mPendingActivationTime = 0.0f;
    private ICinemachineCamera mPendingCamera;
    private CinemachineBlend mActiveBlend = (CinemachineBlend) null;
    private bool mRandomizeNow = false;
    private CinemachineVirtualCameraBase[] m_RandomizedChilden = (CinemachineVirtualCameraBase[]) null;

    public override string Description
    {
      get
      {
        ICinemachineCamera liveChild = this.LiveChild;
        return this.mActiveBlend == null ? (liveChild != null ? "[" + liveChild.Name + "]" : "(none)") : this.mActiveBlend.Description;
      }
    }

    public ICinemachineCamera LiveChild { set; get; }

    public override CameraState State => this.m_State;

    public override ICinemachineCamera LiveChildOrSelf => this.LiveChild;

    public override bool IsLiveChild(ICinemachineCamera vcam)
    {
      return vcam == this.LiveChild || this.mActiveBlend != null && (vcam == this.mActiveBlend.CamA || vcam == this.mActiveBlend.CamB);
    }

    public override Transform LookAt
    {
      get => this.ResolveLookAt(this.m_LookAt);
      set => this.m_LookAt = value;
    }

    public override Transform Follow
    {
      get => this.ResolveFollow(this.m_Follow);
      set => this.m_Follow = value;
    }

    public override void RemovePostPipelineStageHook(
      CinemachineVirtualCameraBase.OnPostPipelineStageDelegate d)
    {
      base.RemovePostPipelineStageHook(d);
      this.UpdateListOfChildren();
      foreach (CinemachineVirtualCameraBase childCamera in this.m_ChildCameras)
        childCamera.RemovePostPipelineStageHook(d);
    }

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      if (!this.PreviousStateIsValid)
        deltaTime = -1f;
      this.UpdateListOfChildren();
      ICinemachineCamera liveChild = this.LiveChild;
      this.LiveChild = this.ChooseCurrentCamera(worldUp, deltaTime);
      if (liveChild != null && this.LiveChild != null && liveChild != this.LiveChild)
      {
        float duration = 0.0f;
        AnimationCurve blendCurve = this.LookupBlendCurve(liveChild, this.LiveChild, out duration);
        this.mActiveBlend = this.CreateBlend(liveChild, this.LiveChild, blendCurve, duration, this.mActiveBlend, deltaTime);
        this.LiveChild.OnTransitionFromCamera(liveChild, worldUp, deltaTime);
        CinemachineCore.Instance.GenerateCameraActivationEvent(this.LiveChild);
        if (this.mActiveBlend == null)
          CinemachineCore.Instance.GenerateCameraCutEvent(this.LiveChild);
      }
      if (this.mActiveBlend != null)
      {
        this.mActiveBlend.TimeInBlend += (double) deltaTime >= 0.0 ? deltaTime : this.mActiveBlend.Duration;
        if (this.mActiveBlend.IsComplete)
          this.mActiveBlend = (CinemachineBlend) null;
      }
      if (this.mActiveBlend != null)
      {
        this.mActiveBlend.UpdateCameraState(worldUp, deltaTime);
        this.m_State = this.mActiveBlend.State;
      }
      else if (this.LiveChild != null)
        this.m_State = this.LiveChild.State;
      this.PreviousStateIsValid = true;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.InvalidateListOfChildren();
      this.mActiveBlend = (CinemachineBlend) null;
    }

    public void OnTransformChildrenChanged() => this.InvalidateListOfChildren();

    public bool IsBlending => this.mActiveBlend != null;

    public CinemachineVirtualCameraBase[] ChildCameras
    {
      get
      {
        this.UpdateListOfChildren();
        return this.m_ChildCameras;
      }
    }

    private void InvalidateListOfChildren()
    {
      this.m_ChildCameras = (CinemachineVirtualCameraBase[]) null;
      this.m_RandomizedChilden = (CinemachineVirtualCameraBase[]) null;
      this.LiveChild = (ICinemachineCamera) null;
    }

    public void ResetRandomization()
    {
      this.m_RandomizedChilden = (CinemachineVirtualCameraBase[]) null;
      this.mRandomizeNow = true;
    }

    private void UpdateListOfChildren()
    {
      if (this.m_ChildCameras != null)
        return;
      List<CinemachineVirtualCameraBase> virtualCameraBaseList = new List<CinemachineVirtualCameraBase>();
      foreach (CinemachineVirtualCameraBase componentsInChild in this.GetComponentsInChildren<CinemachineVirtualCameraBase>(true))
      {
        if ((UnityEngine.Object) componentsInChild.transform.parent == (UnityEngine.Object) this.transform)
          virtualCameraBaseList.Add(componentsInChild);
      }
      this.m_ChildCameras = virtualCameraBaseList.ToArray();
      this.mActivationTime = this.mPendingActivationTime = 0.0f;
      this.mPendingCamera = (ICinemachineCamera) null;
      this.LiveChild = (ICinemachineCamera) null;
      this.mActiveBlend = (CinemachineBlend) null;
    }

    private ICinemachineCamera ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
    {
      if (this.m_ChildCameras == null || this.m_ChildCameras.Length == 0)
      {
        this.mActivationTime = 0.0f;
        return (ICinemachineCamera) null;
      }
      CinemachineVirtualCameraBase[] virtualCameraBaseArray = this.m_ChildCameras;
      if (!this.m_RandomizeChoice)
        this.m_RandomizedChilden = (CinemachineVirtualCameraBase[]) null;
      else if (this.m_ChildCameras.Length > 1)
      {
        if (this.m_RandomizedChilden == null)
          this.m_RandomizedChilden = this.Randomize(this.m_ChildCameras);
        virtualCameraBaseArray = this.m_RandomizedChilden;
      }
      if (this.LiveChild != null && !this.LiveChild.VirtualCameraGameObject.activeSelf)
        this.LiveChild = (ICinemachineCamera) null;
      ICinemachineCamera cinemachineCamera = this.LiveChild;
      for (int index = 0; index < virtualCameraBaseArray.Length; ++index)
      {
        CinemachineVirtualCameraBase virtualCameraBase = virtualCameraBaseArray[index];
        if ((UnityEngine.Object) virtualCameraBase != (UnityEngine.Object) null && virtualCameraBase.VirtualCameraGameObject.activeInHierarchy)
        {
          int num;
          if (cinemachineCamera != null)
          {
            CameraState state = virtualCameraBase.State;
            double shotQuality1 = (double) state.ShotQuality;
            state = cinemachineCamera.State;
            double shotQuality2 = (double) state.ShotQuality;
            if (shotQuality1 <= shotQuality2)
            {
              state = virtualCameraBase.State;
              double shotQuality3 = (double) state.ShotQuality;
              state = cinemachineCamera.State;
              double shotQuality4 = (double) state.ShotQuality;
              if (shotQuality3 != shotQuality4 || virtualCameraBase.Priority <= cinemachineCamera.Priority)
              {
                if (this.m_RandomizeChoice && this.mRandomizeNow && virtualCameraBase != this.LiveChild)
                {
                  state = virtualCameraBase.State;
                  double shotQuality5 = (double) state.ShotQuality;
                  state = cinemachineCamera.State;
                  double shotQuality6 = (double) state.ShotQuality;
                  if (shotQuality5 == shotQuality6)
                  {
                    num = virtualCameraBase.Priority == cinemachineCamera.Priority ? 1 : 0;
                    goto label_20;
                  }
                }
                num = 0;
                goto label_20;
              }
            }
          }
          num = 1;
label_20:
          if (num != 0)
            cinemachineCamera = (ICinemachineCamera) virtualCameraBase;
        }
      }
      this.mRandomizeNow = false;
      float time = Time.time;
      if ((double) this.mActivationTime != 0.0)
      {
        if (this.LiveChild == cinemachineCamera)
        {
          this.mPendingActivationTime = 0.0f;
          this.mPendingCamera = (ICinemachineCamera) null;
          return cinemachineCamera;
        }
        if ((double) deltaTime >= 0.0 && (double) this.mPendingActivationTime != 0.0 && this.mPendingCamera == cinemachineCamera)
        {
          if ((double) time - (double) this.mPendingActivationTime <= (double) this.m_ActivateAfter || (double) time - (double) this.mActivationTime <= (double) this.m_MinDuration)
            return this.LiveChild;
          this.m_RandomizedChilden = (CinemachineVirtualCameraBase[]) null;
          this.mActivationTime = time;
          this.mPendingActivationTime = 0.0f;
          this.mPendingCamera = (ICinemachineCamera) null;
          return cinemachineCamera;
        }
      }
      this.mPendingActivationTime = 0.0f;
      this.mPendingCamera = (ICinemachineCamera) null;
      if ((double) deltaTime >= 0.0 && (double) this.mActivationTime > 0.0 && ((double) this.m_ActivateAfter > 0.0 || (double) time - (double) this.mActivationTime < (double) this.m_MinDuration))
      {
        this.mPendingCamera = cinemachineCamera;
        this.mPendingActivationTime = time;
        return this.LiveChild;
      }
      this.m_RandomizedChilden = (CinemachineVirtualCameraBase[]) null;
      this.mActivationTime = time;
      return cinemachineCamera;
    }

    private CinemachineVirtualCameraBase[] Randomize(CinemachineVirtualCameraBase[] src)
    {
      List<CinemachineClearShot.Pair> pairList = new List<CinemachineClearShot.Pair>();
      for (int index = 0; index < src.Length; ++index)
        pairList.Add(new CinemachineClearShot.Pair()
        {
          a = index,
          b = UnityEngine.Random.Range(0.0f, 1000f)
        });
      pairList.Sort((Comparison<CinemachineClearShot.Pair>) ((p1, p2) => (int) p1.b - (int) p2.b));
      CinemachineVirtualCameraBase[] virtualCameraBaseArray = new CinemachineVirtualCameraBase[src.Length];
      CinemachineClearShot.Pair[] array = pairList.ToArray();
      for (int index = 0; index < src.Length; ++index)
        virtualCameraBaseArray[index] = src[array[index].a];
      return virtualCameraBaseArray;
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
      CinemachineBlend activeBlend,
      float deltaTime)
    {
      if (blendCurve == null || (double) duration <= 0.0 || camA == null && camB == null)
        return (CinemachineBlend) null;
      if (camA == null || activeBlend != null)
        camA = (ICinemachineCamera) new StaticPointVirtualCamera(activeBlend != null ? activeBlend.State : this.State, activeBlend != null ? "Mid-blend" : "(none)");
      return new CinemachineBlend(camA, camB, blendCurve, duration, 0.0f);
    }

    public override void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
      base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
      if (!this.m_RandomizeChoice || this.mActiveBlend != null)
        return;
      this.m_RandomizedChilden = (CinemachineVirtualCameraBase[]) null;
      this.LiveChild = (ICinemachineCamera) null;
      this.UpdateCameraState(worldUp, deltaTime);
    }

    private struct Pair
    {
      public int a;
      public float b;
    }
  }
}
