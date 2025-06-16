using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlayerReputationEffect : IEffect
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    private IParameter<float> parameter;
    private NavigationComponent navigation;
    private PlayerControllerComponent controller;

    public string Name => GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => queue;

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      parameter = Target.GetComponent<ParametersComponent>().GetByName<float>(ParameterNameEnum.Reputation);
      navigation = Target.GetComponent<NavigationComponent>();
      controller = Target.GetComponent<PlayerControllerComponent>();
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if (navigation == null || controller == null || parameter == null)
        return false;
      IRegionComponent region = navigation.Region;
      if (region == null)
      {
        parameter.Value = 0.0f;
        return true;
      }
      parameter.Value = region.Reputation.Value;
      return true;
    }

    public void Cleanup()
    {
      navigation = null;
      controller = null;
      parameter = null;
    }
  }
}
