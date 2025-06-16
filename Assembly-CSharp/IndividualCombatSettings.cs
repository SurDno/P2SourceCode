// Decompiled with JetBrains decompiler
// Type: IndividualCombatSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using Engine.Common.Commons;
using System;
using UnityEngine;

#nullable disable
[Serializable]
public class IndividualCombatSettings
{
  [SerializeField]
  public CombatStyleEnum Name;
  [SerializeField]
  public ExternalBehaviorTree FightAI;
  [SerializeField]
  public ExternalBehaviorTree EscapeAI;
  [SerializeField]
  public ExternalBehaviorTree SurrenderAI;
  [SerializeField]
  public ExternalBehaviorTree WatchFightingAI;
  [SerializeField]
  public ExternalBehaviorTree LootAI;
  [SerializeField]
  public ExternalBehaviorTree GoToPointAI;
  [SerializeField]
  public bool CanWatchFight;
  [SerializeField]
  public bool CanFight;
}
