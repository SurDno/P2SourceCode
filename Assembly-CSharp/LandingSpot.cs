using System.Collections;
using UnityEngine;

public class LandingSpot : MonoBehaviour {
	[HideInInspector] public FlockChild landingChild;
	[HideInInspector] public bool landing;
	private int lerpCounter;
	[HideInInspector] public LandingSpotController _controller;
	private bool _idle;
	public bool _gotcha;

	public void StraightenBird() {
		if (landingChild.transform.eulerAngles.x == 0.0)
			return;
		landingChild.transform.eulerAngles = landingChild.transform.eulerAngles with {
			z = 0.0f
		};
	}

	public void RotateBird() {
		if (_controller._randomRotate && _idle)
			return;
		++lerpCounter;
		var rotation = landingChild.transform.rotation;
		var eulerAngles = rotation.eulerAngles with {
			y = Mathf.LerpAngle(landingChild.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.y,
				lerpCounter * Time.deltaTime * _controller._landedRotateSpeed)
		};
		rotation.eulerAngles = eulerAngles;
		landingChild.transform.rotation = rotation;
	}

	public IEnumerator GetFlockChild(float minDelay, float maxDelay) {
		yield break;
	}

	public void InstantLand() { }

	public void ReleaseFlockChild() { }
}