using UnityEngine;

[CreateAssetMenu(menuName = "Data/Load Window Game Data")]
public class LoadWindowGameData : ScriptableObject {
	[SerializeField] private LoadWindowGameDataItem[] items;

	public LoadWindowGameDataItem GetItem(string gameDataName) {
		for (var index = 0; index < items.Length; ++index) {
			var windowGameDataItem = items[index];
			if (items[index].GameDataName == gameDataName)
				return windowGameDataItem;
		}

		return LoadWindowGameDataItem.Null;
	}
}