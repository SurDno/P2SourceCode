using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Displacement/Twirl")]
public class Twirl : ImageEffectBase {
	public Vector2 radius = new(0.3f, 0.3f);
	[Range(0.0f, 360f)] public float angle = 50f;
	public Vector2 center = new(0.5f, 0.5f);

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		ImageEffects.RenderDistortion(material, source, destination, angle, center, radius);
	}
}