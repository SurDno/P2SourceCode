using System;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("AttackerPlayer", typeof(IAttackerPlayerComponent))]
public class VMAttackerPlayer : VMEngineComponent<IAttackerPlayerComponent> {
	public const string ComponentName = "AttackerPlayer";

	[Property("IsUnholstered", "", false, false)]
	public bool IsUnholstered => Component.IsUnholstered;

	[Event("Hands holstered", "weapon type")]
	public event Action<WeaponKind> HandsHolsteredEvent;

	[Event("Hands unholstered", "weapon type")]
	public event Action<WeaponKind> HandsUnholsteredEvent;

	[Method("Set weapon", "weapon type", "")]
	public void SetWeapon(WeaponKind weaponKind) {
		Component.SetWeapon(weaponKind);
	}

	[Method("Hands unholster", "", "")]
	public void HandsUnholster() {
		Component.HandsUnholster();
	}

	[Method("Hands holster", "", "")]
	public void HandsHolster() {
		Component.HandsHolster();
	}

	[Method("Weapon hands unholster", "", "")]
	public void WeaponHandsUnholster() {
		Component.WeaponHandsUnholster();
	}

	[Method("Weapon firearm unholster", "", "")]
	public void WeaponFirearmUnholster() {
		Component.WeaponFirearmUnholster();
	}

	[Method("Weapon melee unholster", "", "")]
	public void WeaponMeleeUnholster() {
		Component.WeaponMeleeUnholster();
	}

	[Method("Weapon lamp unholster", "", "")]
	public void WeaponLampUnholsterWeapon() {
		Component.WeaponLampUnholster();
	}

	public void OnHandsHolstered(WeaponKind weaponKind) {
		HandsHolsteredEvent(weaponKind);
	}

	public void OnHandsUnHolstered(WeaponKind weaponKind) {
		HandsUnholsteredEvent(weaponKind);
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.WeaponHolsterStartEvent -= OnHandsHolstered;
		Component.WeaponUnholsterEndEvent -= OnHandsUnHolstered;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.WeaponHolsterStartEvent += OnHandsHolstered;
		Component.WeaponUnholsterEndEvent += OnHandsUnHolstered;
	}
}