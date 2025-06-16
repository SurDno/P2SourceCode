using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityStandardAssets.CinematicEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Cinematic/Depth Of Field")]
[RequireComponent(typeof(Camera))]
public class DepthOfField : MonoBehaviour {
	private const float kMaxBlur = 40f;
	public GlobalSettings settings = GlobalSettings.defaultSettings;
	public FocusSettings focus = FocusSettings.defaultSettings;
	public BokehTextureSettings bokehTexture = BokehTextureSettings.defaultSettings;
	[SerializeField] private Shader m_FilmicDepthOfFieldShader;
	[SerializeField] private Shader m_MedianFilterShader;
	[SerializeField] private Shader m_TextureBokehShader;
	private RenderTextureUtility m_RTU = new();
	private Material m_FilmicDepthOfFieldMaterial;
	private Material m_MedianFilterMaterial;
	private Material m_TextureBokehMaterial;
	private ComputeBuffer m_ComputeBufferDrawArgs;
	private ComputeBuffer m_ComputeBufferPoints;
	private QualitySettings m_CurrentQualitySettings;
	private float m_LastApertureOrientation;
	private Vector4 m_OctogonalBokehDirection1;
	private Vector4 m_OctogonalBokehDirection2;
	private Vector4 m_OctogonalBokehDirection3;
	private Vector4 m_OctogonalBokehDirection4;
	private Vector4 m_HexagonalBokehDirection1;
	private Vector4 m_HexagonalBokehDirection2;
	private Vector4 m_HexagonalBokehDirection3;

	public Shader filmicDepthOfFieldShader {
		get {
			if (m_FilmicDepthOfFieldShader == null)
				m_FilmicDepthOfFieldShader = Shader.Find("Hidden/DepthOfField/DepthOfField");
			return m_FilmicDepthOfFieldShader;
		}
	}

	public Shader medianFilterShader {
		get {
			if (m_MedianFilterShader == null)
				m_MedianFilterShader = Shader.Find("Hidden/DepthOfField/MedianFilter");
			return m_MedianFilterShader;
		}
	}

	public Shader textureBokehShader {
		get {
			if (m_TextureBokehShader == null)
				m_TextureBokehShader = Shader.Find("Hidden/DepthOfField/BokehSplatting");
			return m_TextureBokehShader;
		}
	}

	public Material filmicDepthOfFieldMaterial {
		get {
			if (m_FilmicDepthOfFieldMaterial == null)
				m_FilmicDepthOfFieldMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(filmicDepthOfFieldShader);
			return m_FilmicDepthOfFieldMaterial;
		}
	}

	public Material medianFilterMaterial {
		get {
			if (m_MedianFilterMaterial == null)
				m_MedianFilterMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(medianFilterShader);
			return m_MedianFilterMaterial;
		}
	}

	public Material textureBokehMaterial {
		get {
			if (m_TextureBokehMaterial == null)
				m_TextureBokehMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(textureBokehShader);
			return m_TextureBokehMaterial;
		}
	}

	public ComputeBuffer computeBufferDrawArgs {
		get {
			if (m_ComputeBufferDrawArgs == null) {
				m_ComputeBufferDrawArgs = new ComputeBuffer(1, 16, ComputeBufferType.DrawIndirect);
				m_ComputeBufferDrawArgs.SetData(new int[4] {
					0,
					1,
					0,
					0
				});
			}

			return m_ComputeBufferDrawArgs;
		}
	}

	public ComputeBuffer computeBufferPoints {
		get {
			if (m_ComputeBufferPoints == null)
				m_ComputeBufferPoints = new ComputeBuffer(90000, 28, ComputeBufferType.Append);
			return m_ComputeBufferPoints;
		}
	}

