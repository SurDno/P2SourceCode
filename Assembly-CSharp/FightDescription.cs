// Decompiled with JetBrains decompiler
// Type: FightDescription
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "FightDescription", menuName = "Pathologic2/Fight Description", order = 101)]
public class FightDescription : ScriptableObject
{
  [Tooltip("Через это время игрок может ударить повторно.")]
  public float PlayerPunchCooldownTime = 0.5f;
  [Tooltip("Время перехода в блок")]
  public float PlayerBlockStanceTime = 0.33f;
  [Tooltip("Через это время автоматически происходит апперкот.")]
  public float PlayerUppercutTime = 1.5f;
  [Header("NPC")]
  [Tooltip("С этого расстояния NPC попадает.")]
  public float NPCHitDistance = 1.8f;
  [Tooltip("Вероятность контратаки NPC на попадание игрока")]
  public float ContrReactionProbability = 0.75f;
  [Tooltip("Ниже этого значения считается, что стамины мало")]
  public float StaminaRegenerationBorder = 0.2f;
}
