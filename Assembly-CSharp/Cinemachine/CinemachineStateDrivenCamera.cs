using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine;

[DocumentationSorting(13f, DocumentationSortingAttribute.Level.UserRef)]
[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("Cinemachine/CinemachineStateDrivenCamera")]
public class CinemachineStateDrivenCamera : CinemachineVirtualCameraBase {
	[Tooltip(
		"Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty if all of the children define targets of their own.")]
	[NoSaveDuringPlay]
	public Transform m_LookAt;

	[Tooltip(
		"Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all of the children define targets of their own.")]
	[NoSaveDuringPlay]
	public Transform m_Follow;

	[Space] [Tooltip("The state machine whose state changes will drive this camera's choice of active child")]
	public Animator m_AnimatedTarget;

	[Tooltip("Which layer in the target state machine to observe")]
	public int m_LayerIndex;

	[Tooltip("When enabled, the current child camera and blend will be indicated in the game window, for debugging")]
	public bool m_ShowDebugText;

	[Tooltip(
		"Force all child cameras to be enabled.  This is useful if animating them in Timeline, but consumes extra resources")]
	public bool m_EnableAllChildCameras;

	[SerializeField] [HideInInspector] [NoSaveDuringPlay]
	public CinemachineVirtualCameraBase[] m_ChildCameras;

	[Tooltip(
		"The set of instructions associating virtual cameras with states.  These instructions are used to choose the live child at any given moment")]
	public Instruction[] m_Instructions;

	[CinemachineBlendDefinitionProperty]
	[Tooltip("The blend which is used if you don't explicitly define a blend between two Virtual Camera children")]
	public CinemachineBlendDefinition m_DefaultBlend = new(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);

	[Tooltip("This is the asset which contains custom settings for specific child blends")]
	public CinemachineBlenderSettings m_CustomBlends;

	[HideInInspector] [SerializeField] public ParentHash[] m_ParentHash;
	private CameraState m_State = CameraState.Default;
	private float mActivationTime;
	private Instruction mActiveInstruction;
	private float mPendingActivationTime;
	private Instruction mPendingInstruction;
	private CinemachineBlend mActiveBlend;
	private Dictionary<int, int> mInstructionDictionary;
	private Dictionary<int, int> mStateParentLookup;
	private List<AnimatorClipInfo> m_clipInfoList = new();

	public override string Description {
		get {
			var liveChild = LiveChild;
			return mActiveBlend == null
				? liveChild != null ? "[" + liveChild.Name + "]" : "(none)"
				: mActiveBlend.Description;
		}
	}

	public ICinemachineCamera LiveChild { set; get; }

	public override ICinemachineCamera LiveChildOrSelf => LiveChild;

	public override bool IsLiveChild(ICinemachineCamera vcam) {
		return vcam == LiveChild || (mActiveBlend != null && (vcam == mActiveBlend.CamA || vcam == mActiveBlend.CamB));
	}

	public override CameraState State => m_State;

	public override Transform LookAt {
		get => ResolveLookAt(m_LookAt);
		set => m_LookAt = value;
	}

	public override Transform Follow {
		get => ResolveFollow(m_Follow);
		set => m_Follow = value;
	}

	public override void RemovePostPipelineStageHook(
		OnPostPipelineStageDelegate d) {
		base.RemovePostPipelineStageHook(d);
		UpdateListOfChildren();
		foreach (var childCamera in m_ChildCameras)
			childCamera.RemovePostPipelineStageHook(d);
	}

	public override void UpdateCameraState(Vector3 worldUp, float deltaTime) {
		if (!PreviousStateIsValid)
			deltaTime = -1f;
		UpdateListOfChildren();
		var virtualCameraBase = ChooseCurrentCamera(deltaTime);
		if (m_ChildCameras != null)
			for (var index = 0; index < m_ChildCameras.Length; ++index) {
				var childCamera = m_ChildCameras[index];
				if (childCamera != null) {
					var flag = m_EnableAllChildCameras || childCamera == virtualCameraBase;
					if (flag != childCamera.VirtualCameraGameObject.activeInHierarchy) {
						childCamera.gameObject.SetActive(flag);
						if (flag)
							CinemachineCore.Instance.UpdateVirtualCamera(childCamera, worldUp, deltaTime);
					}
				}
			}

		var liveChild = LiveChild;
		LiveChild = virtualCameraBase;
		if (liveChild != null && LiveChild != null && liveChild != LiveChild) {
			var duration = 0.0f;
			var blendCurve = LookupBlendCurve(liveChild, LiveChild, out duration);
			mActiveBlend = CreateBlend(liveChild, LiveChild, blendCurve, duration, mActiveBlend, deltaTime);
			LiveChild.OnTransitionFromCamera(liveChild, worldUp, deltaTime);
			CinemachineCore.Instance.GenerateCameraActivationEvent(LiveChild);
			if (mActiveBlend == null)
				CinemachineCore.Instance.GenerateCameraCutEvent(LiveChild);
		}

		if (mActiveBlend != null) {
			mActiveBlend.TimeInBlend += deltaTime >= 0.0 ? deltaTime : mActiveBlend.Duration;
			if (mActiveBlend.IsComplete)
				mActiveBlend = null;
		}

		if (mActiveBlend != null) {
			mActiveBlend.UpdateCameraState(worldUp, deltaTime);
			m_State = mActiveBlend.State;
		} else if (LiveChild != null)
			m_State = LiveChild.State;

		PreviousStateIsValid = true;
	}

