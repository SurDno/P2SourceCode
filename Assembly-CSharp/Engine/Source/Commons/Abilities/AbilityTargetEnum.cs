// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.AbilityTargetEnum
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
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
