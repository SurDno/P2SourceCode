using System;

namespace AmplifyBloom
{
  [AddComponentMenu("")]
  [Serializable]
  public class AmplifyBloomBase : MonoBehaviour
  {
    public const int MaxGhosts = 5;
    public const int MinDownscales = 1;
    public const int MaxDownscales = 6;
    public const int MaxGaussian = 8;
    private const float MaxDirtIntensity = 1f;
    private const float MaxStarburstIntensity = 1f;
    [SerializeField]
    private Texture m_maskTexture = (Texture) null;
    [SerializeField]
    private RenderTexture m_targetTexture = (RenderTexture) null;
    [SerializeField]
    private bool m_showDebugMessages = true;
    [SerializeField]
    private int m_softMaxdownscales = 6;
    [SerializeField]
    private DebugToScreenEnum m_debugToScreen = DebugToScreenEnum.None;
    [SerializeField]
    private bool m_highPrecision;
    [SerializeField]
    private Vector4 m_bloomRange = new Vector4(500f, 1f, 0.0f, 0.0f);
    [SerializeField]
    private float m_overallThreshold = 0.53f;
    [SerializeField]
    private Vector4 m_bloomParams = new Vector4(0.8f, 1f, 1f, 1f);
    [SerializeField]
    private bool m_temporalFilteringActive;
    [SerializeField]
    private float m_temporalFilteringValue = 0.05f;
    [SerializeField]
    private int m_bloomDownsampleCount = 6;
    [SerializeField]
    private AnimationCurve m_temporalFilteringCurve;
    [SerializeField]
    private bool m_separateFeaturesThreshold;
    [SerializeField]
    private float m_featuresThreshold = 0.05f;
    [SerializeField]
    private AmplifyLensFlare m_lensFlare = new AmplifyLensFlare();
    [SerializeField]
    private bool m_applyLensDirt = true;
    [SerializeField]
    private float m_lensDirtStrength = 2f;
    [SerializeField]
    private Texture m_lensDirtTexture;
    [SerializeField]
    private bool m_applyLensStardurst = true;
    [SerializeField]
    private Texture m_lensStardurstTex;
    [SerializeField]
    private float m_lensStarburstStrength = 2f;
    [SerializeField]
    private AmplifyGlare m_anamorphicGlare = new AmplifyGlare();
    [SerializeField]
    private AmplifyBokeh m_bokehFilter = new AmplifyBokeh();
    [SerializeField]
    private float[] m_upscaleWeights = new float[6]
    {
      0.0842f,
      0.1282f,
      0.1648f,
      0.2197f,
      0.2197f,
      0.1831f
    };
    [SerializeField]
    private float[] m_gaussianRadius = new float[6]
    {
      1f,
      1f,
      1f,
      1f,
      1f,
      1f
    };
    [SerializeField]
    private int[] m_gaussianSteps = new int[6]
    {
      1,
      1,
      1,
      1,
      1,
      1
    };
    [SerializeField]
    private float[] m_lensDirtWeights = new float[6]
    {
      0.067f,
      0.102f,
      0.1311f,
      0.1749f,
      0.2332f,
      0.3f
    };
    [SerializeField]
    private float[] m_lensStarburstWeights = new float[6]
    {
      0.067f,
      0.102f,
      0.1311f,
      0.1749f,
      0.2332f,
      0.3f
    };
    [SerializeField]
    private bool[] m_downscaleSettingsFoldout = new bool[6];
    [SerializeField]
    private int m_featuresSourceId;
    [SerializeField]
    private UpscaleQualityEnum m_upscaleQuality = UpscaleQualityEnum.Realistic;
    [SerializeField]
    private MainThresholdSizeEnum m_mainThresholdSize = MainThresholdSizeEnum.Full;
    private Transform m_cameraTransform;
    private Matrix4x4 m_starburstMat;
    private Shader m_bloomShader;
    private Material m_bloomMaterial;
    private Shader m_finalCompositionShader;
    private Material m_finalCompositionMaterial;
    private RenderTexture m_tempFilterBuffer;
    private Camera m_camera;
    private RenderTexture[] m_tempUpscaleRTs = new RenderTexture[6];
    private RenderTexture[] m_tempAuxDownsampleRTs = new RenderTexture[6];
    private Vector2[] m_tempDownsamplesSizes = new Vector2[6];
    private bool silentError;

