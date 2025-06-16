using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ItemMountAbilityController : IAbilityController {
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)] [DataReadProxy] [DataWriteProxy] [CopyableProxy()]
	protected InventoryGroup group;

	private AbilityItem abilityItem;
	private StorableComponent storable;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		storable = this.abilityItem.Ability.Owner.GetComponent<StorableComponent>();
		if (storable == null)
			return;
		storable.ChangeStorageEvent += ChangeStorageEvent;
		CheckItem();
	}

	public void Shutdown() {
		if (storable != null) {
			storable.ChangeStorageEvent -= ChangeStorageEvent;
			abilityItem.Active = false;
		}

		abilityItem = null;
		storable = null;
	}

	private void CheckItem() {
		var container = storable.Container;
		abilityItem.Active = container != null && container.GetGroup() == group;
	}

	private void ChangeStorageEvent(IStorableComponent sender) {
		CheckItem();
	}
}