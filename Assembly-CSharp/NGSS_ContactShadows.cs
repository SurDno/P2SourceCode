using UnityEngine;
using UnityEngine.Rendering;

[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class NGSS_ContactShadows : MonoBehaviour
{
  public Shader contactShadowsShader;
  [Header("SHADOWS SETTINGS")]
  [Tooltip("Poisson Noise. Randomize samples to remove repeated patterns.")]
  public bool m_noiseFilter = false;
  [Tooltip("Tweak this value to remove soft-shadows leaking around edges.")]
  [Range(0.01f, 1f)]
  public float m_shadowsEdgeTolerance = 0.25f;
  [Tooltip("Overall softness of the shadows.")]
  [Range(0.01f, 1f)]
  public float m_shadowsSoftness = 0.25f;
  [Tooltip("Overall distance of the shadows.")]
  [Range(1f, 4f)]
  public float m_shadowsDistance = 1f;
  [Tooltip("The distance where shadows start to fade.")]
  [Range(0.1f, 4f)]
  public float m_shadowsFade = 1f;
  [Tooltip("Tweak this value if your objects display backface shadows.")]
  [Range(0.0f, 2f)]
  public float m_shadowsOffset = 0.325f;
  [Header("RAY SETTINGS")]
  [Tooltip("The higher the value, the ticker the shadows will look.")]
  [Range(0.0f, 1f)]
  public float m_rayWidth = 0.1f;
  [Tooltip("Number of samplers between each step. The higher values produces less gaps between shadows. Keep this value as low as you can!")]
  [Range(16f, 128f)]
  public int m_raySamples = 64;
  [Tooltip("Samplers scale over distance. Lower this value if you want to speed things up by doing less sampling on far away objects.")]
  [Range(0.0f, 1f)]
  public float m_raySamplesScale = 1f;
  private CommandBuffer blendShadowsCB;
  private CommandBuffer computeShadowsCB;
  private bool isInitialized = false;
  private Camera _mCamera;
  private Material _mMaterial;

  private Camera mCamera
  {
    get
    {
      if ((Object) this._mCamera == (Object) null)
      {
        this._mCamera = this.GetComponent<Camera>();
        if ((Object) this._mCamera == (Object) null)
          this._mCamera = Camera.main;
        if ((Object) this._mCamera == (Object) null)
          Debug.LogError((object) "NGSS Error: No MainCamera found, please provide one.", (Object) this);
        else
          this._mCamera.depthTextureMode |= DepthTextureMode.Depth;
      }
      return this._mCamera;
    }
  }

  private Material mMaterial
  {
    get
    {
      if ((Object) this._mMaterial == (Object) null)
      {
        if ((Object) this.contactShadowsShader != (Object) null)
          this._mMaterial = new Material(this.contactShadowsShader);
        if ((Object) this._mMaterial == (Object) null)
        {
          Debug.LogWarning((object) "NGSS Warning: can't find NGSS_ContactShadows shader, make sure it's on your project.", (Object) this);
          this.enabled = false;
          return (Material) null;
        }
      }
      return this._mMaterial;
    }
  }

  private void AddCommandBuffers()
  {
    this.computeShadowsCB = new CommandBuffer()
    {
      name = "NGSS ContactShadows: Compute"
    };
    this.blendShadowsCB = new CommandBuffer()
    {
      name = "NGSS ContactShadows: Mix"
    };
    bool flag = this.mCamera.actualRenderingPath == RenderingPath.Forward;
    if ((bool) (Object) this.mCamera)
    {
      foreach (CommandBuffer commandBuffer in this.mCamera.GetCommandBuffers(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting))
      {
        if (commandBuffer.name == this.computeShadowsCB.name)
          return;
      }
      this.mCamera.AddCommandBuffer(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting, this.computeShadowsCB);
    }
    Light light = MonoBehaviourInstance<NGSS_ContactShadowsSource>.Instance?.Light;
    if (!(bool) (Object) light)
      return;
    foreach (CommandBuffer commandBuffer in light.GetCommandBuffers(LightEvent.AfterScreenspaceMask))
    {
      if (commandBuffer.name == this.blendShadowsCB.name)
        return;
    }
    light.AddCommandBuffer(LightEvent.AfterScreenspaceMask, this.blendShadowsCB);
  }

  private void Deinitialize()
  {
    if (!this.isInitialized)
      return;
    this._mMaterial = (Material) null;
    bool flag = this.mCamera.actualRenderingPath == RenderingPath.Forward;
    if ((bool) (Object) this.mCamera)
      this.mCamera.RemoveCommandBuffer(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting, this.computeShadowsCB);
    Light light = MonoBehaviourInstance<NGSS_ContactShadowsSource>.Instance?.Light;
    if ((bool) (Object) light)
      light.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, this.blendShadowsCB);
    this.isInitialized = false;
  }

  private void Initialize()
  {
    if (this.isInitialized)
      return;
    Light light = MonoBehaviourInstance<NGSS_ContactShadowsSource>.Instance?.Light;
    if ((Object) light == (Object) null || !light.isActiveAndEnabled)
      return;
    if (this.mCamera.actualRenderingPath == RenderingPath.VertexLit)
    {
      Debug.LogWarning((object) "Vertex Lit Rendering Path is not supported by NGSS Contact Shadows. Please set the Rendering Path in your game camera or Graphics Settings to something else than Vertex Lit.", (Object) this);
      this.enabled = false;
    }
    else
    {
      this.AddCommandBuffers();
      int id1 = Shader.PropertyToID("NGSS_ContactShadowRT");
      int id2 = Shader.PropertyToID("NGSS_ContactShadowRT2");
      int id3 = Shader.PropertyToID("NGSS_DepthSourceRT");
      this.computeShadowsCB.GetTemporaryRT(id1, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
      this.computeShadowsCB.GetTemporaryRT(id2, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
      this.computeShadowsCB.GetTemporaryRT(id3, -1, -1, 0, FilterMode.Point, RenderTextureFormat.RFloat);
      this.computeShadowsCB.Blit((RenderTargetIdentifier) id1, (RenderTargetIdentifier) id3, this.mMaterial, 0);
      this.computeShadowsCB.Blit((RenderTargetIdentifier) id3, (RenderTargetIdentifier) id1, this.mMaterial, 1);
      this.computeShadowsCB.SetGlobalVector("ShadowsKernel", (Vector4) new Vector2(0.0f, 1f));
      this.computeShadowsCB.Blit((RenderTargetIdentifier) id1, (RenderTargetIdentifier) id2, this.mMaterial, 2);
      this.computeShadowsCB.SetGlobalVector("ShadowsKernel", (Vector4) new Vector2(1f, 0.0f));
      this.computeShadowsCB.Blit((RenderTargetIdentifier) id2, (RenderTargetIdentifier) id1, this.mMaterial, 2);
      this.computeShadowsCB.SetGlobalVector("ShadowsKernel", (Vector4) new Vector2(0.0f, 2f));
      this.computeShadowsCB.Blit((RenderTargetIdentifier) id1, (RenderTargetIdentifier) id2, this.mMaterial, 2);
      this.computeShadowsCB.SetGlobalVector("ShadowsKernel", (Vector4) new Vector2(2f, 0.0f));
      this.computeShadowsCB.Blit((RenderTargetIdentifier) id2, (RenderTargetIdentifier) id1, this.mMaterial, 2);
      this.computeShadowsCB.SetGlobalTexture("NGSS_ContactShadowsTexture", (RenderTargetIdentifier) id1);
      this.blendShadowsCB.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CurrentActive, (RenderTargetIdentifier) BuiltinRenderTextureType.CurrentActive, this.mMaterial, 3);
      this.isInitialized = true;
    }
  }

  private bool IsNotSupported()
  {
    return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStationVita || SystemInfo.graphicsDeviceType == GraphicsDeviceType.N3DS;
  }

  private void OnEnable()
  {
    if (this.IsNotSupported())
    {
      Debug.LogWarning((object) "Unsupported graphics API, NGSS requires at least SM3.0 or higher and DX9 is not supported.", (Object) this);
      this.enabled = false;
    }
    else
      this.Initialize();
  }

  private void OnDisable() => this.Deinitialize();

  private void OnApplicationQuit() => this.Deinitialize();

  private void OnPreRender()
  {
    Light light = MonoBehaviourInstance<NGSS_ContactShadowsSource>.Instance?.Light;
    if (this.isInitialized)
    {
      if ((Object) light == (Object) null || !light.isActiveAndEnabled)
      {
        this.Deinitialize();
        return;
      }
    }
    else
    {
      this.Initialize();
      if (!this.isInitialized)
        return;
    }
    this.mMaterial.SetVector("LightDir", (Vector4) this.mCamera.transform.InverseTransformDirection(light.transform.forward));
    this.mMaterial.SetFloat("ShadowsOpacity", 1f - light.shadowStrength);
    this.mMaterial.SetFloat("ShadowsEdgeTolerance", this.m_shadowsEdgeTolerance * 0.075f);
    this.mMaterial.SetFloat("ShadowsSoftness", this.m_shadowsSoftness * 4f);
    this.mMaterial.SetFloat("ShadowsDistance", this.m_shadowsDistance);
    this.mMaterial.SetFloat("ShadowsFade", this.m_shadowsFade);
    this.mMaterial.SetFloat("ShadowsBias", this.m_shadowsOffset * 0.02f);
    this.mMaterial.SetFloat("RayWidth", this.m_rayWidth);
    this.mMaterial.SetFloat("RaySamples", (float) this.m_raySamples);
    this.mMaterial.SetFloat("RaySamplesScale", this.m_raySamplesScale);
    if (this.m_noiseFilter)
      this.mMaterial.EnableKeyword("NGSS_CONTACT_SHADOWS_USE_NOISE");
    else
      this.mMaterial.DisableKeyword("NGSS_CONTACT_SHADOWS_USE_NOISE");
  }
}
