[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
[AddComponentMenu("Time of Day/Camera Cloud Shadows")]
public class TOD_Shadows : TOD_ImageEffect
{
  public Shader ShadowShader = (Shader) null;
  public Texture2D CloudTexture = (Texture2D) null;
  [Range(0.0f, 1f)]
  public float Cutoff = 0.0f;
  [Range(0.0f, 1f)]
  public float Fade = 0.0f;
  [Range(0.0f, 1f)]
  public float Intensity = 0.5f;
  private Material shadowMaterial = (Material) null;

  protected void OnEnable()
  {
    if (!(bool) (Object) ShadowShader)
      ShadowShader = Shader.Find("Hidden/Time of Day/Cloud Shadows");
    shadowMaterial = CreateMaterial(ShadowShader);
  }

  protected void OnDisable()
  {
    if (!(bool) (Object) shadowMaterial)
      return;
    Object.DestroyImmediate((Object) shadowMaterial);
  }

  [ImageEffectOpaque]
  protected void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (!CheckSupport(true))
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      sky.Components.Shadows = this;
      shadowMaterial.SetMatrix("_FrustumCornersWS", FrustumCorners());
      shadowMaterial.SetTexture("_CloudTex", (Texture) CloudTexture);
      shadowMaterial.SetFloat("_Cutoff", Cutoff);
      shadowMaterial.SetFloat("_Fade", Fade);
      shadowMaterial.SetFloat("_Intensity", Intensity * Mathf.Clamp01((float) (1.0 - sky.SunZenith / 90.0)));
      CustomBlit(source, destination, shadowMaterial);
    }
  }
}
