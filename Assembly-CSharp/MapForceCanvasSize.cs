using UnityEngine;

public class MapForceCanvasSize : MonoBehaviour {
	[SerializeField] private Vector2 size = new(1920f, 1080f);

	private void OnEnable() {
		var transform = (RectTransform)this.transform;
		transform.anchorMin = Vector2.zero;
		transform.anchorMax = Vector2.zero;
		transform.pivot = Vector2.zero;
		transform.anchoredPosition = Vector2.zero;
		transform.sizeDelta = size;
	}
}