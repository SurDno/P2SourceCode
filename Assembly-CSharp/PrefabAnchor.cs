using UnityEngine;

public class PrefabAnchor : MonoBehaviour {
	[SerializeField] private GameObject prefab;
	[SerializeField] private bool worldSpace;
	[SerializeField] private bool destroyOnDisable;
	private GameObject instance;

	private void OnEnable() {
		if (instance == null) {
			if (!(prefab != null))
				return;
			instance = !worldSpace ? Instantiate(prefab, transform, false) : Instantiate(prefab);
			instance.name = prefab.name;
		} else {
			if (!worldSpace)
				return;
			instance.SetActive(true);
		}
	}

	private void OnDisable() {
		if (instance == null)
			return;
		if (destroyOnDisable) {
			Destroy(instance);
			instance = null;
		} else {
			if (!worldSpace)
				return;
			instance.SetActive(false);
		}
	}
}