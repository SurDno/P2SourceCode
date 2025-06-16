using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

public class TumbaLightTrigger : MonoBehaviour {
	[Inspected] private bool enabledLights;

	private GameObject GetPlayerGameObject() {
		return ((IEntityView)ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
	}

	private void OnEnable() {
		EnabledLights(false);
	}

	private void OnDisable() {
		enabledLights = false;
		EnabledLights(false);
	}

	private void OnTriggerEnter(Collider other) {
		if (enabledLights)
			return;
		var component = GetComponent<AudioSource>();
		if (component != null)
			component.PlayAndCheck();
		else
			Debug.LogWarning("No audio source", gameObject);
		var playerGameObject = GetPlayerGameObject();
		if (playerGameObject == null || !(other.gameObject == playerGameObject))
			return;
		enabledLights = true;
		EnabledLights(true);
	}

	private void EnabledLights(bool enable) {
		foreach (Behaviour componentsInChild in gameObject.GetComponentsInChildren<Light>())
			componentsInChild.enabled = enable;
	}
}