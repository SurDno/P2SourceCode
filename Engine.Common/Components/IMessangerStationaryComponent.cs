using Engine.Common.Components.MessangerStationary;

namespace Engine.Common.Components
{
  public interface IMessangerStationaryComponent : IComponent
  {
    SpawnpointKindEnum SpawnpointKind { get; set; }

    void StartTeleporting();

    void StopTeleporting();
  }
}
