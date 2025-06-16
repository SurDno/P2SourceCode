using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine;

[DocumentationSorting(23f, DocumentationSortingAttribute.Level.API)]
public abstract class CinemachineExtension : MonoBehaviour {
	protected const float Epsilon = 0.0001f;
	private CinemachineVirtualCameraBase m_vcamOwner;
	private Dictionary<ICinemachineCamera, object> mExtraState;

	public CinemachineVirtualCameraBase VirtualCamera {
		get {
			if (m_vcamOwner == null)
				m_vcamOwner = GetComponent<CinemachineVirtualCameraBase>();
			return m_vcamOwner;
		}
	}

	protected virtual void Awake() {
		ConnectToVcam();
	}

	protected virtual void OnDestroy() {
		if (!(VirtualCamera != null))
			return;
		VirtualCamera.RemovePostPipelineStageHook(PostPipelineStageCallback);
	}

	private void ConnectToVcam() {
		if (VirtualCamera == null)
			Debug.LogError("CinemachineExtension requires a Cinemachine Virtual Camera component");
		else
			VirtualCamera.AddPostPipelineStageHook(PostPipelineStageCallback);
		mExtraState = null;
	}

	protected abstract void PostPipelineStageCallback(
		CinemachineVirtualCameraBase vcam,
		CinemachineCore.Stage stage,
		ref CameraState state,
		float deltaTime);

	protected T GetExtraState<T>(ICinemachineCamera vcam) where T : class, new() {
		if (mExtraState == null)
			mExtraState = new Dictionary<ICinemachineCamera, object>();
		object extraState = null;
		if (!mExtraState.TryGetValue(vcam, out extraState))
			extraState = mExtraState[vcam] = new T();
		return extraState as T;
	}

	protected List<T> GetAllExtraStates<T>() where T : class, new() {
		var allExtraStates = new List<T>();
		if (mExtraState != null)
			foreach (var keyValuePair in mExtraState)
				allExtraStates.Add(keyValuePair.Value as T);
		return allExtraStates;
	}
}