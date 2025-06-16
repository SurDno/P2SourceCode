using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (IWaterSupplyControllerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class WaterSupplyControllerComponent : 
    EngineComponent,
    IWaterSupplyControllerComponent,
    IComponent
  {
    [FromThis]
    private ParametersComponent parameters;

    [Inspected]
    public IParameterValue<int> Bullets { get; } = new ParameterValue<int>();

    [Inspected]
    public IParameterValue<float> Durability { get; } = new ParameterValue<float>();

    [Inspected]
    public IParameterValue<LiquidTypeEnum> LiquidType { get; } = new ParameterValue<LiquidTypeEnum>();

    public override void OnAdded()
    {
      base.OnAdded();
      Bullets.Set(parameters.GetByName<int>(ParameterNameEnum.Bullets));
      Durability.Set(parameters.GetByName<float>(ParameterNameEnum.Durability));
      LiquidType.Set(parameters.GetByName<LiquidTypeEnum>(ParameterNameEnum.LiquidType));
    }

    public override void OnRemoved()
    {
      Bullets.Set(null);
      Durability.Set(null);
      LiquidType.Set(null);
      base.OnRemoved();
    }
  }
}
