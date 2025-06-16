using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
public class ImageEffectBase : MonoBehaviour {
	public Shader shader;
	private Material m_Material;

	protected virtual void Start() {
		if (!SystemInfo.supportsImageEffects)
			enabled = false;
		else {
			if ((bool)(Object)shader && shader.isSupported)
				return;
			enabled = false;
		}
	}

	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material(shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}

			return m_Material;
		}
	}

	protected virtual void OnDisable() {
		if (!(bool)(Object)m_Material)
			return;
		DestroyImmediate(m_Material);
	}
}