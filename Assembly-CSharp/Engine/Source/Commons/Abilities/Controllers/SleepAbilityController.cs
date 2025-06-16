using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SleepAbilityController : IAbilityController, IChangeParameterListener
  {
    private ParametersComponent controller;
    private IParameter<bool> parameter;
    private AbilityItem abilityItem;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      controller = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
      if (controller == null)
        return;
      parameter = controller.GetByName<bool>(ParameterNameEnum.Sleep);
      if (parameter != null)
      {
        parameter.AddListener(this);
        OnParameterChanged(parameter);
      }
    }

    public void OnParameterChanged(IParameter parameter)
    {
      abilityItem.Active = ((IParameter<bool>) parameter).Value;
    }

    public void Shutdown()
    {
      if (controller == null)
        return;
      if (parameter != null)
      {
        parameter.RemoveListener(this);
        parameter = null;
      }
      controller = null;
    }
  }
}
