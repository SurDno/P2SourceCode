namespace UnityEngine.PostProcessing;

public sealed class DitheringComponent : PostProcessingComponentRenderTexture<DitheringModel> {
	private Texture2D[] noiseTextures;
	private int textureIndex;
	private const int k_TextureCount = 64;

	public override bool active => model.enabled && !context.interrupted;

	public override void OnDisable() {
		noiseTextures = null;
	}

	private void LoadNoiseTextures() {
		noiseTextures = new Texture2D[64];
		for (var index = 0; index < 64; ++index)
			noiseTextures[index] = Resources.Load<Texture2D>("Bluenoise64/LDR_LLL1_" + index);
	}

	public override void Prepare(Material uberMaterial) {
		if (++textureIndex >= 64)
			textureIndex = 0;
		var z = Random.value;
		var w = Random.value;
		if (noiseTextures == null)
			LoadNoiseTextures();
		var noiseTexture = noiseTextures[textureIndex];
		uberMaterial.EnableKeyword("DITHERING");
		uberMaterial.SetTexture(Uniforms._DitheringTex, noiseTexture);
		uberMaterial.SetVector(Uniforms._DitheringCoords,
			new Vector4(context.width / (float)noiseTexture.width, context.height / (float)noiseTexture.height, z, w));
	}

	private static class Uniforms {
		internal static readonly int _DitheringTex = Shader.PropertyToID(nameof(_DitheringTex));
		internal static readonly int _DitheringCoords = Shader.PropertyToID(nameof(_DitheringCoords));
	}
}