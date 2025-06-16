using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Color Adjustments/Contrast Enhance (Unsharp Mask)")]
public class ContrastEnhance : PostEffectsBase {
	[Range(0.0f, 1f)] public float intensity = 0.5f;
	[Range(0.0f, 0.999f)] public float threshold;
	private Material separableBlurMaterial;
	private Material contrastCompositeMaterial;
	[Range(0.0f, 1f)] public float blurSpread = 1f;
	public Shader separableBlurShader;
	public Shader contrastCompositeShader;

	public override bool CheckResources() {
		CheckSupport(false);
		contrastCompositeMaterial = CheckShaderAndCreateMaterial(contrastCompositeShader, contrastCompositeMaterial);
		separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, separableBlurMaterial);
		if (!isSupported)
			ReportAutoDisable();
		return isSupported;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (!CheckResources())
			Graphics.Blit(source, destination);
		else {
			var width = source.width;
			var height = source.height;
			var temporary1 = RenderTexture.GetTemporary(width / 2, height / 2, 0);
			Graphics.Blit(source, temporary1);
			var temporary2 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary1, temporary2);
			RenderTexture.ReleaseTemporary(temporary1);
			separableBlurMaterial.SetVector("offsets",
				new Vector4(0.0f, blurSpread * 1f / temporary2.height, 0.0f, 0.0f));
			var temporary3 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary2, temporary3, separableBlurMaterial);
			RenderTexture.ReleaseTemporary(temporary2);
			separableBlurMaterial.SetVector("offsets",
				new Vector4(blurSpread * 1f / temporary2.width, 0.0f, 0.0f, 0.0f));
			var temporary4 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary3, temporary4, separableBlurMaterial);
			RenderTexture.ReleaseTemporary(temporary3);
			contrastCompositeMaterial.SetTexture("_MainTexBlurred", temporary4);
			contrastCompositeMaterial.SetFloat("intensity", intensity);
			contrastCompositeMaterial.SetFloat("threshold", threshold);
			Graphics.Blit(source, destination, contrastCompositeMaterial);
			RenderTexture.ReleaseTemporary(temporary4);
		}
	}
}