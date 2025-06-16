using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Displacement/Fisheye")]
public class Fisheye : PostEffectsBase {
	[Range(0.0f, 1.5f)] public float strengthX = 0.05f;
	[Range(0.0f, 1.5f)] public float strengthY = 0.05f;
	public Shader fishEyeShader;
	private Material fisheyeMaterial;

	public override bool CheckResources() {
		CheckSupport(false);
		fisheyeMaterial = CheckShaderAndCreateMaterial(fishEyeShader, fisheyeMaterial);
		if (!isSupported)
			ReportAutoDisable();
		return isSupported;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (!CheckResources())
			Graphics.Blit(source, destination);
		else {
			var num1 = 5f / 32f;
			var num2 = (float)(source.width * 1.0 / (source.height * 1.0));
			fisheyeMaterial.SetVector("intensity",
				new Vector4(strengthX * num2 * num1, strengthY * num1, strengthX * num2 * num1, strengthY * num1));
			Graphics.Blit(source, destination, fisheyeMaterial);
		}
	}
}