[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
[AddComponentMenu("Time of Day/Camera God Rays")]
public class TOD_Rays : TOD_ImageEffect
{
  public Shader GodRayShader = (Shader) null;
  public Shader ScreenClearShader = (Shader) null;
  [Tooltip("The god ray rendering resolution.")]
  public ResolutionType Resolution = ResolutionType.Normal;
  [Tooltip("The god ray rendering blend mode.")]
  public BlendModeType BlendMode = BlendModeType.Screen;
  [Tooltip("The number of blur iterations to be performed.")]
  [TOD_Range(0.0f, 4f)]
  public int BlurIterations = 2;
  [Tooltip("The radius to blur filter applied to the god rays.")]
  [TOD_Min(0.0f)]
  public float BlurRadius = 2f;
  [Tooltip("The intensity of the god rays.")]
  [TOD_Min(0.0f)]
  public float Intensity = 1f;
  [Tooltip("The maximum radius of the god rays.")]
  [TOD_Min(0.0f)]
  public float MaxRadius = 0.5f;
  [Tooltip("Whether or not to use the depth buffer.")]
  public bool UseDepthTexture = true;
  private Material godRayMaterial = (Material) null;
  private Material screenClearMaterial = (Material) null;
  private const int PASS_DEPTH = 2;
  private const int PASS_NODEPTH = 3;
  private const int PASS_RADIAL = 1;
  private const int PASS_SCREEN = 0;
  private const int PASS_ADD = 4;

  protected void OnEnable()
  {
    if (!(bool) (Object) GodRayShader)
      GodRayShader = Shader.Find("Hidden/Time of Day/God Rays");
    if (!(bool) (Object) ScreenClearShader)
      ScreenClearShader = Shader.Find("Hidden/Time of Day/Screen Clear");
    godRayMaterial = CreateMaterial(GodRayShader);
    screenClearMaterial = CreateMaterial(ScreenClearShader);
  }

  protected void OnDisable()
  {
    if ((bool) (Object) godRayMaterial)
      Object.DestroyImmediate((Object) godRayMaterial);
    if (!(bool) (Object) screenClearMaterial)
      return;
    Object.DestroyImmediate((Object) screenClearMaterial);
  }

  protected void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (!CheckSupport(UseDepthTexture))
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      sky.Components.Rays = this;
      int width;
      int height;
      int depthBuffer;
      if (Resolution == ResolutionType.High)
      {
        width = source.width;
        height = source.height;
        depthBuffer = 0;
      }
      else if (Resolution == ResolutionType.Normal)
      {
        width = source.width / 2;
        height = source.height / 2;
        depthBuffer = 0;
      }
      else
      {
        width = source.width / 4;
        height = source.height / 4;
        depthBuffer = 0;
      }
      Vector3 viewportPoint = cam.WorldToViewportPoint(sky.Components.LightTransform.position);
      godRayMaterial.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0.0f, 0.0f) * BlurRadius);
      godRayMaterial.SetVector("_LightPosition", new Vector4(viewportPoint.x, viewportPoint.y, viewportPoint.z, MaxRadius));
      RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, depthBuffer);
      if (UseDepthTexture)
        Graphics.Blit((Texture) source, temporary1, godRayMaterial, 2);
      else
        Graphics.Blit((Texture) source, temporary1, godRayMaterial, 3);
      DrawBorder(temporary1, screenClearMaterial);
      float num1 = BlurRadius * (1f / 768f);
      godRayMaterial.SetVector("_BlurRadius4", new Vector4(num1, num1, 0.0f, 0.0f));
      godRayMaterial.SetVector("_LightPosition", new Vector4(viewportPoint.x, viewportPoint.y, viewportPoint.z, MaxRadius));
      for (int index = 0; index < BlurIterations; ++index)
      {
        RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, depthBuffer);
        Graphics.Blit((Texture) temporary1, temporary2, godRayMaterial, 1);
        RenderTexture.ReleaseTemporary(temporary1);
        float num2 = (float) (BlurRadius * ((index * 2.0 + 1.0) * 6.0) / 768.0);
        godRayMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, 0.0f, 0.0f));
        temporary1 = RenderTexture.GetTemporary(width, height, depthBuffer);
        Graphics.Blit((Texture) temporary2, temporary1, godRayMaterial, 1);
        RenderTexture.ReleaseTemporary(temporary2);
        float num3 = (float) (BlurRadius * ((index * 2.0 + 2.0) * 6.0) / 768.0);
        godRayMaterial.SetVector("_BlurRadius4", new Vector4(num3, num3, 0.0f, 0.0f));
      }
      Color color = Color.black;
      if ((double) viewportPoint.z >= 0.0)
        color = !sky.IsDay ? Intensity * sky.MoonVisibility * sky.MoonRayColor : Intensity * sky.SunVisibility * sky.SunRayColor;
      godRayMaterial.SetColor("_LightColor", color);
      godRayMaterial.SetTexture("_ColorBuffer", (Texture) temporary1);
      if (BlendMode == BlendModeType.Screen)
        Graphics.Blit((Texture) source, destination, godRayMaterial, 0);
      else
        Graphics.Blit((Texture) source, destination, godRayMaterial, 4);
      RenderTexture.ReleaseTemporary(temporary1);
    }
  }

  public enum ResolutionType
  {
    Low,
    Normal,
    High,
  }

  public enum BlendModeType
  {
    Screen,
    Add,
  }
}