    private void Awake()
    {
      if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
      {
        AmplifyUtils.DebugLog("Null graphics device detected. Skipping effect silently.", LogType.Error);
        silentError = true;
      }
      else
      {
        if (!AmplifyUtils.IsInitialized)
          AmplifyUtils.InitializeIds();
        for (int index = 0; index < 6; ++index)
          m_tempDownsamplesSizes[index] = new Vector2(0.0f, 0.0f);
        m_cameraTransform = this.transform;
        m_tempFilterBuffer = (RenderTexture) null;
        m_starburstMat = Matrix4x4.identity;
        if (m_temporalFilteringCurve == null)
          m_temporalFilteringCurve = new AnimationCurve(new Keyframe[2]
          {
            new Keyframe(0.0f, 0.0f),
            new Keyframe(1f, 0.999f)
          });
        m_bloomShader = Shader.Find("Hidden/AmplifyBloom");
        if ((UnityEngine.Object) m_bloomShader != (UnityEngine.Object) null)
        {
          m_bloomMaterial = new Material(m_bloomShader);
          m_bloomMaterial.hideFlags = HideFlags.DontSave;
        }
        else
        {
          AmplifyUtils.DebugLog("Main Bloom shader not found", LogType.Error);
          this.gameObject.SetActive(false);
        }
        m_finalCompositionShader = Shader.Find("Hidden/BloomFinal");
        if ((UnityEngine.Object) m_finalCompositionShader != (UnityEngine.Object) null)
        {
          m_finalCompositionMaterial = new Material(m_finalCompositionShader);
          if (!m_finalCompositionMaterial.GetTag(AmplifyUtils.ShaderModeTag, false).Equals(AmplifyUtils.ShaderModeValue))
          {
            if (m_showDebugMessages)
              AmplifyUtils.DebugLog("Amplify Bloom is running on a limited hardware and may lead to a decrease on its visual quality.", LogType.Warning);
          }
          else
            m_softMaxdownscales = 6;
          m_finalCompositionMaterial.hideFlags = HideFlags.DontSave;
          if ((UnityEngine.Object) m_lensDirtTexture == (UnityEngine.Object) null)
            m_lensDirtTexture = m_finalCompositionMaterial.GetTexture(AmplifyUtils.LensDirtRTId);
          if ((UnityEngine.Object) m_lensStardurstTex == (UnityEngine.Object) null)
            m_lensStardurstTex = m_finalCompositionMaterial.GetTexture(AmplifyUtils.LensStarburstRTId);
        }
        else
        {
          AmplifyUtils.DebugLog("Bloom Composition shader not found", LogType.Error);
          this.gameObject.SetActive(false);
        }
        m_camera = this.GetComponent<Camera>();
        m_camera.depthTextureMode |= DepthTextureMode.Depth;
        m_lensFlare.CreateLUTexture();
      }
    }

    private void OnDestroy()
    {
      if (m_bokehFilter != null)
      {
        m_bokehFilter.Destroy();
        m_bokehFilter = null;
      }
      if (m_anamorphicGlare != null)
      {
        m_anamorphicGlare.Destroy();
        m_anamorphicGlare = null;
      }
      if (m_lensFlare == null)
        return;
      m_lensFlare.Destroy();
      m_lensFlare = null;
    }

