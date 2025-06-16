using Cinemachine;
using Engine.Common;
using System;
using UnityEngine;

namespace Engine.Source.Services.CameraServices
{
  public class CutsceneCinemachineCameraController : ICameraController
  {
    private CinemachineBrain brain;
    private UnityEngine.Camera cinemachineCamera;
    private float initialFov;
    private float initialFarPlane;
    private float initialNearPlane;
    private GameObject prevGameObjectTarget;

    public void Initialise()
    {
      this.initialFov = GameCamera.Instance.Camera.fieldOfView;
      this.initialFarPlane = GameCamera.Instance.Camera.farClipPlane;
      this.initialNearPlane = GameCamera.Instance.Camera.nearClipPlane;
    }

    private void SetGameObjectTarget(GameObject gameObjectTarget)
    {
      if ((UnityEngine.Object) gameObjectTarget == (UnityEngine.Object) this.prevGameObjectTarget)
        return;
      this.prevGameObjectTarget = gameObjectTarget;
      GameCamera.Instance.SettingsPostProcessingOverride.NestedOverride = gameObjectTarget?.GetComponent<PostProcessingStackOverride>();
      if ((UnityEngine.Object) this.brain != (UnityEngine.Object) null)
        this.brain.CameraProcessedEvent -= new Action(this.OnBrainCameraProcessed);
      this.brain = gameObjectTarget?.GetComponent<CinemachineBrain>();
      if ((UnityEngine.Object) this.brain != (UnityEngine.Object) null)
        this.brain.CameraProcessedEvent += new Action(this.OnBrainCameraProcessed);
      this.cinemachineCamera = gameObjectTarget?.GetComponent<UnityEngine.Camera>();
      if (!((UnityEngine.Object) this.cinemachineCamera != (UnityEngine.Object) null))
        return;
      this.cinemachineCamera.enabled = false;
    }

    public void Shutdown()
    {
      this.SetGameObjectTarget((GameObject) null);
      GameCamera.Instance.ResetCutsceneFov();
      GameCamera.Instance.Camera.nearClipPlane = this.initialNearPlane;
      GameCamera.Instance.Camera.farClipPlane = this.initialFarPlane;
      GameCamera.Instance.SettingsPostProcessingOverride.NestedOverride = GameCamera.Instance.GamePostProcessingOverride;
    }

    private void OnBrainCameraProcessed()
    {
      GameCamera.Instance.CameraTransform.position = this.brain.transform.position;
      GameCamera.Instance.CameraTransform.rotation = this.brain.transform.rotation;
      if (!((UnityEngine.Object) this.cinemachineCamera != (UnityEngine.Object) null))
        return;
      GameCamera.Instance.SetCutsceneFov(this.cinemachineCamera.fieldOfView);
      GameCamera.Instance.Camera.nearClipPlane = this.cinemachineCamera.nearClipPlane;
      GameCamera.Instance.Camera.farClipPlane = this.cinemachineCamera.farClipPlane;
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      this.SetGameObjectTarget(gameObjectTarget);
    }
  }
}
