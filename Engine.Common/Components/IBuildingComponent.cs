using Engine.Common.Components.Regions;

namespace Engine.Common.Components
{
  public interface IBuildingComponent : IComponent
  {
    BuildingEnum Building { get; }
  }
}
