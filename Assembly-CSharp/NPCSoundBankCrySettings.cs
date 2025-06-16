using System;
using Engine.Source.Connections;
using Engine.Source.Services;
using UnityEngine.Serialization;

[Serializable]
public class NPCSoundBankCrySettings
{
  public CombatCryEnum Name;
  public LipSyncObjectSerializable Description;
  [FormerlySerializedAs("chance")]
  public float Chance = 1f;
  public float MinDistance = 1f;
  public float MaxDistance = 30f;
}
