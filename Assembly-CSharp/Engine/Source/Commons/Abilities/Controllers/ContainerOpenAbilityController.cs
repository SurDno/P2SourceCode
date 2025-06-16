using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ContainerOpenAbilityController : IAbilityController, IChangeParameterListener
  {
    private AbilityItem abilityItem;
    private IParameter<ContainerOpenStateEnum> parameter;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.parameter = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>()?.GetByName<ContainerOpenStateEnum>(ParameterNameEnum.OpenState);
      if (this.parameter == null)
        return;
      this.parameter.AddListener((IChangeParameterListener) this);
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (this.parameter.Value != ContainerOpenStateEnum.Open)
        return;
      this.abilityItem.Active = true;
      this.abilityItem.Active = false;
    }

    public void Shutdown()
    {
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      this.abilityItem = (AbilityItem) null;
      this.parameter = (IParameter<ContainerOpenStateEnum>) null;
    }
  }
}