	private void OnEnable() {
		if (!ImageEffectHelper.IsSupported(filmicDepthOfFieldShader, true, true, this) ||
		    !ImageEffectHelper.IsSupported(medianFilterShader, true, true, this))
			enabled = false;
		else if (ImageEffectHelper.supportsDX11 && !ImageEffectHelper.IsSupported(textureBokehShader, true, true, this))
			enabled = false;
		else {
			ComputeBlurDirections(true);
			GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	private void OnDisable() {
		ReleaseComputeResources();
		if (m_FilmicDepthOfFieldMaterial != null)
			DestroyImmediate(m_FilmicDepthOfFieldMaterial);
		if (m_TextureBokehMaterial != null)
			DestroyImmediate(m_TextureBokehMaterial);
		if (m_MedianFilterMaterial != null)
			DestroyImmediate(m_MedianFilterMaterial);
		m_FilmicDepthOfFieldMaterial = null;
		m_TextureBokehMaterial = null;
		m_MedianFilterMaterial = null;
		m_RTU.ReleaseAllTemporaryRenderTextures();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (medianFilterMaterial == null || filmicDepthOfFieldMaterial == null)
			Graphics.Blit(source, destination);
		else {
			if (settings.visualizeFocus) {
				Vector4 blurParams;
				Vector4 blurCoe;
				ComputeCocParameters(out blurParams, out blurCoe);
				filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
				filmicDepthOfFieldMaterial.SetVector("_BlurCoe", blurCoe);
				Graphics.Blit(null, destination, filmicDepthOfFieldMaterial, 5);
			} else
				DoDepthOfField(source, destination);

			m_RTU.ReleaseAllTemporaryRenderTextures();
		}
	}

	private void DoDepthOfField(RenderTexture source, RenderTexture destination) {
		m_CurrentQualitySettings = QualitySettings.presetQualitySettings[(int)settings.filteringQuality];
		var num1 = source.height / 720f;
		var num2 = num1;
		var num3 = (float)(Mathf.Max(focus.nearBlurRadius, focus.farBlurRadius) * (double)num2 * 0.75);
		var a = focus.nearBlurRadius * num1;
		var b = focus.farBlurRadius * num1;
		var maxRadius = Mathf.Max(a, b);
		switch (settings.apertureShape) {
			case ApertureShape.Hexagonal:
				maxRadius *= 1.2f;
				break;
			case ApertureShape.Octogonal:
				maxRadius *= 1.15f;
				break;
		}

		if (maxRadius < 0.5)
			Graphics.Blit(source, destination);
		else {
			var width = source.width / 2;
			var height = source.height / 2;
			var vector4 = new Vector4(a * 0.5f, b * 0.5f, 0.0f, 0.0f);
			var temporaryRenderTexture1 = m_RTU.GetTemporaryRenderTexture(width, height);
			var temporaryRenderTexture2 = m_RTU.GetTemporaryRenderTexture(width, height);
			Vector4 blurParams;
			Vector4 blurCoe;
			ComputeCocParameters(out blurParams, out blurCoe);
			filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
			filmicDepthOfFieldMaterial.SetVector("_BlurCoe", blurCoe);
			Graphics.Blit(source, temporaryRenderTexture2, filmicDepthOfFieldMaterial, 4);
			var src = temporaryRenderTexture2;
			var dst = temporaryRenderTexture1;
			if (shouldPerformBokeh) {
				var temporaryRenderTexture3 = m_RTU.GetTemporaryRenderTexture(width, height);
				Graphics.Blit(src, temporaryRenderTexture3, filmicDepthOfFieldMaterial, 1);
				filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(0.0f, 1.5f, 0.0f, 1.5f));
				Graphics.Blit(temporaryRenderTexture3, dst, filmicDepthOfFieldMaterial, 0);
				filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(1.5f, 0.0f, 0.0f, 1.5f));
				Graphics.Blit(dst, temporaryRenderTexture3, filmicDepthOfFieldMaterial, 0);
				textureBokehMaterial.SetTexture("_BlurredColor", temporaryRenderTexture3);
				textureBokehMaterial.SetFloat("_SpawnHeuristic", bokehTexture.spawnHeuristic);
				textureBokehMaterial.SetVector("_BokehParams",
					new Vector4(bokehTexture.scale * num2, bokehTexture.intensity, bokehTexture.threshold, num3));
				Graphics.SetRandomWriteTarget(1, computeBufferPoints);
				Graphics.Blit(src, dst, textureBokehMaterial, 1);
				Graphics.ClearRandomWriteTargets();
				SwapRenderTexture(ref src, ref dst);
				m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture3);
			}

			filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
			filmicDepthOfFieldMaterial.SetVector("_BlurCoe", vector4);
			RenderTexture renderTexture = null;
			if (m_CurrentQualitySettings.dilateNearBlur) {
				var temporaryRenderTexture4 =
					m_RTU.GetTemporaryRenderTexture(width, height, format: RenderTextureFormat.RGHalf);
				renderTexture = m_RTU.GetTemporaryRenderTexture(width, height, format: RenderTextureFormat.RGHalf);
				filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(0.0f, a * 0.75f, 0.0f, 0.0f));
				Graphics.Blit(src, temporaryRenderTexture4, filmicDepthOfFieldMaterial, 2);
				filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(a * 0.75f, 0.0f, 0.0f, 0.0f));
				Graphics.Blit(temporaryRenderTexture4, renderTexture, filmicDepthOfFieldMaterial, 3);
				m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture4);
				renderTexture.filterMode = FilterMode.Point;
			}

			if (m_CurrentQualitySettings.prefilterBlur) {
				Graphics.Blit(src, dst, filmicDepthOfFieldMaterial, 6);
				SwapRenderTexture(ref src, ref dst);
			}

			switch (settings.apertureShape) {
				case ApertureShape.Circular:
					DoCircularBlur(renderTexture, ref src, ref dst, maxRadius);
					break;
				case ApertureShape.Hexagonal:
					DoHexagonalBlur(renderTexture, ref src, ref dst, maxRadius);
					break;
				case ApertureShape.Octogonal:
					DoOctogonalBlur(renderTexture, ref src, ref dst, maxRadius);
					break;
			}

			switch (m_CurrentQualitySettings.medianFilter) {
				case FilterQuality.Normal:
					medianFilterMaterial.SetVector("_Offsets", new Vector4(1f, 0.0f, 0.0f, 0.0f));
					Graphics.Blit(src, dst, medianFilterMaterial, 0);
					SwapRenderTexture(ref src, ref dst);
					medianFilterMaterial.SetVector("_Offsets", new Vector4(0.0f, 1f, 0.0f, 0.0f));
					Graphics.Blit(src, dst, medianFilterMaterial, 0);
					SwapRenderTexture(ref src, ref dst);
					break;
				case FilterQuality.High:
					Graphics.Blit(src, dst, medianFilterMaterial, 1);
					SwapRenderTexture(ref src, ref dst);
					break;
			}

			filmicDepthOfFieldMaterial.SetVector("_BlurCoe", vector4);
			filmicDepthOfFieldMaterial.SetVector("_Convolved_TexelSize",
				new Vector4(src.width, src.height, 1f / src.width, 1f / src.height));
			filmicDepthOfFieldMaterial.SetTexture("_SecondTex", src);
			var pass = 11;
			if (shouldPerformBokeh) {
				var temporaryRenderTexture5 =
					m_RTU.GetTemporaryRenderTexture(source.height, source.width, format: source.format);
				Graphics.Blit(source, temporaryRenderTexture5, filmicDepthOfFieldMaterial, pass);
				Graphics.SetRenderTarget(temporaryRenderTexture5);
				ComputeBuffer.CopyCount(computeBufferPoints, computeBufferDrawArgs, 0);
				textureBokehMaterial.SetBuffer("pointBuffer", computeBufferPoints);
				textureBokehMaterial.SetTexture("_MainTex", bokehTexture.texture);
				textureBokehMaterial.SetVector("_Screen",
					new Vector3((float)(1.0 / (1.0 * source.width)), (float)(1.0 / (1.0 * source.height)), num3));
				textureBokehMaterial.SetPass(0);
				Graphics.DrawProceduralIndirect(MeshTopology.Points, computeBufferDrawArgs, 0);
				Graphics.Blit(temporaryRenderTexture5, destination);
			} else
				Graphics.Blit(source, destination, filmicDepthOfFieldMaterial, pass);
		}
	}

	private void DoHexagonalBlur(
		RenderTexture blurredFgCoc,
		ref RenderTexture src,
		ref RenderTexture dst,
		float maxRadius) {
		ComputeBlurDirections(false);
		int blurPass;
		int blurAndMergePass;
		GetDirectionalBlurPassesFromRadius(blurredFgCoc, maxRadius, out blurPass, out blurAndMergePass);
		filmicDepthOfFieldMaterial.SetTexture("_SecondTex", blurredFgCoc);
		var temporaryRenderTexture = m_RTU.GetTemporaryRenderTexture(src.width, src.height, format: src.format);
		filmicDepthOfFieldMaterial.SetVector("_Offsets", m_HexagonalBokehDirection1);
		Graphics.Blit(src, temporaryRenderTexture, filmicDepthOfFieldMaterial, blurPass);
		filmicDepthOfFieldMaterial.SetVector("_Offsets", m_HexagonalBokehDirection2);
		Graphics.Blit(temporaryRenderTexture, src, filmicDepthOfFieldMaterial, blurPass);
		filmicDepthOfFieldMaterial.SetVector("_Offsets", m_HexagonalBokehDirection3);
		filmicDepthOfFieldMaterial.SetTexture("_ThirdTex", src);
		Graphics.Blit(temporaryRenderTexture, dst, filmicDepthOfFieldMaterial, blurAndMergePass);
		m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture);
		SwapRenderTexture(ref src, ref dst);
	}

	private void DoOctogonalBlur(
		RenderTexture blurredFgCoc,
		ref RenderTexture src,
		ref RenderTexture dst,
		float maxRadius) {
		ComputeBlurDirections(false);
		int blurPass;
		int blurAndMergePass;
		GetDirectionalBlurPassesFromRadius(blurredFgCoc, maxRadius, out blurPass, out blurAndMergePass);
		filmicDepthOfFieldMaterial.SetTexture("_SecondTex", blurredFgCoc);
		var temporaryRenderTexture = m_RTU.GetTemporaryRenderTexture(src.width, src.height, format: src.format);
		filmicDepthOfFieldMaterial.SetVector("_Offsets", m_OctogonalBokehDirection1);
		Graphics.Blit(src, temporaryRenderTexture, filmicDepthOfFieldMaterial, blurPass);
		filmicDepthOfFieldMaterial.SetVector("_Offsets", m_OctogonalBokehDirection2);
		Graphics.Blit(temporaryRenderTexture, dst, filmicDepthOfFieldMaterial, blurPass);
		filmicDepthOfFieldMaterial.SetVector("_Offsets", m_OctogonalBokehDirection3);
		Graphics.Blit(src, temporaryRenderTexture, filmicDepthOfFieldMaterial, blurPass);
		filmicDepthOfFieldMaterial.SetVector("_Offsets", m_OctogonalBokehDirection4);
		filmicDepthOfFieldMaterial.SetTexture("_ThirdTex", dst);
		Graphics.Blit(temporaryRenderTexture, src, filmicDepthOfFieldMaterial, blurAndMergePass);
		m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture);
	}

	private void DoCircularBlur(
		RenderTexture blurredFgCoc,
		ref RenderTexture src,
		ref RenderTexture dst,
		float maxRadius) {
		int pass;
		if (blurredFgCoc != null) {
			filmicDepthOfFieldMaterial.SetTexture("_SecondTex", blurredFgCoc);
			pass = maxRadius > 10.0 ? 8 : 10;
		} else
			pass = maxRadius > 10.0 ? 7 : 9;

		Graphics.Blit(src, dst, filmicDepthOfFieldMaterial, pass);
		SwapRenderTexture(ref src, ref dst);
	}

	private void ComputeCocParameters(out Vector4 blurParams, out Vector4 blurCoe) {
		var component = GetComponent<Camera>();
		var num1 = focus.nearFalloff * 2f;
		var num2 = focus.farFalloff * 2f;
		var num3 = focus.nearPlane;
		var num4 = focus.farPlane;
		if (settings.tweakMode == TweakMode.Range) {
			var num5 = !(focus.transform != null)
				? focus.focusPlane
				: component.WorldToViewportPoint(focus.transform.position).z;
			var num6 = focus.range * 0.5f;
			num3 = num5 - num6;
			num4 = num5 + num6;
		}

		var num7 = num3 - num1 * 0.5f;
		var num8 = num4 + num2 * 0.5f;
		var num9 = (float)((num7 + (double)num8) * 0.5) / component.farClipPlane;
		var num10 = num7 / component.farClipPlane;
		var num11 = num8 / component.farClipPlane;
		var num12 = num8 - num7;
		var num13 = num11 - num10;
		var num14 = num1 / num12;
		var num15 = num2 / num12;
		var num16 = (float)((1.0 - num14) * (num13 * 0.5));
		var num17 = (float)((1.0 - num15) * (num13 * 0.5));
		if (num9 <= (double)num10)
			num9 = num10 + 1E-06f;
		if (num9 >= (double)num11)
			num9 = num11 - 1E-06f;
		if (num9 - (double)num16 <= num10)
			num16 = (float)(num9 - (double)num10 - 9.9999999747524271E-07);
		if (num9 + (double)num17 >= num11)
			num17 = (float)(num11 - (double)num9 - 9.9999999747524271E-07);
		var num18 = (float)(1.0 / (num10 - (double)num9 + num16));
		var num19 = (float)(1.0 / (num11 - (double)num9 - num17));
		var num20 = (float)(1.0 - num18 * (double)num10);
		var num21 = (float)(1.0 - num19 * (double)num11);
		blurParams = new Vector4(-1f * num18, -1f * num20, 1f * num19, 1f * num21);
		blurCoe = new Vector4(0.0f, 0.0f, (float)((num21 - (double)num20) / (num18 - (double)num19)), 0.0f);
		focus.nearPlane = num7 + num1 * 0.5f;
		focus.farPlane = num8 - num2 * 0.5f;
		focus.focusPlane = (float)((focus.nearPlane + (double)focus.farPlane) * 0.5);
		focus.range = focus.farPlane - focus.nearPlane;
	}

	private void ReleaseComputeResources() {
		if (m_ComputeBufferDrawArgs != null)
			m_ComputeBufferDrawArgs.Release();
		if (m_ComputeBufferPoints != null)
			m_ComputeBufferPoints.Release();
		m_ComputeBufferDrawArgs = null;
		m_ComputeBufferPoints = null;
	}

	private void ComputeBlurDirections(bool force) {
		if (!force && Math.Abs(m_LastApertureOrientation - settings.apertureOrientation) < 1.4012984643248171E-45)
			return;
		m_LastApertureOrientation = settings.apertureOrientation;
		var f = settings.apertureOrientation * ((float)Math.PI / 180f);
		var cosinus = Mathf.Cos(f);
		var sinus = Mathf.Sin(f);
		m_OctogonalBokehDirection1 = new Vector4(0.5f, 0.0f, 0.0f, 0.0f);
		m_OctogonalBokehDirection2 = new Vector4(0.0f, 0.5f, 1f, 0.0f);
		m_OctogonalBokehDirection3 = new Vector4(-0.353553f, 0.353553f, 1f, 0.0f);
		m_OctogonalBokehDirection4 = new Vector4(0.353553f, 0.353553f, 1f, 0.0f);
		m_HexagonalBokehDirection1 = new Vector4(0.5f, 0.0f, 0.0f, 0.0f);
		m_HexagonalBokehDirection2 = new Vector4(0.25f, 0.433013f, 1f, 0.0f);
		m_HexagonalBokehDirection3 = new Vector4(0.25f, -0.433013f, 1f, 0.0f);
		if (f <= 1.4012984643248171E-45)
			return;
		Rotate2D(ref m_OctogonalBokehDirection1, cosinus, sinus);
		Rotate2D(ref m_OctogonalBokehDirection2, cosinus, sinus);
		Rotate2D(ref m_OctogonalBokehDirection3, cosinus, sinus);
		Rotate2D(ref m_OctogonalBokehDirection4, cosinus, sinus);
		Rotate2D(ref m_HexagonalBokehDirection1, cosinus, sinus);
		Rotate2D(ref m_HexagonalBokehDirection2, cosinus, sinus);
		Rotate2D(ref m_HexagonalBokehDirection3, cosinus, sinus);
	}

	private bool shouldPerformBokeh => ImageEffectHelper.supportsDX11 && bokehTexture.texture != null &&
	                                   (bool)(Object)textureBokehMaterial;

	private static void Rotate2D(ref Vector4 direction, float cosinus, float sinus) {
		var vector4 = direction;
		direction.x = (float)(vector4.x * (double)cosinus - vector4.y * (double)sinus);
		direction.y = (float)(vector4.x * (double)sinus + vector4.y * (double)cosinus);
	}

	private static void SwapRenderTexture(ref RenderTexture src, ref RenderTexture dst) {
		var renderTexture = dst;
		dst = src;
		src = renderTexture;
	}

	private static void GetDirectionalBlurPassesFromRadius(
		RenderTexture blurredFgCoc,
		float maxRadius,
		out int blurPass,
		out int blurAndMergePass) {
		if (blurredFgCoc == null) {
			if (maxRadius > 10.0) {
				blurPass = 20;
				blurAndMergePass = 22;
			} else if (maxRadius > 5.0) {
				blurPass = 16;
				blurAndMergePass = 18;
			} else {
				blurPass = 12;
				blurAndMergePass = 14;
			}
		} else if (maxRadius > 10.0) {
			blurPass = 21;
			blurAndMergePass = 23;
		} else if (maxRadius > 5.0) {
			blurPass = 17;
			blurAndMergePass = 19;
		} else {
			blurPass = 13;
			blurAndMergePass = 15;
		}
	}

	private enum Passes {
		BlurAlphaWeighted,
		BoxBlur,
		DilateFgCocFromColor,
		DilateFgCoc,
		CaptureCocExplicit,
		VisualizeCocExplicit,
		CocPrefilter,
		CircleBlur,
		CircleBlurWithDilatedFg,
		CircleBlurLowQuality,
		CircleBlowLowQualityWithDilatedFg,
		MergeExplicit,
		ShapeLowQuality,
		ShapeLowQualityDilateFg,
		ShapeLowQualityMerge,
		ShapeLowQualityMergeDilateFg,
		ShapeMediumQuality,
		ShapeMediumQualityDilateFg,
		ShapeMediumQualityMerge,
		ShapeMediumQualityMergeDilateFg,
		ShapeHighQuality,
		ShapeHighQualityDilateFg,
		ShapeHighQualityMerge,
		ShapeHighQualityMergeDilateFg
	}

	private enum MedianPasses {
		Median3,
		Median3X3
	}

	private enum BokehTexturesPasses {
		Apply,
		Collect
	}

	public enum TweakMode {
		Range,
		Explicit
	}

	public enum ApertureShape {
		Circular,
		Hexagonal,
		Octogonal
	}

	public enum QualityPreset {
		Low,
		Medium,
		High
	}

	public enum FilterQuality {
		None,
		Normal,
		High
	}

	[Serializable]
	public struct GlobalSettings {
		[Tooltip("Allows to view where the blur will be applied. Yellow for near blur, blue for far blur.")]
		public bool visualizeFocus;

		[Tooltip(
			"Setup mode. Use \"Advanced\" if you need more control on blur settings and/or want to use a bokeh texture. \"Explicit\" is the same as \"Advanced\" but makes use of \"Near Plane\" and \"Far Plane\" values instead of \"F-Stop\".")]
		public TweakMode tweakMode;

		[Tooltip("Quality presets. Use \"Custom\" for more advanced settings.")]
		public QualityPreset filteringQuality;

		[Tooltip("\"Circular\" is the fastest, followed by \"Hexagonal\" and \"Octogonal\".")]
		public ApertureShape apertureShape;

		[Range(0.0f, 179f)] [Tooltip("Rotates the aperture when working with \"Hexagonal\" and \"Ortogonal\".")]
		public float apertureOrientation;

		public static GlobalSettings defaultSettings =>
			new() {
				visualizeFocus = false,
				tweakMode = TweakMode.Range,
				filteringQuality = QualityPreset.High,
				apertureShape = ApertureShape.Circular,
				apertureOrientation = 0.0f
			};
	}

	[Serializable]
	public struct QualitySettings {
		[Tooltip("Enable this to get smooth bokeh.")]
		public bool prefilterBlur;

		[Tooltip("Applies a median filter for even smoother bokeh.")]
		public FilterQuality medianFilter;

		[Tooltip("Dilates near blur over in focus area.")]
		public bool dilateNearBlur;

		public static QualitySettings[] presetQualitySettings = new QualitySettings[3] {
			new() {
				prefilterBlur = false,
				medianFilter = FilterQuality.None,
				dilateNearBlur = false
			},
			new() {
				prefilterBlur = true,
				medianFilter = FilterQuality.Normal,
				dilateNearBlur = false
			},
			new() {
				prefilterBlur = true,
				medianFilter = FilterQuality.High,
				dilateNearBlur = true
			}
		};
	}

	[Serializable]
	public struct FocusSettings {
		[Tooltip("Auto-focus on a selected transform.")]
		public Transform transform;

		[Min(0.0f)] [Tooltip("Focus distance (in world units).")]
		public float focusPlane;

		[Min(0.1f)] [Tooltip("Focus range (in world units). The focus plane is located in the center of the range.")]
		public float range;

		[Min(0.0f)] [Tooltip("Near focus distance (in world units).")]
		public float nearPlane;

		[Min(0.0f)] [Tooltip("Near blur falloff (in world units).")]
		public float nearFalloff;

		[Min(0.0f)] [Tooltip("Far focus distance (in world units).")]
		public float farPlane;

		[Min(0.0f)] [Tooltip("Far blur falloff (in world units).")]
		public float farFalloff;

		[Range(0.0f, 40f)] [Tooltip("Maximum blur radius for the near plane.")]
		public float nearBlurRadius;

		[Range(0.0f, 40f)] [Tooltip("Maximum blur radius for the far plane.")]
		public float farBlurRadius;

		public static FocusSettings defaultSettings =>
			new() {
				transform = null,
				focusPlane = 20f,
				range = 35f,
				nearPlane = 2.5f,
				nearFalloff = 15f,
				farPlane = 37.5f,
				farFalloff = 50f,
				nearBlurRadius = 15f,
				farBlurRadius = 20f
			};
	}

	[Serializable]
	public struct BokehTextureSettings {
		[Tooltip(
			"Adding a texture to this field will enable the use of \"Bokeh Textures\". Use with care. This feature is only available on Shader Model 5 compatible-hardware and performance scale with the amount of bokeh.")]
		public Texture2D texture;

		[Range(0.01f, 10f)] [Tooltip("Maximum size of bokeh textures on screen.")]
		public float scale;

		[Range(0.01f, 100f)] [Tooltip("Bokeh brightness.")]
		public float intensity;

		[Range(0.01f, 5f)] [Tooltip("Controls the amount of bokeh textures. Lower values mean more bokeh splats.")]
		public float threshold;

		[Range(0.01f, 1f)] [Tooltip("Controls the spawn conditions. Lower values mean more visible bokeh.")]
		public float spawnHeuristic;

		public static BokehTextureSettings defaultSettings =>
			new() {
				texture = null,
				scale = 1f,
				intensity = 50f,
				threshold = 2f,
				spawnHeuristic = 0.15f
			};
	}
}