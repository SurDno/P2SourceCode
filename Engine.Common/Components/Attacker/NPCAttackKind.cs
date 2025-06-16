// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Attacker.NPCAttackKind
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;
using System.ComponentModel;

#nullable disable
namespace Engine.Common.Components.Attacker
{
  [EnumType("AttackerNPCAttackType")]
  public enum NPCAttackKind
  {
    [Description("Frontal punch")] FrontPunch = 1,
    [Description("Frontal dodge and counter punch")] FrontDodgeCounterPunch = 2,
    [Description("Frontal push")] FrontPush = 3,
    [Description("Frontal punch to block")] FrontPunchBlocked = 4,
    [Description("Frontal punch to block passed")] FrontPunchBlockPassed = 5,
  }
}
