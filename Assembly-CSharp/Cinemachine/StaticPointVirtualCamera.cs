using UnityEngine;

namespace Cinemachine
{
  internal class StaticPointVirtualCamera(CameraState state, string name) : ICinemachineCamera 
  {
    public void SetState(CameraState state) => State = state;

    public string Name { get; private set; } = name;

    public string Description => "";

    public int Priority { get; set; }

    public Transform LookAt { get; set; }

    public Transform Follow { get; set; }

    public CameraState State { get; private set; } = state;

    public GameObject VirtualCameraGameObject => null;

    public ICinemachineCamera LiveChildOrSelf => this;

    public ICinemachineCamera ParentCamera => null;

    public bool IsLiveChild(ICinemachineCamera vcam) => false;

    public void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
    }

    public void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
    }
  }
}
