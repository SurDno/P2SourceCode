using UnityEngine;

namespace SRF;

public static class SRFGameObjectExtensions {
	public static T GetIComponent<T>(this GameObject t) where T : class {
		return t.GetComponent(typeof(T)) as T;
	}

	public static T GetComponentOrAdd<T>(this GameObject obj) where T : Component {
		var componentOrAdd = obj.GetComponent<T>();
		if (componentOrAdd == null)
			componentOrAdd = obj.AddComponent<T>();
		return componentOrAdd;
	}

	public static void RemoveComponentIfExists<T>(this GameObject obj) where T : Component {
		var component = obj.GetComponent<T>();
		if (!(component != null))
			return;
		Object.Destroy(component);
	}

	public static void RemoveComponentsIfExists<T>(this GameObject obj) where T : Component {
		foreach (var component in obj.GetComponents<T>())
			Object.Destroy(component);
	}

	public static bool EnableComponentIfExists<T>(this GameObject obj, bool enable = true) where T : MonoBehaviour {
		var component = obj.GetComponent<T>();
		if (component == null)
			return false;
		component.enabled = enable;
		return true;
	}

	public static void SetLayerRecursive(this GameObject o, int layer) {
		SetLayerInternal(o.transform, layer);
	}

	private static void SetLayerInternal(Transform t, int layer) {
		t.gameObject.layer = layer;
		foreach (Transform t1 in t)
			SetLayerInternal(t1, layer);
	}
}