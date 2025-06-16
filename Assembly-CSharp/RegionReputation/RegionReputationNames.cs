using System.Linq;
using Engine.Common.Components.Regions;
using Engine.Source.Commons;
using Scripts.Data;
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
      RegionReputationNameItem reputationNameItem = Items.FirstOrDefault(o => o.DataName == gameData.GameName);
      if (reputationNameItem == null)
      {
        Debug.LogError("Data not found : " + gameData.GameName);
        return "";
      }
      int index = reputationNameItem.RegionExceptions.FindIndex(v => v.Region == region);
      return (index != -1 ? reputationNameItem.RegionExceptions[index].Signature : ValueLevel.GetSignature(reputationNameItem.ReputationLevels, reputation)) ?? reputation.ToString();
    }
  }
}
