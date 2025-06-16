using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Rendering/Global Fog")]
public class GlobalFog : GlobalFogBase {
	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		OnRenderImage_Internal(source, destination);
	}
}