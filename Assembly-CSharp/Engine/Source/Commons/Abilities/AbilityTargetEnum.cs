using System;

namespace Engine.Source.Commons.Abilities;

[Flags]
public enum AbilityTargetEnum {
	ItemPlayer = 1,
	ItemNpc = 2,
	Item = ItemNpc | ItemPlayer,
	SelfPlayer = 4,
	SelfNpc = 8,
	Self = SelfNpc | SelfPlayer,
	TargetPlayer = 16,
	TargetNpc = 32,
	Target = TargetNpc | TargetPlayer
}