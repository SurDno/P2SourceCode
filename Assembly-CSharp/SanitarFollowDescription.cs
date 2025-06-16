// Decompiled with JetBrains decompiler
// Type: SanitarFollowDescription
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "Sanitar follow", menuName = "Pathologic2/States/Sanitar follow", order = 101)]
public class SanitarFollowDescription : ScriptableObject
{
  [Tooltip("NPC стремится сохранять эту дистанцию")]
  public float KeepDistance = 2f;
  [Tooltip("Если враг ближе, то НПС отступает")]
  public float RetreatDistance = 1f;
  [Tooltip("Если игрок удалился на эту дистанцию, то NPC переходит на бег.")]
  public float RunDistance = 5f;
}
