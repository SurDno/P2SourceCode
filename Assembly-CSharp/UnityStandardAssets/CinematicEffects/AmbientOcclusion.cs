using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Cinematic/Ambient Occlusion")]
  [ImageEffectAllowedInSceneView]
  public class AmbientOcclusion : MonoBehaviour
  {
    [SerializeField]
    public AmbientOcclusion.Settings settings = AmbientOcclusion.Settings.defaultSettings;
    [SerializeField]
    private Shader _aoShader;
    private Material _aoMaterial;
    private CommandBuffer _aoCommands;
    [SerializeField]
    private Mesh _quadMesh;

    public bool isAmbientOnlySupported
    {
      get
      {
        return this.targetCamera.allowHDR && this.occlusionSource == AmbientOcclusion.OcclusionSource.GBuffer;
      }
    }

    public bool isGBufferAvailable
    {
      get => this.targetCamera.actualRenderingPath == RenderingPath.DeferredShading;
    }

    private float intensity => this.settings.intensity;

    private float radius => Mathf.Max(this.settings.radius, 0.0001f);

    private AmbientOcclusion.SampleCount sampleCount => this.settings.sampleCount;

    private int sampleCountValue
    {
      get
      {
        switch (this.settings.sampleCount)
        {
          case AmbientOcclusion.SampleCount.Lowest:
            return 3;
          case AmbientOcclusion.SampleCount.Low:
            return 6;
          case AmbientOcclusion.SampleCount.Medium:
            return 12;
          case AmbientOcclusion.SampleCount.High:
            return 20;
          default:
            return Mathf.Clamp(this.settings.sampleCountValue, 1, 256);
        }
      }
    }

    private AmbientOcclusion.OcclusionSource occlusionSource
    {
      get
      {
        return this.settings.occlusionSource == AmbientOcclusion.OcclusionSource.GBuffer && !this.isGBufferAvailable ? AmbientOcclusion.OcclusionSource.DepthNormalsTexture : this.settings.occlusionSource;
      }
    }

    private bool downsampling => this.settings.downsampling;

    private bool ambientOnly
    {
      get => this.settings.ambientOnly && !this.settings.debug && this.isAmbientOnlySupported;
    }

    private RenderTextureFormat aoTextureFormat
    {
      get
      {
        return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8) ? RenderTextureFormat.R8 : RenderTextureFormat.Default;
      }
    }

    private Shader aoShader
    {
      get
      {
        if ((UnityEngine.Object) this._aoShader == (UnityEngine.Object) null)
          this._aoShader = Shader.Find("Hidden/Image Effects/Cinematic/AmbientOcclusion");
        return this._aoShader;
      }
    }

    private Material aoMaterial
    {
      get
      {
        if ((UnityEngine.Object) this._aoMaterial == (UnityEngine.Object) null)
          this._aoMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(this.aoShader);
        return this._aoMaterial;
      }
    }

    private CommandBuffer aoCommands
    {
      get
      {
        if (this._aoCommands == null)
        {
          this._aoCommands = new CommandBuffer();
          this._aoCommands.name = nameof (AmbientOcclusion);
        }
        return this._aoCommands;
      }
    }

    private Camera targetCamera => this.GetComponent<Camera>();

    private AmbientOcclusion.PropertyObserver propertyObserver { get; set; }

    private Mesh quadMesh => this._quadMesh;

    private void BuildAOCommands()
    {
      CommandBuffer aoCommands = this.aoCommands;
      int pixelWidth = this.targetCamera.pixelWidth;
      int pixelHeight = this.targetCamera.pixelHeight;
      int num = this.downsampling ? 2 : 1;
      RenderTextureFormat aoTextureFormat = this.aoTextureFormat;
      RenderTextureReadWrite readWrite = RenderTextureReadWrite.Linear;
      FilterMode filter = FilterMode.Bilinear;
      Material aoMaterial = this.aoMaterial;
      int id1 = Shader.PropertyToID("_OcclusionTexture");
      aoCommands.GetTemporaryRT(id1, pixelWidth / num, pixelHeight / num, 0, filter, aoTextureFormat, readWrite);
      aoCommands.Blit((Texture) null, (RenderTargetIdentifier) id1, aoMaterial, 2);
      int id2 = Shader.PropertyToID("_OcclusionBlurTexture");
      aoCommands.GetTemporaryRT(id2, pixelWidth, pixelHeight, 0, filter, aoTextureFormat, readWrite);
      aoCommands.SetGlobalVector("_BlurVector", (Vector4) (Vector2.right * 2f));
      aoCommands.Blit((RenderTargetIdentifier) id1, (RenderTargetIdentifier) id2, aoMaterial, 4);
      aoCommands.ReleaseTemporaryRT(id1);
      aoCommands.GetTemporaryRT(id1, pixelWidth, pixelHeight, 0, filter, aoTextureFormat, readWrite);
      aoCommands.SetGlobalVector("_BlurVector", (Vector4) (Vector2.up * 2f * (float) num));
      aoCommands.Blit((RenderTargetIdentifier) id2, (RenderTargetIdentifier) id1, aoMaterial, 4);
      aoCommands.ReleaseTemporaryRT(id2);
      aoCommands.GetTemporaryRT(id2, pixelWidth, pixelHeight, 0, filter, aoTextureFormat, readWrite);
      aoCommands.SetGlobalVector("_BlurVector", (Vector4) (Vector2.right * (float) num));
      aoCommands.Blit((RenderTargetIdentifier) id1, (RenderTargetIdentifier) id2, aoMaterial, 6);
      aoCommands.ReleaseTemporaryRT(id1);
      aoCommands.GetTemporaryRT(id1, pixelWidth, pixelHeight, 0, filter, aoTextureFormat, readWrite);
      aoCommands.SetGlobalVector("_BlurVector", (Vector4) (Vector2.up * (float) num));
      aoCommands.Blit((RenderTargetIdentifier) id2, (RenderTargetIdentifier) id1, aoMaterial, 6);
      aoCommands.ReleaseTemporaryRT(id2);
      RenderTargetIdentifier[] colors = new RenderTargetIdentifier[2]
      {
        (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer0,
        (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget
      };
      aoCommands.SetRenderTarget(colors, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget);
      aoCommands.SetGlobalTexture("_OcclusionTexture", (RenderTargetIdentifier) id1);
      aoCommands.DrawMesh(this.quadMesh, Matrix4x4.identity, aoMaterial, 0, 8);
      aoCommands.ReleaseTemporaryRT(id1);
    }

    private void ExecuteAOPass(RenderTexture source, RenderTexture destination)
    {
      int width = source.width;
      int height = source.height;
      int num = this.downsampling ? 2 : 1;
      RenderTextureFormat aoTextureFormat = this.aoTextureFormat;
      RenderTextureReadWrite readWrite = RenderTextureReadWrite.Linear;
      bool flag = this.settings.occlusionSource == AmbientOcclusion.OcclusionSource.GBuffer;
      Material aoMaterial = this.aoMaterial;
      RenderTexture temporary1 = RenderTexture.GetTemporary(width / num, height / num, 0, aoTextureFormat, readWrite);
      Graphics.Blit((Texture) null, temporary1, aoMaterial, (int) this.occlusionSource);
      RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, aoTextureFormat, readWrite);
      aoMaterial.SetVector("_BlurVector", (Vector4) (Vector2.right * 2f));
      Graphics.Blit((Texture) temporary1, temporary2, aoMaterial, flag ? 4 : 3);
      RenderTexture.ReleaseTemporary(temporary1);
      RenderTexture temporary3 = RenderTexture.GetTemporary(width, height, 0, aoTextureFormat, readWrite);
      aoMaterial.SetVector("_BlurVector", (Vector4) (Vector2.up * 2f * (float) num));
      Graphics.Blit((Texture) temporary2, temporary3, aoMaterial, flag ? 4 : 3);
      RenderTexture.ReleaseTemporary(temporary2);
      RenderTexture temporary4 = RenderTexture.GetTemporary(width, height, 0, aoTextureFormat, readWrite);
      aoMaterial.SetVector("_BlurVector", (Vector4) (Vector2.right * (float) num));
      Graphics.Blit((Texture) temporary3, temporary4, aoMaterial, flag ? 6 : 5);
      RenderTexture.ReleaseTemporary(temporary3);
      RenderTexture temporary5 = RenderTexture.GetTemporary(width, height, 0, aoTextureFormat, readWrite);
      aoMaterial.SetVector("_BlurVector", (Vector4) (Vector2.up * (float) num));
      Graphics.Blit((Texture) temporary4, temporary5, aoMaterial, flag ? 6 : 5);
      RenderTexture.ReleaseTemporary(temporary4);
      aoMaterial.SetTexture("_OcclusionTexture", (Texture) temporary5);
      if (!this.settings.debug)
        Graphics.Blit((Texture) source, destination, aoMaterial, 7);
      else
        Graphics.Blit((Texture) source, destination, aoMaterial, 9);
      RenderTexture.ReleaseTemporary(temporary5);
    }

    private void UpdateMaterialProperties()
    {
      Material aoMaterial = this.aoMaterial;
      aoMaterial.SetFloat("_Intensity", this.intensity);
      aoMaterial.SetFloat("_Radius", this.radius);
      aoMaterial.SetFloat("_TargetScale", this.downsampling ? 0.5f : 1f);
      aoMaterial.SetInt("_SampleCount", this.sampleCountValue);
    }

    private void OnEnable()
    {
      if (!ImageEffectHelper.IsSupported(this.aoShader, true, false, (MonoBehaviour) this))
      {
        this.enabled = false;
      }
      else
      {
        if (this.ambientOnly)
          this.targetCamera.AddCommandBuffer(CameraEvent.BeforeReflections, this.aoCommands);
        if (this.occlusionSource == AmbientOcclusion.OcclusionSource.DepthTexture)
          this.targetCamera.depthTextureMode |= DepthTextureMode.Depth;
        if (this.occlusionSource == AmbientOcclusion.OcclusionSource.GBuffer)
          return;
        this.targetCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
      }
    }

    private void OnDisable()
    {
      if ((UnityEngine.Object) this._aoMaterial != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._aoMaterial);
      this._aoMaterial = (Material) null;
      if (this._aoCommands != null)
        this.targetCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, this._aoCommands);
      this._aoCommands = (CommandBuffer) null;
    }

    private void Update()
    {
      if (this.propertyObserver.CheckNeedsReset(this.settings, this.targetCamera))
      {
        this.OnDisable();
        this.OnEnable();
        if (this.ambientOnly)
        {
          this.aoCommands.Clear();
          this.BuildAOCommands();
        }
        this.propertyObserver.Update(this.settings, this.targetCamera);
      }
      if (!this.ambientOnly)
        return;
      this.UpdateMaterialProperties();
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (this.ambientOnly)
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        this.UpdateMaterialProperties();
        this.ExecuteAOPass(source, destination);
      }
    }

    private struct PropertyObserver
    {
      private bool _downsampling;
      private AmbientOcclusion.OcclusionSource _occlusionSource;
      private bool _ambientOnly;
      private bool _debug;
      private int _pixelWidth;
      private int _pixelHeight;

      public bool CheckNeedsReset(AmbientOcclusion.Settings setting, Camera camera)
      {
        return this._downsampling != setting.downsampling || this._occlusionSource != setting.occlusionSource || this._ambientOnly != setting.ambientOnly || this._debug != setting.debug || this._pixelWidth != camera.pixelWidth || this._pixelHeight != camera.pixelHeight;
      }

      public void Update(AmbientOcclusion.Settings setting, Camera camera)
      {
        this._downsampling = setting.downsampling;
        this._occlusionSource = setting.occlusionSource;
        this._ambientOnly = setting.ambientOnly;
        this._debug = setting.debug;
        this._pixelWidth = camera.pixelWidth;
        this._pixelHeight = camera.pixelHeight;
      }
    }

    public enum SampleCount
    {
      Lowest,
      Low,
      Medium,
      High,
      Variable,
    }

    public enum OcclusionSource
    {
      DepthTexture,
      DepthNormalsTexture,
      GBuffer,
    }

    [Serializable]
    public class Settings
    {
      [SerializeField]
      [Range(0.0f, 4f)]
      [Tooltip("Degree of darkness produced by the effect.")]
      public float intensity;
      [SerializeField]
      [Tooltip("Radius of sample points, which affects extent of darkened areas.")]
      public float radius;
      [SerializeField]
      [Tooltip("Number of sample points, which affects quality and performance.")]
      public AmbientOcclusion.SampleCount sampleCount;
      [SerializeField]
      [Tooltip("Determines the sample count when SampleCount.Variable is used.")]
      public int sampleCountValue;
      [SerializeField]
      [Tooltip("Halves the resolution of the effect to increase performance.")]
      public bool downsampling;
      [SerializeField]
      [Tooltip("If checked, the effect only affects ambient lighting.")]
      public bool ambientOnly;
      [SerializeField]
      [Tooltip("Source buffer on which the occlusion estimator is based.")]
      public AmbientOcclusion.OcclusionSource occlusionSource;
      [SerializeField]
      [Tooltip("Displays occlusion for debug purpose.")]
      public bool debug;

      public static AmbientOcclusion.Settings defaultSettings
      {
        get
        {
          return new AmbientOcclusion.Settings()
          {
            intensity = 1f,
            radius = 0.3f,
            sampleCount = AmbientOcclusion.SampleCount.Medium,
            sampleCountValue = 24,
            downsampling = false,
            ambientOnly = false,
            occlusionSource = AmbientOcclusion.OcclusionSource.DepthNormalsTexture
          };
        }
      }
    }
  }
}
