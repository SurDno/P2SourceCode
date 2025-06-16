using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Rendering/Screen Space Ambient Occlusion")]
public class ScreenSpaceAmbientOcclusion : MonoBehaviour {
	[Range(0.05f, 1f)] public float m_Radius = 0.4f;
	public SSAOSamples m_SampleCount = SSAOSamples.Medium;
	[Range(0.5f, 4f)] public float m_OcclusionIntensity = 1.5f;
	[Range(0.0f, 4f)] public int m_Blur = 2;
	[Range(1f, 6f)] public int m_Downsampling = 2;
	[Range(0.2f, 2f)] public float m_OcclusionAttenuation = 1f;
	[Range(1E-05f, 0.5f)] public float m_MinZ = 0.01f;
	public Shader m_SSAOShader;
	private Material m_SSAOMaterial;
	public Texture2D m_RandomTexture;
	private bool m_Supported;

	private static Material CreateMaterial(Shader shader) {
		if (!(bool)(Object)shader)
			return null;
		var material = new Material(shader);
		material.hideFlags = HideFlags.HideAndDontSave;
		return material;
	}

	private static void DestroyMaterial(Material mat) {
		if (!(bool)(Object)mat)
			return;
		DestroyImmediate(mat);
		mat = null;
	}

	private void OnDisable() {
		DestroyMaterial(m_SSAOMaterial);
	}

	private void Start() {
		if (!SystemInfo.supportsImageEffects || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)) {
			m_Supported = false;
			enabled = false;
		} else {
			CreateMaterials();
			if (!(bool)(Object)m_SSAOMaterial || m_SSAOMaterial.passCount != 5) {
				m_Supported = false;
				enabled = false;
			} else
				m_Supported = true;
		}
	}

	private void OnEnable() {
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	private void CreateMaterials() {
		if ((bool)(Object)m_SSAOMaterial || !m_SSAOShader.isSupported)
			return;
		m_SSAOMaterial = CreateMaterial(m_SSAOShader);
		m_SSAOMaterial.SetTexture("_RandomTexture", m_RandomTexture);
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (!m_Supported || !m_SSAOShader.isSupported)
			enabled = false;
		else {
			CreateMaterials();
			m_Downsampling = Mathf.Clamp(m_Downsampling, 1, 6);
			m_Radius = Mathf.Clamp(m_Radius, 0.05f, 1f);
			m_MinZ = Mathf.Clamp(m_MinZ, 1E-05f, 0.5f);
			m_OcclusionIntensity = Mathf.Clamp(m_OcclusionIntensity, 0.5f, 4f);
			m_OcclusionAttenuation = Mathf.Clamp(m_OcclusionAttenuation, 0.2f, 2f);
			m_Blur = Mathf.Clamp(m_Blur, 0, 4);
			var renderTexture =
				RenderTexture.GetTemporary(source.width / m_Downsampling, source.height / m_Downsampling, 0);
			var fieldOfView = GetComponent<Camera>().fieldOfView;
			var farClipPlane = GetComponent<Camera>().farClipPlane;
			var y = Mathf.Tan((float)(fieldOfView * (Math.PI / 180.0) * 0.5)) * farClipPlane;
			m_SSAOMaterial.SetVector("_FarCorner", new Vector3(y * GetComponent<Camera>().aspect, y, farClipPlane));
			int num1;
			int num2;
			if ((bool)(Object)m_RandomTexture) {
				num1 = m_RandomTexture.width;
				num2 = m_RandomTexture.height;
			} else {
				num1 = 1;
				num2 = 1;
			}

			m_SSAOMaterial.SetVector("_NoiseScale",
				new Vector3(renderTexture.width / (float)num1, renderTexture.height / (float)num2, 0.0f));
			m_SSAOMaterial.SetVector("_Params",
				new Vector4(m_Radius, m_MinZ, 1f / m_OcclusionAttenuation, m_OcclusionIntensity));
			var flag = m_Blur > 0;
			Graphics.Blit(flag ? null : (Texture)source, renderTexture, m_SSAOMaterial, (int)m_SampleCount);
			if (flag) {
				var temporary1 = RenderTexture.GetTemporary(source.width, source.height, 0);
				m_SSAOMaterial.SetVector("_TexelOffsetScale",
					new Vector4(m_Blur / (float)source.width, 0.0f, 0.0f, 0.0f));
				m_SSAOMaterial.SetTexture("_SSAO", renderTexture);
				Graphics.Blit(null, temporary1, m_SSAOMaterial, 3);
				RenderTexture.ReleaseTemporary(renderTexture);
				var temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0);
				m_SSAOMaterial.SetVector("_TexelOffsetScale",
					new Vector4(0.0f, m_Blur / (float)source.height, 0.0f, 0.0f));
				m_SSAOMaterial.SetTexture("_SSAO", temporary1);
				Graphics.Blit(source, temporary2, m_SSAOMaterial, 3);
				RenderTexture.ReleaseTemporary(temporary1);
				renderTexture = temporary2;
			}

			m_SSAOMaterial.SetTexture("_SSAO", renderTexture);
			Graphics.Blit(source, destination, m_SSAOMaterial, 4);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	public enum SSAOSamples {
		Low,
		Medium,
		High
	}
}