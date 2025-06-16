using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class HolsterAbilityController : IAbilityController {
	[DataReadProxy(Name = "Weapon")]
	[DataWriteProxy(Name = "Weapon")]
	[CopyableProxy()]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected WeaponKind weaponKind = WeaponKind.Unknown;

	private AbilityItem abilityItem;
	private StorableComponent storable;
	private IAttackerPlayerComponent attacker;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		storable = this.abilityItem.Ability.Owner.GetComponent<StorableComponent>();
		if (storable == null)
			return;
		storable.ChangeStorageEvent += ChangeStorageEvent;
		CheckItem();
	}

	public void Shutdown() {
		Cleanup();
		if (storable != null) {
			storable.ChangeStorageEvent -= ChangeStorageEvent;
			abilityItem.Active = false;
		}

		abilityItem = null;
		storable = null;
	}

	private void ChangeStorageEvent(IStorableComponent sender) {
		CheckItem();
	}

	private void CheckItem() {
		Cleanup();
		var storage = storable.Storage;
		if (storage == null)
			return;
		attacker = storage.Owner.GetComponent<IAttackerPlayerComponent>();
		if (attacker != null) {
			attacker.WeaponHolsterStartEvent += WeaponHolsterStartEvent;
			attacker.WeaponUnholsterEndEvent += WeaponUnholsterEndEvent;
			UpdateAbility();
		}
	}

	private void Cleanup() {
		if (attacker == null)
			return;
		attacker.WeaponHolsterStartEvent -= WeaponHolsterStartEvent;
		attacker.WeaponUnholsterEndEvent -= WeaponUnholsterEndEvent;
		attacker = null;
		UpdateAbility();
	}

	private void WeaponHolsterStartEvent(WeaponKind weapon) {
		UpdateAbility();
	}

	private void WeaponUnholsterEndEvent(WeaponKind weapon) {
		UpdateAbility();
	}

	private void UpdateAbility() {
		abilityItem.Active = attacker != null && attacker.CurrentWeapon == weaponKind;
	}
}