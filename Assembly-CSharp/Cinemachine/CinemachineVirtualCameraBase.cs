using System;

namespace Cinemachine
{
  [SaveDuringPlay]
  public abstract class CinemachineVirtualCameraBase : MonoBehaviour, ICinemachineCamera
  {
    [HideInInspector]
    [NoSaveDuringPlay]
    public Action CinemachineGUIDebuggerCallback = null;
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
    private int m_ValidatingStreamVersion;
    private bool m_OnValidateCalled;
    [HideInInspector]
    [SerializeField]
    [NoSaveDuringPlay]
    private int m_StreamingVersion;
    [NoSaveDuringPlay]
    [Tooltip("The priority will determine which camera becomes active based on the state of other cameras and this camera.  Higher numbers have greater priority.")]
    public int m_Priority = 10;
    protected OnPostPipelineStageDelegate OnPostPipelineStage;
    private bool m_previousStateIsValid;
    private Transform m_previousLookAtTarget;
    private Transform m_previousFollowTarget;
    private bool mSlaveStatusUpdated;
    private CinemachineVirtualCameraBase m_parentVcam = null;
    private int m_QueuePriority = int.MaxValue;

    public int ValidatingStreamVersion
    {
      get
      {
        return m_OnValidateCalled ? m_ValidatingStreamVersion : CinemachineCore.kStreamingVersion;
      }
      private set => m_ValidatingStreamVersion = value;
    }

    public virtual void AddPostPipelineStageHook(
      OnPostPipelineStageDelegate d)
    {
      OnPostPipelineStage -= d;
      OnPostPipelineStage += d;
    }

    public virtual void RemovePostPipelineStageHook(
      OnPostPipelineStageDelegate d)
    {
      OnPostPipelineStage -= d;
    }

    protected void InvokePostPipelineStageCallback(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState newState,
      float deltaTime)
    {
      if (OnPostPipelineStage != null)
        OnPostPipelineStage(vcam, stage, ref newState, deltaTime);
      CinemachineVirtualCameraBase parentCamera = ParentCamera as CinemachineVirtualCameraBase;
      if (!((UnityEngine.Object) parentCamera != (UnityEngine.Object) null))
        return;
      parentCamera.InvokePostPipelineStageCallback(vcam, stage, ref newState, deltaTime);
    }

    public string Name => this.name;

    public virtual string Description => "";

    public int Priority
    {
      get => m_Priority;
      set => m_Priority = value;
    }

    public GameObject VirtualCameraGameObject
    {
      get => (UnityEngine.Object) this == (UnityEngine.Object) null ? (GameObject) null : this.gameObject;
    }

    public abstract CameraState State { get; }

    public virtual ICinemachineCamera LiveChildOrSelf => this;

    public ICinemachineCamera ParentCamera
    {
      get
      {
        if (!mSlaveStatusUpdated || !Application.isPlaying)
          UpdateSlaveStatus();
        return m_parentVcam;
      }
    }

    public virtual bool IsLiveChild(ICinemachineCamera vcam) => false;

    public abstract Transform LookAt { get; set; }

    public abstract Transform Follow { get; set; }

    public bool PreviousStateIsValid
    {
      get
      {
        if ((UnityEngine.Object) LookAt != (UnityEngine.Object) m_previousLookAtTarget)
        {
          m_previousLookAtTarget = LookAt;
          m_previousStateIsValid = false;
        }
        if ((UnityEngine.Object) Follow != (UnityEngine.Object) m_previousFollowTarget)
        {
          m_previousFollowTarget = Follow;
          m_previousStateIsValid = false;
        }
        return m_previousStateIsValid;
      }
      set => m_previousStateIsValid = value;
    }

    public abstract void UpdateCameraState(Vector3 worldUp, float deltaTime);

    public virtual void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
      if (this.gameObject.activeInHierarchy)
        return;
      PreviousStateIsValid = false;
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnDestroy()
    {
      CinemachineCore.Instance.RemoveActiveCamera(this);
    }

    protected virtual void OnValidate()
    {
      m_OnValidateCalled = true;
      ValidatingStreamVersion = m_StreamingVersion;
      m_StreamingVersion = CinemachineCore.kStreamingVersion;
    }

    protected virtual void OnEnable()
    {
      CinemachineVirtualCameraBase[] components = this.GetComponents<CinemachineVirtualCameraBase>();
      for (int index = 0; index < components.Length; ++index)
      {
        if (components[index].enabled && (UnityEngine.Object) components[index] != (UnityEngine.Object) this)
        {
          Debug.LogError((object) (Name + " has multiple CinemachineVirtualCameraBase-derived components.  Disabling " + this.GetType().Name + "."));
          this.enabled = false;
        }
      }
      UpdateSlaveStatus();
      UpdateVcamPoolStatus();
      PreviousStateIsValid = false;
    }

    protected virtual void OnDisable() => UpdateVcamPoolStatus();

    protected virtual void Update()
    {
      if (m_Priority == m_QueuePriority)
        return;
      UpdateVcamPoolStatus();
    }

    protected virtual void OnTransformParentChanged()
    {
      UpdateSlaveStatus();
      UpdateVcamPoolStatus();
    }

    private void UpdateSlaveStatus()
    {
      mSlaveStatusUpdated = true;
      m_parentVcam = null;
      Transform parent = this.transform.parent;
      if (!((UnityEngine.Object) parent != (UnityEngine.Object) null))
        return;
      m_parentVcam = parent.GetComponent<CinemachineVirtualCameraBase>();
    }

    protected Transform ResolveLookAt(Transform localLookAt)
    {
      Transform transform = localLookAt;
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null && ParentCamera != null)
        transform = ParentCamera.LookAt;
      return transform;
    }

    protected Transform ResolveFollow(Transform localFollow)
    {
      Transform transform = localFollow;
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null && ParentCamera != null)
        transform = ParentCamera.Follow;
      return transform;
    }

    private void UpdateVcamPoolStatus()
    {
      m_QueuePriority = int.MaxValue;
      CinemachineCore.Instance.RemoveActiveCamera(this);
      CinemachineCore.Instance.RemoveChildCamera(this);
      if ((UnityEngine.Object) m_parentVcam == (UnityEngine.Object) null)
      {
        if (!this.isActiveAndEnabled)
          return;
        CinemachineCore.Instance.AddActiveCamera(this);
        m_QueuePriority = m_Priority;
      }
      else if (this.isActiveAndEnabled)
        CinemachineCore.Instance.AddChildCamera(this);
    }

    public void MoveToTopOfPrioritySubqueue() => UpdateVcamPoolStatus();

    public delegate void OnPostPipelineStageDelegate(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState newState,
      float deltaTime);
  }
}
