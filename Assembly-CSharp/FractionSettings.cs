// Decompiled with JetBrains decompiler
// Type: FractionSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
