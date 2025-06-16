using System.ComponentModel;
using Engine.Common.Binders;

namespace Engine.Common.Components.Attacker;

[EnumType("AttackerAttackType")]
public enum AttackKind {
	[Description("Frontal punch")] FrontPunch = 1,

	[Description("Frontal dodge and counter punch")]
	FrontDodgeCounterPunch = 2,
	[Description("Frontal push")] FrontPush = 3
}