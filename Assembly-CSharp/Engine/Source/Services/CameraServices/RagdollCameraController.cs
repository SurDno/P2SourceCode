using Engine.Common;

namespace Engine.Source.Services.CameraServices
{
  public class RagdollCameraController : ICameraController
  {
    private float smoothTime = 0.05f;
    private float speedSmoothTime = 0.5f;

    public void Initialise()
    {
    }

    public void Shutdown()
    {
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      if ((Object) gameObjectTarget == (Object) null)
        return;
      smoothTime += speedSmoothTime * Time.deltaTime;
      Transform cameraTransform = GameCamera.Instance.CameraTransform;
      cameraTransform.position = Vector3.Lerp(cameraTransform.position, gameObjectTarget.transform.position, Time.deltaTime / smoothTime);
      cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, gameObjectTarget.transform.rotation, Time.deltaTime / smoothTime);
    }
  }
}
