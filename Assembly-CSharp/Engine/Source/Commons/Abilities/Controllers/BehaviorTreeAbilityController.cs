using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class BehaviorTreeAbilityController : IAbilityController {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected string name = "";

	private AbilityItem abilityItem;
	private BehaviorComponent behavior;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		behavior = abilityItem.Ability.Owner.GetComponent<BehaviorComponent>();
		if (behavior == null)
			return;
		behavior.OnAbility += OnAbility;
	}

	public void Shutdown() {
		if (behavior == null)
			return;
		behavior.OnAbility -= OnAbility;
		behavior = null;
	}

	private void OnAbility(string name, bool enable) {
		if (!(this.name == name))
			return;
		abilityItem.Active = enable;
	}
}