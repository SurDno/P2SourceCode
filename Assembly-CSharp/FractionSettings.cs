using Engine.Common.Commons;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FractionSettings
{
  [SerializeField]
  public FractionEnum Name;
  [SerializeField]
  public List<FractionRelationGroup> Relations;
  [SerializeField]
  public float PlayerReputationThreshold;
  [SerializeField]
  public float PlayerTradeReputationThreshold;
  [SerializeField]
  public float InfectionThreshold;
}
