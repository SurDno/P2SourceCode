using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class LightShaftsSourceCapture : MonoBehaviour {
	public LayerMask shadowCastingLayers;

	private void OnDisable() {
		LightShafts.isLightActive = false;
		Shader.SetGlobalColor("_SunlightColor", Color.black);
	}

	private void Update() {
		var component = GetComponent<Light>();
		LightShafts.isLightActive = component.intensity > 0.0;
		LightShafts.LightDirection = transform.forward;
		LightShafts.shadowCastingColliders = shadowCastingLayers;
		Shader.SetGlobalColor("_SunlightColor", (component.color * component.intensity) with {
			a = 1f
		});
	}
}