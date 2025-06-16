using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing;

public sealed class ScreenSpaceReflectionComponent :
	PostProcessingComponentCommandBuffer<ScreenSpaceReflectionModel> {
	private bool k_HighlightSuppression = false;
	private bool k_TraceBehindObjects = true;
	private bool k_TreatBackfaceHitAsMiss = false;
	private bool k_BilateralUpsample = true;
	private readonly int[] m_ReflectionTextures = new int[5];

	public override DepthTextureMode GetCameraFlags() {
		return DepthTextureMode.Depth;
	}

	public override bool active => model.enabled && context.isGBufferAvailable && !context.interrupted;

	public override void OnEnable() {
		m_ReflectionTextures[0] = Shader.PropertyToID("_ReflectionTexture0");
		m_ReflectionTextures[1] = Shader.PropertyToID("_ReflectionTexture1");
		m_ReflectionTextures[2] = Shader.PropertyToID("_ReflectionTexture2");
		m_ReflectionTextures[3] = Shader.PropertyToID("_ReflectionTexture3");
		m_ReflectionTextures[4] = Shader.PropertyToID("_ReflectionTexture4");
	}

	public override string GetName() {
		return "Screen Space Reflection";
	}

	public override CameraEvent GetCameraEvent() {
		return CameraEvent.AfterFinalPass;
	}

	public override void PopulateCommandBuffer(CommandBuffer cb) {
		var settings = model.settings;
		var camera = context.camera;
		var num1 = settings.reflection.reflectionQuality == ScreenSpaceReflectionModel.SSRResolution.High ? 1 : 2;
		var num2 = context.width / num1;
		var num3 = context.height / num1;
		float width = context.width;
		float height = context.height;
		var num4 = width / 2f;
		var num5 = height / 2f;
		var mat = context.materialFactory.Get("Hidden/Post FX/Screen Space Reflection");
		mat.SetInt(Uniforms._RayStepSize, settings.reflection.stepSize);
		mat.SetInt(Uniforms._AdditiveReflection,
			settings.reflection.blendType == ScreenSpaceReflectionModel.SSRReflectionBlendType.Additive ? 1 : 0);
		mat.SetInt(Uniforms._BilateralUpsampling, k_BilateralUpsample ? 1 : 0);
		mat.SetInt(Uniforms._TreatBackfaceHitAsMiss, k_TreatBackfaceHitAsMiss ? 1 : 0);
		mat.SetInt(Uniforms._AllowBackwardsRays, settings.reflection.reflectBackfaces ? 1 : 0);
		mat.SetInt(Uniforms._TraceBehindObjects, k_TraceBehindObjects ? 1 : 0);
		mat.SetInt(Uniforms._MaxSteps, settings.reflection.iterationCount);
		mat.SetInt(Uniforms._FullResolutionFiltering, 0);
		mat.SetInt(Uniforms._HalfResolution,
			settings.reflection.reflectionQuality != ScreenSpaceReflectionModel.SSRResolution.High ? 1 : 0);
		mat.SetInt(Uniforms._HighlightSuppression, k_HighlightSuppression ? 1 : 0);
		var num6 = width / (-2f * Mathf.Tan((float)(camera.fieldOfView / 180.0 * 3.1415927410125732 * 0.5)));
		mat.SetFloat(Uniforms._PixelsPerMeterAtOneMeter, num6);
		mat.SetFloat(Uniforms._ScreenEdgeFading, settings.screenEdgeMask.intensity);
		mat.SetFloat(Uniforms._ReflectionBlur, settings.reflection.reflectionBlur);
		mat.SetFloat(Uniforms._MaxRayTraceDistance, settings.reflection.maxDistance);
		mat.SetFloat(Uniforms._FadeDistance, settings.intensity.fadeDistance);
		mat.SetFloat(Uniforms._LayerThickness, settings.reflection.widthModifier);
		mat.SetFloat(Uniforms._SSRMultiplier, settings.intensity.reflectionMultiplier);
		mat.SetFloat(Uniforms._FresnelFade, settings.intensity.fresnelFade);
		mat.SetFloat(Uniforms._FresnelFadePower, settings.intensity.fresnelFadePower);
		var projectionMatrix = camera.projectionMatrix;
		var vector4 = new Vector4((float)(-2.0 / (width * (double)projectionMatrix[0])),
			(float)(-2.0 / (height * (double)projectionMatrix[5])), (1f - projectionMatrix[2]) / projectionMatrix[0],
			(1f + projectionMatrix[6]) / projectionMatrix[5]);
		var vector3 = float.IsPositiveInfinity(camera.farClipPlane)
			? new Vector3(camera.nearClipPlane, -1f, 1f)
			: new Vector3(camera.nearClipPlane * camera.farClipPlane, camera.nearClipPlane - camera.farClipPlane,
				camera.farClipPlane);
		mat.SetVector(Uniforms._ReflectionBufferSize, new Vector2(num2, num3));
		mat.SetVector(Uniforms._ScreenSize, new Vector2(width, height));
		mat.SetVector(Uniforms._InvScreenSize, new Vector2(1f / width, 1f / height));
		mat.SetVector(Uniforms._ProjInfo, vector4);
		mat.SetVector(Uniforms._CameraClipInfo, vector3);
		var matrix4x4_1 = new Matrix4x4();
		matrix4x4_1.SetRow(0, new Vector4(num4, 0.0f, 0.0f, num4));
		matrix4x4_1.SetRow(1, new Vector4(0.0f, num5, 0.0f, num5));
		matrix4x4_1.SetRow(2, new Vector4(0.0f, 0.0f, 1f, 0.0f));
		matrix4x4_1.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1f));
		var matrix4x4_2 = matrix4x4_1 * projectionMatrix;
		mat.SetMatrix(Uniforms._ProjectToPixelMatrix, matrix4x4_2);
		mat.SetMatrix(Uniforms._WorldToCameraMatrix, camera.worldToCameraMatrix);
		mat.SetMatrix(Uniforms._CameraToWorldMatrix, camera.worldToCameraMatrix.inverse);
		var format = context.isHdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
		var roughnessTexture = Uniforms._NormalAndRoughnessTexture;
		var hitPointTexture = Uniforms._HitPointTexture;
		var blurTexture = Uniforms._BlurTexture;
		var filteredReflections = Uniforms._FilteredReflections;
		var reflectionTexture1 = Uniforms._FinalReflectionTexture;
		var tempTexture = Uniforms._TempTexture;
		cb.GetTemporaryRT(roughnessTexture, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32,
			RenderTextureReadWrite.Linear);
		cb.GetTemporaryRT(hitPointTexture, num2, num3, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf,
			RenderTextureReadWrite.Linear);
		for (var index = 0; index < 5; ++index)
			cb.GetTemporaryRT(m_ReflectionTextures[index], num2 >> index, num3 >> index, 0, FilterMode.Bilinear,
				format);
		cb.GetTemporaryRT(filteredReflections, num2, num3, 0,
			k_BilateralUpsample ? FilterMode.Point : FilterMode.Bilinear, format);
		cb.GetTemporaryRT(reflectionTexture1, num2, num3, 0, FilterMode.Point, format);
		cb.Blit(BuiltinRenderTextureType.CameraTarget, roughnessTexture, mat, 6);
		cb.Blit(BuiltinRenderTextureType.CameraTarget, hitPointTexture, mat, 0);
		cb.Blit(BuiltinRenderTextureType.CameraTarget, filteredReflections, mat, 5);
		cb.Blit(filteredReflections, m_ReflectionTextures[0], mat, 8);
		for (var index = 1; index < 5; ++index) {
			var reflectionTexture2 = m_ReflectionTextures[index - 1];
			var num7 = index;
			cb.GetTemporaryRT(blurTexture, num2 >> num7, num3 >> num7, 0, FilterMode.Bilinear, format);
			cb.SetGlobalVector(Uniforms._Axis, new Vector4(1f, 0.0f, 0.0f, 0.0f));
			cb.SetGlobalFloat(Uniforms._CurrentMipLevel, index - 1f);
			cb.Blit(reflectionTexture2, blurTexture, mat, 2);
			cb.SetGlobalVector(Uniforms._Axis, new Vector4(0.0f, 1f, 0.0f, 0.0f));
			var reflectionTexture3 = m_ReflectionTextures[index];
			cb.Blit(blurTexture, reflectionTexture3, mat, 2);
			cb.ReleaseTemporaryRT(blurTexture);
		}

		cb.Blit(m_ReflectionTextures[0], reflectionTexture1, mat, 3);
		cb.GetTemporaryRT(tempTexture, camera.pixelWidth, camera.pixelHeight, 0, FilterMode.Bilinear, format);
		cb.Blit(BuiltinRenderTextureType.CameraTarget, tempTexture, mat, 1);
		cb.Blit(tempTexture, BuiltinRenderTextureType.CameraTarget);
		cb.ReleaseTemporaryRT(tempTexture);
	}

	private static class Uniforms {
		internal static readonly int _RayStepSize = Shader.PropertyToID(nameof(_RayStepSize));
		internal static readonly int _AdditiveReflection = Shader.PropertyToID(nameof(_AdditiveReflection));
		internal static readonly int _BilateralUpsampling = Shader.PropertyToID(nameof(_BilateralUpsampling));
		internal static readonly int _TreatBackfaceHitAsMiss = Shader.PropertyToID(nameof(_TreatBackfaceHitAsMiss));
		internal static readonly int _AllowBackwardsRays = Shader.PropertyToID(nameof(_AllowBackwardsRays));
		internal static readonly int _TraceBehindObjects = Shader.PropertyToID(nameof(_TraceBehindObjects));
		internal static readonly int _MaxSteps = Shader.PropertyToID(nameof(_MaxSteps));
		internal static readonly int _FullResolutionFiltering = Shader.PropertyToID(nameof(_FullResolutionFiltering));
		internal static readonly int _HalfResolution = Shader.PropertyToID(nameof(_HalfResolution));
		internal static readonly int _HighlightSuppression = Shader.PropertyToID(nameof(_HighlightSuppression));
		internal static readonly int _PixelsPerMeterAtOneMeter = Shader.PropertyToID(nameof(_PixelsPerMeterAtOneMeter));
		internal static readonly int _ScreenEdgeFading = Shader.PropertyToID(nameof(_ScreenEdgeFading));
		internal static readonly int _ReflectionBlur = Shader.PropertyToID(nameof(_ReflectionBlur));
		internal static readonly int _MaxRayTraceDistance = Shader.PropertyToID(nameof(_MaxRayTraceDistance));
		internal static readonly int _FadeDistance = Shader.PropertyToID(nameof(_FadeDistance));
		internal static readonly int _LayerThickness = Shader.PropertyToID(nameof(_LayerThickness));
		internal static readonly int _SSRMultiplier = Shader.PropertyToID(nameof(_SSRMultiplier));
		internal static readonly int _FresnelFade = Shader.PropertyToID(nameof(_FresnelFade));
		internal static readonly int _FresnelFadePower = Shader.PropertyToID(nameof(_FresnelFadePower));
		internal static readonly int _ReflectionBufferSize = Shader.PropertyToID(nameof(_ReflectionBufferSize));
		internal static readonly int _ScreenSize = Shader.PropertyToID(nameof(_ScreenSize));
		internal static readonly int _InvScreenSize = Shader.PropertyToID(nameof(_InvScreenSize));
		internal static readonly int _ProjInfo = Shader.PropertyToID(nameof(_ProjInfo));
		internal static readonly int _CameraClipInfo = Shader.PropertyToID(nameof(_CameraClipInfo));
		internal static readonly int _ProjectToPixelMatrix = Shader.PropertyToID(nameof(_ProjectToPixelMatrix));
		internal static readonly int _WorldToCameraMatrix = Shader.PropertyToID(nameof(_WorldToCameraMatrix));
		internal static readonly int _CameraToWorldMatrix = Shader.PropertyToID(nameof(_CameraToWorldMatrix));
		internal static readonly int _Axis = Shader.PropertyToID(nameof(_Axis));
		internal static readonly int _CurrentMipLevel = Shader.PropertyToID(nameof(_CurrentMipLevel));

		internal static readonly int _NormalAndRoughnessTexture =
			Shader.PropertyToID(nameof(_NormalAndRoughnessTexture));

		internal static readonly int _HitPointTexture = Shader.PropertyToID(nameof(_HitPointTexture));
		internal static readonly int _BlurTexture = Shader.PropertyToID(nameof(_BlurTexture));
		internal static readonly int _FilteredReflections = Shader.PropertyToID(nameof(_FilteredReflections));
		internal static readonly int _FinalReflectionTexture = Shader.PropertyToID(nameof(_FinalReflectionTexture));
		internal static readonly int _TempTexture = Shader.PropertyToID(nameof(_TempTexture));
	}

	private enum PassIndex {
		RayTraceStep,
		CompositeFinal,
		Blur,
		CompositeSSR,
		MinMipGeneration,
		HitPointToReflections,
		BilateralKeyPack,
		BlitDepthAsCSZ,
		PoissonBlur
	}
}