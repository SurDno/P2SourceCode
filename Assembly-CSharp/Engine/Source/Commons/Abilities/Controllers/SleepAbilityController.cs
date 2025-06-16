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
      this.controller = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>();
      if (this.controller == null)
        return;
      this.parameter = this.controller.GetByName<bool>(ParameterNameEnum.Sleep);
      if (this.parameter != null)
      {
        this.parameter.AddListener((IChangeParameterListener) this);
        this.OnParameterChanged((IParameter) this.parameter);
      }
    }

    public void OnParameterChanged(IParameter parameter)
    {
      this.abilityItem.Active = ((IParameter<bool>) parameter).Value;
    }

    public void Shutdown()
    {
      if (this.controller == null)
        return;
      if (this.parameter != null)
      {
        this.parameter.RemoveListener((IChangeParameterListener) this);
        this.parameter = (IParameter<bool>) null;
      }
      this.controller = (ParametersComponent) null;
    }
  }
}
