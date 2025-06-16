namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Rendering/Sun Shafts")]
  public class SunShafts : PostEffectsBase
  {
    public SunShaftsResolution resolution = SunShaftsResolution.Normal;
    public ShaftsScreenBlendMode screenBlendMode = ShaftsScreenBlendMode.Screen;
    public Transform sunTransform;
    public int radialBlurIterations = 2;
    public Color sunColor = Color.white;
    public Color sunThreshold = new Color(0.87f, 0.74f, 0.65f);
    public float sunShaftBlurRadius = 2.5f;
    public float sunShaftIntensity = 1.15f;
    public float maxRadius = 0.75f;
    public bool useDepthTexture = true;
    public Shader sunShaftsShader;
    private Material sunShaftsMaterial;
    public Shader simpleClearShader;
    private Material simpleClearMaterial;

    public override bool CheckResources()
    {
      CheckSupport(useDepthTexture);
      sunShaftsMaterial = CheckShaderAndCreateMaterial(sunShaftsShader, sunShaftsMaterial);
      simpleClearMaterial = CheckShaderAndCreateMaterial(simpleClearShader, simpleClearMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        if (useDepthTexture)
          this.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        int num1 = 4;
        if (resolution == SunShaftsResolution.Normal)
          num1 = 2;
        else if (resolution == SunShaftsResolution.High)
          num1 = 1;
        Vector3 vector3 = Vector3.one * 0.5f;
        vector3 = !(bool) (Object) sunTransform ? new Vector3(0.5f, 0.5f, 0.0f) : this.GetComponent<Camera>().WorldToViewportPoint(sunTransform.position);
        int width = source.width / num1;
        int height = source.height / num1;
        RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 0);
        sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0.0f, 0.0f) * sunShaftBlurRadius);
        sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector3.x, vector3.y, vector3.z, maxRadius));
        sunShaftsMaterial.SetVector("_SunThreshold", (Vector4) sunThreshold);
        if (!useDepthTexture)
        {
          RenderTextureFormat format = this.GetComponent<Camera>().allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
          RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0, format);
          RenderTexture.active = temporary2;
          GL.ClearWithSkybox(false, this.GetComponent<Camera>());
          sunShaftsMaterial.SetTexture("_Skybox", (Texture) temporary2);
          Graphics.Blit((Texture) source, temporary1, sunShaftsMaterial, 3);
          RenderTexture.ReleaseTemporary(temporary2);
        }
        else
          Graphics.Blit((Texture) source, temporary1, sunShaftsMaterial, 2);
        DrawBorder(temporary1, simpleClearMaterial);
        radialBlurIterations = Mathf.Clamp(radialBlurIterations, 1, 4);
        float num2 = sunShaftBlurRadius * (1f / 768f);
        sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, 0.0f, 0.0f));
        sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector3.x, vector3.y, vector3.z, maxRadius));
        for (int index = 0; index < radialBlurIterations; ++index)
        {
          RenderTexture temporary3 = RenderTexture.GetTemporary(width, height, 0);
          Graphics.Blit((Texture) temporary1, temporary3, sunShaftsMaterial, 1);
          RenderTexture.ReleaseTemporary(temporary1);
          float num3 = (float) (sunShaftBlurRadius * ((index * 2.0 + 1.0) * 6.0) / 768.0);
          sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num3, num3, 0.0f, 0.0f));
          temporary1 = RenderTexture.GetTemporary(width, height, 0);
          Graphics.Blit((Texture) temporary3, temporary1, sunShaftsMaterial, 1);
          RenderTexture.ReleaseTemporary(temporary3);
          float num4 = (float) (sunShaftBlurRadius * ((index * 2.0 + 2.0) * 6.0) / 768.0);
          sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num4, num4, 0.0f, 0.0f));
        }
        if ((double) vector3.z >= 0.0)
          sunShaftsMaterial.SetVector("_SunColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, sunColor.a) * sunShaftIntensity);
        else
          sunShaftsMaterial.SetVector("_SunColor", Vector4.zero);
        sunShaftsMaterial.SetTexture("_ColorBuffer", (Texture) temporary1);
        Graphics.Blit((Texture) source, destination, sunShaftsMaterial, screenBlendMode == ShaftsScreenBlendMode.Screen ? 0 : 4);
        RenderTexture.ReleaseTemporary(temporary1);
      }
    }

    public enum SunShaftsResolution
    {
      Low,
      Normal,
      High,
    }

    public enum ShaftsScreenBlendMode
    {
      Screen,
      Add,
    }
  }
}