    private void ApplyGaussianBlur(
      RenderTexture renderTexture,
      int amount,
      float radius = 1f,
      bool applyTemporal = false)
    {
      if (amount == 0)
        return;
      m_bloomMaterial.SetFloat(AmplifyUtils.BlurRadiusId, radius);
      RenderTexture tempRenderTarget = AmplifyUtils.GetTempRenderTarget(renderTexture.width, renderTexture.height);
      for (int index = 0; index < amount; ++index)
      {
        tempRenderTarget.DiscardContents();
        Graphics.Blit((Texture) renderTexture, tempRenderTarget, m_bloomMaterial, 14);
        if (m_temporalFilteringActive & applyTemporal && index == amount - 1)
        {
          if ((UnityEngine.Object) m_tempFilterBuffer != (UnityEngine.Object) null && m_temporalFilteringActive)
          {
            float num = m_temporalFilteringCurve.Evaluate(m_temporalFilteringValue);
            m_bloomMaterial.SetFloat(AmplifyUtils.TempFilterValueId, num);
            m_bloomMaterial.SetTexture(AmplifyUtils.AnamorphicRTS[0], (Texture) m_tempFilterBuffer);
            renderTexture.DiscardContents();
            Graphics.Blit((Texture) tempRenderTarget, renderTexture, m_bloomMaterial, 16);
          }
          else
          {
            renderTexture.DiscardContents();
            Graphics.Blit((Texture) tempRenderTarget, renderTexture, m_bloomMaterial, 15);
          }
          bool flag = false;
          if ((UnityEngine.Object) m_tempFilterBuffer != (UnityEngine.Object) null)
          {
            if (m_tempFilterBuffer.format != renderTexture.format || m_tempFilterBuffer.width != renderTexture.width || m_tempFilterBuffer.height != renderTexture.height)
            {
              CleanTempFilterRT();
              flag = true;
            }
          }
          else
            flag = true;
          if (flag)
            CreateTempFilterRT(renderTexture);
          m_tempFilterBuffer.DiscardContents();
          Graphics.Blit((Texture) renderTexture, m_tempFilterBuffer);
        }
        else
        {
          renderTexture.DiscardContents();
          Graphics.Blit((Texture) tempRenderTarget, renderTexture, m_bloomMaterial, 15);
        }
      }
      AmplifyUtils.ReleaseTempRenderTarget(tempRenderTarget);
    }

    private void CreateTempFilterRT(RenderTexture source)
    {
      if ((UnityEngine.Object) m_tempFilterBuffer != (UnityEngine.Object) null)
        CleanTempFilterRT();
      m_tempFilterBuffer = new RenderTexture(source.width, source.height, 0, source.format, AmplifyUtils.CurrentReadWriteMode);
      m_tempFilterBuffer.filterMode = AmplifyUtils.CurrentFilterMode;
      m_tempFilterBuffer.wrapMode = AmplifyUtils.CurrentWrapMode;
      m_tempFilterBuffer.Create();
    }

