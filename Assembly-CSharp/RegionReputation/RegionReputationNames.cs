using Engine.Common.Components.Regions;
using Engine.Source.Commons;
using Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
