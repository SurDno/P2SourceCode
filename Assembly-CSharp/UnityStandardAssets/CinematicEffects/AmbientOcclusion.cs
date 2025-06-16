using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityStandardAssets.CinematicEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Cinematic/Ambient Occlusion")]
[ImageEffectAllowedInSceneView]
public class AmbientOcclusion : MonoBehaviour {
	[SerializeField] public Settings settings = Settings.defaultSettings;
	[SerializeField] private Shader _aoShader;
	private Material _aoMaterial;
	private CommandBuffer _aoCommands;
	[SerializeField] private Mesh _quadMesh;

	public bool isAmbientOnlySupported => targetCamera.allowHDR && occlusionSource == OcclusionSource.GBuffer;

	public bool isGBufferAvailable => targetCamera.actualRenderingPath == RenderingPath.DeferredShading;

	private float intensity => settings.intensity;

	private float radius => Mathf.Max(settings.radius, 0.0001f);

	private SampleCount sampleCount => settings.sampleCount;

	private int sampleCountValue {
		get {
			switch (settings.sampleCount) {
				case SampleCount.Lowest:
					return 3;
				case SampleCount.Low:
					return 6;
				case SampleCount.Medium:
					return 12;
				case SampleCount.High:
					return 20;
				default:
					return Mathf.Clamp(settings.sampleCountValue, 1, 256);
			}
		}
	}

	private OcclusionSource occlusionSource =>
		settings.occlusionSource == OcclusionSource.GBuffer && !isGBufferAvailable
			? OcclusionSource.DepthNormalsTexture
			: settings.occlusionSource;

	private bool downsampling => settings.downsampling;

	private bool ambientOnly => settings.ambientOnly && !settings.debug && isAmbientOnlySupported;

