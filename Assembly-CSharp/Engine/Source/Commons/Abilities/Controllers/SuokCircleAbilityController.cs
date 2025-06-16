using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Effects.Values;
using Engine.Source.Services;
using Inspectors;
using System;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SuokCircleAbilityController : IAbilityController, IAbilityValueContainer, IUpdatable
  {
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected SuokCircleTutorialStateEnum state;
    private SuokCircleService suokService;
    private AbilityItem abilityItem;
    private AbilityValue<float> abilityValue;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.suokService = ServiceLocator.GetService<SuokCircleService>();
      this.abilityValue = new AbilityValue<float>();
      if (this.suokService != null)
      {
        this.abilityValue.Value = this.suokService.CurrentStamina;
        this.suokService.OnStateChangedEvent -= new Action(this.StateChanged);
        this.suokService.OnStateChangedEvent += new Action(this.StateChanged);
      }
      this.StateChanged();
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Shutdown()
    {
      if (this.suokService != null)
        this.suokService.OnStateChangedEvent -= new Action(this.StateChanged);
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    private void StateChanged()
    {
      this.abilityItem.Active = this.suokService.GetState() == this.state;
    }

    public void ComputeUpdate()
    {
      if (this.suokService == null)
        return;
      this.abilityValue.Value = this.suokService.CurrentStamina;
    }

    public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum parameter) where T : struct
    {
      return parameter == AbilityValueNameEnum.SuokCircleStamina && this.suokService != null ? (IAbilityValue<T>) (this.abilityValue as AbilityValue<T>) : (IAbilityValue<T>) null;
    }
  }
}
