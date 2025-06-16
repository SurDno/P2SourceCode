using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
[AddComponentMenu("Time of Day/Camera Atmospheric Scattering")]
public class TOD_Scattering : TOD_ImageEffect
{
  public Shader ScatteringShader = (Shader) null;
  public Texture2D DitheringTexture = (Texture2D) null;
  [Range(0.0f, 1f)]
  public float GlobalDensity = 1f / 1000f;
  [Range(0.0f, 1000f)]
  public float StartDistance = 0.0f;
  [Range(0.0f, 1f)]
  public float HeightFalloff = 1f / 1000f;
  public float ZeroLevel = 0.0f;
  public float TransparentDensityMultiplier = 5f;
  private Material scatteringMaterial = (Material) null;

  protected void OnEnable()
  {
    if (!(bool) (Object) this.ScatteringShader)
      this.ScatteringShader = Shader.Find("Hidden/Time of Day/Scattering");
    this.scatteringMaterial = this.CreateMaterial(this.ScatteringShader);
  }

  protected void OnDisable()
  {
    if (!(bool) (Object) this.scatteringMaterial)
      return;
    Object.DestroyImmediate((Object) this.scatteringMaterial);
  }

  protected void OnPreCull()
  {
    if (!(bool) (Object) this.sky || !this.sky.Initialized)
      return;
    this.sky.Components.AtmosphereRenderer.enabled = false;
  }

  protected void OnPostRender()
  {
    if (!(bool) (Object) this.sky || !this.sky.Initialized)
      return;
    this.sky.Components.AtmosphereRenderer.enabled = true;
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
      this.sky.Components.Scattering = this;
      float heightFalloff = this.HeightFalloff;
      float y = Mathf.Exp((float) (-(double) heightFalloff * ((double) this.cam.transform.position.y - (double) this.ZeroLevel)));
      float globalDensity = this.GlobalDensity;
      RenderSettings.fog = true;
      RenderSettings.fogMode = FogMode.Exponential;
      RenderSettings.fogDensity = globalDensity * this.TransparentDensityMultiplier;
      this.scatteringMaterial.SetMatrix("_FrustumCornersWS", this.FrustumCorners());
      this.scatteringMaterial.SetTexture("_DitheringTexture", (Texture) this.DitheringTexture);
      this.scatteringMaterial.SetVector("_Density", new Vector4(heightFalloff, y, globalDensity, this.StartDistance));
      this.CustomBlit(source, destination, this.scatteringMaterial);
    }
  }
}
