using UnityEngine;

public class ParticleBursterFire : MonoBehaviour {
	private ParticleBurster particleBurster;

	private void Awake() {
		particleBurster = GetComponent<ParticleBurster>();
	}

	private void OnEnable() {
		if (particleBurster == null)
			Debug.Log("particleBurster is null");
		else
			particleBurster.Fire();
	}
}