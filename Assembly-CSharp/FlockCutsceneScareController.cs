using System.Collections;
using UnityEngine;

public class FlockCutsceneScareController : MonoBehaviour {
	public LandingSpotController landingSpotController;

	private void OnEnable() {
		StartCoroutine(Scare());
	}

	private IEnumerator Scare() {
		if (landingSpotController != null) {
			yield return new WaitForEndOfFrame();
			landingSpotController.ScareAll();
		}
	}
}