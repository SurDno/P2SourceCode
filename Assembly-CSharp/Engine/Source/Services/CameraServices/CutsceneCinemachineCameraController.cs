using Cinemachine;
using Engine.Common;

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
      initialFov = GameCamera.Instance.Camera.fieldOfView;
      initialFarPlane = GameCamera.Instance.Camera.farClipPlane;
      initialNearPlane = GameCamera.Instance.Camera.nearClipPlane;
    }

    private void SetGameObjectTarget(GameObject gameObjectTarget)
    {
      if ((UnityEngine.Object) gameObjectTarget == (UnityEngine.Object) prevGameObjectTarget)
        return;
      prevGameObjectTarget = gameObjectTarget;
      GameCamera.Instance.SettingsPostProcessingOverride.NestedOverride = gameObjectTarget?.GetComponent<PostProcessingStackOverride>();
      if ((UnityEngine.Object) brain != (UnityEngine.Object) null)
        brain.CameraProcessedEvent -= OnBrainCameraProcessed;
      brain = gameObjectTarget?.GetComponent<CinemachineBrain>();
      if ((UnityEngine.Object) brain != (UnityEngine.Object) null)
        brain.CameraProcessedEvent += OnBrainCameraProcessed;
      cinemachineCamera = gameObjectTarget?.GetComponent<UnityEngine.Camera>();
      if (!((UnityEngine.Object) cinemachineCamera != (UnityEngine.Object) null))
        return;
      cinemachineCamera.enabled = false;
    }

    public void Shutdown()
    {
      SetGameObjectTarget((GameObject) null);
      GameCamera.Instance.ResetCutsceneFov();
      GameCamera.Instance.Camera.nearClipPlane = initialNearPlane;
      GameCamera.Instance.Camera.farClipPlane = initialFarPlane;
      GameCamera.Instance.SettingsPostProcessingOverride.NestedOverride = GameCamera.Instance.GamePostProcessingOverride;
    }

    private void OnBrainCameraProcessed()
    {
      GameCamera.Instance.CameraTransform.position = brain.transform.position;
      GameCamera.Instance.CameraTransform.rotation = brain.transform.rotation;
      if (!((UnityEngine.Object) cinemachineCamera != (UnityEngine.Object) null))
        return;
      GameCamera.Instance.SetCutsceneFov(cinemachineCamera.fieldOfView);
      GameCamera.Instance.Camera.nearClipPlane = cinemachineCamera.nearClipPlane;
      GameCamera.Instance.Camera.farClipPlane = cinemachineCamera.farClipPlane;
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      SetGameObjectTarget(gameObjectTarget);
    }
  }
}
