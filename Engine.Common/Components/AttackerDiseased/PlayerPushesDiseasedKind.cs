using System.ComponentModel;
using Engine.Common.Binders;

namespace Engine.Common.Components.AttackerDiseased
{
  [EnumType("AttackerDiseasedPlayerPushKind")]
  public enum PlayerPushesDiseasedKind
  {
    [Description("Unknown")] Unknown,
    [Description("Unknown")] FrontalPush,
    [Description("Push to left")] PushToLeft,
    [Description("Push to left")] PushToRight,
  }
}