	protected override void OnEnable() {
		base.OnEnable();
		InvalidateListOfChildren();
		mActiveBlend = null;
	}

	public void OnTransformChildrenChanged() {
		InvalidateListOfChildren();
	}

	public CinemachineVirtualCameraBase[] ChildCameras {
		get {
			UpdateListOfChildren();
			return m_ChildCameras;
		}
	}

	public bool IsBlending => mActiveBlend != null;

	public static string CreateFakeHashName(int parentHash, string stateName) {
		return parentHash + "_" + stateName;
	}

	private void InvalidateListOfChildren() {
		m_ChildCameras = null;
		LiveChild = null;
	}

	private void UpdateListOfChildren() {
		if (m_ChildCameras != null && mInstructionDictionary != null && mStateParentLookup != null)
			return;
		var virtualCameraBaseList = new List<CinemachineVirtualCameraBase>();
		foreach (var componentsInChild in GetComponentsInChildren<CinemachineVirtualCameraBase>(true))
			if (componentsInChild.transform.parent == transform)
				virtualCameraBaseList.Add(componentsInChild);
		m_ChildCameras = virtualCameraBaseList.ToArray();
		ValidateInstructions();
	}

	public void ValidateInstructions() {
		if (m_Instructions == null)
			m_Instructions = new Instruction[0];
		mInstructionDictionary = new Dictionary<int, int>();
		for (var index = 0; index < m_Instructions.Length; ++index) {
			if (m_Instructions[index].m_VirtualCamera != null &&
			    m_Instructions[index].m_VirtualCamera.transform.parent != transform)
				m_Instructions[index].m_VirtualCamera = null;
			mInstructionDictionary[m_Instructions[index].m_FullHash] = index;
		}

		mStateParentLookup = new Dictionary<int, int>();
		if (m_ParentHash != null)
			foreach (var parentHash in m_ParentHash)
				mStateParentLookup[parentHash.m_Hash] = parentHash.m_ParentHash;
		mActivationTime = mPendingActivationTime = 0.0f;
		mActiveBlend = null;
	}

	private CinemachineVirtualCameraBase ChooseCurrentCamera(float deltaTime) {
		if (m_ChildCameras == null || m_ChildCameras.Length == 0) {
			mActivationTime = 0.0f;
			return null;
		}

		var childCamera = m_ChildCameras[0];
		if (m_AnimatedTarget == null || !m_AnimatedTarget.gameObject.activeSelf ||
		    m_AnimatedTarget.runtimeAnimatorController == null || m_LayerIndex < 0 ||
		    m_LayerIndex >= m_AnimatedTarget.layerCount) {
			mActivationTime = 0.0f;
			return childCamera;
		}

		int key;
		if (m_AnimatedTarget.IsInTransition(m_LayerIndex)) {
			var animatorStateInfo = m_AnimatedTarget.GetNextAnimatorStateInfo(m_LayerIndex);
			key = animatorStateInfo.fullPathHash;
			if (m_AnimatedTarget.GetNextAnimatorClipInfoCount(m_LayerIndex) > 1) {
				m_AnimatedTarget.GetNextAnimatorClipInfo(m_LayerIndex, m_clipInfoList);
				key = GetClipHash(animatorStateInfo.fullPathHash, m_clipInfoList);
			}
		} else {
			var animatorStateInfo = m_AnimatedTarget.GetCurrentAnimatorStateInfo(m_LayerIndex);
			key = animatorStateInfo.fullPathHash;
			if (m_AnimatedTarget.GetCurrentAnimatorClipInfoCount(m_LayerIndex) > 1) {
				m_AnimatedTarget.GetCurrentAnimatorClipInfo(m_LayerIndex, m_clipInfoList);
				key = GetClipHash(animatorStateInfo.fullPathHash, m_clipInfoList);
			}
		}

		while (key != 0 && !mInstructionDictionary.ContainsKey(key))
			key = mStateParentLookup.ContainsKey(key) ? mStateParentLookup[key] : 0;
		var time = Time.time;
		if (mActivationTime != 0.0) {
			if (mActiveInstruction.m_FullHash == key) {
				mPendingActivationTime = 0.0f;
				return mActiveInstruction.m_VirtualCamera;
			}

			if (deltaTime >= 0.0 && mPendingActivationTime != 0.0 && mPendingInstruction.m_FullHash == key) {
				if (time - (double)mPendingActivationTime > mPendingInstruction.m_ActivateAfter &&
				    (time - (double)mActivationTime > mActiveInstruction.m_MinDuration ||
				     mPendingInstruction.m_VirtualCamera.Priority > mActiveInstruction.m_VirtualCamera.Priority)) {
					mActiveInstruction = mPendingInstruction;
					mActivationTime = time;
					mPendingActivationTime = 0.0f;
				}

				return mActiveInstruction.m_VirtualCamera;
			}
		}

		mPendingActivationTime = 0.0f;
		if (!mInstructionDictionary.ContainsKey(key))
			return mActivationTime != 0.0 ? mActiveInstruction.m_VirtualCamera : childCamera;
		var instruction = m_Instructions[mInstructionDictionary[key]];
		if (instruction.m_VirtualCamera == null)
			instruction.m_VirtualCamera = childCamera;
		if (deltaTime >= 0.0 && mActivationTime > 0.0 && (instruction.m_ActivateAfter > 0.0 ||
		                                                  (time - (double)mActivationTime <
		                                                   mActiveInstruction.m_MinDuration &&
		                                                   instruction.m_VirtualCamera.Priority <=
		                                                   mActiveInstruction.m_VirtualCamera.Priority))) {
			mPendingInstruction = instruction;
			mPendingActivationTime = time;
			return mActivationTime != 0.0 ? mActiveInstruction.m_VirtualCamera : childCamera;
		}

		mActiveInstruction = instruction;
		mActivationTime = time;
		return mActiveInstruction.m_VirtualCamera;
	}