	private RenderTextureFormat aoTextureFormat => SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8)
		? RenderTextureFormat.R8
		: RenderTextureFormat.Default;

	private Shader aoShader {
		get {
			if (_aoShader == null)
				_aoShader = Shader.Find("Hidden/Image Effects/Cinematic/AmbientOcclusion");
			return _aoShader;
		}
	}

	private Material aoMaterial {
		get {
			if (_aoMaterial == null)
				_aoMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(aoShader);
			return _aoMaterial;
		}
	}

	private CommandBuffer aoCommands {
		get {
			if (_aoCommands == null) {
				_aoCommands = new CommandBuffer();
				_aoCommands.name = nameof(AmbientOcclusion);
			}

			return _aoCommands;
		}
	}

	private Camera targetCamera => GetComponent<Camera>();

	private PropertyObserver propertyObserver { get; set; }

	private Mesh quadMesh => _quadMesh;

	private void BuildAOCommands() {
		var aoCommands = this.aoCommands;
		var pixelWidth = targetCamera.pixelWidth;
		var pixelHeight = targetCamera.pixelHeight;
		var num = downsampling ? 2 : 1;
		var aoTextureFormat = this.aoTextureFormat;
		var readWrite = RenderTextureReadWrite.Linear;
		var filter = FilterMode.Bilinear;
		var aoMaterial = this.aoMaterial;
		var id1 = Shader.PropertyToID("_OcclusionTexture");
		aoCommands.GetTemporaryRT(id1, pixelWidth / num, pixelHeight / num, 0, filter, aoTextureFormat, readWrite);
		aoCommands.Blit(null, id1, aoMaterial, 2);
		var id2 = Shader.PropertyToID("_OcclusionBlurTexture");
		aoCommands.GetTemporaryRT(id2, pixelWidth, pixelHeight, 0, filter, aoTextureFormat, readWrite);
		aoCommands.SetGlobalVector("_BlurVector", Vector2.right * 2f);
		aoCommands.Blit(id1, id2, aoMaterial, 4);
		aoCommands.ReleaseTemporaryRT(id1);
		aoCommands.GetTemporaryRT(id1, pixelWidth, pixelHeight, 0, filter, aoTextureFormat, readWrite);
		aoCommands.SetGlobalVector("_BlurVector", Vector2.up * 2f * num);
		aoCommands.Blit(id2, id1, aoMaterial, 4);
		aoCommands.ReleaseTemporaryRT(id2);
		aoCommands.GetTemporaryRT(id2, pixelWidth, pixelHeight, 0, filter, aoTextureFormat, readWrite);
		aoCommands.SetGlobalVector("_BlurVector", Vector2.right * num);
		aoCommands.Blit(id1, id2, aoMaterial, 6);
		aoCommands.ReleaseTemporaryRT(id1);
		aoCommands.GetTemporaryRT(id1, pixelWidth, pixelHeight, 0, filter, aoTextureFormat, readWrite);
		aoCommands.SetGlobalVector("_BlurVector", Vector2.up * num);
		aoCommands.Blit(id2, id1, aoMaterial, 6);
		aoCommands.ReleaseTemporaryRT(id2);
		var colors = new RenderTargetIdentifier[2] {
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.CameraTarget
		};
		aoCommands.SetRenderTarget(colors, BuiltinRenderTextureType.CameraTarget);
		aoCommands.SetGlobalTexture("_OcclusionTexture", id1);
		aoCommands.DrawMesh(quadMesh, Matrix4x4.identity, aoMaterial, 0, 8);
		aoCommands.ReleaseTemporaryRT(id1);
	}

	private void ExecuteAOPass(RenderTexture source, RenderTexture destination) {
		var width = source.width;
		var height = source.height;
		var num = downsampling ? 2 : 1;
		var aoTextureFormat = this.aoTextureFormat;
		var readWrite = RenderTextureReadWrite.Linear;
		var flag = settings.occlusionSource == OcclusionSource.GBuffer;
		var aoMaterial = this.aoMaterial;
		var temporary1 = RenderTexture.GetTemporary(width / num, height / num, 0, aoTextureFormat, readWrite);
		Graphics.Blit(null, temporary1, aoMaterial, (int)occlusionSource);
		var temporary2 = RenderTexture.GetTemporary(width, height, 0, aoTextureFormat, readWrite);
		aoMaterial.SetVector("_BlurVector", Vector2.right * 2f);
		Graphics.Blit(temporary1, temporary2, aoMaterial, flag ? 4 : 3);
		RenderTexture.ReleaseTemporary(temporary1);
		var temporary3 = RenderTexture.GetTemporary(width, height, 0, aoTextureFormat, readWrite);
		aoMaterial.SetVector("_BlurVector", Vector2.up * 2f * num);
		Graphics.Blit(temporary2, temporary3, aoMaterial, flag ? 4 : 3);
		RenderTexture.ReleaseTemporary(temporary2);
		var temporary4 = RenderTexture.GetTemporary(width, height, 0, aoTextureFormat, readWrite);
		aoMaterial.SetVector("_BlurVector", Vector2.right * num);
		Graphics.Blit(temporary3, temporary4, aoMaterial, flag ? 6 : 5);
		RenderTexture.ReleaseTemporary(temporary3);
		var temporary5 = RenderTexture.GetTemporary(width, height, 0, aoTextureFormat, readWrite);
		aoMaterial.SetVector("_BlurVector", Vector2.up * num);
		Graphics.Blit(temporary4, temporary5, aoMaterial, flag ? 6 : 5);
		RenderTexture.ReleaseTemporary(temporary4);
		aoMaterial.SetTexture("_OcclusionTexture", temporary5);
		if (!settings.debug)
			Graphics.Blit(source, destination, aoMaterial, 7);
		else
			Graphics.Blit(source, destination, aoMaterial, 9);
		RenderTexture.ReleaseTemporary(temporary5);
	}

	private void UpdateMaterialProperties() {
		var aoMaterial = this.aoMaterial;
		aoMaterial.SetFloat("_Intensity", intensity);
		aoMaterial.SetFloat("_Radius", radius);
		aoMaterial.SetFloat("_TargetScale", downsampling ? 0.5f : 1f);
		aoMaterial.SetInt("_SampleCount", sampleCountValue);
	}

	private void OnEnable() {
		if (!ImageEffectHelper.IsSupported(aoShader, true, false, this))
			enabled = false;
		else {
			if (ambientOnly)
				targetCamera.AddCommandBuffer(CameraEvent.BeforeReflections, aoCommands);
			if (occlusionSource == OcclusionSource.DepthTexture)
				targetCamera.depthTextureMode |= DepthTextureMode.Depth;
			if (occlusionSource == OcclusionSource.GBuffer)
				return;
			targetCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		}
	}

	private void OnDisable() {
		if (_aoMaterial != null)
			DestroyImmediate(_aoMaterial);
		_aoMaterial = null;
		if (_aoCommands != null)
			targetCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, _aoCommands);
		_aoCommands = null;
	}

	private void Update() {
		if (propertyObserver.CheckNeedsReset(settings, targetCamera)) {
			OnDisable();
			OnEnable();
			if (ambientOnly) {
				aoCommands.Clear();
				BuildAOCommands();
			}

			propertyObserver.Update(settings, targetCamera);
		}

		if (!ambientOnly)
			return;
		UpdateMaterialProperties();
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (ambientOnly)
			Graphics.Blit(source, destination);
		else {
			UpdateMaterialProperties();
			ExecuteAOPass(source, destination);
		}
	}

	private struct PropertyObserver {
		private bool _downsampling;
		private OcclusionSource _occlusionSource;
		private bool _ambientOnly;
		private bool _debug;
		private int _pixelWidth;
		private int _pixelHeight;

		public bool CheckNeedsReset(Settings setting, Camera camera) {
			return _downsampling != setting.downsampling || _occlusionSource != setting.occlusionSource ||
			       _ambientOnly != setting.ambientOnly || _debug != setting.debug || _pixelWidth != camera.pixelWidth ||
			       _pixelHeight != camera.pixelHeight;
		}

		public void Update(Settings setting, Camera camera) {
			_downsampling = setting.downsampling;
			_occlusionSource = setting.occlusionSource;
			_ambientOnly = setting.ambientOnly;
			_debug = setting.debug;
			_pixelWidth = camera.pixelWidth;
			_pixelHeight = camera.pixelHeight;
		}
	}

	public enum SampleCount {
		Lowest,
		Low,
		Medium,
		High,
		Variable
	}

	public enum OcclusionSource {
		DepthTexture,
		DepthNormalsTexture,
		GBuffer
	}

	[Serializable]
	public class Settings {
		[SerializeField] [Range(0.0f, 4f)] [Tooltip("Degree of darkness produced by the effect.")]
		public float intensity;

		[SerializeField] [Tooltip("Radius of sample points, which affects extent of darkened areas.")]
		public float radius;

		[SerializeField] [Tooltip("Number of sample points, which affects quality and performance.")]
		public SampleCount sampleCount;

		[SerializeField] [Tooltip("Determines the sample count when SampleCount.Variable is used.")]
		public int sampleCountValue;

		[SerializeField] [Tooltip("Halves the resolution of the effect to increase performance.")]
		public bool downsampling;

		[SerializeField] [Tooltip("If checked, the effect only affects ambient lighting.")]
		public bool ambientOnly;

		[SerializeField] [Tooltip("Source buffer on which the occlusion estimator is based.")]
		public OcclusionSource occlusionSource;

		[SerializeField] [Tooltip("Displays occlusion for debug purpose.")]
		public bool debug;

		public static Settings defaultSettings =>
			new() {
				intensity = 1f,
				radius = 0.3f,
				sampleCount = SampleCount.Medium,
				sampleCountValue = 24,
				downsampling = false,
				ambientOnly = false,
				occlusionSource = OcclusionSource.DepthNormalsTexture
			};
	}
}