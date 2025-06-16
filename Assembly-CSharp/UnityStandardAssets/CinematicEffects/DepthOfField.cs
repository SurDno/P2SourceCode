// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CinematicEffects.DepthOfField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Cinematic/Depth Of Field")]
  [RequireComponent(typeof (Camera))]
  public class DepthOfField : MonoBehaviour
  {
    private const float kMaxBlur = 40f;
    public DepthOfField.GlobalSettings settings = DepthOfField.GlobalSettings.defaultSettings;
    public DepthOfField.FocusSettings focus = DepthOfField.FocusSettings.defaultSettings;
    public DepthOfField.BokehTextureSettings bokehTexture = DepthOfField.BokehTextureSettings.defaultSettings;
    [SerializeField]
    private Shader m_FilmicDepthOfFieldShader;
    [SerializeField]
    private Shader m_MedianFilterShader;
    [SerializeField]
    private Shader m_TextureBokehShader;
    private RenderTextureUtility m_RTU = new RenderTextureUtility();
    private Material m_FilmicDepthOfFieldMaterial;
    private Material m_MedianFilterMaterial;
    private Material m_TextureBokehMaterial;
    private ComputeBuffer m_ComputeBufferDrawArgs;
    private ComputeBuffer m_ComputeBufferPoints;
    private DepthOfField.QualitySettings m_CurrentQualitySettings;
    private float m_LastApertureOrientation;
    private Vector4 m_OctogonalBokehDirection1;
    private Vector4 m_OctogonalBokehDirection2;
    private Vector4 m_OctogonalBokehDirection3;
    private Vector4 m_OctogonalBokehDirection4;
    private Vector4 m_HexagonalBokehDirection1;
    private Vector4 m_HexagonalBokehDirection2;
    private Vector4 m_HexagonalBokehDirection3;

    public Shader filmicDepthOfFieldShader
    {
      get
      {
        if ((UnityEngine.Object) this.m_FilmicDepthOfFieldShader == (UnityEngine.Object) null)
          this.m_FilmicDepthOfFieldShader = Shader.Find("Hidden/DepthOfField/DepthOfField");
        return this.m_FilmicDepthOfFieldShader;
      }
    }

    public Shader medianFilterShader
    {
      get
      {
        if ((UnityEngine.Object) this.m_MedianFilterShader == (UnityEngine.Object) null)
          this.m_MedianFilterShader = Shader.Find("Hidden/DepthOfField/MedianFilter");
        return this.m_MedianFilterShader;
      }
    }

    public Shader textureBokehShader
    {
      get
      {
        if ((UnityEngine.Object) this.m_TextureBokehShader == (UnityEngine.Object) null)
          this.m_TextureBokehShader = Shader.Find("Hidden/DepthOfField/BokehSplatting");
        return this.m_TextureBokehShader;
      }
    }

    public Material filmicDepthOfFieldMaterial
    {
      get
      {
        if ((UnityEngine.Object) this.m_FilmicDepthOfFieldMaterial == (UnityEngine.Object) null)
          this.m_FilmicDepthOfFieldMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(this.filmicDepthOfFieldShader);
        return this.m_FilmicDepthOfFieldMaterial;
      }
    }

    public Material medianFilterMaterial
    {
      get
      {
        if ((UnityEngine.Object) this.m_MedianFilterMaterial == (UnityEngine.Object) null)
          this.m_MedianFilterMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(this.medianFilterShader);
        return this.m_MedianFilterMaterial;
      }
    }

    public Material textureBokehMaterial
    {
      get
      {
        if ((UnityEngine.Object) this.m_TextureBokehMaterial == (UnityEngine.Object) null)
          this.m_TextureBokehMaterial = ImageEffectHelper.CheckShaderAndCreateMaterial(this.textureBokehShader);
        return this.m_TextureBokehMaterial;
      }
    }

    public ComputeBuffer computeBufferDrawArgs
    {
      get
      {
        if (this.m_ComputeBufferDrawArgs == null)
        {
          this.m_ComputeBufferDrawArgs = new ComputeBuffer(1, 16, ComputeBufferType.DrawIndirect);
          this.m_ComputeBufferDrawArgs.SetData((Array) new int[4]
          {
            0,
            1,
            0,
            0
          });
        }
        return this.m_ComputeBufferDrawArgs;
      }
    }

    public ComputeBuffer computeBufferPoints
    {
      get
      {
        if (this.m_ComputeBufferPoints == null)
          this.m_ComputeBufferPoints = new ComputeBuffer(90000, 28, ComputeBufferType.Append);
        return this.m_ComputeBufferPoints;
      }
    }

    private void OnEnable()
    {
      if (!ImageEffectHelper.IsSupported(this.filmicDepthOfFieldShader, true, true, (MonoBehaviour) this) || !ImageEffectHelper.IsSupported(this.medianFilterShader, true, true, (MonoBehaviour) this))
        this.enabled = false;
      else if (ImageEffectHelper.supportsDX11 && !ImageEffectHelper.IsSupported(this.textureBokehShader, true, true, (MonoBehaviour) this))
      {
        this.enabled = false;
      }
      else
      {
        this.ComputeBlurDirections(true);
        this.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
      }
    }

    private void OnDisable()
    {
      this.ReleaseComputeResources();
      if ((UnityEngine.Object) this.m_FilmicDepthOfFieldMaterial != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_FilmicDepthOfFieldMaterial);
      if ((UnityEngine.Object) this.m_TextureBokehMaterial != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_TextureBokehMaterial);
      if ((UnityEngine.Object) this.m_MedianFilterMaterial != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_MedianFilterMaterial);
      this.m_FilmicDepthOfFieldMaterial = (Material) null;
      this.m_TextureBokehMaterial = (Material) null;
      this.m_MedianFilterMaterial = (Material) null;
      this.m_RTU.ReleaseAllTemporaryRenderTextures();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if ((UnityEngine.Object) this.medianFilterMaterial == (UnityEngine.Object) null || (UnityEngine.Object) this.filmicDepthOfFieldMaterial == (UnityEngine.Object) null)
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        if (this.settings.visualizeFocus)
        {
          Vector4 blurParams;
          Vector4 blurCoe;
          this.ComputeCocParameters(out blurParams, out blurCoe);
          this.filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
          this.filmicDepthOfFieldMaterial.SetVector("_BlurCoe", blurCoe);
          Graphics.Blit((Texture) null, destination, this.filmicDepthOfFieldMaterial, 5);
        }
        else
          this.DoDepthOfField(source, destination);
        this.m_RTU.ReleaseAllTemporaryRenderTextures();
      }
    }

    private void DoDepthOfField(RenderTexture source, RenderTexture destination)
    {
      this.m_CurrentQualitySettings = DepthOfField.QualitySettings.presetQualitySettings[(int) this.settings.filteringQuality];
      float num1 = (float) source.height / 720f;
      float num2 = num1;
      float num3 = (float) ((double) Mathf.Max(this.focus.nearBlurRadius, this.focus.farBlurRadius) * (double) num2 * 0.75);
      float a = this.focus.nearBlurRadius * num1;
      float b = this.focus.farBlurRadius * num1;
      float maxRadius = Mathf.Max(a, b);
      switch (this.settings.apertureShape)
      {
        case DepthOfField.ApertureShape.Hexagonal:
          maxRadius *= 1.2f;
          break;
        case DepthOfField.ApertureShape.Octogonal:
          maxRadius *= 1.15f;
          break;
      }
      if ((double) maxRadius < 0.5)
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        int width = source.width / 2;
        int height = source.height / 2;
        Vector4 vector4 = new Vector4(a * 0.5f, b * 0.5f, 0.0f, 0.0f);
        RenderTexture temporaryRenderTexture1 = this.m_RTU.GetTemporaryRenderTexture(width, height);
        RenderTexture temporaryRenderTexture2 = this.m_RTU.GetTemporaryRenderTexture(width, height);
        Vector4 blurParams;
        Vector4 blurCoe;
        this.ComputeCocParameters(out blurParams, out blurCoe);
        this.filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
        this.filmicDepthOfFieldMaterial.SetVector("_BlurCoe", blurCoe);
        Graphics.Blit((Texture) source, temporaryRenderTexture2, this.filmicDepthOfFieldMaterial, 4);
        RenderTexture src = temporaryRenderTexture2;
        RenderTexture dst = temporaryRenderTexture1;
        if (this.shouldPerformBokeh)
        {
          RenderTexture temporaryRenderTexture3 = this.m_RTU.GetTemporaryRenderTexture(width, height);
          Graphics.Blit((Texture) src, temporaryRenderTexture3, this.filmicDepthOfFieldMaterial, 1);
          this.filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(0.0f, 1.5f, 0.0f, 1.5f));
          Graphics.Blit((Texture) temporaryRenderTexture3, dst, this.filmicDepthOfFieldMaterial, 0);
          this.filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(1.5f, 0.0f, 0.0f, 1.5f));
          Graphics.Blit((Texture) dst, temporaryRenderTexture3, this.filmicDepthOfFieldMaterial, 0);
          this.textureBokehMaterial.SetTexture("_BlurredColor", (Texture) temporaryRenderTexture3);
          this.textureBokehMaterial.SetFloat("_SpawnHeuristic", this.bokehTexture.spawnHeuristic);
          this.textureBokehMaterial.SetVector("_BokehParams", new Vector4(this.bokehTexture.scale * num2, this.bokehTexture.intensity, this.bokehTexture.threshold, num3));
          Graphics.SetRandomWriteTarget(1, this.computeBufferPoints);
          Graphics.Blit((Texture) src, dst, this.textureBokehMaterial, 1);
          Graphics.ClearRandomWriteTargets();
          DepthOfField.SwapRenderTexture(ref src, ref dst);
          this.m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture3);
        }
        this.filmicDepthOfFieldMaterial.SetVector("_BlurParams", blurParams);
        this.filmicDepthOfFieldMaterial.SetVector("_BlurCoe", vector4);
        RenderTexture renderTexture = (RenderTexture) null;
        if (this.m_CurrentQualitySettings.dilateNearBlur)
        {
          RenderTexture temporaryRenderTexture4 = this.m_RTU.GetTemporaryRenderTexture(width, height, format: RenderTextureFormat.RGHalf);
          renderTexture = this.m_RTU.GetTemporaryRenderTexture(width, height, format: RenderTextureFormat.RGHalf);
          this.filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(0.0f, a * 0.75f, 0.0f, 0.0f));
          Graphics.Blit((Texture) src, temporaryRenderTexture4, this.filmicDepthOfFieldMaterial, 2);
          this.filmicDepthOfFieldMaterial.SetVector("_Offsets", new Vector4(a * 0.75f, 0.0f, 0.0f, 0.0f));
          Graphics.Blit((Texture) temporaryRenderTexture4, renderTexture, this.filmicDepthOfFieldMaterial, 3);
          this.m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture4);
          renderTexture.filterMode = FilterMode.Point;
        }
        if (this.m_CurrentQualitySettings.prefilterBlur)
        {
          Graphics.Blit((Texture) src, dst, this.filmicDepthOfFieldMaterial, 6);
          DepthOfField.SwapRenderTexture(ref src, ref dst);
        }
        switch (this.settings.apertureShape)
        {
          case DepthOfField.ApertureShape.Circular:
            this.DoCircularBlur(renderTexture, ref src, ref dst, maxRadius);
            break;
          case DepthOfField.ApertureShape.Hexagonal:
            this.DoHexagonalBlur(renderTexture, ref src, ref dst, maxRadius);
            break;
          case DepthOfField.ApertureShape.Octogonal:
            this.DoOctogonalBlur(renderTexture, ref src, ref dst, maxRadius);
            break;
        }
        switch (this.m_CurrentQualitySettings.medianFilter)
        {
          case DepthOfField.FilterQuality.Normal:
            this.medianFilterMaterial.SetVector("_Offsets", new Vector4(1f, 0.0f, 0.0f, 0.0f));
            Graphics.Blit((Texture) src, dst, this.medianFilterMaterial, 0);
            DepthOfField.SwapRenderTexture(ref src, ref dst);
            this.medianFilterMaterial.SetVector("_Offsets", new Vector4(0.0f, 1f, 0.0f, 0.0f));
            Graphics.Blit((Texture) src, dst, this.medianFilterMaterial, 0);
            DepthOfField.SwapRenderTexture(ref src, ref dst);
            break;
          case DepthOfField.FilterQuality.High:
            Graphics.Blit((Texture) src, dst, this.medianFilterMaterial, 1);
            DepthOfField.SwapRenderTexture(ref src, ref dst);
            break;
        }
        this.filmicDepthOfFieldMaterial.SetVector("_BlurCoe", vector4);
        this.filmicDepthOfFieldMaterial.SetVector("_Convolved_TexelSize", new Vector4((float) src.width, (float) src.height, 1f / (float) src.width, 1f / (float) src.height));
        this.filmicDepthOfFieldMaterial.SetTexture("_SecondTex", (Texture) src);
        int pass = 11;
        if (this.shouldPerformBokeh)
        {
          RenderTexture temporaryRenderTexture5 = this.m_RTU.GetTemporaryRenderTexture(source.height, source.width, format: source.format);
          Graphics.Blit((Texture) source, temporaryRenderTexture5, this.filmicDepthOfFieldMaterial, pass);
          Graphics.SetRenderTarget(temporaryRenderTexture5);
          ComputeBuffer.CopyCount(this.computeBufferPoints, this.computeBufferDrawArgs, 0);
          this.textureBokehMaterial.SetBuffer("pointBuffer", this.computeBufferPoints);
          this.textureBokehMaterial.SetTexture("_MainTex", (Texture) this.bokehTexture.texture);
          this.textureBokehMaterial.SetVector("_Screen", (Vector4) new Vector3((float) (1.0 / (1.0 * (double) source.width)), (float) (1.0 / (1.0 * (double) source.height)), num3));
          this.textureBokehMaterial.SetPass(0);
          Graphics.DrawProceduralIndirect(MeshTopology.Points, this.computeBufferDrawArgs, 0);
          Graphics.Blit((Texture) temporaryRenderTexture5, destination);
        }
        else
          Graphics.Blit((Texture) source, destination, this.filmicDepthOfFieldMaterial, pass);
      }
    }

    private void DoHexagonalBlur(
      RenderTexture blurredFgCoc,
      ref RenderTexture src,
      ref RenderTexture dst,
      float maxRadius)
    {
      this.ComputeBlurDirections(false);
      int blurPass;
      int blurAndMergePass;
      DepthOfField.GetDirectionalBlurPassesFromRadius(blurredFgCoc, maxRadius, out blurPass, out blurAndMergePass);
      this.filmicDepthOfFieldMaterial.SetTexture("_SecondTex", (Texture) blurredFgCoc);
      RenderTexture temporaryRenderTexture = this.m_RTU.GetTemporaryRenderTexture(src.width, src.height, format: src.format);
      this.filmicDepthOfFieldMaterial.SetVector("_Offsets", this.m_HexagonalBokehDirection1);
      Graphics.Blit((Texture) src, temporaryRenderTexture, this.filmicDepthOfFieldMaterial, blurPass);
      this.filmicDepthOfFieldMaterial.SetVector("_Offsets", this.m_HexagonalBokehDirection2);
      Graphics.Blit((Texture) temporaryRenderTexture, src, this.filmicDepthOfFieldMaterial, blurPass);
      this.filmicDepthOfFieldMaterial.SetVector("_Offsets", this.m_HexagonalBokehDirection3);
      this.filmicDepthOfFieldMaterial.SetTexture("_ThirdTex", (Texture) src);
      Graphics.Blit((Texture) temporaryRenderTexture, dst, this.filmicDepthOfFieldMaterial, blurAndMergePass);
      this.m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture);
      DepthOfField.SwapRenderTexture(ref src, ref dst);
    }

    private void DoOctogonalBlur(
      RenderTexture blurredFgCoc,
      ref RenderTexture src,
      ref RenderTexture dst,
      float maxRadius)
    {
      this.ComputeBlurDirections(false);
      int blurPass;
      int blurAndMergePass;
      DepthOfField.GetDirectionalBlurPassesFromRadius(blurredFgCoc, maxRadius, out blurPass, out blurAndMergePass);
      this.filmicDepthOfFieldMaterial.SetTexture("_SecondTex", (Texture) blurredFgCoc);
      RenderTexture temporaryRenderTexture = this.m_RTU.GetTemporaryRenderTexture(src.width, src.height, format: src.format);
      this.filmicDepthOfFieldMaterial.SetVector("_Offsets", this.m_OctogonalBokehDirection1);
      Graphics.Blit((Texture) src, temporaryRenderTexture, this.filmicDepthOfFieldMaterial, blurPass);
      this.filmicDepthOfFieldMaterial.SetVector("_Offsets", this.m_OctogonalBokehDirection2);
      Graphics.Blit((Texture) temporaryRenderTexture, dst, this.filmicDepthOfFieldMaterial, blurPass);
      this.filmicDepthOfFieldMaterial.SetVector("_Offsets", this.m_OctogonalBokehDirection3);
      Graphics.Blit((Texture) src, temporaryRenderTexture, this.filmicDepthOfFieldMaterial, blurPass);
      this.filmicDepthOfFieldMaterial.SetVector("_Offsets", this.m_OctogonalBokehDirection4);
      this.filmicDepthOfFieldMaterial.SetTexture("_ThirdTex", (Texture) dst);
      Graphics.Blit((Texture) temporaryRenderTexture, src, this.filmicDepthOfFieldMaterial, blurAndMergePass);
      this.m_RTU.ReleaseTemporaryRenderTexture(temporaryRenderTexture);
    }

    private void DoCircularBlur(
      RenderTexture blurredFgCoc,
      ref RenderTexture src,
      ref RenderTexture dst,
      float maxRadius)
    {
      int pass;
      if ((UnityEngine.Object) blurredFgCoc != (UnityEngine.Object) null)
      {
        this.filmicDepthOfFieldMaterial.SetTexture("_SecondTex", (Texture) blurredFgCoc);
        pass = (double) maxRadius > 10.0 ? 8 : 10;
      }
      else
        pass = (double) maxRadius > 10.0 ? 7 : 9;
      Graphics.Blit((Texture) src, dst, this.filmicDepthOfFieldMaterial, pass);
      DepthOfField.SwapRenderTexture(ref src, ref dst);
    }

    private void ComputeCocParameters(out Vector4 blurParams, out Vector4 blurCoe)
    {
      Camera component = this.GetComponent<Camera>();
      float num1 = this.focus.nearFalloff * 2f;
      float num2 = this.focus.farFalloff * 2f;
      float num3 = this.focus.nearPlane;
      float num4 = this.focus.farPlane;
      if (this.settings.tweakMode == DepthOfField.TweakMode.Range)
      {
        float num5 = !((UnityEngine.Object) this.focus.transform != (UnityEngine.Object) null) ? this.focus.focusPlane : component.WorldToViewportPoint(this.focus.transform.position).z;
        float num6 = this.focus.range * 0.5f;
        num3 = num5 - num6;
        num4 = num5 + num6;
      }
      float num7 = num3 - num1 * 0.5f;
      float num8 = num4 + num2 * 0.5f;
      float num9 = (float) (((double) num7 + (double) num8) * 0.5) / component.farClipPlane;
      float num10 = num7 / component.farClipPlane;
      float num11 = num8 / component.farClipPlane;
      float num12 = num8 - num7;
      float num13 = num11 - num10;
      float num14 = num1 / num12;
      float num15 = num2 / num12;
      float num16 = (float) ((1.0 - (double) num14) * ((double) num13 * 0.5));
      float num17 = (float) ((1.0 - (double) num15) * ((double) num13 * 0.5));
      if ((double) num9 <= (double) num10)
        num9 = num10 + 1E-06f;
      if ((double) num9 >= (double) num11)
        num9 = num11 - 1E-06f;
      if ((double) num9 - (double) num16 <= (double) num10)
        num16 = (float) ((double) num9 - (double) num10 - 9.9999999747524271E-07);
      if ((double) num9 + (double) num17 >= (double) num11)
        num17 = (float) ((double) num11 - (double) num9 - 9.9999999747524271E-07);
      float num18 = (float) (1.0 / ((double) num10 - (double) num9 + (double) num16));
      float num19 = (float) (1.0 / ((double) num11 - (double) num9 - (double) num17));
      float num20 = (float) (1.0 - (double) num18 * (double) num10);
      float num21 = (float) (1.0 - (double) num19 * (double) num11);
      blurParams = new Vector4(-1f * num18, -1f * num20, 1f * num19, 1f * num21);
      blurCoe = new Vector4(0.0f, 0.0f, (float) (((double) num21 - (double) num20) / ((double) num18 - (double) num19)), 0.0f);
      this.focus.nearPlane = num7 + num1 * 0.5f;
      this.focus.farPlane = num8 - num2 * 0.5f;
      this.focus.focusPlane = (float) (((double) this.focus.nearPlane + (double) this.focus.farPlane) * 0.5);
      this.focus.range = this.focus.farPlane - this.focus.nearPlane;
    }

    private void ReleaseComputeResources()
    {
      if (this.m_ComputeBufferDrawArgs != null)
        this.m_ComputeBufferDrawArgs.Release();
      if (this.m_ComputeBufferPoints != null)
        this.m_ComputeBufferPoints.Release();
      this.m_ComputeBufferDrawArgs = (ComputeBuffer) null;
      this.m_ComputeBufferPoints = (ComputeBuffer) null;
    }

    private void ComputeBlurDirections(bool force)
    {
      if (!force && (double) Math.Abs(this.m_LastApertureOrientation - this.settings.apertureOrientation) < 1.4012984643248171E-45)
        return;
      this.m_LastApertureOrientation = this.settings.apertureOrientation;
      float f = this.settings.apertureOrientation * ((float) Math.PI / 180f);
      float cosinus = Mathf.Cos(f);
      float sinus = Mathf.Sin(f);
      this.m_OctogonalBokehDirection1 = new Vector4(0.5f, 0.0f, 0.0f, 0.0f);
      this.m_OctogonalBokehDirection2 = new Vector4(0.0f, 0.5f, 1f, 0.0f);
      this.m_OctogonalBokehDirection3 = new Vector4(-0.353553f, 0.353553f, 1f, 0.0f);
      this.m_OctogonalBokehDirection4 = new Vector4(0.353553f, 0.353553f, 1f, 0.0f);
      this.m_HexagonalBokehDirection1 = new Vector4(0.5f, 0.0f, 0.0f, 0.0f);
      this.m_HexagonalBokehDirection2 = new Vector4(0.25f, 0.433013f, 1f, 0.0f);
      this.m_HexagonalBokehDirection3 = new Vector4(0.25f, -0.433013f, 1f, 0.0f);
      if ((double) f <= 1.4012984643248171E-45)
        return;
      DepthOfField.Rotate2D(ref this.m_OctogonalBokehDirection1, cosinus, sinus);
      DepthOfField.Rotate2D(ref this.m_OctogonalBokehDirection2, cosinus, sinus);
      DepthOfField.Rotate2D(ref this.m_OctogonalBokehDirection3, cosinus, sinus);
      DepthOfField.Rotate2D(ref this.m_OctogonalBokehDirection4, cosinus, sinus);
      DepthOfField.Rotate2D(ref this.m_HexagonalBokehDirection1, cosinus, sinus);
      DepthOfField.Rotate2D(ref this.m_HexagonalBokehDirection2, cosinus, sinus);
      DepthOfField.Rotate2D(ref this.m_HexagonalBokehDirection3, cosinus, sinus);
    }

    private bool shouldPerformBokeh
    {
      get
      {
        return ImageEffectHelper.supportsDX11 && (UnityEngine.Object) this.bokehTexture.texture != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.textureBokehMaterial;
      }
    }

    private static void Rotate2D(ref Vector4 direction, float cosinus, float sinus)
    {
      Vector4 vector4 = direction;
      direction.x = (float) ((double) vector4.x * (double) cosinus - (double) vector4.y * (double) sinus);
      direction.y = (float) ((double) vector4.x * (double) sinus + (double) vector4.y * (double) cosinus);
    }

    private static void SwapRenderTexture(ref RenderTexture src, ref RenderTexture dst)
    {
      RenderTexture renderTexture = dst;
      dst = src;
      src = renderTexture;
    }

    private static void GetDirectionalBlurPassesFromRadius(
      RenderTexture blurredFgCoc,
      float maxRadius,
      out int blurPass,
      out int blurAndMergePass)
    {
      if ((UnityEngine.Object) blurredFgCoc == (UnityEngine.Object) null)
      {
        if ((double) maxRadius > 10.0)
        {
          blurPass = 20;
          blurAndMergePass = 22;
        }
        else if ((double) maxRadius > 5.0)
        {
          blurPass = 16;
          blurAndMergePass = 18;
        }
        else
        {
          blurPass = 12;
          blurAndMergePass = 14;
        }
      }
      else if ((double) maxRadius > 10.0)
      {
        blurPass = 21;
        blurAndMergePass = 23;
      }
      else if ((double) maxRadius > 5.0)
      {
        blurPass = 17;
        blurAndMergePass = 19;
      }
      else
      {
        blurPass = 13;
        blurAndMergePass = 15;
      }
    }

    private enum Passes
    {
      BlurAlphaWeighted,
      BoxBlur,
      DilateFgCocFromColor,
      DilateFgCoc,
      CaptureCocExplicit,
      VisualizeCocExplicit,
      CocPrefilter,
      CircleBlur,
      CircleBlurWithDilatedFg,
      CircleBlurLowQuality,
      CircleBlowLowQualityWithDilatedFg,
      MergeExplicit,
      ShapeLowQuality,
      ShapeLowQualityDilateFg,
      ShapeLowQualityMerge,
      ShapeLowQualityMergeDilateFg,
      ShapeMediumQuality,
      ShapeMediumQualityDilateFg,
      ShapeMediumQualityMerge,
      ShapeMediumQualityMergeDilateFg,
      ShapeHighQuality,
      ShapeHighQualityDilateFg,
      ShapeHighQualityMerge,
      ShapeHighQualityMergeDilateFg,
    }

    private enum MedianPasses
    {
      Median3,
      Median3X3,
    }

    private enum BokehTexturesPasses
    {
      Apply,
      Collect,
    }

    public enum TweakMode
    {
      Range,
      Explicit,
    }

    public enum ApertureShape
    {
      Circular,
      Hexagonal,
      Octogonal,
    }

    public enum QualityPreset
    {
      Low,
      Medium,
      High,
    }

    public enum FilterQuality
    {
      None,
      Normal,
      High,
    }

    [Serializable]
    public struct GlobalSettings
    {
      [Tooltip("Allows to view where the blur will be applied. Yellow for near blur, blue for far blur.")]
      public bool visualizeFocus;
      [Tooltip("Setup mode. Use \"Advanced\" if you need more control on blur settings and/or want to use a bokeh texture. \"Explicit\" is the same as \"Advanced\" but makes use of \"Near Plane\" and \"Far Plane\" values instead of \"F-Stop\".")]
      public DepthOfField.TweakMode tweakMode;
      [Tooltip("Quality presets. Use \"Custom\" for more advanced settings.")]
      public DepthOfField.QualityPreset filteringQuality;
      [Tooltip("\"Circular\" is the fastest, followed by \"Hexagonal\" and \"Octogonal\".")]
      public DepthOfField.ApertureShape apertureShape;
      [Range(0.0f, 179f)]
      [Tooltip("Rotates the aperture when working with \"Hexagonal\" and \"Ortogonal\".")]
      public float apertureOrientation;

      public static DepthOfField.GlobalSettings defaultSettings
      {
        get
        {
          return new DepthOfField.GlobalSettings()
          {
            visualizeFocus = false,
            tweakMode = DepthOfField.TweakMode.Range,
            filteringQuality = DepthOfField.QualityPreset.High,
            apertureShape = DepthOfField.ApertureShape.Circular,
            apertureOrientation = 0.0f
          };
        }
      }
    }

    [Serializable]
    public struct QualitySettings
    {
      [Tooltip("Enable this to get smooth bokeh.")]
      public bool prefilterBlur;
      [Tooltip("Applies a median filter for even smoother bokeh.")]
      public DepthOfField.FilterQuality medianFilter;
      [Tooltip("Dilates near blur over in focus area.")]
      public bool dilateNearBlur;
      public static DepthOfField.QualitySettings[] presetQualitySettings = new DepthOfField.QualitySettings[3]
      {
        new DepthOfField.QualitySettings()
        {
          prefilterBlur = false,
          medianFilter = DepthOfField.FilterQuality.None,
          dilateNearBlur = false
        },
        new DepthOfField.QualitySettings()
        {
          prefilterBlur = true,
          medianFilter = DepthOfField.FilterQuality.Normal,
          dilateNearBlur = false
        },
        new DepthOfField.QualitySettings()
        {
          prefilterBlur = true,
          medianFilter = DepthOfField.FilterQuality.High,
          dilateNearBlur = true
        }
      };
    }

    [Serializable]
    public struct FocusSettings
    {
      [Tooltip("Auto-focus on a selected transform.")]
      public Transform transform;
      [Min(0.0f)]
      [Tooltip("Focus distance (in world units).")]
      public float focusPlane;
      [Min(0.1f)]
      [Tooltip("Focus range (in world units). The focus plane is located in the center of the range.")]
      public float range;
      [Min(0.0f)]
      [Tooltip("Near focus distance (in world units).")]
      public float nearPlane;
      [Min(0.0f)]
      [Tooltip("Near blur falloff (in world units).")]
      public float nearFalloff;
      [Min(0.0f)]
      [Tooltip("Far focus distance (in world units).")]
      public float farPlane;
      [Min(0.0f)]
      [Tooltip("Far blur falloff (in world units).")]
      public float farFalloff;
      [Range(0.0f, 40f)]
      [Tooltip("Maximum blur radius for the near plane.")]
      public float nearBlurRadius;
      [Range(0.0f, 40f)]
      [Tooltip("Maximum blur radius for the far plane.")]
      public float farBlurRadius;

      public static DepthOfField.FocusSettings defaultSettings
      {
        get
        {
          return new DepthOfField.FocusSettings()
          {
            transform = (Transform) null,
            focusPlane = 20f,
            range = 35f,
            nearPlane = 2.5f,
            nearFalloff = 15f,
            farPlane = 37.5f,
            farFalloff = 50f,
            nearBlurRadius = 15f,
            farBlurRadius = 20f
          };
        }
      }
    }

    [Serializable]
    public struct BokehTextureSettings
    {
      [Tooltip("Adding a texture to this field will enable the use of \"Bokeh Textures\". Use with care. This feature is only available on Shader Model 5 compatible-hardware and performance scale with the amount of bokeh.")]
      public Texture2D texture;
      [Range(0.01f, 10f)]
      [Tooltip("Maximum size of bokeh textures on screen.")]
      public float scale;
      [Range(0.01f, 100f)]
      [Tooltip("Bokeh brightness.")]
      public float intensity;
      [Range(0.01f, 5f)]
      [Tooltip("Controls the amount of bokeh textures. Lower values mean more bokeh splats.")]
      public float threshold;
      [Range(0.01f, 1f)]
      [Tooltip("Controls the spawn conditions. Lower values mean more visible bokeh.")]
      public float spawnHeuristic;

      public static DepthOfField.BokehTextureSettings defaultSettings
      {
        get
        {
          return new DepthOfField.BokehTextureSettings()
          {
            texture = (Texture2D) null,
            scale = 1f,
            intensity = 50f,
            threshold = 2f,
            spawnHeuristic = 0.15f
          };
        }
      }
    }
  }
}
