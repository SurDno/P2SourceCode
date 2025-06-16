using UnityEngine;

namespace Cinemachine
{
  internal class BlendSourceVirtualCamera : ICinemachineCamera
  {
    public BlendSourceVirtualCamera(CinemachineBlend blend, float deltaTime)
    {
      this.Blend = blend;
      this.UpdateCameraState(blend.CamA.State.ReferenceUp, deltaTime);
    }

    public CinemachineBlend Blend { get; private set; }

    public string Name => "Blend";

    public string Description => this.Blend.Description;

    public int Priority { get; set; }

    public Transform LookAt { get; set; }

    public Transform Follow { get; set; }

    public CameraState State { get; private set; }

    public GameObject VirtualCameraGameObject => (GameObject) null;

    public ICinemachineCamera LiveChildOrSelf => this.Blend.CamB;

    public ICinemachineCamera ParentCamera => (ICinemachineCamera) null;

    public bool IsLiveChild(ICinemachineCamera vcam)
    {
      return vcam == this.Blend.CamA || vcam == this.Blend.CamB;
    }

    public CameraState CalculateNewState(float deltaTime) => this.State;

    public void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      this.Blend.UpdateCameraState(worldUp, deltaTime);
      this.State = this.Blend.State;
    }

    public void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
    }
  }
}
