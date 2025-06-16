using System.Collections.Generic;
using Cinemachine.Utility;

namespace Cinemachine
{
  [DocumentationSorting(1f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachineVirtualCamera")]
  public class CinemachineVirtualCamera : CinemachineVirtualCameraBase
  {
    [Tooltip("The object that the camera wants to look at (the Aim target).  If this is null, then the vcam's Transform orientation will define the camera's orientation.")]
    [NoSaveDuringPlay]
    public Transform m_LookAt = (Transform) null;
    [Tooltip("The object that the camera wants to move with (the Body target).  If this is null, then the vcam's Transform position will define the camera's position.")]
    [NoSaveDuringPlay]
    public Transform m_Follow = (Transform) null;
    [FormerlySerializedAs("m_LensAttributes")]
    [Tooltip("Specifies the lens properties of this Virtual Camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active.")]
    [LensSettingsProperty]
    public LensSettings m_Lens = LensSettings.Default;
    public const string PipelineName = "cm";
    public static CreatePipelineDelegate CreatePipelineOverride;
    public static DestroyPipelineDelegate DestroyPipelineOverride;
    private CameraState m_State = CameraState.Default;
    private CinemachineComponentBase[] m_ComponentPipeline = null;
    [SerializeField]
    [HideInInspector]
    private Transform m_ComponentOwner = (Transform) null;

    public override CameraState State => m_State;

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

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      if (!PreviousStateIsValid)
        deltaTime = -1f;
      if (deltaTime < 0.0)
        m_State = PullStateFromVirtualCamera(worldUp);
      m_State = CalculateNewState(worldUp, deltaTime);
      if (!UserIsDragging)
      {
        CameraState state;
        if ((UnityEngine.Object) Follow != (UnityEngine.Object) null)
        {
          Transform transform = this.transform;
          state = State;
          Vector3 rawPosition = state.RawPosition;
          transform.position = rawPosition;
        }
        if ((UnityEngine.Object) LookAt != (UnityEngine.Object) null)
        {
          Transform transform = this.transform;
          state = State;
          Quaternion rawOrientation = state.RawOrientation;
          transform.rotation = rawOrientation;
        }
      }
      PreviousStateIsValid = true;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      InvalidateComponentPipeline();
      if (ValidatingStreamVersion >= 20170927)
        return;
      if ((UnityEngine.Object) Follow != (UnityEngine.Object) null && (UnityEngine.Object) GetCinemachineComponent(CinemachineCore.Stage.Body) == (UnityEngine.Object) null)
        AddCinemachineComponent<CinemachineHardLockToTarget>();
      if ((UnityEngine.Object) LookAt != (UnityEngine.Object) null && (UnityEngine.Object) GetCinemachineComponent(CinemachineCore.Stage.Aim) == (UnityEngine.Object) null)
        AddCinemachineComponent<CinemachineHardLookAt>();
    }

    protected override void OnDestroy()
    {
      foreach (Transform transform in this.transform)
      {
        if ((UnityEngine.Object) transform.GetComponent<CinemachinePipeline>() != (UnityEngine.Object) null)
          transform.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
      }
      base.OnDestroy();
    }

    protected override void OnValidate()
    {
      base.OnValidate();
      m_Lens.Validate();
    }

    private void OnTransformChildrenChanged() => InvalidateComponentPipeline();

    private void Reset() => DestroyPipeline();

    private void DestroyPipeline()
    {
      List<Transform> transformList = new List<Transform>();
      foreach (Transform transform in this.transform)
      {
        if ((UnityEngine.Object) transform.GetComponent<CinemachinePipeline>() != (UnityEngine.Object) null)
          transformList.Add(transform);
      }
      foreach (Transform transform in transformList)
      {
        if (DestroyPipelineOverride != null)
          DestroyPipelineOverride(transform.gameObject);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) transform.gameObject);
      }
      m_ComponentOwner = (Transform) null;
      PreviousStateIsValid = false;
    }

