// Decompiled with JetBrains decompiler
// Type: Engine.Common.Commons.ActionEnum
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;

#nullable disable
namespace Engine.Common.Commons
{
  [EnumType("Action")]
  public enum ActionEnum
  {
    None,
    Theft,
    BreakPicklock,
    BreakContainer,
    LootDeadCharacter,
    CollectItem,
    EnterWithoutKnock,
    Autopsy,
    ShootNpc,
    ShootThugNpc,
    MurderNpc,
    MurderThugNpc,
    SafeNpc,
    HitNpc,
    HitThugNpc,
    PacifiedTheAgony,
    SeeInfected,
    TakeItemsFromSurrender,
    HitAnotherNPC,
    FirstAttackNPC,
    FirstAttackThugNPC,
    KillDyingNpc,
    EutanizeDyingNpc,
    HealNpcPain,
    HealNpcInfection,
    HitAnotherGoodNPC,
    CureInfection,
    GiftNPC,
    RepairHydrant,
  }
}
