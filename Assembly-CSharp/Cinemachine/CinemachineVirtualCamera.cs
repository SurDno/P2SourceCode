// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineVirtualCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
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
    public static CinemachineVirtualCamera.CreatePipelineDelegate CreatePipelineOverride;
    public static CinemachineVirtualCamera.DestroyPipelineDelegate DestroyPipelineOverride;
    private CameraState m_State = CameraState.Default;
    private CinemachineComponentBase[] m_ComponentPipeline = (CinemachineComponentBase[]) null;
    [SerializeField]
    [HideInInspector]
    private Transform m_ComponentOwner = (Transform) null;

    public override CameraState State => this.m_State;

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

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      if (!this.PreviousStateIsValid)
        deltaTime = -1f;
      if ((double) deltaTime < 0.0)
        this.m_State = this.PullStateFromVirtualCamera(worldUp);
      this.m_State = this.CalculateNewState(worldUp, deltaTime);
      if (!this.UserIsDragging)
      {
        CameraState state;
        if ((UnityEngine.Object) this.Follow != (UnityEngine.Object) null)
        {
          Transform transform = this.transform;
          state = this.State;
          Vector3 rawPosition = state.RawPosition;
          transform.position = rawPosition;
        }
        if ((UnityEngine.Object) this.LookAt != (UnityEngine.Object) null)
        {
          Transform transform = this.transform;
          state = this.State;
          Quaternion rawOrientation = state.RawOrientation;
          transform.rotation = rawOrientation;
        }
      }
      this.PreviousStateIsValid = true;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.InvalidateComponentPipeline();
      if (this.ValidatingStreamVersion >= 20170927)
        return;
      if ((UnityEngine.Object) this.Follow != (UnityEngine.Object) null && (UnityEngine.Object) this.GetCinemachineComponent(CinemachineCore.Stage.Body) == (UnityEngine.Object) null)
        this.AddCinemachineComponent<CinemachineHardLockToTarget>();
      if ((UnityEngine.Object) this.LookAt != (UnityEngine.Object) null && (UnityEngine.Object) this.GetCinemachineComponent(CinemachineCore.Stage.Aim) == (UnityEngine.Object) null)
        this.AddCinemachineComponent<CinemachineHardLookAt>();
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
      this.m_Lens.Validate();
    }

    private void OnTransformChildrenChanged() => this.InvalidateComponentPipeline();

    private void Reset() => this.DestroyPipeline();

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
        if (CinemachineVirtualCamera.DestroyPipelineOverride != null)
          CinemachineVirtualCamera.DestroyPipelineOverride(transform.gameObject);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) transform.gameObject);
      }
      this.m_ComponentOwner = (Transform) null;
      this.PreviousStateIsValid = false;
    }

    private Transform CreatePipeline(CinemachineVirtualCamera copyFrom)
    {
      CinemachineComponentBase[] copyFrom1 = (CinemachineComponentBase[]) null;
      if ((UnityEngine.Object) copyFrom != (UnityEngine.Object) null)
      {
        copyFrom.InvalidateComponentPipeline();
        copyFrom1 = copyFrom.GetComponentPipeline();
      }
      Transform pipeline;
      if (CinemachineVirtualCamera.CreatePipelineOverride != null)
      {
        pipeline = CinemachineVirtualCamera.CreatePipelineOverride(this, "cm", copyFrom1);
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
      this.PreviousStateIsValid = false;
      return pipeline;
    }

    public void InvalidateComponentPipeline()
    {
      this.m_ComponentPipeline = (CinemachineComponentBase[]) null;
    }

    public Transform GetComponentOwner()
    {
      this.UpdateComponentPipeline();
      return this.m_ComponentOwner;
    }

    public CinemachineComponentBase[] GetComponentPipeline()
    {
      this.UpdateComponentPipeline();
      return this.m_ComponentPipeline;
    }

    public CinemachineComponentBase GetCinemachineComponent(CinemachineCore.Stage stage)
    {
      CinemachineComponentBase[] componentPipeline = this.GetComponentPipeline();
      if (componentPipeline != null)
      {
        foreach (CinemachineComponentBase cinemachineComponent in componentPipeline)
        {
          if (cinemachineComponent.Stage == stage)
            return cinemachineComponent;
        }
      }
      return (CinemachineComponentBase) null;
    }

    public T GetCinemachineComponent<T>() where T : CinemachineComponentBase
    {
      CinemachineComponentBase[] componentPipeline = this.GetComponentPipeline();
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
      Transform componentOwner = this.GetComponentOwner();
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
      this.InvalidateComponentPipeline();
      return obj;
    }

    public void DestroyCinemachineComponent<T>() where T : CinemachineComponentBase
    {
      CinemachineComponentBase[] componentPipeline = this.GetComponentPipeline();
      if (componentPipeline == null)
        return;
      foreach (CinemachineComponentBase cinemachineComponentBase in componentPipeline)
      {
        if (cinemachineComponentBase is T)
        {
          cinemachineComponentBase.enabled = false;
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) cinemachineComponentBase);
          this.InvalidateComponentPipeline();
        }
      }
    }

    public bool UserIsDragging { get; set; }

    public void OnPositionDragged(Vector3 delta)
    {
      CinemachineComponentBase[] componentPipeline = this.GetComponentPipeline();
      if (componentPipeline == null)
        return;
      for (int index = 0; index < componentPipeline.Length; ++index)
        componentPipeline[index].OnPositionDragged(delta);
    }

    private void UpdateComponentPipeline()
    {
      if ((UnityEngine.Object) this.m_ComponentOwner != (UnityEngine.Object) null && (UnityEngine.Object) this.m_ComponentOwner.parent != (UnityEngine.Object) this.transform)
      {
        CinemachineVirtualCamera component = (UnityEngine.Object) this.m_ComponentOwner.parent != (UnityEngine.Object) null ? this.m_ComponentOwner.parent.gameObject.GetComponent<CinemachineVirtualCamera>() : (CinemachineVirtualCamera) null;
        this.DestroyPipeline();
        this.m_ComponentOwner = this.CreatePipeline(component);
      }
      if ((UnityEngine.Object) this.m_ComponentOwner != (UnityEngine.Object) null && this.m_ComponentPipeline != null)
        return;
      this.m_ComponentOwner = (Transform) null;
      List<CinemachineComponentBase> cinemachineComponentBaseList = new List<CinemachineComponentBase>();
      foreach (Transform transform in this.transform)
      {
        if ((UnityEngine.Object) transform.GetComponent<CinemachinePipeline>() != (UnityEngine.Object) null)
        {
          this.m_ComponentOwner = transform;
          foreach (CinemachineComponentBase component in transform.GetComponents<CinemachineComponentBase>())
            cinemachineComponentBaseList.Add(component);
        }
      }
      if ((UnityEngine.Object) this.m_ComponentOwner == (UnityEngine.Object) null)
        this.m_ComponentOwner = this.CreatePipeline((CinemachineVirtualCamera) null);
      if (CinemachineCore.sShowHiddenObjects)
        this.m_ComponentOwner.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
      else
        this.m_ComponentOwner.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
      cinemachineComponentBaseList.Sort((Comparison<CinemachineComponentBase>) ((c1, c2) => c1.Stage - c2.Stage));
      this.m_ComponentPipeline = cinemachineComponentBaseList.ToArray();
    }

    private CameraState CalculateNewState(Vector3 worldUp, float deltaTime)
    {
      CameraState newState = this.PullStateFromVirtualCamera(worldUp);
      if ((UnityEngine.Object) this.LookAt != (UnityEngine.Object) null)
        newState.ReferenceLookAt = this.LookAt.position;
      CinemachineCore.Stage curStage = CinemachineCore.Stage.Body;
      this.UpdateComponentPipeline();
      if (this.m_ComponentPipeline != null)
      {
        for (int index = 0; index < this.m_ComponentPipeline.Length; ++index)
          this.m_ComponentPipeline[index].PrePipelineMutateCameraState(ref newState);
        for (int index = 0; index < this.m_ComponentPipeline.Length; ++index)
        {
          curStage = this.AdvancePipelineStage(ref newState, deltaTime, curStage, (int) this.m_ComponentPipeline[index].Stage);
          this.m_ComponentPipeline[index].MutateCameraState(ref newState, deltaTime);
        }
      }
      int maxStage = 3;
      int num = (int) this.AdvancePipelineStage(ref newState, deltaTime, curStage, maxStage);
      return newState;
    }

    private CinemachineCore.Stage AdvancePipelineStage(
      ref CameraState state,
      float deltaTime,
      CinemachineCore.Stage curStage,
      int maxStage)
    {
      for (; curStage < (CinemachineCore.Stage) maxStage; ++curStage)
        this.InvokePostPipelineStageCallback((CinemachineVirtualCameraBase) this, curStage, ref state, deltaTime);
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
      CinemachineBrain potentialTargetBrain = CinemachineCore.Instance.FindPotentialTargetBrain((ICinemachineCamera) this);
      this.m_Lens.Aspect = (UnityEngine.Object) potentialTargetBrain != (UnityEngine.Object) null ? potentialTargetBrain.OutputCamera.aspect : 1f;
      this.m_Lens.Orthographic = (UnityEngine.Object) potentialTargetBrain != (UnityEngine.Object) null && potentialTargetBrain.OutputCamera.orthographic;
      cameraState.Lens = this.m_Lens;
      return cameraState;
    }

    internal void SetStateRawPosition(Vector3 pos) => this.m_State.RawPosition = pos;

    public delegate Transform CreatePipelineDelegate(
      CinemachineVirtualCamera vcam,
      string name,
      CinemachineComponentBase[] copyFrom);

    public delegate void DestroyPipelineDelegate(GameObject pipeline);
  }
}
