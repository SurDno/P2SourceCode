using Engine.Common.Binders;
using System.ComponentModel;

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
