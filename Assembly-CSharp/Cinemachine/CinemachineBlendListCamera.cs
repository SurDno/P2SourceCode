using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(13f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachineBlendListCamera")]
  public class CinemachineBlendListCamera : CinemachineVirtualCameraBase
  {
    [Tooltip("Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty if all of the children define targets of their own.")]
    [NoSaveDuringPlay]
    public Transform m_LookAt = (Transform) null;
    [Tooltip("Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all of the children define targets of their own.")]
    [NoSaveDuringPlay]
    public Transform m_Follow = (Transform) null;
    [Tooltip("When enabled, the current child camera and blend will be indicated in the game window, for debugging")]
    public bool m_ShowDebugText = false;
    [Tooltip("Force all child cameras to be enabled.  This is useful if animating them in Timeline, but consumes extra resources")]
    public bool m_EnableAllChildCameras;
    [SerializeField]
    [HideInInspector]
    [NoSaveDuringPlay]
    public CinemachineVirtualCameraBase[] m_ChildCameras = (CinemachineVirtualCameraBase[]) null;
    [Tooltip("The set of instructions for enabling child cameras.")]
    public CinemachineBlendListCamera.Instruction[] m_Instructions;
    private CameraState m_State = CameraState.Default;
    private float mActivationTime = -1f;
    private int mCurrentInstruction = 0;
    private CinemachineBlend mActiveBlend = (CinemachineBlend) null;

    public override string Description
    {
      get
      {
        ICinemachineCamera liveChild = this.LiveChild;
        return this.mActiveBlend == null ? (liveChild != null ? "[" + liveChild.Name + "]" : "(none)") : this.mActiveBlend.Description;
      }
    }

    public ICinemachineCamera LiveChild { set; get; }

    public override ICinemachineCamera LiveChildOrSelf => this.LiveChild;

    public override bool IsLiveChild(ICinemachineCamera vcam)
    {
      return vcam == this.LiveChild || this.mActiveBlend != null && (vcam == this.mActiveBlend.CamA || vcam == this.mActiveBlend.CamB);
    }

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

    public override void RemovePostPipelineStageHook(
      CinemachineVirtualCameraBase.OnPostPipelineStageDelegate d)
    {
      base.RemovePostPipelineStageHook(d);
      this.UpdateListOfChildren();
      foreach (CinemachineVirtualCameraBase childCamera in this.m_ChildCameras)
        childCamera.RemovePostPipelineStageHook(d);
    }

    public override void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
      base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
      this.mActivationTime = Time.time;
      this.mCurrentInstruction = -1;
      this.LiveChild = (ICinemachineCamera) null;
      this.mActiveBlend = (CinemachineBlend) null;
      this.UpdateCameraState(worldUp, deltaTime);
    }

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      if (!this.PreviousStateIsValid)
        deltaTime = -1f;
      this.UpdateListOfChildren();
      this.AdvanceCurrentInstruction();
      CinemachineVirtualCameraBase virtualCameraBase = (CinemachineVirtualCameraBase) null;
      if (this.mCurrentInstruction >= 0 && this.mCurrentInstruction < this.m_Instructions.Length)
        virtualCameraBase = this.m_Instructions[this.mCurrentInstruction].m_VirtualCamera;
      if (this.m_ChildCameras != null)
      {
        for (int index = 0; index < this.m_ChildCameras.Length; ++index)
        {
          CinemachineVirtualCameraBase childCamera = this.m_ChildCameras[index];
          if ((UnityEngine.Object) childCamera != (UnityEngine.Object) null)
          {
            bool flag = this.m_EnableAllChildCameras || (UnityEngine.Object) childCamera == (UnityEngine.Object) virtualCameraBase;
            if (flag != childCamera.VirtualCameraGameObject.activeInHierarchy)
            {
              childCamera.gameObject.SetActive(flag);
              if (flag)
                CinemachineCore.Instance.UpdateVirtualCamera((ICinemachineCamera) childCamera, worldUp, deltaTime);
            }
          }
        }
      }
      if ((UnityEngine.Object) virtualCameraBase != (UnityEngine.Object) null)
      {
        ICinemachineCamera liveChild = this.LiveChild;
        this.LiveChild = (ICinemachineCamera) virtualCameraBase;
        if (liveChild != null && this.LiveChild != null && liveChild != this.LiveChild && this.mCurrentInstruction > 0)
        {
          this.mActiveBlend = this.CreateBlend(liveChild, this.LiveChild, this.m_Instructions[this.mCurrentInstruction].m_Blend.BlendCurve, this.m_Instructions[this.mCurrentInstruction].m_Blend.m_Time, this.mActiveBlend, deltaTime);
          this.LiveChild.OnTransitionFromCamera(liveChild, worldUp, deltaTime);
          CinemachineCore.Instance.GenerateCameraActivationEvent(this.LiveChild);
          if (this.mActiveBlend == null)
            CinemachineCore.Instance.GenerateCameraCutEvent(this.LiveChild);
        }
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

    public CinemachineVirtualCameraBase[] ChildCameras
    {
      get
      {
        this.UpdateListOfChildren();
        return this.m_ChildCameras;
      }
    }

    public bool IsBlending => this.mActiveBlend != null;

    private void InvalidateListOfChildren()
    {
      this.m_ChildCameras = (CinemachineVirtualCameraBase[]) null;
      this.LiveChild = (ICinemachineCamera) null;
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
      this.ValidateInstructions();
    }

    public void ValidateInstructions()
    {
      if (this.m_Instructions == null)
        this.m_Instructions = new CinemachineBlendListCamera.Instruction[0];
      for (int index = 0; index < this.m_Instructions.Length; ++index)
      {
        if ((UnityEngine.Object) this.m_Instructions[index].m_VirtualCamera != (UnityEngine.Object) null && (UnityEngine.Object) this.m_Instructions[index].m_VirtualCamera.transform.parent != (UnityEngine.Object) this.transform)
          this.m_Instructions[index].m_VirtualCamera = (CinemachineVirtualCameraBase) null;
      }
      this.mActiveBlend = (CinemachineBlend) null;
    }

    private void AdvanceCurrentInstruction()
    {
      if (this.m_ChildCameras == null || this.m_ChildCameras.Length == 0 || (double) this.mActivationTime < 0.0 || this.m_Instructions.Length == 0)
      {
        this.mActivationTime = -1f;
        this.mCurrentInstruction = -1;
        this.mActiveBlend = (CinemachineBlend) null;
      }
      else if (this.mCurrentInstruction >= this.m_Instructions.Length - 1)
      {
        this.mCurrentInstruction = this.m_Instructions.Length - 1;
      }
      else
      {
        float time = Time.time;
        if (this.mCurrentInstruction < 0)
        {
          this.mActivationTime = time;
          this.mCurrentInstruction = 0;
        }
        else if ((double) time - (double) this.mActivationTime > (double) Mathf.Max(0.0f, this.m_Instructions[this.mCurrentInstruction].m_Hold))
        {
          this.mActivationTime = time;
          ++this.mCurrentInstruction;
        }
      }
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

    [Serializable]
    public struct Instruction
    {
      [Tooltip("The virtual camera to activate when this instruction becomes active")]
      public CinemachineVirtualCameraBase m_VirtualCamera;
      [Tooltip("How long to wait (in seconds) before activating the next virtual camera in the list (if any)")]
      public float m_Hold;
      [CinemachineBlendDefinitionProperty]
      [Tooltip("How to blend to the next virtual camera in the list (if any)")]
      public CinemachineBlendDefinition m_Blend;
    }
  }
}
