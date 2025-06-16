using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Blur/Blur (Optimized)")]
public class BlurOptimized : PostEffectsBase {
	[Range(0.0f, 2f)] public int downsample = 1;
	[Range(0.0f, 10f)] public float blurSize = 3f;
	[Range(1f, 4f)] public int blurIterations = 2;
	public BlurType blurType = BlurType.StandardGauss;
	public Shader blurShader;
	private Material blurMaterial;

	public override bool CheckResources() {
		CheckSupport(false);
		blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
		if (!isSupported)
			ReportAutoDisable();
		return isSupported;
	}

	public void OnDisable() {
		if (!(bool)(Object)blurMaterial)
			return;
		DestroyImmediate(blurMaterial);
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (!CheckResources())
			Graphics.Blit(source, destination);
		else {
			var num1 = (float)(1.0 / (1.0 * (1 << downsample)));
			blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num1, -blurSize * num1, 0.0f, 0.0f));
			source.filterMode = FilterMode.Bilinear;
			var width = source.width >> downsample;
			var height = source.height >> downsample;
			var renderTexture1 = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture1.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture1, blurMaterial, 0);
			var num2 = blurType == BlurType.StandardGauss ? 0 : 2;
			for (var index = 0; index < blurIterations; ++index) {
				var num3 = index * 1f;
				blurMaterial.SetVector("_Parameter",
					new Vector4(blurSize * num1 + num3, -blurSize * num1 - num3, 0.0f, 0.0f));
				var temporary1 = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary1.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture1, temporary1, blurMaterial, 1 + num2);
				RenderTexture.ReleaseTemporary(renderTexture1);
				var renderTexture2 = temporary1;
				var temporary2 = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary2.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture2, temporary2, blurMaterial, 2 + num2);
				RenderTexture.ReleaseTemporary(renderTexture2);
				renderTexture1 = temporary2;
			}

			Graphics.Blit(renderTexture1, destination);
			RenderTexture.ReleaseTemporary(renderTexture1);
		}
	}

	public enum BlurType {
		StandardGauss,
		SgxGauss
	}
}