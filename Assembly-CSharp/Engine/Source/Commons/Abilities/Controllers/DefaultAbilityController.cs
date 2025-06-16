using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class DefaultAbilityController : IAbilityController {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected bool active = true;

	public void Initialise(AbilityItem abilityItem) {
		abilityItem.Active = active;
	}

	public void Shutdown() { }
}