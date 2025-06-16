// Decompiled with JetBrains decompiler
// Type: KeepDistanceDescription
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "KeepDistance", menuName = "Pathologic2/States/Keep distance", order = 101)]
public class KeepDistanceDescription : ScriptableObject
{
  [Tooltip("NPC стремится сохранять эту дистанцию")]
  public float KeepDistance = 3f;
  [Tooltip("NPC не отступает, если стена сзади ближе этого расстояния")]
  public float BackDistance = 0.5f;
  [Tooltip("Если дистанция до игрока меньше, то NPC может атаковать")]
  public float AttackDistance = 1.75f;
  [Header("Удары")]
  [Tooltip("Среднее время выбрасывания удара на месте.")]
  public float PunchCooldownTime = 0.5f;
  [Tooltip("Среднее время выбрасывания удара с шагом вперед.")]
  public float StepPunchCooldownTime = 0.75f;
  [Tooltip("Кулдаун бросания бомбы")]
  public float ThrowCooldownTime = 3f;
  [Tooltip("Среднее время выбрасывания удара с телеграфированием.")]
  public float TelegraphPunchCooldownTime = 1f;
  [Tooltip("Среднее время выбрасывания обманного движения без удара")]
  public float CheatCooldownTime = 0.5f;
  [Tooltip("Вероятность, что удар - обманка")]
  public float CheatProbability = 0.25f;
}
