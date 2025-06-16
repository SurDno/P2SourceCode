using System.ComponentModel;
using Engine.Common.Binders;

namespace Engine.Common.Components.Attacker;

[EnumType("AttackerNPCAttackType")]
public enum NPCAttackKind {
	[Description("Frontal punch")] FrontPunch = 1,

	[Description("Frontal dodge and counter punch")]
	FrontDodgeCounterPunch = 2,
	[Description("Frontal push")] FrontPush = 3,

	[Description("Frontal punch to block")]
	FrontPunchBlocked = 4,

	[Description("Frontal punch to block passed")]
	FrontPunchBlockPassed = 5
}