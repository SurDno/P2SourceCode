using Engine.Common;

namespace Engine.Source.Services.CameraServices
{
  public interface ICameraController
  {
    void Initialise();

    void Shutdown();

    void Update(IEntity target, GameObject gameObjectTarget);
  }
}
