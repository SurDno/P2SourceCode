using Engine.Source.UI.Menu.Main;
using UnityEngine;

public class UIRoot : MonoBehaviour {
	[SerializeField] private GameObject root;
	[SerializeField] private GameObject[] prefabs;
	[Space] [SerializeField] private SmallLoading smallLoading;
	[SerializeField] private GameObject virtualCursor;

	public SmallLoading SmallLoading => smallLoading;

	public GameObject VirtualCursor => virtualCursor;

	public GameObject Root => root;

	public void Initialize() {
		var transform = root?.transform;
		if (transform == null)
			transform = this.transform;
		foreach (var prefab in prefabs)
			if (!(prefab == null)) {
				var gameObject = Instantiate(prefab, transform, false);
				gameObject.SetActive(false);
				gameObject.name = prefab.name;
			}
	}
}