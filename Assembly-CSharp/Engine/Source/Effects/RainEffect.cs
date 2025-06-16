using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using Rain;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RainEffect : IEffect
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    private IParameter<float> rainParameter;

    public string Name => GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      rainParameter = Target.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Rain);
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      RainManager instance = RainManager.Instance;
      if (instance == null || rainParameter == null)
        return false;
      if (rainParameter.Value != (double) instance.rainIntensity)
      {
        rainParameter.BaseValue = instance.rainIntensity;
        rainParameter.Value = instance.rainIntensity;
      }
      return true;
    }

    public void Cleanup() => rainParameter = null;
  }
}
