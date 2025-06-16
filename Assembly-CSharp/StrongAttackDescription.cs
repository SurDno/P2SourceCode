// Decompiled with JetBrains decompiler
// Type: StrongAttackDescription
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "StrongAttack", menuName = "Pathologic2/States/Strong attack", order = 101)]
public class StrongAttackDescription : ScriptableObject
{
  [Tooltip("Время атаки")]
  public float AttackTime = 7.5f;
}
