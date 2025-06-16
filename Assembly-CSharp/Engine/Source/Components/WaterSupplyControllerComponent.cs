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
    public IParameterValue<int> Bullets { get; } = (IParameterValue<int>) new ParameterValue<int>();

    [Inspected]
    public IParameterValue<float> Durability { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<LiquidTypeEnum> LiquidType { get; } = (IParameterValue<LiquidTypeEnum>) new ParameterValue<LiquidTypeEnum>();

    public override void OnAdded()
    {
      base.OnAdded();
      this.Bullets.Set<int>(this.parameters.GetByName<int>(ParameterNameEnum.Bullets));
      this.Durability.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Durability));
      this.LiquidType.Set<LiquidTypeEnum>(this.parameters.GetByName<LiquidTypeEnum>(ParameterNameEnum.LiquidType));
    }

    public override void OnRemoved()
    {
      this.Bullets.Set<int>((IParameter<int>) null);
      this.Durability.Set<float>((IParameter<float>) null);
      this.LiquidType.Set<LiquidTypeEnum>((IParameter<LiquidTypeEnum>) null);
      base.OnRemoved();
    }
  }
}
