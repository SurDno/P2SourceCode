using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Camera/Tilt Shift (Lens Blur)")]
internal class TiltShift : PostEffectsBase {
	public TiltShiftMode mode = TiltShiftMode.TiltShiftMode;
	public TiltShiftQuality quality = TiltShiftQuality.Normal;
	[Range(0.0f, 15f)] public float blurArea = 1f;
	[Range(0.0f, 25f)] public float maxBlurSize = 5f;
	[Range(0.0f, 1f)] public int downsample;
	public Shader tiltShiftShader;
	private Material tiltShiftMaterial;

	public override bool CheckResources() {
		CheckSupport(true);
		tiltShiftMaterial = CheckShaderAndCreateMaterial(tiltShiftShader, tiltShiftMaterial);
		if (!isSupported)
			ReportAutoDisable();
		return isSupported;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (!CheckResources())
			Graphics.Blit(source, destination);
		else {
			tiltShiftMaterial.SetFloat("_BlurSize", maxBlurSize < 0.0 ? 0.0f : maxBlurSize);
			tiltShiftMaterial.SetFloat("_BlurArea", blurArea);
			source.filterMode = FilterMode.Bilinear;
			var renderTexture = destination;
			if (downsample > 0.0) {
				renderTexture = RenderTexture.GetTemporary(source.width >> downsample, source.height >> downsample, 0,
					source.format);
				renderTexture.filterMode = FilterMode.Bilinear;
			}

			var num = (int)quality * 2;
			Graphics.Blit(source, renderTexture, tiltShiftMaterial,
				mode == TiltShiftMode.TiltShiftMode ? num : num + 1);
			if (downsample > 0) {
				tiltShiftMaterial.SetTexture("_Blurred", renderTexture);
				Graphics.Blit(source, destination, tiltShiftMaterial, 6);
			}

			if (!(renderTexture != destination))
				return;
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	public enum TiltShiftMode {
		TiltShiftMode,
		IrisMode
	}

	public enum TiltShiftQuality {
		Preview,
		Normal,
		High
	}
}