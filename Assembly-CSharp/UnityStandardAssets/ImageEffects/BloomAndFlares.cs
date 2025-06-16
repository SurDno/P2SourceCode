namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Bloom and Glow/BloomAndFlares (3.5, Deprecated)")]
  public class BloomAndFlares : PostEffectsBase
  {
    public TweakMode34 tweakMode = TweakMode34.Basic;
    public BloomScreenBlendMode screenBlendMode = BloomScreenBlendMode.Add;
    public HDRBloomMode hdr = HDRBloomMode.Auto;
    private bool doHdr;
    public float sepBlurSpread = 1.5f;
    public float useSrcAlphaAsMask = 0.5f;
    public float bloomIntensity = 1f;
    public float bloomThreshold = 0.5f;
    public int bloomBlurIterations = 2;
    public bool lensflares = false;
    public int hollywoodFlareBlurIterations = 2;
    public LensflareStyle34 lensflareMode = LensflareStyle34.Anamorphic;
    public float hollyStretchWidth = 3.5f;
    public float lensflareIntensity = 1f;
    public float lensflareThreshold = 0.3f;
    public Color flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);
    public Color flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);
    public Color flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);
    public Color flareColorD = new Color(0.8f, 0.4f, 0.0f, 0.75f);
    public Texture2D lensFlareVignetteMask;
    public Shader lensFlareShader;
    private Material lensFlareMaterial;
    public Shader vignetteShader;
    private Material vignetteMaterial;
    public Shader separableBlurShader;
    private Material separableBlurMaterial;
    public Shader addBrightStuffOneOneShader;
    private Material addBrightStuffBlendOneOneMaterial;
    public Shader screenBlendShader;
    private Material screenBlend;
    public Shader hollywoodFlaresShader;
    private Material hollywoodFlaresMaterial;
    public Shader brightPassFilterShader;
    private Material brightPassFilterMaterial;

    public override bool CheckResources()
    {
      CheckSupport(false);
      screenBlend = CheckShaderAndCreateMaterial(screenBlendShader, screenBlend);
      lensFlareMaterial = CheckShaderAndCreateMaterial(lensFlareShader, lensFlareMaterial);
      vignetteMaterial = CheckShaderAndCreateMaterial(vignetteShader, vignetteMaterial);
      separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, separableBlurMaterial);
      addBrightStuffBlendOneOneMaterial = CheckShaderAndCreateMaterial(addBrightStuffOneOneShader, addBrightStuffBlendOneOneMaterial);
      hollywoodFlaresMaterial = CheckShaderAndCreateMaterial(hollywoodFlaresShader, hollywoodFlaresMaterial);
      brightPassFilterMaterial = CheckShaderAndCreateMaterial(brightPassFilterShader, brightPassFilterMaterial);
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
        doHdr = false;
        doHdr = hdr != HDRBloomMode.Auto ? hdr == HDRBloomMode.On : source.format == RenderTextureFormat.ARGBHalf && this.GetComponent<Camera>().allowHDR;
        doHdr = doHdr && supportHDRTextures;
        BloomScreenBlendMode pass = screenBlendMode;
        if (doHdr)
          pass = BloomScreenBlendMode.Add;
        RenderTextureFormat format = doHdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.Default;
        RenderTexture temporary1 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, format);
        RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
        RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
        RenderTexture temporary4 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, format);
        float num1 = (float) (1.0 * (double) source.width / (1.0 * (double) source.height));
        float num2 = 1f / 512f;
        Graphics.Blit((Texture) source, temporary1, screenBlend, 2);
        Graphics.Blit((Texture) temporary1, temporary2, screenBlend, 2);
        RenderTexture.ReleaseTemporary(temporary1);
        BrightFilter(bloomThreshold, useSrcAlphaAsMask, temporary2, temporary3);
        temporary2.DiscardContents();
        if (bloomBlurIterations < 1)
          bloomBlurIterations = 1;
        for (int index = 0; index < bloomBlurIterations; ++index)
        {
          float num3 = (float) (1.0 + index * 0.5) * sepBlurSpread;
          separableBlurMaterial.SetVector("offsets", new Vector4(0.0f, num3 * num2, 0.0f, 0.0f));
          RenderTexture source1 = index == 0 ? temporary3 : temporary2;
          Graphics.Blit((Texture) source1, temporary4, separableBlurMaterial);
          source1.DiscardContents();
          separableBlurMaterial.SetVector("offsets", new Vector4(num3 / num1 * num2, 0.0f, 0.0f, 0.0f));
          Graphics.Blit((Texture) temporary4, temporary2, separableBlurMaterial);
          temporary4.DiscardContents();
        }
        if (lensflares)
        {
          if (lensflareMode == LensflareStyle34.Ghosting)
          {
            BrightFilter(lensflareThreshold, 0.0f, temporary2, temporary4);
            temporary2.DiscardContents();
            Vignette(0.975f, temporary4, temporary3);
            temporary4.DiscardContents();
            BlendFlares(temporary3, temporary2);
            temporary3.DiscardContents();
          }
          else
          {
            hollywoodFlaresMaterial.SetVector("_threshold", new Vector4(lensflareThreshold, (float) (1.0 / (1.0 - lensflareThreshold)), 0.0f, 0.0f));
            hollywoodFlaresMaterial.SetVector("tintColor", new Vector4(flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * flareColorA.a * lensflareIntensity);
            Graphics.Blit((Texture) temporary4, temporary3, hollywoodFlaresMaterial, 2);
            temporary4.DiscardContents();
            Graphics.Blit((Texture) temporary3, temporary4, hollywoodFlaresMaterial, 3);
            temporary3.DiscardContents();
            hollywoodFlaresMaterial.SetVector("offsets", new Vector4(sepBlurSpread * 1f / num1 * num2, 0.0f, 0.0f, 0.0f));
            hollywoodFlaresMaterial.SetFloat("stretchWidth", hollyStretchWidth);
            Graphics.Blit((Texture) temporary4, temporary3, hollywoodFlaresMaterial, 1);
            temporary4.DiscardContents();
            hollywoodFlaresMaterial.SetFloat("stretchWidth", hollyStretchWidth * 2f);
            Graphics.Blit((Texture) temporary3, temporary4, hollywoodFlaresMaterial, 1);
            temporary3.DiscardContents();
            hollywoodFlaresMaterial.SetFloat("stretchWidth", hollyStretchWidth * 4f);
            Graphics.Blit((Texture) temporary4, temporary3, hollywoodFlaresMaterial, 1);
            temporary4.DiscardContents();
            if (lensflareMode == LensflareStyle34.Anamorphic)
            {
              for (int index = 0; index < hollywoodFlareBlurIterations; ++index)
              {
                separableBlurMaterial.SetVector("offsets", new Vector4(hollyStretchWidth * 2f / num1 * num2, 0.0f, 0.0f, 0.0f));
                Graphics.Blit((Texture) temporary3, temporary4, separableBlurMaterial);
                temporary3.DiscardContents();
                separableBlurMaterial.SetVector("offsets", new Vector4(hollyStretchWidth * 2f / num1 * num2, 0.0f, 0.0f, 0.0f));
                Graphics.Blit((Texture) temporary4, temporary3, separableBlurMaterial);
                temporary4.DiscardContents();
              }
              AddTo(1f, temporary3, temporary2);
              temporary3.DiscardContents();
            }
            else
            {
              for (int index = 0; index < hollywoodFlareBlurIterations; ++index)
              {
                separableBlurMaterial.SetVector("offsets", new Vector4(hollyStretchWidth * 2f / num1 * num2, 0.0f, 0.0f, 0.0f));
                Graphics.Blit((Texture) temporary3, temporary4, separableBlurMaterial);
                temporary3.DiscardContents();
                separableBlurMaterial.SetVector("offsets", new Vector4(hollyStretchWidth * 2f / num1 * num2, 0.0f, 0.0f, 0.0f));
                Graphics.Blit((Texture) temporary4, temporary3, separableBlurMaterial);
                temporary4.DiscardContents();
              }
              Vignette(1f, temporary3, temporary4);
              temporary3.DiscardContents();
              BlendFlares(temporary4, temporary3);
              temporary4.DiscardContents();
              AddTo(1f, temporary3, temporary2);
              temporary3.DiscardContents();
            }
          }
        }
        screenBlend.SetFloat("_Intensity", bloomIntensity);
        screenBlend.SetTexture("_ColorBuffer", (Texture) source);
        Graphics.Blit((Texture) temporary2, destination, screenBlend, (int) pass);
        RenderTexture.ReleaseTemporary(temporary2);
        RenderTexture.ReleaseTemporary(temporary3);
        RenderTexture.ReleaseTemporary(temporary4);
      }
    }

    private void AddTo(float intensity_, RenderTexture from, RenderTexture to)
    {
      addBrightStuffBlendOneOneMaterial.SetFloat("_Intensity", intensity_);
      Graphics.Blit((Texture) from, to, addBrightStuffBlendOneOneMaterial);
    }

    private void BlendFlares(RenderTexture from, RenderTexture to)
    {
      lensFlareMaterial.SetVector("colorA", new Vector4(flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * lensflareIntensity);
      lensFlareMaterial.SetVector("colorB", new Vector4(flareColorB.r, flareColorB.g, flareColorB.b, flareColorB.a) * lensflareIntensity);
      lensFlareMaterial.SetVector("colorC", new Vector4(flareColorC.r, flareColorC.g, flareColorC.b, flareColorC.a) * lensflareIntensity);
      lensFlareMaterial.SetVector("colorD", new Vector4(flareColorD.r, flareColorD.g, flareColorD.b, flareColorD.a) * lensflareIntensity);
      Graphics.Blit((Texture) from, to, lensFlareMaterial);
    }

    private void BrightFilter(
      float thresh,
      float useAlphaAsMask,
      RenderTexture from,
      RenderTexture to)
    {
      if (doHdr)
        brightPassFilterMaterial.SetVector("threshold", new Vector4(thresh, 1f, 0.0f, 0.0f));
      else
        brightPassFilterMaterial.SetVector("threshold", new Vector4(thresh, (float) (1.0 / (1.0 - thresh)), 0.0f, 0.0f));
      brightPassFilterMaterial.SetFloat("useSrcAlphaAsMask", useAlphaAsMask);
      Graphics.Blit((Texture) from, to, brightPassFilterMaterial);
    }

    private void Vignette(float amount, RenderTexture from, RenderTexture to)
    {
      if ((bool) (Object) lensFlareVignetteMask)
      {
        screenBlend.SetTexture("_ColorBuffer", (Texture) lensFlareVignetteMask);
        Graphics.Blit((Texture) from, to, screenBlend, 3);
      }
      else
      {
        vignetteMaterial.SetFloat("vignetteIntensity", amount);
        Graphics.Blit((Texture) from, to, vignetteMaterial);
      }
    }
  }
}
