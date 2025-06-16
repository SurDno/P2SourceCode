using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ContainerOpenAbilityController : IAbilityController, IChangeParameterListener {
	private AbilityItem abilityItem;
	private IParameter<ContainerOpenStateEnum> parameter;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		parameter = this.abilityItem.Ability.Owner.GetComponent<ParametersComponent>()
			?.GetByName<ContainerOpenStateEnum>(ParameterNameEnum.OpenState);
		if (parameter == null)
			return;
		parameter.AddListener(this);
	}

	public void OnParameterChanged(IParameter parameter) {
		if (this.parameter.Value != ContainerOpenStateEnum.Open)
			return;
		abilityItem.Active = true;
		abilityItem.Active = false;
	}

	public void Shutdown() {
		if (parameter != null)
			parameter.RemoveListener(this);
		abilityItem = null;
		parameter = null;
	}
}