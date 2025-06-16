using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(Light))]
public class LightCameraShadowDistance : MonoBehaviour {
	[SerializeField] private float shadowDistance = 15f;
	private Transform cachedTransform;
	private Light cachedLight;
	private LightShadows shadows;

	private void OnPreCullEvent(Camera camera) {
		if (Profiler.enabled)
			Profiler.BeginSample(nameof(LightCameraShadowDistance));
		OnPreCullEvent2(camera);
		if (!Profiler.enabled)
			return;
		Profiler.EndSample();
	}

	private void OnPreCullEvent2(Camera camera) {
		cachedLight.shadows =
			Vector3.Distance(camera.transform.position, cachedTransform.position) > (double)shadowDistance
				? LightShadows.None
				: shadows;
	}

	private void OnEnable() {
		cachedTransform = transform;
		cachedLight = GetComponent<Light>();
		shadows = cachedLight.shadows;
		Camera.onPreCull += OnPreCullEvent;
	}

	private void OnDisable() {
		Camera.onPreCull -= OnPreCullEvent;
		cachedLight.shadows = shadows;
		cachedLight = null;
		cachedTransform = null;
	}
}