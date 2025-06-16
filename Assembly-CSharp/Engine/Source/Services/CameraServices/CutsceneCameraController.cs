using Engine.Common;
using UnityEngine;

namespace Engine.Source.Services.CameraServices
{
  public class CutsceneCameraController : ICameraController
  {
    public void Initialise()
    {
    }

    public void Shutdown()
    {
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      if (gameObjectTarget == null)
        return;
      GameCamera.Instance.CameraTransform.position = gameObjectTarget.transform.position;
      GameCamera.Instance.CameraTransform.rotation = gameObjectTarget.transform.rotation;
    }
  }
}