    private Transform CreatePipeline(CinemachineVirtualCamera copyFrom)
    {
      CinemachineComponentBase[] copyFrom1 = null;
      if ((UnityEngine.Object) copyFrom != (UnityEngine.Object) null)
      {
        copyFrom.InvalidateComponentPipeline();
        copyFrom1 = copyFrom.GetComponentPipeline();
      }
      Transform pipeline;
      if (CreatePipelineOverride != null)
      {
        pipeline = CreatePipelineOverride(this, "cm", copyFrom1);
      }
      else
      {
        GameObject gameObject = new GameObject("cm");
        gameObject.transform.parent = this.transform;
        gameObject.AddComponent<CinemachinePipeline>();
        pipeline = gameObject.transform;
        if (copyFrom1 != null)
        {
          foreach (Component src in copyFrom1)
            ReflectionHelpers.CopyFields((object) src, (object) gameObject.AddComponent(((object) src).GetType()));
        }
      }
      PreviousStateIsValid = false;
      return pipeline;
    }

    public void InvalidateComponentPipeline()
    {
      m_ComponentPipeline = null;
    }

    public Transform GetComponentOwner()
    {
      UpdateComponentPipeline();
      return m_ComponentOwner;
    }

    public CinemachineComponentBase[] GetComponentPipeline()
    {
      UpdateComponentPipeline();
      return m_ComponentPipeline;
    }

    public CinemachineComponentBase GetCinemachineComponent(CinemachineCore.Stage stage)
    {
      CinemachineComponentBase[] componentPipeline = GetComponentPipeline();
      if (componentPipeline != null)
      {
        foreach (CinemachineComponentBase cinemachineComponent in componentPipeline)
        {
          if (cinemachineComponent.Stage == stage)
            return cinemachineComponent;
        }
      }
      return null;
    }

    public T GetCinemachineComponent<T>() where T : CinemachineComponentBase
    {
      CinemachineComponentBase[] componentPipeline = GetComponentPipeline();
      if (componentPipeline != null)
      {
        foreach (CinemachineComponentBase cinemachineComponent in componentPipeline)
        {
          if (cinemachineComponent is T)
            return cinemachineComponent as T;
        }
      }
      return default (T);
    }

