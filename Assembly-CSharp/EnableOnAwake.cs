using UnityEngine;

public class EnableOnAwake : MonoBehaviour {
	[SerializeField] private GameObject[] objects;

	private void Awake() {
		foreach (var gameObject in objects)
			if (gameObject != null)
				gameObject.SetActive(true);
	}
}