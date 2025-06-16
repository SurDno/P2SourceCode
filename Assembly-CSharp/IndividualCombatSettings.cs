using System;
using BehaviorDesigner.Runtime;
using Engine.Common.Commons;

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
