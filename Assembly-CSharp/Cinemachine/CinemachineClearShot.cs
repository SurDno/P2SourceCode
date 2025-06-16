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
    public Transform m_LookAt;
    [Tooltip("Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all children specify targets of their own.")]
    [NoSaveDuringPlay]
    public Transform m_Follow;
    [Tooltip("When enabled, the current child camera and blend will be indicated in the game window, for debugging")]
    [NoSaveDuringPlay]
    public bool m_ShowDebugText;
    [SerializeField]
    [HideInInspector]
    [NoSaveDuringPlay]
    public CinemachineVirtualCameraBase[] m_ChildCameras;
    [Tooltip("Wait this many seconds before activating a new child camera")]
    public float m_ActivateAfter;
    [Tooltip("An active camera must be active for at least this many seconds")]
    public float m_MinDuration;
    [Tooltip("If checked, camera choice will be randomized if multiple cameras are equally desirable.  Otherwise, child list order and child camera priority will be used.")]
    public bool m_RandomizeChoice;
    [CinemachineBlendDefinitionProperty]
    [Tooltip("The blend which is used if you don't explicitly define a blend between two Virtual Cameras")]
    public CinemachineBlendDefinition m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0.0f);
    [HideInInspector]
    public CinemachineBlenderSettings m_CustomBlends;
    private CameraState m_State = CameraState.Default;
    private float mActivationTime;
    private float mPendingActivationTime;
    private ICinemachineCamera mPendingCamera;
    private CinemachineBlend mActiveBlend;
    private bool mRandomizeNow;
    private CinemachineVirtualCameraBase[] m_RandomizedChilden;

    public override string Description
    {
      get
      {
        ICinemachineCamera liveChild = LiveChild;
        return mActiveBlend == null ? (liveChild != null ? "[" + liveChild.Name + "]" : "(none)") : mActiveBlend.Description;
      }
    }

    public ICinemachineCamera LiveChild { set; get; }

    public override CameraState State => m_State;

    public override ICinemachineCamera LiveChildOrSelf => LiveChild;

    public override bool IsLiveChild(ICinemachineCamera vcam)
    {
      return vcam == LiveChild || mActiveBlend != null && (vcam == mActiveBlend.CamA || vcam == mActiveBlend.CamB);
    }

    public override Transform LookAt
    {
      get => ResolveLookAt(m_LookAt);
      set => m_LookAt = value;
    }

    public override Transform Follow
    {
      get => ResolveFollow(m_Follow);
      set => m_Follow = value;
    }

    public override void RemovePostPipelineStageHook(
      OnPostPipelineStageDelegate d)
    {
      base.RemovePostPipelineStageHook(d);
      UpdateListOfChildren();
      foreach (CinemachineVirtualCameraBase childCamera in m_ChildCameras)
        childCamera.RemovePostPipelineStageHook(d);
    }

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      if (!PreviousStateIsValid)
        deltaTime = -1f;
      UpdateListOfChildren();
      ICinemachineCamera liveChild = LiveChild;
      LiveChild = ChooseCurrentCamera(worldUp, deltaTime);
      if (liveChild != null && LiveChild != null && liveChild != LiveChild)
      {
        float duration = 0.0f;
        AnimationCurve blendCurve = LookupBlendCurve(liveChild, LiveChild, out duration);
        mActiveBlend = CreateBlend(liveChild, LiveChild, blendCurve, duration, mActiveBlend, deltaTime);
        LiveChild.OnTransitionFromCamera(liveChild, worldUp, deltaTime);
        CinemachineCore.Instance.GenerateCameraActivationEvent(LiveChild);
        if (mActiveBlend == null)
          CinemachineCore.Instance.GenerateCameraCutEvent(LiveChild);
      }
      if (mActiveBlend != null)
      {
        mActiveBlend.TimeInBlend += deltaTime >= 0.0 ? deltaTime : mActiveBlend.Duration;
        if (mActiveBlend.IsComplete)
          mActiveBlend = null;
      }
      if (mActiveBlend != null)
      {
        mActiveBlend.UpdateCameraState(worldUp, deltaTime);
        m_State = mActiveBlend.State;
      }
      else if (LiveChild != null)
        m_State = LiveChild.State;
      PreviousStateIsValid = true;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      InvalidateListOfChildren();
      mActiveBlend = null;
    }

    public void OnTransformChildrenChanged() => InvalidateListOfChildren();

    public bool IsBlending => mActiveBlend != null;

    public CinemachineVirtualCameraBase[] ChildCameras
    {
      get
      {
        UpdateListOfChildren();
        return m_ChildCameras;
      }
    }

    private void InvalidateListOfChildren()
    {
      m_ChildCameras = null;
      m_RandomizedChilden = null;
      LiveChild = null;
    }

    public void ResetRandomization()
    {
      m_RandomizedChilden = null;
      mRandomizeNow = true;
    }

    private void UpdateListOfChildren()
    {
      if (m_ChildCameras != null)
        return;
      List<CinemachineVirtualCameraBase> virtualCameraBaseList = new List<CinemachineVirtualCameraBase>();
      foreach (CinemachineVirtualCameraBase componentsInChild in GetComponentsInChildren<CinemachineVirtualCameraBase>(true))
      {
        if (componentsInChild.transform.parent == transform)
          virtualCameraBaseList.Add(componentsInChild);
      }
      m_ChildCameras = virtualCameraBaseList.ToArray();
      mActivationTime = mPendingActivationTime = 0.0f;
      mPendingCamera = null;
      LiveChild = null;
      mActiveBlend = null;
    }

    private ICinemachineCamera ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
    {
      if (m_ChildCameras == null || m_ChildCameras.Length == 0)
      {
        mActivationTime = 0.0f;
        return null;
      }
      CinemachineVirtualCameraBase[] virtualCameraBaseArray = m_ChildCameras;
      if (!m_RandomizeChoice)
        m_RandomizedChilden = null;
      else if (m_ChildCameras.Length > 1)
      {
        if (m_RandomizedChilden == null)
          m_RandomizedChilden = Randomize(m_ChildCameras);
        virtualCameraBaseArray = m_RandomizedChilden;
      }
      if (LiveChild != null && !LiveChild.VirtualCameraGameObject.activeSelf)
        LiveChild = null;
      ICinemachineCamera cinemachineCamera = LiveChild;
      for (int index = 0; index < virtualCameraBaseArray.Length; ++index)
      {
        CinemachineVirtualCameraBase virtualCameraBase = virtualCameraBaseArray[index];
        if (virtualCameraBase != null && virtualCameraBase.VirtualCameraGameObject.activeInHierarchy)
        {
          int num;
          if (cinemachineCamera != null)
          {
            CameraState state = virtualCameraBase.State;
            double shotQuality1 = state.ShotQuality;
            state = cinemachineCamera.State;
            double shotQuality2 = state.ShotQuality;
            if (shotQuality1 <= shotQuality2)
            {
              state = virtualCameraBase.State;
              double shotQuality3 = state.ShotQuality;
              state = cinemachineCamera.State;
              double shotQuality4 = state.ShotQuality;
              if (shotQuality3 != shotQuality4 || virtualCameraBase.Priority <= cinemachineCamera.Priority)
              {
                if (m_RandomizeChoice && mRandomizeNow && virtualCameraBase != LiveChild)
                {
                  state = virtualCameraBase.State;
                  double shotQuality5 = state.ShotQuality;
                  state = cinemachineCamera.State;
                  double shotQuality6 = state.ShotQuality;
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
            cinemachineCamera = virtualCameraBase;
        }
      }
      mRandomizeNow = false;
      float time = Time.time;
      if (mActivationTime != 0.0)
      {
        if (LiveChild == cinemachineCamera)
        {
          mPendingActivationTime = 0.0f;
          mPendingCamera = null;
          return cinemachineCamera;
        }
        if (deltaTime >= 0.0 && mPendingActivationTime != 0.0 && mPendingCamera == cinemachineCamera)
        {
          if (time - (double) mPendingActivationTime <= m_ActivateAfter || time - (double) mActivationTime <= m_MinDuration)
            return LiveChild;
          m_RandomizedChilden = null;
          mActivationTime = time;
          mPendingActivationTime = 0.0f;
          mPendingCamera = null;
          return cinemachineCamera;
        }
      }
      mPendingActivationTime = 0.0f;
      mPendingCamera = null;
      if (deltaTime >= 0.0 && mActivationTime > 0.0 && (m_ActivateAfter > 0.0 || time - (double) mActivationTime < m_MinDuration))
      {
        mPendingCamera = cinemachineCamera;
        mPendingActivationTime = time;
        return LiveChild;
      }
      m_RandomizedChilden = null;
      mActivationTime = time;
      return cinemachineCamera;
    }

    private CinemachineVirtualCameraBase[] Randomize(CinemachineVirtualCameraBase[] src)
    {
      List<Pair> pairList = new List<Pair>();
      for (int index = 0; index < src.Length; ++index)
        pairList.Add(new Pair {
          a = index,
          b = Random.Range(0.0f, 1000f)
        });
      pairList.Sort((p1, p2) => (int) p1.b - (int) p2.b);
      CinemachineVirtualCameraBase[] virtualCameraBaseArray = new CinemachineVirtualCameraBase[src.Length];
      Pair[] array = pairList.ToArray();
      for (int index = 0; index < src.Length; ++index)
        virtualCameraBaseArray[index] = src[array[index].a];
      return virtualCameraBaseArray;
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
      CinemachineBlend activeBlend,
      float deltaTime)
    {
      if (blendCurve == null || duration <= 0.0 || camA == null && camB == null)
        return null;
      if (camA == null || activeBlend != null)
        camA = new StaticPointVirtualCamera(activeBlend != null ? activeBlend.State : State, activeBlend != null ? "Mid-blend" : "(none)");
      return new CinemachineBlend(camA, camB, blendCurve, duration, 0.0f);
    }

    public override void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
      base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
      if (!m_RandomizeChoice || mActiveBlend != null)
        return;
      m_RandomizedChilden = null;
      LiveChild = null;
      UpdateCameraState(worldUp, deltaTime);
    }

    private struct Pair
    {
      public int a;
      public float b;
    }
  }
}
