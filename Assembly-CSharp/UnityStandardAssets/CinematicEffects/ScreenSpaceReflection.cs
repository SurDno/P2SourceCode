// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CinematicEffects.ScreenSpaceReflection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [ImageEffectAllowedInSceneView]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Cinematic Image Effects/Screen Space Reflections")]
  public class ScreenSpaceReflection : MonoBehaviour
  {
    [SerializeField]
    public ScreenSpaceReflection.SSRSettings settings = ScreenSpaceReflection.SSRSettings.defaultSettings;
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
        if ((UnityEngine.Object) this.m_Shader == (UnityEngine.Object) null)
          this.m_Shader = Shader.Find("Hidden/ScreenSpaceReflection");
        return this.m_Shader;
      }
    }

    public Material material
    {
      get
      {
        if ((UnityEngine.Object) this.m_Material == (UnityEngine.Object) null)
          this.m_Material = ImageEffectHelper.CheckShaderAndCreateMaterial(this.shader);
        return this.m_Material;
      }
    }

    public Camera camera_
    {
      get
      {
        if ((UnityEngine.Object) this.m_Camera == (UnityEngine.Object) null)
          this.m_Camera = this.GetComponent<Camera>();
        return this.m_Camera;
      }
    }

    private void OnEnable()
    {
      if (!ImageEffectHelper.IsSupported(this.shader, false, true, (MonoBehaviour) this))
      {
        this.enabled = false;
      }
      else
      {
        this.camera_.depthTextureMode |= DepthTextureMode.Depth;
        ScreenSpaceReflection.kReflectionTextures = new int[5];
        ScreenSpaceReflection.kNormalAndRoughnessTexture = Shader.PropertyToID("_NormalAndRoughnessTexture");
        ScreenSpaceReflection.kHitPointTexture = Shader.PropertyToID("_HitPointTexture");
        ScreenSpaceReflection.kReflectionTextures[0] = Shader.PropertyToID("_ReflectionTexture0");
        ScreenSpaceReflection.kReflectionTextures[1] = Shader.PropertyToID("_ReflectionTexture1");
        ScreenSpaceReflection.kReflectionTextures[2] = Shader.PropertyToID("_ReflectionTexture2");
        ScreenSpaceReflection.kReflectionTextures[3] = Shader.PropertyToID("_ReflectionTexture3");
        ScreenSpaceReflection.kReflectionTextures[4] = Shader.PropertyToID("_ReflectionTexture4");
        ScreenSpaceReflection.kBlurTexture = Shader.PropertyToID("_BlurTexture");
        ScreenSpaceReflection.kFilteredReflections = Shader.PropertyToID("_FilteredReflections");
        ScreenSpaceReflection.kFinalReflectionTexture = Shader.PropertyToID("_FinalReflectionTexture");
        ScreenSpaceReflection.kTempTexture = Shader.PropertyToID("_TempTexture");
      }
    }

    private void OnDisable()
    {
      if ((bool) (UnityEngine.Object) this.m_Material)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Material);
      this.m_Material = (Material) null;
      if (!((UnityEngine.Object) this.camera_ != (UnityEngine.Object) null))
        return;
      if (this.m_CommandBuffer != null)
        this.camera_.RemoveCommandBuffer(CameraEvent.AfterFinalPass, this.m_CommandBuffer);
      this.m_CommandBuffer = (CommandBuffer) null;
    }

    public void OnPreRender()
    {
      if ((UnityEngine.Object) this.material == (UnityEngine.Object) null || Camera.current.actualRenderingPath != RenderingPath.DeferredShading)
        return;
      int num1 = this.settings.reflectionSettings.reflectionQuality == ScreenSpaceReflection.SSRResolution.High ? 1 : 2;
      int num2 = this.camera_.pixelWidth / num1;
      int num3 = this.camera_.pixelHeight / num1;
      float pixelWidth = (float) this.camera_.pixelWidth;
      float pixelHeight = (float) this.camera_.pixelHeight;
      float num4 = pixelWidth / 2f;
      float num5 = pixelHeight / 2f;
      RenderTextureFormat format = this.camera_.allowHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
      this.material.SetInt("_RayStepSize", this.settings.reflectionSettings.stepSize);
      this.material.SetInt("_AdditiveReflection", this.settings.reflectionSettings.blendType == ScreenSpaceReflection.SSRReflectionBlendType.Additive ? 1 : 0);
      this.material.SetInt("_BilateralUpsampling", this.bilateralUpsample ? 1 : 0);
      this.material.SetInt("_TreatBackfaceHitAsMiss", this.treatBackfaceHitAsMiss ? 1 : 0);
      this.material.SetInt("_AllowBackwardsRays", this.settings.reflectionSettings.reflectBackfaces ? 1 : 0);
      this.material.SetInt("_TraceBehindObjects", this.traceBehindObjects ? 1 : 0);
      this.material.SetInt("_MaxSteps", this.settings.reflectionSettings.iterationCount);
      this.material.SetInt("_FullResolutionFiltering", 0);
      this.material.SetInt("_HalfResolution", this.settings.reflectionSettings.reflectionQuality != ScreenSpaceReflection.SSRResolution.High ? 1 : 0);
      this.material.SetInt("_HighlightSuppression", this.highlightSuppression ? 1 : 0);
      this.material.SetFloat("_PixelsPerMeterAtOneMeter", pixelWidth / (-2f * (float) Math.Tan((double) this.camera_.fieldOfView / 180.0 * Math.PI * 0.5)));
      this.material.SetFloat("_ScreenEdgeFading", this.settings.screenEdgeMask.intensity);
      this.material.SetFloat("_ReflectionBlur", this.settings.reflectionSettings.reflectionBlur);
      this.material.SetFloat("_MaxRayTraceDistance", this.settings.reflectionSettings.maxDistance);
      this.material.SetFloat("_FadeDistance", this.settings.intensitySettings.fadeDistance);
      this.material.SetFloat("_LayerThickness", this.settings.reflectionSettings.widthModifier);
      this.material.SetFloat("_SSRMultiplier", this.settings.intensitySettings.reflectionMultiplier);
      this.material.SetFloat("_FresnelFade", this.settings.intensitySettings.fresnelFade);
      this.material.SetFloat("_FresnelFadePower", this.settings.intensitySettings.fresnelFadePower);
      Matrix4x4 projectionMatrix = this.camera_.projectionMatrix;
      Vector4 vector4 = new Vector4((float) (-2.0 / ((double) pixelWidth * (double) projectionMatrix[0])), (float) (-2.0 / ((double) pixelHeight * (double) projectionMatrix[5])), (1f - projectionMatrix[2]) / projectionMatrix[0], (1f + projectionMatrix[6]) / projectionMatrix[5]);
      Vector3 vector3 = float.IsPositiveInfinity(this.camera_.farClipPlane) ? new Vector3(this.camera_.nearClipPlane, -1f, 1f) : new Vector3(this.camera_.nearClipPlane * this.camera_.farClipPlane, this.camera_.nearClipPlane - this.camera_.farClipPlane, this.camera_.farClipPlane);
      this.material.SetVector("_ReflectionBufferSize", (Vector4) new Vector2((float) num2, (float) num3));
      this.material.SetVector("_ScreenSize", (Vector4) new Vector2(pixelWidth, pixelHeight));
      this.material.SetVector("_InvScreenSize", (Vector4) new Vector2(1f / pixelWidth, 1f / pixelHeight));
      this.material.SetVector("_ProjInfo", vector4);
      this.material.SetVector("_CameraClipInfo", (Vector4) vector3);
      Matrix4x4 matrix4x4 = new Matrix4x4();
      matrix4x4.SetRow(0, new Vector4(num4, 0.0f, 0.0f, num4));
      matrix4x4.SetRow(1, new Vector4(0.0f, num5, 0.0f, num5));
      matrix4x4.SetRow(2, new Vector4(0.0f, 0.0f, 1f, 0.0f));
      matrix4x4.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1f));
      this.material.SetMatrix("_ProjectToPixelMatrix", matrix4x4 * projectionMatrix);
      this.material.SetMatrix("_WorldToCameraMatrix", this.camera_.worldToCameraMatrix);
      this.material.SetMatrix("_CameraToWorldMatrix", this.camera_.worldToCameraMatrix.inverse);
      if (this.m_CommandBuffer != null)
        return;
      this.m_CommandBuffer = new CommandBuffer();
      this.m_CommandBuffer.name = "Screen Space Reflections";
      this.m_CommandBuffer.GetTemporaryRT(ScreenSpaceReflection.kNormalAndRoughnessTexture, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
      this.m_CommandBuffer.GetTemporaryRT(ScreenSpaceReflection.kHitPointTexture, num2, num3, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
      for (int index = 0; index < 5; ++index)
        this.m_CommandBuffer.GetTemporaryRT(ScreenSpaceReflection.kReflectionTextures[index], num2 >> index, num3 >> index, 0, FilterMode.Bilinear, format);
      this.m_CommandBuffer.GetTemporaryRT(ScreenSpaceReflection.kFilteredReflections, num2, num3, 0, this.bilateralUpsample ? FilterMode.Point : FilterMode.Bilinear, format);
      this.m_CommandBuffer.GetTemporaryRT(ScreenSpaceReflection.kFinalReflectionTexture, num2, num3, 0, FilterMode.Point, format);
      this.m_CommandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) ScreenSpaceReflection.kNormalAndRoughnessTexture, this.material, 6);
      this.m_CommandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) ScreenSpaceReflection.kHitPointTexture, this.material, 0);
      this.m_CommandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) ScreenSpaceReflection.kFilteredReflections, this.material, 5);
      this.m_CommandBuffer.Blit((RenderTargetIdentifier) ScreenSpaceReflection.kFilteredReflections, (RenderTargetIdentifier) ScreenSpaceReflection.kReflectionTextures[0], this.material, 8);
      for (int index = 1; index < 5; ++index)
      {
        int reflectionTexture1 = ScreenSpaceReflection.kReflectionTextures[index - 1];
        int num6 = index;
        this.m_CommandBuffer.GetTemporaryRT(ScreenSpaceReflection.kBlurTexture, num2 >> num6, num3 >> num6, 0, FilterMode.Bilinear, format);
        this.m_CommandBuffer.SetGlobalVector("_Axis", new Vector4(1f, 0.0f, 0.0f, 0.0f));
        this.m_CommandBuffer.SetGlobalFloat("_CurrentMipLevel", (float) index - 1f);
        this.m_CommandBuffer.Blit((RenderTargetIdentifier) reflectionTexture1, (RenderTargetIdentifier) ScreenSpaceReflection.kBlurTexture, this.material, 2);
        this.m_CommandBuffer.SetGlobalVector("_Axis", new Vector4(0.0f, 1f, 0.0f, 0.0f));
        int reflectionTexture2 = ScreenSpaceReflection.kReflectionTextures[index];
        this.m_CommandBuffer.Blit((RenderTargetIdentifier) ScreenSpaceReflection.kBlurTexture, (RenderTargetIdentifier) reflectionTexture2, this.material, 2);
        this.m_CommandBuffer.ReleaseTemporaryRT(ScreenSpaceReflection.kBlurTexture);
      }
      this.m_CommandBuffer.Blit((RenderTargetIdentifier) ScreenSpaceReflection.kReflectionTextures[0], (RenderTargetIdentifier) ScreenSpaceReflection.kFinalReflectionTexture, this.material, 3);
      this.m_CommandBuffer.GetTemporaryRT(ScreenSpaceReflection.kTempTexture, this.camera_.pixelWidth, this.camera_.pixelHeight, 0, FilterMode.Bilinear, format);
      this.m_CommandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) ScreenSpaceReflection.kTempTexture, this.material, 1);
      this.m_CommandBuffer.Blit((RenderTargetIdentifier) ScreenSpaceReflection.kTempTexture, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget);
      this.m_CommandBuffer.ReleaseTemporaryRT(ScreenSpaceReflection.kTempTexture);
      this.camera_.AddCommandBuffer(CameraEvent.AfterFinalPass, this.m_CommandBuffer);
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
      [ScreenSpaceReflection.SSRSettings.Layout]
      public ScreenSpaceReflection.ReflectionSettings reflectionSettings;
      [ScreenSpaceReflection.SSRSettings.Layout]
      public ScreenSpaceReflection.IntensitySettings intensitySettings;
      [ScreenSpaceReflection.SSRSettings.Layout]
      public ScreenSpaceReflection.ScreenEdgeMask screenEdgeMask;
      private static readonly ScreenSpaceReflection.SSRSettings s_Default = new ScreenSpaceReflection.SSRSettings()
      {
        reflectionSettings = new ScreenSpaceReflection.ReflectionSettings()
        {
          blendType = ScreenSpaceReflection.SSRReflectionBlendType.PhysicallyBased,
          reflectionQuality = ScreenSpaceReflection.SSRResolution.High,
          maxDistance = 100f,
          iterationCount = 256,
          stepSize = 3,
          widthModifier = 0.5f,
          reflectionBlur = 1f,
          reflectBackfaces = true
        },
        intensitySettings = new ScreenSpaceReflection.IntensitySettings()
        {
          reflectionMultiplier = 1f,
          fadeDistance = 100f,
          fresnelFade = 1f,
          fresnelFadePower = 1f
        },
        screenEdgeMask = new ScreenSpaceReflection.ScreenEdgeMask()
        {
          intensity = 0.03f
        }
      };

      public static ScreenSpaceReflection.SSRSettings defaultSettings
      {
        get => ScreenSpaceReflection.SSRSettings.s_Default;
      }

      [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
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
      public ScreenSpaceReflection.SSRReflectionBlendType blendType;
      [Tooltip("Half resolution SSRR is much faster, but less accurate.")]
      public ScreenSpaceReflection.SSRResolution reflectionQuality;
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
