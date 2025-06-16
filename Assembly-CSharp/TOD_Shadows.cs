using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Time of Day/Camera Cloud Shadows")]
public class TOD_Shadows : TOD_ImageEffect {
	public Shader ShadowShader;
	public Texture2D CloudTexture;
	[Range(0.0f, 1f)] public float Cutoff;
	[Range(0.0f, 1f)] public float Fade;
	[Range(0.0f, 1f)] public float Intensity = 0.5f;
	private Material shadowMaterial;

	protected void OnEnable() {
		if (!(bool)(Object)ShadowShader)
			ShadowShader = Shader.Find("Hidden/Time of Day/Cloud Shadows");
		shadowMaterial = CreateMaterial(ShadowShader);
	}

	protected void OnDisable() {
		if (!(bool)(Object)shadowMaterial)
			return;
		DestroyImmediate(shadowMaterial);
	}

	[ImageEffectOpaque]
	protected void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (!CheckSupport(true))
			Graphics.Blit(source, destination);
		else {
			sky.Components.Shadows = this;
			shadowMaterial.SetMatrix("_FrustumCornersWS", FrustumCorners());
			shadowMaterial.SetTexture("_CloudTex", CloudTexture);
			shadowMaterial.SetFloat("_Cutoff", Cutoff);
			shadowMaterial.SetFloat("_Fade", Fade);
			shadowMaterial.SetFloat("_Intensity", Intensity * Mathf.Clamp01((float)(1.0 - sky.SunZenith / 90.0)));
			CustomBlit(source, destination, shadowMaterial);
		}
	}
}