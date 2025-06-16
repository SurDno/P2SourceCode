// Decompiled with JetBrains decompiler
// Type: RetreatDescription
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "Retreat", menuName = "Pathologic2/States/Retreat", order = 101)]
public class RetreatDescription : ScriptableObject
{
  [Header("Передвижение")]
  [Tooltip("Если дистанция до игрока меньше, то NPC бежит задом")]
  public float RunDistance = 5f;
  [Tooltip("Если дистанция до игрока меньше, то NPC идет задом")]
  public float WalkDistance = 9f;
  [Tooltip("Если дистанция до игрока меньше, то NPC ждет приближения игрока")]
  public float StopDistance = 12f;
  [Tooltip("Если дистанция до игрока больше но нода возвращает успех и завершается")]
  public float EscapeDistance = 13f;
  [Header("Удары")]
  [Tooltip("Среднее время выбрасывания удара на месте.")]
  public float PunchCooldownTime = 1.5f;
  [Tooltip("Среднее время выбрасывания удара с шагом вперед.")]
  public float StepPunchCooldownTime = 2f;
  [Tooltip("Среднее время выбрасывания удара с телеграфированием.")]
  public float TelegraphPunchCooldownTime = 3f;
  [Tooltip("Среднее время выбрасывания обманного движения без удара")]
  public float CheatCooldownTime = 1.5f;
  [Tooltip("Вероятность, что удар - обманка")]
  public float CheatProbability = 0.25f;
  [Header("Другое")]
  [Tooltip("Если дистанция до игрока меньше, то NPC может атаковать")]
  public float AttackDistance = 2.5f;
}
