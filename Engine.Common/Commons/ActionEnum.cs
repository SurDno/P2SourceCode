using Engine.Common.Binders;

namespace Engine.Common.Commons;

[EnumType("Action")]
public enum ActionEnum {
	None,
	Theft,
	BreakPicklock,
	BreakContainer,
	LootDeadCharacter,
	CollectItem,
	EnterWithoutKnock,
	Autopsy,
	ShootNpc,
	ShootThugNpc,
	MurderNpc,
	MurderThugNpc,
	SafeNpc,
	HitNpc,
	HitThugNpc,
	PacifiedTheAgony,
	SeeInfected,
	TakeItemsFromSurrender,
	HitAnotherNPC,
	FirstAttackNPC,
	FirstAttackThugNPC,
	KillDyingNpc,
	EutanizeDyingNpc,
	HealNpcPain,
	HealNpcInfection,
	HitAnotherGoodNPC,
	CureInfection,
	GiftNPC,
	RepairHydrant
}