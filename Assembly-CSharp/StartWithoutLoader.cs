using UnityEngine;
using UnityEngine.SceneManagement;

public class StartWithoutLoader : MonoBehaviour {
	[SerializeField] private GameObject[] prefabs;

	private void Awake() {
		if (SceneManager.GetActiveScene() == this.gameObject.scene) {
			var siblingIndex = transform.GetSiblingIndex();
			foreach (var prefab in prefabs)
				if (!(prefab == null)) {
					var gameObject = Instantiate(prefab);
					gameObject.name = prefab.name;
					gameObject.transform.SetSiblingIndex(siblingIndex);
					++siblingIndex;
				}
		}

		Destroy(this.gameObject);
	}
}