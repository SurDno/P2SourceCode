using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Bloom and Glow/Bloom")]
  public class Bloom : PostEffectsBase
  {
    public TweakMode tweakMode = TweakMode.Basic;
    public BloomScreenBlendMode screenBlendMode = BloomScreenBlendMode.Add;
    public HDRBloomMode hdr = HDRBloomMode.Auto;
    private bool doHdr;
    public float sepBlurSpread = 2.5f;
    public BloomQuality quality = BloomQuality.High;
    public float bloomIntensity = 0.5f;
    public float bloomThreshold = 0.5f;
    public Color bloomThresholdColor = Color.white;
    public int bloomBlurIterations = 2;
    public int hollywoodFlareBlurIterations = 2;
    public float flareRotation;
    public LensFlareStyle lensflareMode = LensFlareStyle.Anamorphic;
    public float hollyStretchWidth = 2.5f;
    public float lensflareIntensity;
    public float lensflareThreshold = 0.3f;
    public float lensFlareSaturation = 0.75f;
    public Color flareColorA = new(0.4f, 0.4f, 0.8f, 0.75f);
    public Color flareColorB = new(0.4f, 0.8f, 0.8f, 0.75f);
    public Color flareColorC = new(0.8f, 0.4f, 0.8f, 0.75f);
    public Color flareColorD = new(0.8f, 0.4f, 0.0f, 0.75f);
    public Texture2D lensFlareVignetteMask;
    public Shader lensFlareShader;
    private Material lensFlareMaterial;
    public Shader screenBlendShader;
    private Material screenBlend;
    public Shader blurAndFlaresShader;
    private Material blurAndFlaresMaterial;
    public Shader brightPassFilterShader;
    private Material brightPassFilterMaterial;

    public override bool CheckResources()
    {
      CheckSupport(false);
      screenBlend = CheckShaderAndCreateMaterial(screenBlendShader, screenBlend);
      lensFlareMaterial = CheckShaderAndCreateMaterial(lensFlareShader, lensFlareMaterial);
      blurAndFlaresMaterial = CheckShaderAndCreateMaterial(blurAndFlaresShader, blurAndFlaresMaterial);
      brightPassFilterMaterial = CheckShaderAndCreateMaterial(brightPassFilterShader, brightPassFilterMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        doHdr = false;
        doHdr = hdr != HDRBloomMode.Auto ? hdr == HDRBloomMode.On : source.format == RenderTextureFormat.ARGBHalf && GetComponent<Camera>().allowHDR;
        doHdr = doHdr && supportHDRTextures;
        BloomScreenBlendMode bloomScreenBlendMode = screenBlendMode;
        if (doHdr)
          bloomScreenBlendMode = BloomScreenBlendMode.Add;
        RenderTextureFormat format = doHdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.Default;
        int width1 = source.width / 2;
        int height1 = source.height / 2;
        int width2 = source.width / 4;
        int height2 = source.height / 4;
        float num1 = (float) (1.0 * source.width / (1.0 * source.height));
        float num2 = 1f / 512f;
        RenderTexture temporary1 = RenderTexture.GetTemporary(width2, height2, 0, format);
        RenderTexture temporary2 = RenderTexture.GetTemporary(width1, height1, 0, format);
        if (quality > BloomQuality.Cheap)
        {
          Graphics.Blit(source, temporary2, screenBlend, 2);
          RenderTexture temporary3 = RenderTexture.GetTemporary(width2, height2, 0, format);
          Graphics.Blit(temporary2, temporary3, screenBlend, 2);
          Graphics.Blit(temporary3, temporary1, screenBlend, 6);
          RenderTexture.ReleaseTemporary(temporary3);
        }
        else
        {
          Graphics.Blit(source, temporary2);
          Graphics.Blit(temporary2, temporary1, screenBlend, 6);
        }
        RenderTexture.ReleaseTemporary(temporary2);
        RenderTexture renderTexture1 = RenderTexture.GetTemporary(width2, height2, 0, format);
        BrightFilter(bloomThreshold * bloomThresholdColor, temporary1, renderTexture1);
        if (bloomBlurIterations < 1)
          bloomBlurIterations = 1;
        else if (bloomBlurIterations > 10)
          bloomBlurIterations = 10;
        for (int index = 0; index < bloomBlurIterations; ++index)
        {
          float num3 = (float) (1.0 + index * 0.25) * sepBlurSpread;
          RenderTexture temporary4 = RenderTexture.GetTemporary(width2, height2, 0, format);
          blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(0.0f, num3 * num2, 0.0f, 0.0f));
          Graphics.Blit(renderTexture1, temporary4, blurAndFlaresMaterial, 4);
          RenderTexture.ReleaseTemporary(renderTexture1);
          RenderTexture renderTexture2 = temporary4;
          RenderTexture temporary5 = RenderTexture.GetTemporary(width2, height2, 0, format);
          blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num3 / num1 * num2, 0.0f, 0.0f, 0.0f));
          Graphics.Blit(renderTexture2, temporary5, blurAndFlaresMaterial, 4);
          RenderTexture.ReleaseTemporary(renderTexture2);
          renderTexture1 = temporary5;
          if (quality > BloomQuality.Cheap)
          {
            if (index == 0)
            {
              Graphics.SetRenderTarget(temporary1);
              GL.Clear(false, true, Color.black);
              Graphics.Blit(renderTexture1, temporary1);
            }
            else
            {
              temporary1.MarkRestoreExpected();
              Graphics.Blit(renderTexture1, temporary1, screenBlend, 10);
            }
          }
        }
        if (quality > BloomQuality.Cheap)
        {
          Graphics.SetRenderTarget(renderTexture1);
          GL.Clear(false, true, Color.black);
          Graphics.Blit(temporary1, renderTexture1, screenBlend, 6);
        }
        if (lensflareIntensity > (double) Mathf.Epsilon)
        {
          RenderTexture temporary6 = RenderTexture.GetTemporary(width2, height2, 0, format);
          if (lensflareMode == LensFlareStyle.Ghosting)
          {
            BrightFilter(lensflareThreshold, renderTexture1, temporary6);
            if (quality > BloomQuality.Cheap)
            {
              blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(0.0f, (float) (1.5 / (1.0 * temporary1.height)), 0.0f, 0.0f));
              Graphics.SetRenderTarget(temporary1);
              GL.Clear(false, true, Color.black);
              Graphics.Blit(temporary6, temporary1, blurAndFlaresMaterial, 4);
              blurAndFlaresMaterial.SetVector("_Offsets", new Vector4((float) (1.5 / (1.0 * temporary1.width)), 0.0f, 0.0f, 0.0f));
              Graphics.SetRenderTarget(temporary6);
              GL.Clear(false, true, Color.black);
              Graphics.Blit(temporary1, temporary6, blurAndFlaresMaterial, 4);
            }
            Vignette(0.975f, temporary6, temporary6);
            BlendFlares(temporary6, renderTexture1);
          }
          else
          {
            float x = 1f * Mathf.Cos(flareRotation);
            float y = 1f * Mathf.Sin(flareRotation);
            float num4 = hollyStretchWidth * 1f / num1 * num2;
            blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(x, y, 0.0f, 0.0f));
            blurAndFlaresMaterial.SetVector("_Threshhold", new Vector4(lensflareThreshold, 1f, 0.0f, 0.0f));
            blurAndFlaresMaterial.SetVector("_TintColor", new Vector4(flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * flareColorA.a * lensflareIntensity);
            blurAndFlaresMaterial.SetFloat("_Saturation", lensFlareSaturation);
            temporary1.DiscardContents();
            Graphics.Blit(temporary6, temporary1, blurAndFlaresMaterial, 2);
            temporary6.DiscardContents();
            Graphics.Blit(temporary1, temporary6, blurAndFlaresMaterial, 3);
            blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(x * num4, y * num4, 0.0f, 0.0f));
            blurAndFlaresMaterial.SetFloat("_StretchWidth", hollyStretchWidth);
            temporary1.DiscardContents();
            Graphics.Blit(temporary6, temporary1, blurAndFlaresMaterial, 1);
            blurAndFlaresMaterial.SetFloat("_StretchWidth", hollyStretchWidth * 2f);
            temporary6.DiscardContents();
            Graphics.Blit(temporary1, temporary6, blurAndFlaresMaterial, 1);
            blurAndFlaresMaterial.SetFloat("_StretchWidth", hollyStretchWidth * 4f);
            temporary1.DiscardContents();
            Graphics.Blit(temporary6, temporary1, blurAndFlaresMaterial, 1);
            for (int index = 0; index < hollywoodFlareBlurIterations; ++index)
            {
              float num5 = hollyStretchWidth * 2f / num1 * num2;
              blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num5 * x, num5 * y, 0.0f, 0.0f));
              temporary6.DiscardContents();
              Graphics.Blit(temporary1, temporary6, blurAndFlaresMaterial, 4);
              blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num5 * x, num5 * y, 0.0f, 0.0f));
              temporary1.DiscardContents();
              Graphics.Blit(temporary6, temporary1, blurAndFlaresMaterial, 4);
            }
            if (lensflareMode == LensFlareStyle.Anamorphic)
            {
              AddTo(1f, temporary1, renderTexture1);
            }
            else
            {
              Vignette(1f, temporary1, temporary6);
              BlendFlares(temporary6, temporary1);
              AddTo(1f, temporary1, renderTexture1);
            }
          }
          RenderTexture.ReleaseTemporary(temporary6);
        }
        int pass = (int) bloomScreenBlendMode;
        screenBlend.SetFloat("_Intensity", bloomIntensity);
        screenBlend.SetTexture("_ColorBuffer", source);
        if (quality > BloomQuality.Cheap)
        {
          RenderTexture temporary7 = RenderTexture.GetTemporary(width1, height1, 0, format);
          Graphics.Blit(renderTexture1, temporary7);
          Graphics.Blit(temporary7, destination, screenBlend, pass);
          RenderTexture.ReleaseTemporary(temporary7);
        }
        else
          Graphics.Blit(renderTexture1, destination, screenBlend, pass);
        RenderTexture.ReleaseTemporary(temporary1);
        RenderTexture.ReleaseTemporary(renderTexture1);
      }
    }

    private void AddTo(float intensity_, RenderTexture from, RenderTexture to)
    {
      screenBlend.SetFloat("_Intensity", intensity_);
      to.MarkRestoreExpected();
      Graphics.Blit(from, to, screenBlend, 9);
    }

    private void BlendFlares(RenderTexture from, RenderTexture to)
    {
      lensFlareMaterial.SetVector("colorA", new Vector4(flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * lensflareIntensity);
      lensFlareMaterial.SetVector("colorB", new Vector4(flareColorB.r, flareColorB.g, flareColorB.b, flareColorB.a) * lensflareIntensity);
      lensFlareMaterial.SetVector("colorC", new Vector4(flareColorC.r, flareColorC.g, flareColorC.b, flareColorC.a) * lensflareIntensity);
      lensFlareMaterial.SetVector("colorD", new Vector4(flareColorD.r, flareColorD.g, flareColorD.b, flareColorD.a) * lensflareIntensity);
      to.MarkRestoreExpected();
      Graphics.Blit(from, to, lensFlareMaterial);
    }

    private void BrightFilter(float thresh, RenderTexture from, RenderTexture to)
    {
      brightPassFilterMaterial.SetVector("_Threshhold", new Vector4(thresh, thresh, thresh, thresh));
      Graphics.Blit(from, to, brightPassFilterMaterial, 0);
    }

    private void BrightFilter(Color threshColor, RenderTexture from, RenderTexture to)
    {
      brightPassFilterMaterial.SetVector("_Threshhold", threshColor);
      Graphics.Blit(from, to, brightPassFilterMaterial, 1);
    }

    private void Vignette(float amount, RenderTexture from, RenderTexture to)
    {
      if ((bool) (Object) lensFlareVignetteMask)
      {
        screenBlend.SetTexture("_ColorBuffer", lensFlareVignetteMask);
        to.MarkRestoreExpected();
        Graphics.Blit(from == to ? null : (Texture) from, to, screenBlend, from == to ? 7 : 3);
      }
      else
      {
        if (!(from != to))
          return;
        Graphics.SetRenderTarget(to);
        GL.Clear(false, true, Color.black);
        Graphics.Blit(from, to);
      }
    }

    public enum LensFlareStyle
    {
      Ghosting,
      Anamorphic,
      Combined,
    }

    public enum TweakMode
    {
      Basic,
      Complex,
    }

    public enum HDRBloomMode
    {
      Auto,
      On,
      Off,
    }

    public enum BloomScreenBlendMode
    {
      Screen,
      Add,
    }

    public enum BloomQuality
    {
      Cheap,
      High,
    }
  }
}
