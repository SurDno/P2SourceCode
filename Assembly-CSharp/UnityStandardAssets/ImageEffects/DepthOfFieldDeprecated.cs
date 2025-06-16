using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Camera/Depth of Field (deprecated)")]
  public class DepthOfFieldDeprecated : PostEffectsBase
  {
    private static int SMOOTH_DOWNSAMPLE_PASS = 6;
    private static float BOKEH_EXTRA_BLUR = 2f;
    public Dof34QualitySetting quality = Dof34QualitySetting.OnlyBackground;
    public DofResolution resolution = DofResolution.Low;
    public bool simpleTweakMode = true;
    public float focalPoint = 1f;
    public float smoothness = 0.5f;
    public float focalZDistance;
    public float focalZStartCurve = 1f;
    public float focalZEndCurve = 1f;
    private float focalStartCurve = 2f;
    private float focalEndCurve = 2f;
    private float focalDistance01 = 0.1f;
    public Transform objectFocus;
    public float focalSize;
    public DofBlurriness bluriness = DofBlurriness.High;
    public float maxBlurSpread = 1.75f;
    public float foregroundBlurExtrude = 1.15f;
    public Shader dofBlurShader;
    private Material dofBlurMaterial;
    public Shader dofShader;
    private Material dofMaterial;
    public bool visualize;
    public BokehDestination bokehDestination = BokehDestination.Background;
    private float widthOverHeight = 1.25f;
    private float oneOverBaseSize = 1f / 512f;
    public bool bokeh;
    public bool bokehSupport = true;
    public Shader bokehShader;
    public Texture2D bokehTexture;
    public float bokehScale = 2.4f;
    public float bokehIntensity = 0.15f;
    public float bokehThresholdContrast = 0.1f;
    public float bokehThresholdLuminance = 0.55f;
    public int bokehDownsample = 1;
    private Material bokehMaterial;
    private Camera _camera;
    private RenderTexture foregroundTexture;
    private RenderTexture mediumRezWorkTexture;
    private RenderTexture finalDefocus;
    private RenderTexture lowRezWorkTexture;
    private RenderTexture bokehSource;
    private RenderTexture bokehSource2;

    private void CreateMaterials()
    {
      dofBlurMaterial = CheckShaderAndCreateMaterial(dofBlurShader, dofBlurMaterial);
      dofMaterial = CheckShaderAndCreateMaterial(dofShader, dofMaterial);
      bokehSupport = bokehShader.isSupported;
      if (!bokeh || !bokehSupport || !(bool) (Object) bokehShader)
        return;
      bokehMaterial = CheckShaderAndCreateMaterial(bokehShader, bokehMaterial);
    }

    public override bool CheckResources()
    {
      CheckSupport(true);
      dofBlurMaterial = CheckShaderAndCreateMaterial(dofBlurShader, dofBlurMaterial);
      dofMaterial = CheckShaderAndCreateMaterial(dofShader, dofMaterial);
      bokehSupport = bokehShader.isSupported;
      if (bokeh && bokehSupport && (bool) (Object) bokehShader)
        bokehMaterial = CheckShaderAndCreateMaterial(bokehShader, bokehMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnDisable() => Quads.Cleanup();

    private void OnEnable()
    {
      _camera = GetComponent<Camera>();
      _camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    private float FocalDistance01(float worldDist)
    {
      return _camera.WorldToViewportPoint((worldDist - _camera.nearClipPlane) * _camera.transform.forward + _camera.transform.position).z / (_camera.farClipPlane - _camera.nearClipPlane);
    }

    private int GetDividerBasedOnQuality()
    {
      int dividerBasedOnQuality = 1;
      if (resolution == DofResolution.Medium)
        dividerBasedOnQuality = 2;
      else if (resolution == DofResolution.Low)
        dividerBasedOnQuality = 2;
      return dividerBasedOnQuality;
    }

    private int GetLowResolutionDividerBasedOnQuality(int baseDivider)
    {
      int dividerBasedOnQuality = baseDivider;
      if (resolution == DofResolution.High)
        dividerBasedOnQuality *= 2;
      if (resolution == DofResolution.Low)
        dividerBasedOnQuality *= 2;
      return dividerBasedOnQuality;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        if (smoothness < 0.10000000149011612)
          smoothness = 0.1f;
        bokeh = bokeh && bokehSupport;
        float num1 = bokeh ? BOKEH_EXTRA_BLUR : 1f;
        bool flag = quality > Dof34QualitySetting.OnlyBackground;
        float num2 = focalSize / (_camera.farClipPlane - _camera.nearClipPlane);
        bool blurForeground;
        if (simpleTweakMode)
        {
          focalDistance01 = (bool) (Object) objectFocus ? _camera.WorldToViewportPoint(objectFocus.position).z / _camera.farClipPlane : FocalDistance01(focalPoint);
          focalStartCurve = focalDistance01 * smoothness;
          focalEndCurve = focalStartCurve;
          blurForeground = flag && focalPoint > _camera.nearClipPlane + (double) Mathf.Epsilon;
        }
        else
        {
          if ((bool) (Object) objectFocus)
          {
            Vector3 viewportPoint = _camera.WorldToViewportPoint(objectFocus.position);
            viewportPoint.z /= _camera.farClipPlane;
            focalDistance01 = viewportPoint.z;
          }
          else
            focalDistance01 = FocalDistance01(focalZDistance);
          focalStartCurve = focalZStartCurve;
          focalEndCurve = focalZEndCurve;
          blurForeground = flag && focalPoint > _camera.nearClipPlane + (double) Mathf.Epsilon;
        }
        widthOverHeight = (float) (1.0 * source.width / (1.0 * source.height));
        oneOverBaseSize = 1f / 512f;
        dofMaterial.SetFloat("_ForegroundBlurExtrude", foregroundBlurExtrude);
        dofMaterial.SetVector("_CurveParams", new Vector4(simpleTweakMode ? 1f / focalStartCurve : focalStartCurve, simpleTweakMode ? 1f / focalEndCurve : focalEndCurve, num2 * 0.5f, focalDistance01));
        dofMaterial.SetVector("_InvRenderTargetSize", new Vector4((float) (1.0 / (1.0 * source.width)), (float) (1.0 / (1.0 * source.height)), 0.0f, 0.0f));
        int dividerBasedOnQuality1 = GetDividerBasedOnQuality();
        int dividerBasedOnQuality2 = GetLowResolutionDividerBasedOnQuality(dividerBasedOnQuality1);
        AllocateTextures(blurForeground, source, dividerBasedOnQuality1, dividerBasedOnQuality2);
        Graphics.Blit(source, source, dofMaterial, 3);
        Downsample(source, mediumRezWorkTexture);
        Blur(mediumRezWorkTexture, mediumRezWorkTexture, DofBlurriness.Low, 4, maxBlurSpread);
        if (bokeh && (BokehDestination.Foreground & bokehDestination) != 0)
        {
          dofMaterial.SetVector("_Threshhold", new Vector4(bokehThresholdContrast, bokehThresholdLuminance, 0.95f, 0.0f));
          Graphics.Blit(mediumRezWorkTexture, bokehSource2, dofMaterial, 11);
          Graphics.Blit(mediumRezWorkTexture, lowRezWorkTexture);
          Blur(lowRezWorkTexture, lowRezWorkTexture, bluriness, 0, maxBlurSpread * num1);
        }
        else
        {
          Downsample(mediumRezWorkTexture, lowRezWorkTexture);
          Blur(lowRezWorkTexture, lowRezWorkTexture, bluriness, 0, maxBlurSpread);
        }
        dofBlurMaterial.SetTexture("_TapLow", lowRezWorkTexture);
        dofBlurMaterial.SetTexture("_TapMedium", mediumRezWorkTexture);
        Graphics.Blit(null, finalDefocus, dofBlurMaterial, 3);
        if (bokeh && (BokehDestination.Foreground & bokehDestination) != 0)
          AddBokeh(bokehSource2, bokehSource, finalDefocus);
        dofMaterial.SetTexture("_TapLowBackground", finalDefocus);
        dofMaterial.SetTexture("_TapMedium", mediumRezWorkTexture);
        Graphics.Blit(source, blurForeground ? foregroundTexture : destination, dofMaterial, visualize ? 2 : 0);
        if (blurForeground)
        {
          Graphics.Blit(foregroundTexture, source, dofMaterial, 5);
          Downsample(source, mediumRezWorkTexture);
          BlurFg(mediumRezWorkTexture, mediumRezWorkTexture, DofBlurriness.Low, 2, maxBlurSpread);
          if (bokeh && (BokehDestination.Foreground & bokehDestination) != 0)
          {
            dofMaterial.SetVector("_Threshhold", new Vector4(bokehThresholdContrast * 0.5f, bokehThresholdLuminance, 0.0f, 0.0f));
            Graphics.Blit(mediumRezWorkTexture, bokehSource2, dofMaterial, 11);
            Graphics.Blit(mediumRezWorkTexture, lowRezWorkTexture);
            BlurFg(lowRezWorkTexture, lowRezWorkTexture, bluriness, 1, maxBlurSpread * num1);
          }
          else
            BlurFg(mediumRezWorkTexture, lowRezWorkTexture, bluriness, 1, maxBlurSpread);
          Graphics.Blit(lowRezWorkTexture, finalDefocus);
          dofMaterial.SetTexture("_TapLowForeground", finalDefocus);
          Graphics.Blit(source, destination, dofMaterial, visualize ? 1 : 4);
          if (bokeh && (BokehDestination.Foreground & bokehDestination) != 0)
            AddBokeh(bokehSource2, bokehSource, destination);
        }
        ReleaseTextures();
      }
    }

    private void Blur(
      RenderTexture from,
      RenderTexture to,
      DofBlurriness iterations,
      int blurPass,
      float spread)
    {
      RenderTexture temporary = RenderTexture.GetTemporary(to.width, to.height);
      if (iterations > DofBlurriness.Low)
      {
        BlurHex(from, to, blurPass, spread, temporary);
        if (iterations > DofBlurriness.High)
        {
          dofBlurMaterial.SetVector("offsets", new Vector4(0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
          Graphics.Blit(to, temporary, dofBlurMaterial, blurPass);
          dofBlurMaterial.SetVector("offsets", new Vector4(spread / widthOverHeight * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
          Graphics.Blit(temporary, to, dofBlurMaterial, blurPass);
        }
      }
      else
      {
        dofBlurMaterial.SetVector("offsets", new Vector4(0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
        Graphics.Blit(from, temporary, dofBlurMaterial, blurPass);
        dofBlurMaterial.SetVector("offsets", new Vector4(spread / widthOverHeight * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
        Graphics.Blit(temporary, to, dofBlurMaterial, blurPass);
      }
      RenderTexture.ReleaseTemporary(temporary);
    }

    private void BlurFg(
      RenderTexture from,
      RenderTexture to,
      DofBlurriness iterations,
      int blurPass,
      float spread)
    {
      dofBlurMaterial.SetTexture("_TapHigh", from);
      RenderTexture temporary = RenderTexture.GetTemporary(to.width, to.height);
      if (iterations > DofBlurriness.Low)
      {
        BlurHex(from, to, blurPass, spread, temporary);
        if (iterations > DofBlurriness.High)
        {
          dofBlurMaterial.SetVector("offsets", new Vector4(0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
          Graphics.Blit(to, temporary, dofBlurMaterial, blurPass);
          dofBlurMaterial.SetVector("offsets", new Vector4(spread / widthOverHeight * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
          Graphics.Blit(temporary, to, dofBlurMaterial, blurPass);
        }
      }
      else
      {
        dofBlurMaterial.SetVector("offsets", new Vector4(0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
        Graphics.Blit(from, temporary, dofBlurMaterial, blurPass);
        dofBlurMaterial.SetVector("offsets", new Vector4(spread / widthOverHeight * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
        Graphics.Blit(temporary, to, dofBlurMaterial, blurPass);
      }
      RenderTexture.ReleaseTemporary(temporary);
    }

    private void BlurHex(
      RenderTexture from,
      RenderTexture to,
      int blurPass,
      float spread,
      RenderTexture tmp)
    {
      dofBlurMaterial.SetVector("offsets", new Vector4(0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
      Graphics.Blit(from, tmp, dofBlurMaterial, blurPass);
      dofBlurMaterial.SetVector("offsets", new Vector4(spread / widthOverHeight * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
      Graphics.Blit(tmp, to, dofBlurMaterial, blurPass);
      dofBlurMaterial.SetVector("offsets", new Vector4(spread / widthOverHeight * oneOverBaseSize, spread * oneOverBaseSize, 0.0f, 0.0f));
      Graphics.Blit(to, tmp, dofBlurMaterial, blurPass);
      dofBlurMaterial.SetVector("offsets", new Vector4(spread / widthOverHeight * oneOverBaseSize, -spread * oneOverBaseSize, 0.0f, 0.0f));
      Graphics.Blit(tmp, to, dofBlurMaterial, blurPass);
    }

    private void Downsample(RenderTexture from, RenderTexture to)
    {
      dofMaterial.SetVector("_InvRenderTargetSize", new Vector4((float) (1.0 / (1.0 * to.width)), (float) (1.0 / (1.0 * to.height)), 0.0f, 0.0f));
      Graphics.Blit(from, to, dofMaterial, SMOOTH_DOWNSAMPLE_PASS);
    }

    private void AddBokeh(
      RenderTexture bokehInfo,
      RenderTexture tempTex,
      RenderTexture finalTarget)
    {
      if (!(bool) (Object) bokehMaterial)
        return;
      Mesh[] meshes = Quads.GetMeshes(tempTex.width, tempTex.height);
      RenderTexture.active = tempTex;
      GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
      GL.PushMatrix();
      GL.LoadIdentity();
      bokehInfo.filterMode = FilterMode.Point;
      float num = (float) (bokehInfo.width * 1.0 / (bokehInfo.height * 1.0));
      float x = (float) (2.0 / (1.0 * bokehInfo.width)) + bokehScale * maxBlurSpread * BOKEH_EXTRA_BLUR * oneOverBaseSize;
      bokehMaterial.SetTexture("_Source", bokehInfo);
      bokehMaterial.SetTexture("_MainTex", bokehTexture);
      bokehMaterial.SetVector("_ArScale", new Vector4(x, x * num, 0.5f, 0.5f * num));
      bokehMaterial.SetFloat("_Intensity", bokehIntensity);
      bokehMaterial.SetPass(0);
      foreach (Mesh mesh in meshes)
      {
        if ((bool) (Object) mesh)
          Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
      }
      GL.PopMatrix();
      Graphics.Blit(tempTex, finalTarget, dofMaterial, 8);
      bokehInfo.filterMode = FilterMode.Bilinear;
    }

    private void ReleaseTextures()
    {
      if ((bool) (Object) foregroundTexture)
        RenderTexture.ReleaseTemporary(foregroundTexture);
      if ((bool) (Object) finalDefocus)
        RenderTexture.ReleaseTemporary(finalDefocus);
      if ((bool) (Object) mediumRezWorkTexture)
        RenderTexture.ReleaseTemporary(mediumRezWorkTexture);
      if ((bool) (Object) lowRezWorkTexture)
        RenderTexture.ReleaseTemporary(lowRezWorkTexture);
      if ((bool) (Object) bokehSource)
        RenderTexture.ReleaseTemporary(bokehSource);
      if (!(bool) (Object) bokehSource2)
        return;
      RenderTexture.ReleaseTemporary(bokehSource2);
    }

    private void AllocateTextures(
      bool blurForeground,
      RenderTexture source,
      int divider,
      int lowTexDivider)
    {
      foregroundTexture = null;
      if (blurForeground)
        foregroundTexture = RenderTexture.GetTemporary(source.width, source.height, 0);
      mediumRezWorkTexture = RenderTexture.GetTemporary(source.width / divider, source.height / divider, 0);
      finalDefocus = RenderTexture.GetTemporary(source.width / divider, source.height / divider, 0);
      lowRezWorkTexture = RenderTexture.GetTemporary(source.width / lowTexDivider, source.height / lowTexDivider, 0);
      bokehSource = null;
      bokehSource2 = null;
      if (bokeh)
      {
        bokehSource = RenderTexture.GetTemporary(source.width / (lowTexDivider * bokehDownsample), source.height / (lowTexDivider * bokehDownsample), 0, RenderTextureFormat.ARGBHalf);
        bokehSource2 = RenderTexture.GetTemporary(source.width / (lowTexDivider * bokehDownsample), source.height / (lowTexDivider * bokehDownsample), 0, RenderTextureFormat.ARGBHalf);
        bokehSource.filterMode = FilterMode.Bilinear;
        bokehSource2.filterMode = FilterMode.Bilinear;
        RenderTexture.active = bokehSource2;
        GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
      }
      source.filterMode = FilterMode.Bilinear;
      finalDefocus.filterMode = FilterMode.Bilinear;
      mediumRezWorkTexture.filterMode = FilterMode.Bilinear;
      lowRezWorkTexture.filterMode = FilterMode.Bilinear;
      if (!(bool) (Object) foregroundTexture)
        return;
      foregroundTexture.filterMode = FilterMode.Bilinear;
    }

    public enum Dof34QualitySetting
    {
      OnlyBackground = 1,
      BackgroundAndForeground = 2,
    }

    public enum DofResolution
    {
      High = 2,
      Medium = 3,
      Low = 4,
    }

    public enum DofBlurriness
    {
      Low = 1,
      High = 2,
      VeryHigh = 4,
    }

    public enum BokehDestination
    {
      Background = 1,
      Foreground = 2,
      BackgroundAndForeground = 3,
    }
  }
}
