// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Interactable.InteractType
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;
using System.ComponentModel;

#nullable disable
namespace Engine.Common.Components.Interactable
{
  [EnumType("InteractType")]
  public enum InteractType
  {
    [Description("None")] None,
    [Description("Interact")] Interact,
    [Description("Indoor")] Indoor,
    [Description("Outdoor")] Outdoor,
    [Description("Break")] Break,
    [Description("Keyhole")] Keyhole,
    [Description("Dialog")] Dialog,
    [Description("Trade")] Trade,
    [Description("Loot")] Loot,
    [Description("Autopsy")] Autopsy,
    [Description("OpenDoor")] OpenDoor,
    [Description("CloseDoor")] CloseDoor,
    [Description("Knock")] Knock,
    [Description("Block")] Block,
    [Description("Unblock")] Unblock,
    [Description("Mark")] Mark,
    [Description("Unmark")] Unmark,
    [Description("Collect")] Collect,
    [Description("Break Picklock")] BreakPicklock,
    [Description("Key Opening")] KeyOpening,
    [Description("Sleep")] Sleep,
    [Description("Heal")] Heal,
    [Description("Hydrant")] Hydrant,
    [Description("HydrantNoBottles")] HydrantNoBottles,
    [Description("TradeImpossible")] TradeImpossible,
    [Description("Repair")] Repair,
    [Description("AskForRepair")] AskForRepair,
    [Description("Upgrade")] Upgrade,
    [Description("Broken")] Broken,
    [Description("Empty")] Empty,
    [Description("IconNormal")] IconNormal,
    [Description("IconLocked")] IconLocked,
    [Description("IconBlocked")] IconBlocked,
    [Description("Drink")] Drink,
    [Description("BreakPicklockNoPick")] BreakPicklockNoPick,
    [Description("Prophylaxis")] Prophylaxis,
    [Description("HealEarthNoBlood")] HealEarthNoBlood,
    [Description("FastTravel")] FastTravel,
    [Description("Surgery")] Surgery,
    [Description("LightCandleNoMatches")] LightCandleNoMatches,
    [Description("TheaterAfisha")] TheaterAfisha,
    [Description("HealEarthExausted")] HealEarthExausted,
    [Description("CraftTable")] CraftTable,
  }
}
