using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Effects.Values;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DeadBurningAbilityController : IAbilityController, IChangeParameterListener
  {
    private AbilityItem abilityItem;
    private IEntity itemOwner;
    private Dictionary<AbilityValueNameEnum, IAbilityValue> values = new Dictionary<AbilityValueNameEnum, IAbilityValue>();
    private IParameter<bool> burningParameter;
    private IParameter<float> healthParameter;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      ParametersComponent component = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
      burningParameter = component.GetByName<bool>(ParameterNameEnum.IsBurning);
      if (burningParameter == null)
        return;
      healthParameter = component.GetByName<float>(ParameterNameEnum.Health);
      if (healthParameter == null)
        return;
      burningParameter.AddListener(this);
      healthParameter.AddListener(this);
    }

    private void CheckParameters()
    {
      if (burningParameter == null || healthParameter == null || !burningParameter.Value || healthParameter.Value > 0.0)
        return;
      abilityItem.Active = true;
    }

    public void Shutdown()
    {
      if (burningParameter != null)
        burningParameter.RemoveListener(this);
      if (healthParameter != null)
        healthParameter.RemoveListener(this);
      abilityItem.Active = false;
      abilityItem = null;
    }

    public void OnParameterChanged(IParameter parameter) => CheckParameters();
  }
}
