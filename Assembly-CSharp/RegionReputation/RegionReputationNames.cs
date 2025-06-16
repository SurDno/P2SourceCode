// Decompiled with JetBrains decompiler
// Type: RegionReputation.RegionReputationNames
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Regions;
using Engine.Source.Commons;
using Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace RegionReputation
{
  [CreateAssetMenu(menuName = "Data/RegionReputationNames")]
  public class RegionReputationNames : ScriptableObject
  {
    [SerializeField]
    private RegionReputationNameItem[] Items;

    public string GetReputationName(RegionEnum region, float reputation)
    {
      GameDataInfo gameData = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData();
      RegionReputationNameItem reputationNameItem = ((IEnumerable<RegionReputationNameItem>) this.Items).FirstOrDefault<RegionReputationNameItem>((Func<RegionReputationNameItem, bool>) (o => o.DataName == gameData.GameName));
      if (reputationNameItem == null)
      {
        Debug.LogError((object) ("Data not found : " + gameData.GameName));
        return "";
      }
      int index = ((IEnumerable<RegionException>) reputationNameItem.RegionExceptions).FindIndex<RegionException>((Func<RegionException, bool>) (v => v.Region == region));
      return (index != -1 ? reputationNameItem.RegionExceptions[index].Signature : ValueLevel.GetSignature(reputationNameItem.ReputationLevels, reputation)) ?? reputation.ToString();
    }
  }
}
