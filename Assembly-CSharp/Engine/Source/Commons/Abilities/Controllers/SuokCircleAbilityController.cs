using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Effects.Values;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SuokCircleAbilityController : IAbilityController, IAbilityValueContainer, IUpdatable
  {
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    protected SuokCircleTutorialStateEnum state;
    private SuokCircleService suokService;
    private AbilityItem abilityItem;
    private AbilityValue<float> abilityValue;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      suokService = ServiceLocator.GetService<SuokCircleService>();
      abilityValue = new AbilityValue<float>();
      if (suokService != null)
      {
        abilityValue.Value = suokService.CurrentStamina;
        suokService.OnStateChangedEvent -= StateChanged;
        suokService.OnStateChangedEvent += StateChanged;
      }
      StateChanged();
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Shutdown()
    {
      if (suokService != null)
        suokService.OnStateChangedEvent -= StateChanged;
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    private void StateChanged()
    {
      abilityItem.Active = suokService.GetState() == state;
    }

    public void ComputeUpdate()
    {
      if (suokService == null)
        return;
      abilityValue.Value = suokService.CurrentStamina;
    }

    public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum parameter) where T : struct
    {
      return parameter == AbilityValueNameEnum.SuokCircleStamina && suokService != null ? abilityValue as AbilityValue<T> : (IAbilityValue<T>) null;
    }
  }
}
