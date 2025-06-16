// Decompiled with JetBrains decompiler
// Type: NPCSoundBankCrySettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Connections;
using Engine.Source.Services;
using System;
using UnityEngine.Serialization;

#nullable disable
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