    public T AddCinemachineComponent<T>() where T : CinemachineComponentBase
    {
      Transform componentOwner = GetComponentOwner();
      CinemachineComponentBase[] components = componentOwner.GetComponents<CinemachineComponentBase>();
      T obj = componentOwner.gameObject.AddComponent<T>();
      if ((UnityEngine.Object) obj != (UnityEngine.Object) null && components != null)
      {
        CinemachineCore.Stage stage = obj.Stage;
        for (int index = components.Length - 1; index >= 0; --index)
        {
          if (components[index].Stage == stage)
          {
            components[index].enabled = false;
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) components[index]);
          }
        }
      }
      InvalidateComponentPipeline();
      return obj;
    }

    public void DestroyCinemachineComponent<T>() where T : CinemachineComponentBase
    {
      CinemachineComponentBase[] componentPipeline = GetComponentPipeline();
      if (componentPipeline == null)
        return;
      foreach (CinemachineComponentBase cinemachineComponentBase in componentPipeline)
      {
        if (cinemachineComponentBase is T)
        {
          cinemachineComponentBase.enabled = false;
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) cinemachineComponentBase);
          InvalidateComponentPipeline();
        }
      }
    }

    public bool UserIsDragging { get; set; }

    public void OnPositionDragged(Vector3 delta)
    {
      CinemachineComponentBase[] componentPipeline = GetComponentPipeline();
      if (componentPipeline == null)
        return;
      for (int index = 0; index < componentPipeline.Length; ++index)
        componentPipeline[index].OnPositionDragged(delta);
    }

    private void UpdateComponentPipeline()
    {
      if ((UnityEngine.Object) m_ComponentOwner != (UnityEngine.Object) null && (UnityEngine.Object) m_ComponentOwner.parent != (UnityEngine.Object) this.transform)
      {
        CinemachineVirtualCamera component = (UnityEngine.Object) m_ComponentOwner.parent != (UnityEngine.Object) null ? m_ComponentOwner.parent.gameObject.GetComponent<CinemachineVirtualCamera>() : (CinemachineVirtualCamera) null;
        DestroyPipeline();
        m_ComponentOwner = CreatePipeline(component);
      }
      if ((UnityEngine.Object) m_ComponentOwner != (UnityEngine.Object) null && m_ComponentPipeline != null)
        return;
      m_ComponentOwner = (Transform) null;
      List<CinemachineComponentBase> cinemachineComponentBaseList = new List<CinemachineComponentBase>();
      foreach (Transform transform in this.transform)
      {
        if ((UnityEngine.Object) transform.GetComponent<CinemachinePipeline>() != (UnityEngine.Object) null)
        {
          m_ComponentOwner = transform;
          foreach (CinemachineComponentBase component in transform.GetComponents<CinemachineComponentBase>())
            cinemachineComponentBaseList.Add(component);
        }
      }
      if ((UnityEngine.Object) m_ComponentOwner == (UnityEngine.Object) null)
        m_ComponentOwner = CreatePipeline(null);
      if (CinemachineCore.sShowHiddenObjects)
        m_ComponentOwner.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
      else
        m_ComponentOwner.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
      cinemachineComponentBaseList.Sort((c1, c2) => c1.Stage - c2.Stage);
      m_ComponentPipeline = cinemachineComponentBaseList.ToArray();
    }

    private CameraState CalculateNewState(Vector3 worldUp, float deltaTime)
    {
      CameraState newState = PullStateFromVirtualCamera(worldUp);
      if ((UnityEngine.Object) LookAt != (UnityEngine.Object) null)
        newState.ReferenceLookAt = LookAt.position;
      CinemachineCore.Stage curStage = CinemachineCore.Stage.Body;
      UpdateComponentPipeline();
      if (m_ComponentPipeline != null)
      {
        for (int index = 0; index < m_ComponentPipeline.Length; ++index)
          m_ComponentPipeline[index].PrePipelineMutateCameraState(ref newState);
        for (int index = 0; index < m_ComponentPipeline.Length; ++index)
        {
          curStage = AdvancePipelineStage(ref newState, deltaTime, curStage, (int) m_ComponentPipeline[index].Stage);
          m_ComponentPipeline[index].MutateCameraState(ref newState, deltaTime);
        }
      }
      int maxStage = 3;
      int num = (int) AdvancePipelineStage(ref newState, deltaTime, curStage, maxStage);
      return newState;
    }

    private CinemachineCore.Stage AdvancePipelineStage(
      ref CameraState state,
      float deltaTime,
      CinemachineCore.Stage curStage,
      int maxStage)
    {
      for (; curStage < (CinemachineCore.Stage) maxStage; ++curStage)
        InvokePostPipelineStageCallback(this, curStage, ref state, deltaTime);
      return curStage;
    }

    private CameraState PullStateFromVirtualCamera(Vector3 worldUp)
    {
      CameraState cameraState = CameraState.Default with
      {
        RawPosition = this.transform.position,
        RawOrientation = this.transform.rotation,
        ReferenceUp = worldUp
      };
      CinemachineBrain potentialTargetBrain = CinemachineCore.Instance.FindPotentialTargetBrain(this);
      m_Lens.Aspect = (UnityEngine.Object) potentialTargetBrain != (UnityEngine.Object) null ? potentialTargetBrain.OutputCamera.aspect : 1f;
      m_Lens.Orthographic = (UnityEngine.Object) potentialTargetBrain != (UnityEngine.Object) null && potentialTargetBrain.OutputCamera.orthographic;
      cameraState.Lens = m_Lens;
      return cameraState;
    }

    internal void SetStateRawPosition(Vector3 pos) => m_State.RawPosition = pos;

    public delegate Transform CreatePipelineDelegate(
      CinemachineVirtualCamera vcam,
      string name,
      CinemachineComponentBase[] copyFrom);

    public delegate void DestroyPipelineDelegate(GameObject pipeline);
  }
}
