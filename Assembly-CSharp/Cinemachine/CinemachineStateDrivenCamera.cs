// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineStateDrivenCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(13f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachineStateDrivenCamera")]
  public class CinemachineStateDrivenCamera : CinemachineVirtualCameraBase
  {
    [Tooltip("Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty if all of the children define targets of their own.")]
    [NoSaveDuringPlay]
    public Transform m_LookAt = (Transform) null;
    [Tooltip("Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all of the children define targets of their own.")]
    [NoSaveDuringPlay]
    public Transform m_Follow = (Transform) null;
    [Space]
    [Tooltip("The state machine whose state changes will drive this camera's choice of active child")]
    public Animator m_AnimatedTarget;
    [Tooltip("Which layer in the target state machine to observe")]
    public int m_LayerIndex;
    [Tooltip("When enabled, the current child camera and blend will be indicated in the game window, for debugging")]
    public bool m_ShowDebugText = false;
    [Tooltip("Force all child cameras to be enabled.  This is useful if animating them in Timeline, but consumes extra resources")]
    public bool m_EnableAllChildCameras;
    [SerializeField]
    [HideInInspector]
    [NoSaveDuringPlay]
    public CinemachineVirtualCameraBase[] m_ChildCameras = (CinemachineVirtualCameraBase[]) null;
    [Tooltip("The set of instructions associating virtual cameras with states.  These instructions are used to choose the live child at any given moment")]
    public CinemachineStateDrivenCamera.Instruction[] m_Instructions;
    [CinemachineBlendDefinitionProperty]
    [Tooltip("The blend which is used if you don't explicitly define a blend between two Virtual Camera children")]
    public CinemachineBlendDefinition m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);
    [Tooltip("This is the asset which contains custom settings for specific child blends")]
    public CinemachineBlenderSettings m_CustomBlends = (CinemachineBlenderSettings) null;
    [HideInInspector]
    [SerializeField]
    public CinemachineStateDrivenCamera.ParentHash[] m_ParentHash = (CinemachineStateDrivenCamera.ParentHash[]) null;
    private CameraState m_State = CameraState.Default;
    private float mActivationTime = 0.0f;
    private CinemachineStateDrivenCamera.Instruction mActiveInstruction;
    private float mPendingActivationTime = 0.0f;
    private CinemachineStateDrivenCamera.Instruction mPendingInstruction;
    private CinemachineBlend mActiveBlend = (CinemachineBlend) null;
    private Dictionary<int, int> mInstructionDictionary;
    private Dictionary<int, int> mStateParentLookup;
    private List<AnimatorClipInfo> m_clipInfoList = new List<AnimatorClipInfo>();

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

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      if (!this.PreviousStateIsValid)
        deltaTime = -1f;
      this.UpdateListOfChildren();
      CinemachineVirtualCameraBase virtualCameraBase = this.ChooseCurrentCamera(deltaTime);
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
      ICinemachineCamera liveChild = this.LiveChild;
      this.LiveChild = (ICinemachineCamera) virtualCameraBase;
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

    public CinemachineVirtualCameraBase[] ChildCameras
    {
      get
      {
        this.UpdateListOfChildren();
        return this.m_ChildCameras;
      }
    }

    public bool IsBlending => this.mActiveBlend != null;

    public static string CreateFakeHashName(int parentHash, string stateName)
    {
      return parentHash.ToString() + "_" + stateName;
    }

    private void InvalidateListOfChildren()
    {
      this.m_ChildCameras = (CinemachineVirtualCameraBase[]) null;
      this.LiveChild = (ICinemachineCamera) null;
    }

    private void UpdateListOfChildren()
    {
      if (this.m_ChildCameras != null && this.mInstructionDictionary != null && this.mStateParentLookup != null)
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
        this.m_Instructions = new CinemachineStateDrivenCamera.Instruction[0];
      this.mInstructionDictionary = new Dictionary<int, int>();
      for (int index = 0; index < this.m_Instructions.Length; ++index)
      {
        if ((UnityEngine.Object) this.m_Instructions[index].m_VirtualCamera != (UnityEngine.Object) null && (UnityEngine.Object) this.m_Instructions[index].m_VirtualCamera.transform.parent != (UnityEngine.Object) this.transform)
          this.m_Instructions[index].m_VirtualCamera = (CinemachineVirtualCameraBase) null;
        this.mInstructionDictionary[this.m_Instructions[index].m_FullHash] = index;
      }
      this.mStateParentLookup = new Dictionary<int, int>();
      if (this.m_ParentHash != null)
      {
        foreach (CinemachineStateDrivenCamera.ParentHash parentHash in this.m_ParentHash)
          this.mStateParentLookup[parentHash.m_Hash] = parentHash.m_ParentHash;
      }
      this.mActivationTime = this.mPendingActivationTime = 0.0f;
      this.mActiveBlend = (CinemachineBlend) null;
    }

    private CinemachineVirtualCameraBase ChooseCurrentCamera(float deltaTime)
    {
      if (this.m_ChildCameras == null || this.m_ChildCameras.Length == 0)
      {
        this.mActivationTime = 0.0f;
        return (CinemachineVirtualCameraBase) null;
      }
      CinemachineVirtualCameraBase childCamera = this.m_ChildCameras[0];
      if ((UnityEngine.Object) this.m_AnimatedTarget == (UnityEngine.Object) null || !this.m_AnimatedTarget.gameObject.activeSelf || (UnityEngine.Object) this.m_AnimatedTarget.runtimeAnimatorController == (UnityEngine.Object) null || this.m_LayerIndex < 0 || this.m_LayerIndex >= this.m_AnimatedTarget.layerCount)
      {
        this.mActivationTime = 0.0f;
        return childCamera;
      }
      int key;
      if (this.m_AnimatedTarget.IsInTransition(this.m_LayerIndex))
      {
        AnimatorStateInfo animatorStateInfo = this.m_AnimatedTarget.GetNextAnimatorStateInfo(this.m_LayerIndex);
        key = animatorStateInfo.fullPathHash;
        if (this.m_AnimatedTarget.GetNextAnimatorClipInfoCount(this.m_LayerIndex) > 1)
        {
          this.m_AnimatedTarget.GetNextAnimatorClipInfo(this.m_LayerIndex, this.m_clipInfoList);
          key = this.GetClipHash(animatorStateInfo.fullPathHash, this.m_clipInfoList);
        }
      }
      else
      {
        AnimatorStateInfo animatorStateInfo = this.m_AnimatedTarget.GetCurrentAnimatorStateInfo(this.m_LayerIndex);
        key = animatorStateInfo.fullPathHash;
        if (this.m_AnimatedTarget.GetCurrentAnimatorClipInfoCount(this.m_LayerIndex) > 1)
        {
          this.m_AnimatedTarget.GetCurrentAnimatorClipInfo(this.m_LayerIndex, this.m_clipInfoList);
          key = this.GetClipHash(animatorStateInfo.fullPathHash, this.m_clipInfoList);
        }
      }
      while (key != 0 && !this.mInstructionDictionary.ContainsKey(key))
        key = this.mStateParentLookup.ContainsKey(key) ? this.mStateParentLookup[key] : 0;
      float time = Time.time;
      if ((double) this.mActivationTime != 0.0)
      {
        if (this.mActiveInstruction.m_FullHash == key)
        {
          this.mPendingActivationTime = 0.0f;
          return this.mActiveInstruction.m_VirtualCamera;
        }
        if ((double) deltaTime >= 0.0 && (double) this.mPendingActivationTime != 0.0 && this.mPendingInstruction.m_FullHash == key)
        {
          if ((double) time - (double) this.mPendingActivationTime > (double) this.mPendingInstruction.m_ActivateAfter && ((double) time - (double) this.mActivationTime > (double) this.mActiveInstruction.m_MinDuration || this.mPendingInstruction.m_VirtualCamera.Priority > this.mActiveInstruction.m_VirtualCamera.Priority))
          {
            this.mActiveInstruction = this.mPendingInstruction;
            this.mActivationTime = time;
            this.mPendingActivationTime = 0.0f;
          }
          return this.mActiveInstruction.m_VirtualCamera;
        }
      }
      this.mPendingActivationTime = 0.0f;
      if (!this.mInstructionDictionary.ContainsKey(key))
        return (double) this.mActivationTime != 0.0 ? this.mActiveInstruction.m_VirtualCamera : childCamera;
      CinemachineStateDrivenCamera.Instruction instruction = this.m_Instructions[this.mInstructionDictionary[key]];
      if ((UnityEngine.Object) instruction.m_VirtualCamera == (UnityEngine.Object) null)
        instruction.m_VirtualCamera = childCamera;
      if ((double) deltaTime >= 0.0 && (double) this.mActivationTime > 0.0 && ((double) instruction.m_ActivateAfter > 0.0 || (double) time - (double) this.mActivationTime < (double) this.mActiveInstruction.m_MinDuration && instruction.m_VirtualCamera.Priority <= this.mActiveInstruction.m_VirtualCamera.Priority))
      {
        this.mPendingInstruction = instruction;
        this.mPendingActivationTime = time;
        return (double) this.mActivationTime != 0.0 ? this.mActiveInstruction.m_VirtualCamera : childCamera;
      }
      this.mActiveInstruction = instruction;
      this.mActivationTime = time;
      return this.mActiveInstruction.m_VirtualCamera;
    }

    private int GetClipHash(int hash, List<AnimatorClipInfo> clips)
    {
      if (clips.Count > 1)
      {
        int index1 = -1;
        AnimatorClipInfo clip;
        for (int index2 = 0; index2 < clips.Count; ++index2)
        {
          int num;
          if (index1 >= 0)
          {
            clip = clips[index2];
            double weight1 = (double) clip.weight;
            clip = clips[index1];
            double weight2 = (double) clip.weight;
            num = weight1 > weight2 ? 1 : 0;
          }
          else
            num = 1;
          if (num != 0)
            index1 = index2;
        }
        int num1;
        if (index1 >= 0)
        {
          clip = clips[index1];
          num1 = (double) clip.weight > 0.0 ? 1 : 0;
        }
        else
          num1 = 0;
        if (num1 != 0)
        {
          int parentHash = hash;
          clip = clips[index1];
          string name = clip.clip.name;
          hash = Animator.StringToHash(CinemachineStateDrivenCamera.CreateFakeHashName(parentHash, name));
        }
      }
      return hash;
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

    [Serializable]
    public struct Instruction
    {
      [Tooltip("The full hash of the animation state")]
      public int m_FullHash;
      [Tooltip("The virtual camera to activate whrn the animation state becomes active")]
      public CinemachineVirtualCameraBase m_VirtualCamera;
      [Tooltip("How long to wait (in seconds) before activating the virtual camera. This filters out very short state durations")]
      public float m_ActivateAfter;
      [Tooltip("The minimum length of time (in seconds) to keep a virtual camera active")]
      public float m_MinDuration;
    }

    [DocumentationSorting(13.2f, DocumentationSortingAttribute.Level.Undoc)]
    [Serializable]
    public struct ParentHash
    {
      public int m_Hash;
      public int m_ParentHash;

      public ParentHash(int h, int p)
      {
        this.m_Hash = h;
        this.m_ParentHash = p;
      }
    }
  }
}
