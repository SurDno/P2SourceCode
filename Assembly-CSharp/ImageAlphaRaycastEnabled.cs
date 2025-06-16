using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAlphaRaycastEnabled : MonoBehaviour {
	private void Start() {
		GetComponent<Image>().alphaHitTestMinimumThreshold = 0.25f;
	}
}