using UnityEngine;

namespace Cinemachine;

internal class BlendSourceVirtualCamera : ICinemachineCamera {
	public BlendSourceVirtualCamera(CinemachineBlend blend, float deltaTime) {
		Blend = blend;
		UpdateCameraState(blend.CamA.State.ReferenceUp, deltaTime);
	}

	public CinemachineBlend Blend { get; private set; }

	public string Name => "Blend";

	public string Description => Blend.Description;

	public int Priority { get; set; }

	public Transform LookAt { get; set; }

	public Transform Follow { get; set; }

	public CameraState State { get; private set; }

	public GameObject VirtualCameraGameObject => null;

	public ICinemachineCamera LiveChildOrSelf => Blend.CamB;

	public ICinemachineCamera ParentCamera => null;

	public bool IsLiveChild(ICinemachineCamera vcam) {
		return vcam == Blend.CamA || vcam == Blend.CamB;
	}

	public CameraState CalculateNewState(float deltaTime) {
		return State;
	}

	public void UpdateCameraState(Vector3 worldUp, float deltaTime) {
		Blend.UpdateCameraState(worldUp, deltaTime);
		State = Blend.State;
	}

	public void OnTransitionFromCamera(
		ICinemachineCamera fromCam,
		Vector3 worldUp,
		float deltaTime) { }
}