using Cofe.Utility;
using Scripts.Data;
using UnityEngine;

namespace Engine.Source.Commons
{
  public class GameDataService : InstanceByRequest<GameDataService>
  {
    private GameDataInfo currentGameData;

    public GameDataInfo GetCurrentGameData()
    {
      return this.currentGameData != null ? this.currentGameData : BuildSettingsUtility.GetDefaultGameData();
    }

    public void SetCurrentGameData(string projectName)
    {
      this.currentGameData = !projectName.IsNullOrEmpty() ? BuildSettingsUtility.GetGameData(projectName) : (GameDataInfo) null;
      Debug.Log((object) ("GameDataService : SetCurrentGameData : " + projectName));
    }
  }
}
