using System;

namespace Engine.Source.Commons.Abilities
{
  [Flags]
  public enum AbilityTargetEnum
  {
    ItemPlayer = 1,
    ItemNpc = 2,
    Item = ItemNpc | ItemPlayer, // 0x00000003
    SelfPlayer = 4,
    SelfNpc = 8,
    Self = SelfNpc | SelfPlayer, // 0x0000000C
    TargetPlayer = 16, // 0x00000010
    TargetNpc = 32, // 0x00000020
    Target = TargetNpc | TargetPlayer, // 0x00000030
  }
}
