using UnityEngine;
using UnityEngine.UI;

namespace Pathologic.Prototype;

public class PlagueFaceTestController : MonoBehaviour {
	private bool _isInitialized;
	public PlagueFace face;
	public Transform graph;
	public Image overlay;
	public Camera playerCamera;
	public Transform playerCenter;

	private void Disable() {
		face.gameObject.SetActive(false);
	}

	private void Enable() {
		if (!_isInitialized || face.gameObject.activeSelf) {
			face.InitializeAt(graph.GetChild(Random.Range(0, graph.childCount)).GetComponent<PlagueFacePoint>());
			face.navigation.playerCamera = playerCamera;
			_isInitialized = true;
		}

		face.gameObject.SetActive(true);
	}

	private void Update() {
		face.playerPosition = playerCenter.position;
		overlay.color = new Color(0.25f, 0.0f, 0.0f, face.attack * 0.9f);
		if (Input.GetKeyDown(KeyCode.E))
			Enable();
		if (!Input.GetKeyDown(KeyCode.Q))
			return;
		Disable();
	}
}