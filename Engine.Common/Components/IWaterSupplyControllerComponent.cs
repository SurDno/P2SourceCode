using Engine.Common.Commons;
using Engine.Common.Components.Parameters;

namespace Engine.Common.Components
{
  public interface IWaterSupplyControllerComponent : IComponent
  {
    IParameterValue<int> Bullets { get; }

    IParameterValue<float> Durability { get; }

    IParameterValue<LiquidTypeEnum> LiquidType { get; }
  }
}
