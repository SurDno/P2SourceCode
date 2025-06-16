using UnityEngine;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Noise/Noise And Grain (Filmic)")]
public class NoiseAndGrain : PostEffectsBase {
	public float intensityMultiplier = 0.25f;
	public float generalIntensity = 0.5f;
	public float blackIntensity = 1f;
	public float whiteIntensity = 1f;
	public float midGrey = 0.2f;
	public bool dx11Grain;
	public float softness;
	public bool monochrome;
	public Vector3 intensities = new(1f, 1f, 1f);
	public Vector3 tiling = new(64f, 64f, 64f);
	public float monochromeTiling = 64f;
	public FilterMode filterMode = FilterMode.Bilinear;
	public Texture2D noiseTexture;
	public Shader noiseShader;
	private Material noiseMaterial;
	public Shader dx11NoiseShader;
	private Material dx11NoiseMaterial;
	private static float TILE_AMOUNT = 64f;
	private Mesh mesh;

	private void Awake() {
		mesh = new Mesh();
	}

	public override bool CheckResources() {
		CheckSupport(false);
		noiseMaterial = CheckShaderAndCreateMaterial(noiseShader, noiseMaterial);
		if (dx11Grain && supportDX11)
			dx11NoiseMaterial = CheckShaderAndCreateMaterial(dx11NoiseShader, dx11NoiseMaterial);
		if (!isSupported)
			ReportAutoDisable();
		return isSupported;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (!CheckResources() || null == noiseTexture) {
			Graphics.Blit(source, destination);
			if (!(null == noiseTexture))
				return;
			Debug.LogWarning("Noise & Grain effect failing as noise texture is not assigned. please assign.",
				transform);
		} else {
			softness = Mathf.Clamp(softness, 0.0f, 0.99f);
			if (dx11Grain && supportDX11) {
				dx11NoiseMaterial.SetFloat("_DX11NoiseTime", Time.frameCount);
				dx11NoiseMaterial.SetTexture("_NoiseTex", noiseTexture);
				dx11NoiseMaterial.SetVector("_NoisePerChannel", monochrome ? Vector3.one : intensities);
				dx11NoiseMaterial.SetVector("_MidGrey",
					new Vector3(midGrey, (float)(1.0 / (1.0 - midGrey)), -1f / midGrey));
				dx11NoiseMaterial.SetVector("_NoiseAmount",
					new Vector3(generalIntensity, blackIntensity, whiteIntensity) * intensityMultiplier);
				if (softness > (double)Mathf.Epsilon) {
					var temporary = RenderTexture.GetTemporary((int)(source.width * (1.0 - softness)),
						(int)(source.height * (1.0 - softness)));
					DrawNoiseQuadGrid(source, temporary, dx11NoiseMaterial, noiseTexture, mesh, monochrome ? 3 : 2);
					dx11NoiseMaterial.SetTexture("_NoiseTex", temporary);
					Graphics.Blit(source, destination, dx11NoiseMaterial, 4);
					RenderTexture.ReleaseTemporary(temporary);
				} else
					DrawNoiseQuadGrid(source, destination, dx11NoiseMaterial, noiseTexture, mesh, monochrome ? 1 : 0);
			} else {
				if ((bool)(Object)noiseTexture) {
					noiseTexture.wrapMode = TextureWrapMode.Repeat;
					noiseTexture.filterMode = filterMode;
				}

				noiseMaterial.SetTexture("_NoiseTex", noiseTexture);
				noiseMaterial.SetVector("_NoisePerChannel", monochrome ? Vector3.one : intensities);
				noiseMaterial.SetVector("_NoiseTilingPerChannel", monochrome ? Vector3.one * monochromeTiling : tiling);
				noiseMaterial.SetVector("_MidGrey",
					new Vector3(midGrey, (float)(1.0 / (1.0 - midGrey)), -1f / midGrey));
				noiseMaterial.SetVector("_NoiseAmount",
					new Vector3(generalIntensity, blackIntensity, whiteIntensity) * intensityMultiplier);
				if (softness > (double)Mathf.Epsilon) {
					var temporary = RenderTexture.GetTemporary((int)(source.width * (1.0 - softness)),
						(int)(source.height * (1.0 - softness)));
					DrawNoiseQuadGrid(source, temporary, noiseMaterial, noiseTexture, mesh, 2);
					noiseMaterial.SetTexture("_NoiseTex", temporary);
					Graphics.Blit(source, destination, noiseMaterial, 1);
					RenderTexture.ReleaseTemporary(temporary);
				} else
					DrawNoiseQuadGrid(source, destination, noiseMaterial, noiseTexture, mesh, 0);
			}
		}
	}