    private void CleanTempFilterRT()
    {
      if (!((UnityEngine.Object) m_tempFilterBuffer != (UnityEngine.Object) null))
        return;
      RenderTexture.active = (RenderTexture) null;
      m_tempFilterBuffer.Release();
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) m_tempFilterBuffer);
      m_tempFilterBuffer = (RenderTexture) null;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
      if (silentError)
        return;
      if (!AmplifyUtils.IsInitialized)
        AmplifyUtils.InitializeIds();
      if (m_highPrecision)
      {
        AmplifyUtils.EnsureKeywordEnabled(m_bloomMaterial, AmplifyUtils.HighPrecisionKeyword, true);
        AmplifyUtils.EnsureKeywordEnabled(m_finalCompositionMaterial, AmplifyUtils.HighPrecisionKeyword, true);
        AmplifyUtils.CurrentRTFormat = RenderTextureFormat.DefaultHDR;
      }
      else
      {
        AmplifyUtils.EnsureKeywordEnabled(m_bloomMaterial, AmplifyUtils.HighPrecisionKeyword, false);
        AmplifyUtils.EnsureKeywordEnabled(m_finalCompositionMaterial, AmplifyUtils.HighPrecisionKeyword, false);
        AmplifyUtils.CurrentRTFormat = RenderTextureFormat.Default;
      }
      float num1 = Mathf.Acos(Vector3.Dot(m_cameraTransform.right, Vector3.right));
      if ((double) Vector3.Cross(m_cameraTransform.right, Vector3.right).y > 0.0)
        num1 = -num1;
      RenderTexture renderTexture1 = (RenderTexture) null;
      RenderTexture renderTexture2 = (RenderTexture) null;
      if (!m_highPrecision)
      {
        m_bloomRange.y = 1f / m_bloomRange.x;
        m_bloomMaterial.SetVector(AmplifyUtils.BloomRangeId, m_bloomRange);
        m_finalCompositionMaterial.SetVector(AmplifyUtils.BloomRangeId, m_bloomRange);
      }
      m_bloomParams.y = m_overallThreshold;
      m_bloomMaterial.SetVector(AmplifyUtils.BloomParamsId, m_bloomParams);
      m_finalCompositionMaterial.SetVector(AmplifyUtils.BloomParamsId, m_bloomParams);
      int num2 = 1;
      switch (m_mainThresholdSize)
      {
        case MainThresholdSizeEnum.Half:
          num2 = 2;
          break;
        case MainThresholdSizeEnum.Quarter:
          num2 = 4;
          break;
      }
      RenderTexture tempRenderTarget = AmplifyUtils.GetTempRenderTarget(src.width / num2, src.height / num2);
      if ((UnityEngine.Object) m_maskTexture != (UnityEngine.Object) null)
      {
        m_bloomMaterial.SetTexture(AmplifyUtils.MaskTextureId, m_maskTexture);
        Graphics.Blit((Texture) src, tempRenderTarget, m_bloomMaterial, 1);
      }
      else
        Graphics.Blit((Texture) src, tempRenderTarget, m_bloomMaterial, 0);
      if (m_debugToScreen == DebugToScreenEnum.MainThreshold)
      {
        Graphics.Blit((Texture) tempRenderTarget, dest, m_bloomMaterial, 33);
        AmplifyUtils.ReleaseAllRT();
      }
      else
      {
        bool flag1 = true;
        RenderTexture renderTexture3 = tempRenderTarget;
        if (m_bloomDownsampleCount > 0)
        {
          flag1 = false;
          int width = tempRenderTarget.width;
          int height = tempRenderTarget.height;
          for (int index = 0; index < m_bloomDownsampleCount; ++index)
          {
            m_tempDownsamplesSizes[index].x = (float) width;
            m_tempDownsamplesSizes[index].y = (float) height;
            width = width + 1 >> 1;
            height = height + 1 >> 1;
            m_tempAuxDownsampleRTs[index] = AmplifyUtils.GetTempRenderTarget(width, height);
            if (index == 0)
            {
              if (!m_temporalFilteringActive || m_gaussianSteps[index] != 0)
              {
                if (m_upscaleQuality == UpscaleQualityEnum.Realistic)
                  Graphics.Blit((Texture) renderTexture3, m_tempAuxDownsampleRTs[index], m_bloomMaterial, 10);
                else
                  Graphics.Blit((Texture) renderTexture3, m_tempAuxDownsampleRTs[index], m_bloomMaterial, 11);
              }
              else
              {
                if ((UnityEngine.Object) m_tempFilterBuffer != (UnityEngine.Object) null && m_temporalFilteringActive)
                {
                  float num3 = m_temporalFilteringCurve.Evaluate(m_temporalFilteringValue);
                  m_bloomMaterial.SetFloat(AmplifyUtils.TempFilterValueId, num3);
                  m_bloomMaterial.SetTexture(AmplifyUtils.AnamorphicRTS[0], (Texture) m_tempFilterBuffer);
                  if (m_upscaleQuality == UpscaleQualityEnum.Realistic)
                    Graphics.Blit((Texture) renderTexture3, m_tempAuxDownsampleRTs[index], m_bloomMaterial, 12);
                  else
                    Graphics.Blit((Texture) renderTexture3, m_tempAuxDownsampleRTs[index], m_bloomMaterial, 13);
                }
                else if (m_upscaleQuality == UpscaleQualityEnum.Realistic)
                  Graphics.Blit((Texture) renderTexture3, m_tempAuxDownsampleRTs[index], m_bloomMaterial, 10);
                else
                  Graphics.Blit((Texture) renderTexture3, m_tempAuxDownsampleRTs[index], m_bloomMaterial, 11);
                bool flag2 = false;
                if ((UnityEngine.Object) m_tempFilterBuffer != (UnityEngine.Object) null)
                {
                  if (m_tempFilterBuffer.format != m_tempAuxDownsampleRTs[index].format || m_tempFilterBuffer.width != m_tempAuxDownsampleRTs[index].width || m_tempFilterBuffer.height != m_tempAuxDownsampleRTs[index].height)
                  {
                    CleanTempFilterRT();
                    flag2 = true;
                  }
                }
                else
                  flag2 = true;
                if (flag2)
                  CreateTempFilterRT(m_tempAuxDownsampleRTs[index]);
                m_tempFilterBuffer.DiscardContents();
                Graphics.Blit((Texture) m_tempAuxDownsampleRTs[index], m_tempFilterBuffer);
                if (m_debugToScreen == DebugToScreenEnum.TemporalFilter)
                {
                  Graphics.Blit((Texture) m_tempAuxDownsampleRTs[index], dest);
                  AmplifyUtils.ReleaseAllRT();
                  return;
                }
              }
            }
            else
              Graphics.Blit((Texture) m_tempAuxDownsampleRTs[index - 1], m_tempAuxDownsampleRTs[index], m_bloomMaterial, 9);
            if (m_gaussianSteps[index] > 0)
            {
              ApplyGaussianBlur(m_tempAuxDownsampleRTs[index], m_gaussianSteps[index], m_gaussianRadius[index], index == 0);
              if (m_temporalFilteringActive && m_debugToScreen == DebugToScreenEnum.TemporalFilter)
              {
                Graphics.Blit((Texture) m_tempAuxDownsampleRTs[index], dest);
                AmplifyUtils.ReleaseAllRT();
                return;
              }
            }
          }
          renderTexture3 = m_tempAuxDownsampleRTs[m_featuresSourceId];
          AmplifyUtils.ReleaseTempRenderTarget(tempRenderTarget);
        }
        if (m_bokehFilter.ApplyBokeh && m_bokehFilter.ApplyOnBloomSource)
        {
          m_bokehFilter.ApplyBokehFilter(renderTexture3, m_bloomMaterial);
          if (m_debugToScreen == DebugToScreenEnum.BokehFilter)
          {
            Graphics.Blit((Texture) renderTexture3, dest);
            AmplifyUtils.ReleaseAllRT();
            return;
          }
        }
        bool flag3 = false;
        RenderTexture renderTexture4;
        if (m_separateFeaturesThreshold)
        {
          m_bloomParams.y = m_featuresThreshold;
          m_bloomMaterial.SetVector(AmplifyUtils.BloomParamsId, m_bloomParams);
          m_finalCompositionMaterial.SetVector(AmplifyUtils.BloomParamsId, m_bloomParams);
          renderTexture4 = AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
          flag3 = true;
          Graphics.Blit((Texture) renderTexture3, renderTexture4, m_bloomMaterial, 0);
          if (m_debugToScreen == DebugToScreenEnum.FeaturesThreshold)
          {
            Graphics.Blit((Texture) renderTexture4, dest);
            AmplifyUtils.ReleaseAllRT();
            return;
          }
        }
        else
          renderTexture4 = renderTexture3;
        if (m_bokehFilter.ApplyBokeh && !m_bokehFilter.ApplyOnBloomSource)
        {
          if (!flag3)
          {
            flag3 = true;
            renderTexture4 = AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
            Graphics.Blit((Texture) renderTexture3, renderTexture4);
          }
          m_bokehFilter.ApplyBokehFilter(renderTexture4, m_bloomMaterial);
          if (m_debugToScreen == DebugToScreenEnum.BokehFilter)
          {
            Graphics.Blit((Texture) renderTexture4, dest);
            AmplifyUtils.ReleaseAllRT();
            return;
          }
        }
        if (m_lensFlare.ApplyLensFlare && m_debugToScreen != DebugToScreenEnum.Bloom)
        {
          renderTexture1 = m_lensFlare.ApplyFlare(m_bloomMaterial, renderTexture4);
          ApplyGaussianBlur(renderTexture1, m_lensFlare.LensFlareGaussianBlurAmount);
          if (m_debugToScreen == DebugToScreenEnum.LensFlare)
          {
            Graphics.Blit((Texture) renderTexture1, dest);
            AmplifyUtils.ReleaseAllRT();
            return;
          }
        }
        if (m_anamorphicGlare.ApplyLensGlare && m_debugToScreen != DebugToScreenEnum.Bloom)
        {
          renderTexture2 = AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
          m_anamorphicGlare.OnRenderImage(m_bloomMaterial, renderTexture4, renderTexture2, num1);
          if (m_debugToScreen == DebugToScreenEnum.LensGlare)
          {
            Graphics.Blit((Texture) renderTexture2, dest);
            AmplifyUtils.ReleaseAllRT();
            return;
          }
        }
        if (flag3)
          AmplifyUtils.ReleaseTempRenderTarget(renderTexture4);
        if (flag1)
          ApplyGaussianBlur(renderTexture3, m_gaussianSteps[0], m_gaussianRadius[0]);
        if (m_bloomDownsampleCount > 0)
        {
          if (m_bloomDownsampleCount == 1)
          {
            if (m_upscaleQuality == UpscaleQualityEnum.Realistic)
            {
              ApplyUpscale();
              m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[0], (Texture) m_tempUpscaleRTs[0]);
            }
            else
              m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[0], (Texture) m_tempAuxDownsampleRTs[0]);
            m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleWeightsStr[0], m_upscaleWeights[0]);
          }
          else if (m_upscaleQuality == UpscaleQualityEnum.Realistic)
          {
            ApplyUpscale();
            for (int index1 = 0; index1 < m_bloomDownsampleCount; ++index1)
            {
              int index2 = m_bloomDownsampleCount - index1 - 1;
              m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[index2], (Texture) m_tempUpscaleRTs[index1]);
              m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleWeightsStr[index2], m_upscaleWeights[index1]);
            }
          }
          else
          {
            for (int index3 = 0; index3 < m_bloomDownsampleCount; ++index3)
            {
              int index4 = m_bloomDownsampleCount - 1 - index3;
              m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[index4], (Texture) m_tempAuxDownsampleRTs[index4]);
              m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleWeightsStr[index4], m_upscaleWeights[index3]);
            }
          }
        }
        else
        {
          m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[0], (Texture) renderTexture3);
          m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleWeightsStr[0], 1f);
        }
        if (m_debugToScreen == DebugToScreenEnum.Bloom)
        {
          m_finalCompositionMaterial.SetFloat(AmplifyUtils.SourceContributionId, 0.0f);
          FinalComposition(0.0f, 1f, src, dest, 0);
        }
        else
        {
          if (m_bloomDownsampleCount > 1)
          {
            for (int index = 0; index < m_bloomDownsampleCount; ++index)
            {
              m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensDirtWeightsStr[m_bloomDownsampleCount - index - 1], m_lensDirtWeights[index]);
              m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensStarburstWeightsStr[m_bloomDownsampleCount - index - 1], m_lensStarburstWeights[index]);
            }
          }
          else
          {
            m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensDirtWeightsStr[0], m_lensDirtWeights[0]);
            m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensStarburstWeightsStr[0], m_lensStarburstWeights[0]);
          }
          if (m_lensFlare.ApplyLensFlare)
            m_finalCompositionMaterial.SetTexture(AmplifyUtils.LensFlareRTId, (Texture) renderTexture1);
          if (m_anamorphicGlare.ApplyLensGlare)
            m_finalCompositionMaterial.SetTexture(AmplifyUtils.LensGlareRTId, (Texture) renderTexture2);
          if (m_applyLensDirt)
          {
            m_finalCompositionMaterial.SetTexture(AmplifyUtils.LensDirtRTId, m_lensDirtTexture);
            m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensDirtStrengthId, m_lensDirtStrength * 1f);
            if (m_debugToScreen == DebugToScreenEnum.LensDirt)
            {
              FinalComposition(0.0f, 0.0f, src, dest, 2);
              return;
            }
          }
          if (m_applyLensStardurst)
          {
            m_starburstMat[0, 0] = Mathf.Cos(num1);
            m_starburstMat[0, 1] = -Mathf.Sin(num1);
            m_starburstMat[1, 0] = Mathf.Sin(num1);
            m_starburstMat[1, 1] = Mathf.Cos(num1);
            m_finalCompositionMaterial.SetMatrix(AmplifyUtils.LensFlareStarMatrixId, m_starburstMat);
            m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensFlareStarburstStrengthId, m_lensStarburstStrength * 1f);
            m_finalCompositionMaterial.SetTexture(AmplifyUtils.LensStarburstRTId, m_lensStardurstTex);
            if (m_debugToScreen == DebugToScreenEnum.LensStarburst)
            {
              FinalComposition(0.0f, 0.0f, src, dest, 1);
              return;
            }
          }
          if ((UnityEngine.Object) m_targetTexture != (UnityEngine.Object) null)
          {
            m_targetTexture.DiscardContents();
            FinalComposition(0.0f, 1f, src, m_targetTexture, -1);
            Graphics.Blit((Texture) src, dest);
          }
          else
            FinalComposition(1f, 1f, src, dest, -1);
        }
      }
    }

    private void FinalComposition(
      float srcContribution,
      float upscaleContribution,
      RenderTexture src,
      RenderTexture dest,
      int forcePassId)
    {
      m_finalCompositionMaterial.SetFloat(AmplifyUtils.SourceContributionId, srcContribution);
      m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleContributionId, upscaleContribution);
      int num = 0;
      if (forcePassId > -1)
      {
        num = forcePassId;
      }
      else
      {
        if (LensFlareInstance.ApplyLensFlare)
          num |= 8;
        if (LensGlareInstance.ApplyLensGlare)
          num |= 4;
        if (m_applyLensDirt)
          num |= 2;
        if (m_applyLensStardurst)
          num |= 1;
      }
      int pass = num + (m_bloomDownsampleCount - 1) * 16;
      Graphics.Blit((Texture) src, dest, m_finalCompositionMaterial, pass);
      AmplifyUtils.ReleaseAllRT();
    }

    private void ApplyUpscale()
    {
      int index1 = m_bloomDownsampleCount - 1;
      int index2 = 0;
      for (int index3 = index1; index3 > -1; --index3)
      {
        m_tempUpscaleRTs[index2] = AmplifyUtils.GetTempRenderTarget((int) m_tempDownsamplesSizes[index3].x, (int) m_tempDownsamplesSizes[index3].y);
        if (index3 == index1)
        {
          Graphics.Blit((Texture) m_tempAuxDownsampleRTs[index1], m_tempUpscaleRTs[index2], m_bloomMaterial, 17);
        }
        else
        {
          m_bloomMaterial.SetTexture(AmplifyUtils.AnamorphicRTS[0], (Texture) m_tempUpscaleRTs[index2 - 1]);
          Graphics.Blit((Texture) m_tempAuxDownsampleRTs[index3], m_tempUpscaleRTs[index2], m_bloomMaterial, 18);
        }
        ++index2;
      }
    }

    public AmplifyGlare LensGlareInstance => m_anamorphicGlare;

    public AmplifyBokeh BokehFilterInstance => m_bokehFilter;

    public AmplifyLensFlare LensFlareInstance => m_lensFlare;

    public bool ApplyLensDirt
    {
      get => m_applyLensDirt;
      set => m_applyLensDirt = value;
    }

    public float LensDirtStrength
    {
      get => m_lensDirtStrength;
      set => m_lensDirtStrength = value < 0.0 ? 0.0f : value;
    }

    public Texture LensDirtTexture
    {
      get => m_lensDirtTexture;
      set => m_lensDirtTexture = value;
    }

    public bool ApplyLensStardurst
    {
      get => m_applyLensStardurst;
      set => m_applyLensStardurst = value;
    }

    public Texture LensStardurstTex
    {
      get => m_lensStardurstTex;
      set => m_lensStardurstTex = value;
    }

    public float LensStarburstStrength
    {
      get => m_lensStarburstStrength;
      set => m_lensStarburstStrength = value < 0.0 ? 0.0f : value;
    }

    public PrecisionModes CurrentPrecisionMode
    {
      get => m_highPrecision ? PrecisionModes.High : PrecisionModes.Low;
      set => HighPrecision = value == PrecisionModes.High;
    }

    public bool HighPrecision
    {
      get => m_highPrecision;
      set
      {
        if (m_highPrecision == value)
          return;
        m_highPrecision = value;
        CleanTempFilterRT();
      }
    }

    public float BloomRange
    {
      get => m_bloomRange.x;
      set => m_bloomRange.x = value < 0.0 ? 0.0f : value;
    }

    public float OverallThreshold
    {
      get => m_overallThreshold;
      set => m_overallThreshold = value < 0.0 ? 0.0f : value;
    }

    public Vector4 BloomParams
    {
      get => m_bloomParams;
      set => m_bloomParams = value;
    }

    public float OverallIntensity
    {
      get => m_bloomParams.x;
      set => m_bloomParams.x = value < 0.0 ? 0.0f : value;
    }

    public float BloomScale
    {
      get => m_bloomParams.w;
      set => m_bloomParams.w = value < 0.0 ? 0.0f : value;
    }

    public float UpscaleBlurRadius
    {
      get => m_bloomParams.z;
      set => m_bloomParams.z = value;
    }

    public bool TemporalFilteringActive
    {
      get => m_temporalFilteringActive;
      set
      {
        if (m_temporalFilteringActive != value)
          CleanTempFilterRT();
        m_temporalFilteringActive = value;
      }
    }

    public float TemporalFilteringValue
    {
      get => m_temporalFilteringValue;
      set => m_temporalFilteringValue = value;
    }

    public int SoftMaxdownscales => m_softMaxdownscales;

    public int BloomDownsampleCount
    {
      get => m_bloomDownsampleCount;
      set => m_bloomDownsampleCount = Mathf.Clamp(value, 1, m_softMaxdownscales);
    }

    public int FeaturesSourceId
    {
      get => m_featuresSourceId;
      set => m_featuresSourceId = Mathf.Clamp(value, 0, m_bloomDownsampleCount - 1);
    }

    public bool[] DownscaleSettingsFoldout => m_downscaleSettingsFoldout;

    public float[] UpscaleWeights => m_upscaleWeights;

    public float[] LensDirtWeights => m_lensDirtWeights;

    public float[] LensStarburstWeights => m_lensStarburstWeights;

    public float[] GaussianRadius => m_gaussianRadius;

    public int[] GaussianSteps => m_gaussianSteps;

    public AnimationCurve TemporalFilteringCurve
    {
      get => m_temporalFilteringCurve;
      set => m_temporalFilteringCurve = value;
    }

    public bool SeparateFeaturesThreshold
    {
      get => m_separateFeaturesThreshold;
      set => m_separateFeaturesThreshold = value;
    }

    public float FeaturesThreshold
    {
      get => m_featuresThreshold;
      set => m_featuresThreshold = value < 0.0 ? 0.0f : value;
    }

    public DebugToScreenEnum DebugToScreen
    {
      get => m_debugToScreen;
      set => m_debugToScreen = value;
    }

    public UpscaleQualityEnum UpscaleQuality
    {
      get => m_upscaleQuality;
      set => m_upscaleQuality = value;
    }

    public bool ShowDebugMessages
    {
      get => m_showDebugMessages;
      set => m_showDebugMessages = value;
    }

    public MainThresholdSizeEnum MainThresholdSize
    {
      get => m_mainThresholdSize;
      set => m_mainThresholdSize = value;
    }

    public RenderTexture TargetTexture
    {
      get => m_targetTexture;
      set => m_targetTexture = value;
    }

    public Texture MaskTexture
    {
      get => m_maskTexture;
      set => m_maskTexture = value;
    }

    public bool ApplyBokehFilter
    {
      get => m_bokehFilter.ApplyBokeh;
      set => m_bokehFilter.ApplyBokeh = value;
    }

    public bool ApplyLensFlare
    {
      get => m_lensFlare.ApplyLensFlare;
      set => m_lensFlare.ApplyLensFlare = value;
    }

    public bool ApplyLensGlare
    {
      get => m_anamorphicGlare.ApplyLensGlare;
      set => m_anamorphicGlare.ApplyLensGlare = value;
    }
  }
}
