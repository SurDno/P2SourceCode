using System;
using UnityEngine;

namespace Cinemachine
{
  [SaveDuringPlay]
  public abstract class CinemachineVirtualCameraBase : MonoBehaviour, ICinemachineCamera
  {
    [HideInInspector]
    [NoSaveDuringPlay]
    public Action CinemachineGUIDebuggerCallback = (Action) null;
    [HideInInspector]
    [SerializeField]
    [NoSaveDuringPlay]
    public string[] m_ExcludedPropertiesInInspector = new string[1]
    {
      "m_Script"
    };
    [HideInInspector]
    [SerializeField]
    [NoSaveDuringPlay]
    public CinemachineCore.Stage[] m_LockStageInInspector;
    private int m_ValidatingStreamVersion = 0;
    private bool m_OnValidateCalled = false;
    [HideInInspector]
    [SerializeField]
    [NoSaveDuringPlay]
    private int m_StreamingVersion;
    [NoSaveDuringPlay]
    [Tooltip("The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority.")]
    public int m_Priority = 10;
    protected CinemachineVirtualCameraBase.OnPostPipelineStageDelegate OnPostPipelineStage;
    private bool m_previousStateIsValid;
    private Transform m_previousLookAtTarget;
    private Transform m_previousFollowTarget;
    private bool mSlaveStatusUpdated = false;
    private CinemachineVirtualCameraBase m_parentVcam = (CinemachineVirtualCameraBase) null;
    private int m_QueuePriority = int.MaxValue;

    public int ValidatingStreamVersion
    {
      get
      {
        return this.m_OnValidateCalled ? this.m_ValidatingStreamVersion : CinemachineCore.kStreamingVersion;
      }
      private set => this.m_ValidatingStreamVersion = value;
    }

    public virtual void AddPostPipelineStageHook(
      CinemachineVirtualCameraBase.OnPostPipelineStageDelegate d)
    {
      this.OnPostPipelineStage -= d;
      this.OnPostPipelineStage += d;
    }

    public virtual void RemovePostPipelineStageHook(
      CinemachineVirtualCameraBase.OnPostPipelineStageDelegate d)
    {
      this.OnPostPipelineStage -= d;
    }

    protected void InvokePostPipelineStageCallback(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState newState,
      float deltaTime)
    {
      if (this.OnPostPipelineStage != null)
        this.OnPostPipelineStage(vcam, stage, ref newState, deltaTime);
      CinemachineVirtualCameraBase parentCamera = this.ParentCamera as CinemachineVirtualCameraBase;
      if (!((UnityEngine.Object) parentCamera != (UnityEngine.Object) null))
        return;
      parentCamera.InvokePostPipelineStageCallback(vcam, stage, ref newState, deltaTime);
    }

    public string Name => this.name;

    public virtual string Description => "";

    public int Priority
    {
      get => this.m_Priority;
      set => this.m_Priority = value;
    }

    public GameObject VirtualCameraGameObject
    {
      get => (UnityEngine.Object) this == (UnityEngine.Object) null ? (GameObject) null : this.gameObject;
    }

    public abstract CameraState State { get; }

    public virtual ICinemachineCamera LiveChildOrSelf => (ICinemachineCamera) this;

    public ICinemachineCamera ParentCamera
    {
      get
      {
        if (!this.mSlaveStatusUpdated || !Application.isPlaying)
          this.UpdateSlaveStatus();
        return (ICinemachineCamera) this.m_parentVcam;
      }
    }

    public virtual bool IsLiveChild(ICinemachineCamera vcam) => false;

    public abstract Transform LookAt { get; set; }

    public abstract Transform Follow { get; set; }

    public bool PreviousStateIsValid
    {
      get
      {
        if ((UnityEngine.Object) this.LookAt != (UnityEngine.Object) this.m_previousLookAtTarget)
        {
          this.m_previousLookAtTarget = this.LookAt;
          this.m_previousStateIsValid = false;
        }
        if ((UnityEngine.Object) this.Follow != (UnityEngine.Object) this.m_previousFollowTarget)
        {
          this.m_previousFollowTarget = this.Follow;
          this.m_previousStateIsValid = false;
        }
        return this.m_previousStateIsValid;
      }
      set => this.m_previousStateIsValid = value;
    }

    public abstract void UpdateCameraState(Vector3 worldUp, float deltaTime);

    public virtual void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
      if (this.gameObject.activeInHierarchy)
        return;
      this.PreviousStateIsValid = false;
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnDestroy()
    {
      CinemachineCore.Instance.RemoveActiveCamera((ICinemachineCamera) this);
    }

    protected virtual void OnValidate()
    {
      this.m_OnValidateCalled = true;
      this.ValidatingStreamVersion = this.m_StreamingVersion;
      this.m_StreamingVersion = CinemachineCore.kStreamingVersion;
    }

    protected virtual void OnEnable()
    {
      CinemachineVirtualCameraBase[] components = this.GetComponents<CinemachineVirtualCameraBase>();
      for (int index = 0; index < components.Length; ++index)
      {
        if (components[index].enabled && (UnityEngine.Object) components[index] != (UnityEngine.Object) this)
        {
          Debug.LogError((object) (this.Name + " has multiple CinemachineVirtualCameraBase-derived components.  Disabling " + ((object) this).GetType().Name + "."));
          this.enabled = false;
        }
      }
      this.UpdateSlaveStatus();
      this.UpdateVcamPoolStatus();
      this.PreviousStateIsValid = false;
    }

    protected virtual void OnDisable() => this.UpdateVcamPoolStatus();

    protected virtual void Update()
    {
      if (this.m_Priority == this.m_QueuePriority)
        return;
      this.UpdateVcamPoolStatus();
    }

    protected virtual void OnTransformParentChanged()
    {
      this.UpdateSlaveStatus();
      this.UpdateVcamPoolStatus();
    }

    private void UpdateSlaveStatus()
    {
      this.mSlaveStatusUpdated = true;
      this.m_parentVcam = (CinemachineVirtualCameraBase) null;
      Transform parent = this.transform.parent;
      if (!((UnityEngine.Object) parent != (UnityEngine.Object) null))
        return;
      this.m_parentVcam = parent.GetComponent<CinemachineVirtualCameraBase>();
    }

    protected Transform ResolveLookAt(Transform localLookAt)
    {
      Transform transform = localLookAt;
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null && this.ParentCamera != null)
        transform = this.ParentCamera.LookAt;
      return transform;
    }

    protected Transform ResolveFollow(Transform localFollow)
    {
      Transform transform = localFollow;
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null && this.ParentCamera != null)
        transform = this.ParentCamera.Follow;
      return transform;
    }

    private void UpdateVcamPoolStatus()
    {
      this.m_QueuePriority = int.MaxValue;
      CinemachineCore.Instance.RemoveActiveCamera((ICinemachineCamera) this);
      CinemachineCore.Instance.RemoveChildCamera((ICinemachineCamera) this);
      if ((UnityEngine.Object) this.m_parentVcam == (UnityEngine.Object) null)
      {
        if (!this.isActiveAndEnabled)
          return;
        CinemachineCore.Instance.AddActiveCamera((ICinemachineCamera) this);
        this.m_QueuePriority = this.m_Priority;
      }
      else if (this.isActiveAndEnabled)
        CinemachineCore.Instance.AddChildCamera((ICinemachineCamera) this);
    }

    public void MoveToTopOfPrioritySubqueue() => this.UpdateVcamPoolStatus();

    public delegate void OnPostPipelineStageDelegate(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState newState,
      float deltaTime);
  }
}
