// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.AttackerDiseased.PlayerPushesDiseasedKind
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;
using System.ComponentModel;

#nullable disable
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
