using UnityEngine;

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
    if (!(bool) (Object) this.ShadowShader)
      this.ShadowShader = Shader.Find("Hidden/Time of Day/Cloud Shadows");
    this.shadowMaterial = this.CreateMaterial(this.ShadowShader);
  }

  protected void OnDisable()
  {
    if (!(bool) (Object) this.shadowMaterial)
      return;
    Object.DestroyImmediate((Object) this.shadowMaterial);
  }

  [ImageEffectOpaque]
  protected void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (!this.CheckSupport(true))
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      this.sky.Components.Shadows = this;
      this.shadowMaterial.SetMatrix("_FrustumCornersWS", this.FrustumCorners());
      this.shadowMaterial.SetTexture("_CloudTex", (Texture) this.CloudTexture);
      this.shadowMaterial.SetFloat("_Cutoff", this.Cutoff);
      this.shadowMaterial.SetFloat("_Fade", this.Fade);
      this.shadowMaterial.SetFloat("_Intensity", this.Intensity * Mathf.Clamp01((float) (1.0 - (double) this.sky.SunZenith / 90.0)));
      this.CustomBlit(source, destination, this.shadowMaterial);
    }
  }
}
