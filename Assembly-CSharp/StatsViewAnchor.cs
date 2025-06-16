using UnityEngine;

public class StatsViewAnchor : MonoBehaviour {
	private static StatsView viewInstance;
	[SerializeField] private StatsView prefab;
	[SerializeField] private bool fullVersion;

	private void OnEnable() {
		if (viewInstance == null)
			viewInstance = Instantiate(prefab, transform, false);
		else
			viewInstance.transform.SetParent(transform, false);
		viewInstance.SetFullVersion(fullVersion);
	}
}