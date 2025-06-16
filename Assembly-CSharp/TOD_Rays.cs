using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
[AddComponentMenu("Time of Day/Camera God Rays")]
public class TOD_Rays : TOD_ImageEffect
{
  public Shader GodRayShader = (Shader) null;
  public Shader ScreenClearShader = (Shader) null;
  [Tooltip("The god ray rendering resolution.")]
  public TOD_Rays.ResolutionType Resolution = TOD_Rays.ResolutionType.Normal;
  [Tooltip("The god ray rendering blend mode.")]
  public TOD_Rays.BlendModeType BlendMode = TOD_Rays.BlendModeType.Screen;
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
    if (!(bool) (Object) this.GodRayShader)
      this.GodRayShader = Shader.Find("Hidden/Time of Day/God Rays");
    if (!(bool) (Object) this.ScreenClearShader)
      this.ScreenClearShader = Shader.Find("Hidden/Time of Day/Screen Clear");
    this.godRayMaterial = this.CreateMaterial(this.GodRayShader);
    this.screenClearMaterial = this.CreateMaterial(this.ScreenClearShader);
  }

  protected void OnDisable()
  {
    if ((bool) (Object) this.godRayMaterial)
      Object.DestroyImmediate((Object) this.godRayMaterial);
    if (!(bool) (Object) this.screenClearMaterial)
      return;
    Object.DestroyImmediate((Object) this.screenClearMaterial);
  }

  protected void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (!this.CheckSupport(this.UseDepthTexture))
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      this.sky.Components.Rays = this;
      int width;
      int height;
      int depthBuffer;
      if (this.Resolution == TOD_Rays.ResolutionType.High)
      {
        width = source.width;
        height = source.height;
        depthBuffer = 0;
      }
      else if (this.Resolution == TOD_Rays.ResolutionType.Normal)
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
      Vector3 viewportPoint = this.cam.WorldToViewportPoint(this.sky.Components.LightTransform.position);
      this.godRayMaterial.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0.0f, 0.0f) * this.BlurRadius);
      this.godRayMaterial.SetVector("_LightPosition", new Vector4(viewportPoint.x, viewportPoint.y, viewportPoint.z, this.MaxRadius));
      RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, depthBuffer);
      if (this.UseDepthTexture)
        Graphics.Blit((Texture) source, temporary1, this.godRayMaterial, 2);
      else
        Graphics.Blit((Texture) source, temporary1, this.godRayMaterial, 3);
      this.DrawBorder(temporary1, this.screenClearMaterial);
      float num1 = this.BlurRadius * (1f / 768f);
      this.godRayMaterial.SetVector("_BlurRadius4", new Vector4(num1, num1, 0.0f, 0.0f));
      this.godRayMaterial.SetVector("_LightPosition", new Vector4(viewportPoint.x, viewportPoint.y, viewportPoint.z, this.MaxRadius));
      for (int index = 0; index < this.BlurIterations; ++index)
      {
        RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, depthBuffer);
        Graphics.Blit((Texture) temporary1, temporary2, this.godRayMaterial, 1);
        RenderTexture.ReleaseTemporary(temporary1);
        float num2 = (float) ((double) this.BlurRadius * (((double) index * 2.0 + 1.0) * 6.0) / 768.0);
        this.godRayMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, 0.0f, 0.0f));
        temporary1 = RenderTexture.GetTemporary(width, height, depthBuffer);
        Graphics.Blit((Texture) temporary2, temporary1, this.godRayMaterial, 1);
        RenderTexture.ReleaseTemporary(temporary2);
        float num3 = (float) ((double) this.BlurRadius * (((double) index * 2.0 + 2.0) * 6.0) / 768.0);
        this.godRayMaterial.SetVector("_BlurRadius4", new Vector4(num3, num3, 0.0f, 0.0f));
      }
      Color color = Color.black;
      if ((double) viewportPoint.z >= 0.0)
        color = !this.sky.IsDay ? this.Intensity * this.sky.MoonVisibility * this.sky.MoonRayColor : this.Intensity * this.sky.SunVisibility * this.sky.SunRayColor;
      this.godRayMaterial.SetColor("_LightColor", color);
      this.godRayMaterial.SetTexture("_ColorBuffer", (Texture) temporary1);
      if (this.BlendMode == TOD_Rays.BlendModeType.Screen)
        Graphics.Blit((Texture) source, destination, this.godRayMaterial, 0);
      else
        Graphics.Blit((Texture) source, destination, this.godRayMaterial, 4);
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
