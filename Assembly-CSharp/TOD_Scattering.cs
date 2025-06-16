using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Time of Day/Camera Atmospheric Scattering")]
public class TOD_Scattering : TOD_ImageEffect {
	public Shader ScatteringShader;
	public Texture2D DitheringTexture;
	[Range(0.0f, 1f)] public float GlobalDensity = 1f / 1000f;
	[Range(0.0f, 1000f)] public float StartDistance;
	[Range(0.0f, 1f)] public float HeightFalloff = 1f / 1000f;
	public float ZeroLevel;
	public float TransparentDensityMultiplier = 5f;
	private Material scatteringMaterial;

	protected void OnEnable() {
		if (!(bool)(Object)ScatteringShader)
			ScatteringShader = Shader.Find("Hidden/Time of Day/Scattering");
		scatteringMaterial = CreateMaterial(ScatteringShader);
	}

	protected void OnDisable() {
		if (!(bool)(Object)scatteringMaterial)
			return;
		DestroyImmediate(scatteringMaterial);
	}

	protected void OnPreCull() {
		if (!(bool)(Object)sky || !sky.Initialized)
			return;
		sky.Components.AtmosphereRenderer.enabled = false;
	}

	protected void OnPostRender() {
		if (!(bool)(Object)sky || !sky.Initialized)
			return;
		sky.Components.AtmosphereRenderer.enabled = true;
	}

	[ImageEffectOpaque]
	protected void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (!CheckSupport(true))
			Graphics.Blit(source, destination);
		else {
			sky.Components.Scattering = this;
			var heightFalloff = HeightFalloff;
			var y = Mathf.Exp((float)(-(double)heightFalloff * (cam.transform.position.y - (double)ZeroLevel)));
			var globalDensity = GlobalDensity;
			RenderSettings.fog = true;
			RenderSettings.fogMode = FogMode.Exponential;
			RenderSettings.fogDensity = globalDensity * TransparentDensityMultiplier;
			scatteringMaterial.SetMatrix("_FrustumCornersWS", FrustumCorners());
			scatteringMaterial.SetTexture("_DitheringTexture", DitheringTexture);
			scatteringMaterial.SetVector("_Density", new Vector4(heightFalloff, y, globalDensity, StartDistance));
			CustomBlit(source, destination, scatteringMaterial);
		}
	}
}