	private int GetClipHash(int hash, List<AnimatorClipInfo> clips) {
		if (clips.Count > 1) {
			var index1 = -1;
			AnimatorClipInfo clip;
			for (var index2 = 0; index2 < clips.Count; ++index2) {
				int num;
				if (index1 >= 0) {
					clip = clips[index2];
					double weight1 = clip.weight;
					clip = clips[index1];
					double weight2 = clip.weight;
					num = weight1 > weight2 ? 1 : 0;
				} else
					num = 1;

				if (num != 0)
					index1 = index2;
			}

			int num1;
			if (index1 >= 0) {
				clip = clips[index1];
				num1 = clip.weight > 0.0 ? 1 : 0;
			} else
				num1 = 0;

			if (num1 != 0) {
				var parentHash = hash;
				clip = clips[index1];
				var name = clip.clip.name;
				hash = Animator.StringToHash(CreateFakeHashName(parentHash, name));
			}
		}

		return hash;
	}

	private AnimationCurve LookupBlendCurve(
		ICinemachineCamera fromKey,
		ICinemachineCamera toKey,
		out float duration) {
		var defaultCurve = m_DefaultBlend.BlendCurve;
		if (m_CustomBlends != null)
			defaultCurve = m_CustomBlends.GetBlendCurveForVirtualCameras(fromKey != null ? fromKey.Name : string.Empty,
				toKey != null ? toKey.Name : string.Empty, defaultCurve);
		var keys = defaultCurve.keys;
		duration = keys == null || keys.Length == 0 ? 0.0f : keys[keys.Length - 1].time;
		return defaultCurve;
	}

	private CinemachineBlend CreateBlend(
		ICinemachineCamera camA,
		ICinemachineCamera camB,
		AnimationCurve blendCurve,
		float duration,
		CinemachineBlend activeBlend,
		float deltaTime) {
		if (blendCurve == null || duration <= 0.0 || (camA == null && camB == null))
			return null;
		if (camA == null || activeBlend != null)
			camA = new StaticPointVirtualCamera(activeBlend != null ? activeBlend.State : State,
				activeBlend != null ? "Mid-blend" : "(none)");
		return new CinemachineBlend(camA, camB, blendCurve, duration, 0.0f);
	}

	[Serializable]
	public struct Instruction {
		[Tooltip("The full hash of the animation state")]
		public int m_FullHash;

		[Tooltip("The virtual camera to activate whrn the animation state becomes active")]
		public CinemachineVirtualCameraBase m_VirtualCamera;

		[Tooltip(
			"How long to wait (in seconds) before activating the virtual camera. This filters out very short state durations")]
		public float m_ActivateAfter;

		[Tooltip("The minimum length of time (in seconds) to keep a virtual camera active")]
		public float m_MinDuration;
	}

	[DocumentationSorting(13.2f, DocumentationSortingAttribute.Level.Undoc)]
	[Serializable]
	public struct ParentHash {
		public int m_Hash;
		public int m_ParentHash;

		public ParentHash(int h, int p) {
			m_Hash = h;
			m_ParentHash = p;
		}
	}
}