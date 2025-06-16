using System;

namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [ImageEffectAllowedInSceneView]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Cinematic Image Effects/Screen Space Reflections")]
  public class ScreenSpaceReflection : MonoBehaviour
  {
    [SerializeField]
    public SSRSettings settings = SSRSettings.defaultSettings;
    [Tooltip("Enable to limit the effect a few bright pixels can have on rougher surfaces")]
    private bool highlightSuppression = false;
    [Tooltip("Enable to allow rays to pass behind objects. This can lead to more screen-space reflections, but the reflections are more likely to be wrong.")]
    private bool traceBehindObjects = true;
    [Tooltip("Enable to force more surfaces to use reflection probes if you see streaks on the sides of objects or bad reflections of their backs.")]
    private bool treatBackfaceHitAsMiss = false;
    [Tooltip("Drastically improves reflection reconstruction quality at the expense of some performance.")]
    private bool bilateralUpsample = true;
    [SerializeField]
    private Shader m_Shader;
    private Material m_Material;
    private Camera m_Camera;
    private CommandBuffer m_CommandBuffer;
    private static int kNormalAndRoughnessTexture;
    private static int kHitPointTexture;
    private static int[] kReflectionTextures;
    private static int kFilteredReflections;
    private static int kBlurTexture;
    private static int kFinalReflectionTexture;
    private static int kTempTexture;

    public Shader shader
    {
      get
      {
        if ((UnityEngine.Object) m_Shader == (UnityEngine.Object) null)
          m_Shader = Shader.Find("Hidden/ScreenSpaceReflection");
        return m_Shader;
      }
    }

    public Material material
    {
      get
      {
        if ((UnityEngine.Object) m_Material == (UnityEngine.Object) null)
          m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(shader);
        return m_Material;
      }
    }

    public Camera camera_
    {
      get
      {
        if ((UnityEngine.Object) m_Camera == (UnityEngine.Object) null)
          m_Camera = this.GetComponent<Camera>();
        return m_Camera;
      }
    }

    private void OnEnable()
    {
      if (!ImageEffectHelper.IsSupported(shader, false, true, (MonoBehaviour) this))
      {
        this.enabled = false;
      }
      else
      {
        camera_.depthTextureMode |= DepthTextureMode.Depth;
        kReflectionTextures = new int[5];
        kNormalAndRoughnessTexture = Shader.PropertyToID("_NormalAndRoughnessTexture");
        kHitPointTexture = Shader.PropertyToID("_HitPointTexture");
        kReflectionTextures[0] = Shader.PropertyToID("_ReflectionTexture0");
        kReflectionTextures[1] = Shader.PropertyToID("_ReflectionTexture1");
        kReflectionTextures[2] = Shader.PropertyToID("_ReflectionTexture2");
        kReflectionTextures[3] = Shader.PropertyToID("_ReflectionTexture3");
        kReflectionTextures[4] = Shader.PropertyToID("_ReflectionTexture4");
        kBlurTexture = Shader.PropertyToID("_BlurTexture");
        kFilteredReflections = Shader.PropertyToID("_FilteredReflections");
        kFinalReflectionTexture = Shader.PropertyToID("_FinalReflectionTexture");
        kTempTexture = Shader.PropertyToID("_TempTexture");
      }
    }

    private void OnDisable()
    {
      if ((bool) (UnityEngine.Object) m_Material)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_Material);
      m_Material = (Material) null;
      if (!((UnityEngine.Object) camera_ != (UnityEngine.Object) null))
        return;
      if (m_CommandBuffer != null)
        camera_.RemoveCommandBuffer(CameraEvent.AfterFinalPass, m_CommandBuffer);
      m_CommandBuffer = (CommandBuffer) null;
    }

    public void OnPreRender()
    {
      if ((UnityEngine.Object) material == (UnityEngine.Object) null || Camera.current.actualRenderingPath != RenderingPath.DeferredShading)
        return;
      int num1 = settings.reflectionSettings.reflectionQuality == SSRResolution.High ? 1 : 2;
      int num2 = camera_.pixelWidth / num1;
      int num3 = camera_.pixelHeight / num1;
      float pixelWidth = (float) camera_.pixelWidth;
      float pixelHeight = (float) camera_.pixelHeight;
      float num4 = pixelWidth / 2f;
      float num5 = pixelHeight / 2f;
      RenderTextureFormat format = camera_.allowHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
      material.SetInt("_RayStepSize", settings.reflectionSettings.stepSize);
      material.SetInt("_AdditiveReflection", settings.reflectionSettings.blendType == SSRReflectionBlendType.Additive ? 1 : 0);
      material.SetInt("_BilateralUpsampling", bilateralUpsample ? 1 : 0);
      material.SetInt("_TreatBackfaceHitAsMiss", treatBackfaceHitAsMiss ? 1 : 0);
      material.SetInt("_AllowBackwardsRays", settings.reflectionSettings.reflectBackfaces ? 1 : 0);
      material.SetInt("_TraceBehindObjects", traceBehindObjects ? 1 : 0);
      material.SetInt("_MaxSteps", settings.reflectionSettings.iterationCount);
      material.SetInt("_FullResolutionFiltering", 0);
      material.SetInt("_HalfResolution", settings.reflectionSettings.reflectionQuality != SSRResolution.High ? 1 : 0);
      material.SetInt("_HighlightSuppression", highlightSuppression ? 1 : 0);
      material.SetFloat("_PixelsPerMeterAtOneMeter", pixelWidth / (-2f * (float) Math.Tan((double) camera_.fieldOfView / 180.0 * Math.PI * 0.5)));
      material.SetFloat("_ScreenEdgeFading", settings.screenEdgeMask.intensity);
      material.SetFloat("_ReflectionBlur", settings.reflectionSettings.reflectionBlur);
      material.SetFloat("_MaxRayTraceDistance", settings.reflectionSettings.maxDistance);
      material.SetFloat("_FadeDistance", settings.intensitySettings.fadeDistance);
      material.SetFloat("_LayerThickness", settings.reflectionSettings.widthModifier);
      material.SetFloat("_SSRMultiplier", settings.intensitySettings.reflectionMultiplier);
      material.SetFloat("_FresnelFade", settings.intensitySettings.fresnelFade);
      material.SetFloat("_FresnelFadePower", settings.intensitySettings.fresnelFadePower);
      Matrix4x4 projectionMatrix = camera_.projectionMatrix;
      Vector4 vector4 = new Vector4((float) (-2.0 / (pixelWidth * (double) projectionMatrix[0])), (float) (-2.0 / (pixelHeight * (double) projectionMatrix[5])), (1f - projectionMatrix[2]) / projectionMatrix[0], (1f + projectionMatrix[6]) / projectionMatrix[5]);
      Vector3 vector3 = float.IsPositiveInfinity(camera_.farClipPlane) ? new Vector3(camera_.nearClipPlane, -1f, 1f) : new Vector3(camera_.nearClipPlane * camera_.farClipPlane, camera_.nearClipPlane - camera_.farClipPlane, camera_.farClipPlane);
      material.SetVector("_ReflectionBufferSize", (Vector4) new Vector2((float) num2, (float) num3));
      material.SetVector("_ScreenSize", (Vector4) new Vector2(pixelWidth, pixelHeight));
      material.SetVector("_InvScreenSize", (Vector4) new Vector2(1f / pixelWidth, 1f / pixelHeight));
      material.SetVector("_ProjInfo", vector4);
      material.SetVector("_CameraClipInfo", (Vector4) vector3);
      Matrix4x4 matrix4x4 = new Matrix4x4();
      matrix4x4.SetRow(0, new Vector4(num4, 0.0f, 0.0f, num4));
      matrix4x4.SetRow(1, new Vector4(0.0f, num5, 0.0f, num5));
      matrix4x4.SetRow(2, new Vector4(0.0f, 0.0f, 1f, 0.0f));
      matrix4x4.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1f));
      material.SetMatrix("_ProjectToPixelMatrix", matrix4x4 * projectionMatrix);
      material.SetMatrix("_WorldToCameraMatrix", camera_.worldToCameraMatrix);
      material.SetMatrix("_CameraToWorldMatrix", camera_.worldToCameraMatrix.inverse);
      if (m_CommandBuffer != null)
        return;
      m_CommandBuffer = new CommandBuffer();
      m_CommandBuffer.name = "Screen Space Reflections";
      m_CommandBuffer.GetTemporaryRT(kNormalAndRoughnessTexture, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
      m_CommandBuffer.GetTemporaryRT(kHitPointTexture, num2, num3, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
      for (int index = 0; index < 5; ++index)
        m_CommandBuffer.GetTemporaryRT(kReflectionTextures[index], num2 >> index, num3 >> index, 0, FilterMode.Bilinear, format);
      m_CommandBuffer.GetTemporaryRT(kFilteredReflections, num2, num3, 0, bilateralUpsample ? FilterMode.Point : FilterMode.Bilinear, format);
      m_CommandBuffer.GetTemporaryRT(kFinalReflectionTexture, num2, num3, 0, FilterMode.Point, format);
      m_CommandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) kNormalAndRoughnessTexture, material, 6);
      m_CommandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) kHitPointTexture, material, 0);
      m_CommandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) kFilteredReflections, material, 5);
      m_CommandBuffer.Blit((RenderTargetIdentifier) kFilteredReflections, (RenderTargetIdentifier) kReflectionTextures[0], material, 8);
      for (int index = 1; index < 5; ++index)
      {
        int reflectionTexture1 = kReflectionTextures[index - 1];
        int num6 = index;
        m_CommandBuffer.GetTemporaryRT(kBlurTexture, num2 >> num6, num3 >> num6, 0, FilterMode.Bilinear, format);
        m_CommandBuffer.SetGlobalVector("_Axis", new Vector4(1f, 0.0f, 0.0f, 0.0f));
        m_CommandBuffer.SetGlobalFloat("_CurrentMipLevel", index - 1f);
        m_CommandBuffer.Blit((RenderTargetIdentifier) reflectionTexture1, (RenderTargetIdentifier) kBlurTexture, material, 2);
        m_CommandBuffer.SetGlobalVector("_Axis", new Vector4(0.0f, 1f, 0.0f, 0.0f));
        int reflectionTexture2 = kReflectionTextures[index];
        m_CommandBuffer.Blit((RenderTargetIdentifier) kBlurTexture, (RenderTargetIdentifier) reflectionTexture2, material, 2);
        m_CommandBuffer.ReleaseTemporaryRT(kBlurTexture);
      }
      m_CommandBuffer.Blit((RenderTargetIdentifier) kReflectionTextures[0], (RenderTargetIdentifier) kFinalReflectionTexture, material, 3);
      m_CommandBuffer.GetTemporaryRT(kTempTexture, camera_.pixelWidth, camera_.pixelHeight, 0, FilterMode.Bilinear, format);
      m_CommandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) kTempTexture, material, 1);
      m_CommandBuffer.Blit((RenderTargetIdentifier) kTempTexture, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget);
      m_CommandBuffer.ReleaseTemporaryRT(kTempTexture);
      camera_.AddCommandBuffer(CameraEvent.AfterFinalPass, m_CommandBuffer);
    }

    public enum SSRResolution
    {
      High = 0,
      Low = 2,
    }

    public enum SSRReflectionBlendType
    {
      PhysicallyBased,
      Additive,
    }

    [Serializable]
    public struct SSRSettings
    {
      [SSRSettings.Layout]
      public ReflectionSettings reflectionSettings;
      [SSRSettings.Layout]
      public IntensitySettings intensitySettings;
      [SSRSettings.Layout]
      public ScreenEdgeMask screenEdgeMask;
      private static readonly SSRSettings s_Default = new SSRSettings {
        reflectionSettings = new ReflectionSettings {
          blendType = SSRReflectionBlendType.PhysicallyBased,
          reflectionQuality = SSRResolution.High,
          maxDistance = 100f,
          iterationCount = 256,
          stepSize = 3,
          widthModifier = 0.5f,
          reflectionBlur = 1f,
          reflectBackfaces = true
        },
        intensitySettings = new IntensitySettings {
          reflectionMultiplier = 1f,
          fadeDistance = 100f,
          fresnelFade = 1f,
          fresnelFadePower = 1f
        },
        screenEdgeMask = new ScreenEdgeMask {
          intensity = 0.03f
        }
      };

      public static SSRSettings defaultSettings
      {
        get => s_Default;
      }

      [AttributeUsage(AttributeTargets.Field)]
      public class LayoutAttribute : PropertyAttribute
      {
      }
    }

    [Serializable]
    public struct IntensitySettings
    {
      [Tooltip("Nonphysical multiplier for the SSR reflections. 1.0 is physically based.")]
      [Range(0.0f, 2f)]
      public float reflectionMultiplier;
      [Tooltip("How far away from the maxDistance to begin fading SSR.")]
      [Range(0.0f, 1000f)]
      public float fadeDistance;
      [Tooltip("Amplify Fresnel fade out. Increase if floor reflections look good close to the surface and bad farther 'under' the floor.")]
      [Range(0.0f, 1f)]
      public float fresnelFade;
      [Tooltip("Higher values correspond to a faster Fresnel fade as the reflection changes from the grazing angle.")]
      [Range(0.1f, 10f)]
      public float fresnelFadePower;
    }

    [Serializable]
    public struct ReflectionSettings
    {
      [Tooltip("How the reflections are blended into the render.")]
      public SSRReflectionBlendType blendType;
      [Tooltip("Half resolution SSRR is much faster, but less accurate.")]
      public SSRResolution reflectionQuality;
      [Tooltip("Maximum reflection distance in world units.")]
      [Range(0.1f, 300f)]
      public float maxDistance;
      [Tooltip("Max raytracing length.")]
      [Range(16f, 1024f)]
      public int iterationCount;
      [Tooltip("Log base 2 of ray tracing coarse step size. Higher traces farther, lower gives better quality silhouettes.")]
      [Range(1f, 16f)]
      public int stepSize;
      [Tooltip("Typical thickness of columns, walls, furniture, and other objects that reflection rays might pass behind.")]
      [Range(0.01f, 10f)]
      public float widthModifier;
      [Tooltip("Blurriness of reflections.")]
      [Range(0.1f, 8f)]
      public float reflectionBlur;
      [Tooltip("Enable for a performance gain in scenes where most glossy objects are horizontal, like floors, water, and tables. Leave on for scenes with glossy vertical objects.")]
      public bool reflectBackfaces;
    }

    [Serializable]
    public struct ScreenEdgeMask
    {
      [Tooltip("Higher = fade out SSRR near the edge of the screen so that reflections don't pop under camera motion.")]
      [Range(0.0f, 1f)]
      public float intensity;
    }

    private enum PassIndex
    {
      RayTraceStep,
      CompositeFinal,
      Blur,
      CompositeSSR,
      MinMipGeneration,
      HitPointToReflections,
      BilateralKeyPack,
      BlitDepthAsCSZ,
      PoissonBlur,
    }
  }
}
