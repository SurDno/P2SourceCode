using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;

namespace Engine.Common.Components
{
  public interface IRegionComponent : IComponent
  {
    RegionEnum Region { get; }

    IParameterValue<int> DiseaseLevel { get; }

    IParameterValue<float> Reputation { get; }
  }
}