	private static void DrawNoiseQuadGrid(
		RenderTexture source,
		RenderTexture dest,
		Material fxMaterial,
		Texture2D noise,
		Mesh mesh,
		int passNr) {
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", source);
		GL.PushMatrix();
		GL.LoadOrtho();
		fxMaterial.SetPass(passNr);
		BuildMesh(mesh, source, noise);
		var transform = Camera.main.transform;
		var position = transform.position;
		var rotation = transform.rotation;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
		transform.position = position;
		transform.rotation = rotation;
		GL.PopMatrix();
	}

	private static void BuildMesh(Mesh mesh, RenderTexture source, Texture2D noise) {
		var noiseSize = noise.width * 1f;
		var f = 1f * source.width / TILE_AMOUNT;
		var num1 = (float)(1.0 * source.width / (1.0 * source.height));
		var num2 = 1f / f;
		var num3 = num2 * num1;
		var width = (int)Mathf.Ceil(f);
		var height = (int)Mathf.Ceil(1f / num3);
		if (mesh.vertices.Length != width * height * 4) {
			var vector3Array = new Vector3[width * height * 4];
			var vector2Array = new Vector2[width * height * 4];
			var numArray = new int[width * height * 6];
			var index1 = 0;
			var index2 = 0;
			for (var x = 0.0f; x < 1.0; x += num2) {
				for (var y = 0.0f; y < 1.0; y += num3) {
					vector3Array[index1] = new Vector3(x, y, 0.1f);
					vector3Array[index1 + 1] = new Vector3(x + num2, y, 0.1f);
					vector3Array[index1 + 2] = new Vector3(x + num2, y + num3, 0.1f);
					vector3Array[index1 + 3] = new Vector3(x, y + num3, 0.1f);
					vector2Array[index1] = new Vector2(0.0f, 0.0f);
					vector2Array[index1 + 1] = new Vector2(1f, 0.0f);
					vector2Array[index1 + 2] = new Vector2(1f, 1f);
					vector2Array[index1 + 3] = new Vector2(0.0f, 1f);
					numArray[index2] = index1;
					numArray[index2 + 1] = index1 + 1;
					numArray[index2 + 2] = index1 + 2;
					numArray[index2 + 3] = index1;
					numArray[index2 + 4] = index1 + 2;
					numArray[index2 + 5] = index1 + 3;
					index1 += 4;
					index2 += 6;
				}
			}

			mesh.vertices = vector3Array;
			mesh.uv2 = vector2Array;
			mesh.triangles = numArray;
		}

		BuildMeshUV0(mesh, width, height, noiseSize, noise.width);
	}

	private static void BuildMeshUV0(
		Mesh mesh,
		int width,
		int height,
		float noiseSize,
		int noiseWidth) {
		var num1 = noiseSize / (noiseWidth * 1f);
		var num2 = 1f / noiseSize;
		var vector2Array = new Vector2[width * height * 4];
		var index1 = 0;
		for (var index2 = 0; index2 < width * height; ++index2) {
			var f1 = Random.Range(0.0f, noiseSize);
			var f2 = Random.Range(0.0f, noiseSize);
			var x = Mathf.Floor(f1) * num2;
			var y = Mathf.Floor(f2) * num2;
			vector2Array[index1] = new Vector2(x, y);
			vector2Array[index1 + 1] = new Vector2(x + num1 * num2, y);
			vector2Array[index1 + 2] = new Vector2(x + num1 * num2, y + num1 * num2);
			vector2Array[index1 + 3] = new Vector2(x, y + num1 * num2);
			index1 += 4;
		}

		mesh.uv = vector2Array;
	}
}