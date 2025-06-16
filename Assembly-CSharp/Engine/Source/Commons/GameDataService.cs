using Cofe.Utility;
using Scripts.Data;
using UnityEngine;

namespace Engine.Source.Commons;

public class GameDataService : InstanceByRequest<GameDataService> {
	private GameDataInfo currentGameData;

	public GameDataInfo GetCurrentGameData() {
		return currentGameData != null ? currentGameData : BuildSettingsUtility.GetDefaultGameData();
	}

	public void SetCurrentGameData(string projectName) {
		currentGameData = !projectName.IsNullOrEmpty() ? BuildSettingsUtility.GetGameData(projectName) : null;
		Debug.Log("GameDataService : SetCurrentGameData : " + projectName);
	}
}