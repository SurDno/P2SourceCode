using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteInEditMode]
public class CameraDistance : MonoBehaviour {
	[SerializeField] private FloatView view;
	private Transform cachedTransform;

	private void OnPreCullEvent(Camera camera) {
		if (Profiler.enabled)
			Profiler.BeginSample(nameof(CameraDistance));
		OnPreCullEvent2(camera);
		if (!Profiler.enabled)
			return;
		Profiler.EndSample();
	}

	private void OnPreCullEvent2(Camera camera) {
		if (((1 << gameObject.layer) & camera.cullingMask) == 0 || view == null)
			return;
		view.FloatValue = Vector3.Distance(camera.transform.position, cachedTransform.position);
	}

	private void OnEnable() {
		cachedTransform = transform;
		Camera.onPreCull += OnPreCullEvent;
	}

	private void OnDisable() {
		Camera.onPreCull -= OnPreCullEvent;
		cachedTransform = null;
	}
}