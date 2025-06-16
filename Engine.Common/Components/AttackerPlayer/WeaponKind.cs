// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.AttackerPlayer.WeaponKind
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;
using System.ComponentModel;

#nullable disable
namespace Engine.Common.Components.AttackerPlayer
{
  [EnumType("WeaponKind")]
  public enum WeaponKind
  {
    [Description("Unknown")] Unknown,
    [Description("Hands")] Hands,
    [Description("Knife")] Knife,
    [Description("Revolver")] Revolver,
    [Description("Rifle")] Rifle,
    [Description("Visir")] Visir,
    [Description("Flashlight")] Flashlight,
    [Description("Scalpel")] Scalpel,
    [Description("Lockpick")] Lockpick,
    [Description("Bomb")] Bomb,
    [Description("Samopal")] Samopal,
    [Description("Shotgun")] Shotgun,
  }
}
