using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public sealed class AmplifyGlare : IAmplifyItem
  {
    public const int MaxLineSamples = 8;
    public const int MaxTotalSamples = 16;
    public const int MaxStarLines = 4;
    public const int MaxPasses = 4;
    public const int MaxCustomGlare = 32;
    [SerializeField]
    private GlareDefData[] m_customGlareDef;
    [SerializeField]
    private int m_customGlareDefIdx;
    [SerializeField]
    private int m_customGlareDefAmount;
    [SerializeField]
    private bool m_applyGlare = true;
    [SerializeField]
    private Color _overallTint = Color.white;
    [SerializeField]
    private Gradient m_cromaticAberrationGrad;
    [SerializeField]
    private int m_glareMaxPassCount = 4;
    private StarDefData[] m_starDefArr;
    private GlareDefData[] m_glareDefArr;
    private Matrix4x4[] m_weigthsMat;
    private Matrix4x4[] m_offsetsMat;
    private Color m_whiteReference;
    private float m_aTanFoV;
    private AmplifyGlareCache m_amplifyGlareCache;
    [SerializeField]
    private int m_currentWidth;
    [SerializeField]
    private int m_currentHeight;
    [SerializeField]
    private GlareLibType m_currentGlareType = GlareLibType.CheapLens;
    [SerializeField]
    private int m_currentGlareIdx;
    [SerializeField]
    private float m_perPassDisplacement = 4f;
    [SerializeField]
    private float m_intensity = 0.17f;
    [SerializeField]
    private float m_overallStreakScale = 1f;
    private bool m_isDirty = true;
    private RenderTexture[] _rtBuffer;

    public AmplifyGlare()
    {
      m_currentGlareIdx = (int) m_currentGlareType;
      m_cromaticAberrationGrad = new Gradient();
      m_cromaticAberrationGrad.SetKeys([
        new(Color.white, 0.0f),
        new(Color.blue, 0.25f),
        new(Color.green, 0.5f),
        new(Color.yellow, 0.75f),
        new(Color.red, 1f)
      ], [
        new(1f, 0.0f),
        new(1f, 0.25f),
        new(1f, 0.5f),
        new(1f, 0.75f),
        new(1f, 1f)
      ]);
      _rtBuffer = new RenderTexture[16];
      m_weigthsMat = new Matrix4x4[4];
      m_offsetsMat = new Matrix4x4[4];
      m_amplifyGlareCache = new AmplifyGlareCache();
      m_whiteReference = new Color(0.63f, 0.63f, 0.63f, 0.0f);
      m_aTanFoV = Mathf.Atan(0.3926991f);
      m_starDefArr = [
        new(StarLibType.Cross, "Cross", 2, 4, 1f, 0.85f, 0.0f, 0.5f, -1f, 90f),
        new(StarLibType.Cross_Filter, "CrossFilter", 2, 4, 1f, 0.95f, 0.0f, 0.5f, -1f, 90f),
        new(StarLibType.Snow_Cross, "snowCross", 3, 4, 1f, 0.96f, 0.349f, 0.5f, -1f),
        new(StarLibType.Vertical, "Vertical", 1, 4, 1f, 0.96f, 0.0f, 0.0f, -1f),
        new(StarLibType.Sunny_Cross, "SunnyCross", 4, 4, 1f, 0.88f, 0.0f, 0.0f, 0.95f, 45f)
      ];
      m_glareDefArr = [
        new(StarLibType.Cross, 0.0f, 0.5f),
        new(StarLibType.Cross_Filter, 0.44f, 0.5f),
        new(StarLibType.Cross_Filter, 1.22f, 1.5f),
        new(StarLibType.Snow_Cross, 0.17f, 0.5f),
        new(StarLibType.Snow_Cross, 0.7f, 1.5f),
        new(StarLibType.Sunny_Cross, 0.0f, 0.5f),
        new(StarLibType.Sunny_Cross, 0.79f, 1.5f),
        new(StarLibType.Vertical, 1.57f, 0.5f),
        new(StarLibType.Vertical, 0.0f, 0.5f)
      ];
    }

    public void Destroy()
    {
      for (int index = 0; index < m_starDefArr.Length; ++index)
        m_starDefArr[index].Destroy();
      m_glareDefArr = null;
      m_weigthsMat = null;
      m_offsetsMat = null;
      for (int index = 0; index < _rtBuffer.Length; ++index)
      {
        if (_rtBuffer[index] != null)
        {
          AmplifyUtils.ReleaseTempRenderTarget(_rtBuffer[index]);
          _rtBuffer[index] = null;
        }
      }
      _rtBuffer = null;
      m_amplifyGlareCache.Destroy();
      m_amplifyGlareCache = null;
    }

    public void SetDirty() => m_isDirty = true;

    public void OnRenderFromCache(
      RenderTexture source,
      RenderTexture dest,
      Material material,
      float glareIntensity,
      float cameraRotation)
    {
      for (int index = 0; index < m_amplifyGlareCache.TotalRT; ++index)
        _rtBuffer[index] = AmplifyUtils.GetTempRenderTarget(source.width, source.height);
      int index1 = 0;
      for (int index2 = 0; index2 < m_amplifyGlareCache.StarDef.StarlinesCount; ++index2)
      {
        for (int index3 = 0; index3 < m_amplifyGlareCache.CurrentPassCount; ++index3)
        {
          UpdateMatrixesForPass(material, m_amplifyGlareCache.Starlines[index2].Passes[index3].Offsets, m_amplifyGlareCache.Starlines[index2].Passes[index3].Weights, glareIntensity, cameraRotation * m_amplifyGlareCache.StarDef.CameraRotInfluence);
          if (index3 == 0)
            Graphics.Blit(source, _rtBuffer[index1], material, 2);
          else
            Graphics.Blit(_rtBuffer[index1 - 1], _rtBuffer[index1], material, 2);
          ++index1;
        }
      }
      for (int index4 = 0; index4 < m_amplifyGlareCache.StarDef.StarlinesCount; ++index4)
      {
        material.SetVector(AmplifyUtils.AnamorphicGlareWeightsStr[index4], m_amplifyGlareCache.AverageWeight);
        int index5 = (index4 + 1) * m_amplifyGlareCache.CurrentPassCount - 1;
        material.SetTexture(AmplifyUtils.AnamorphicRTS[index4], _rtBuffer[index5]);
      }
      int pass = 19 + m_amplifyGlareCache.StarDef.StarlinesCount - 1;
      dest.DiscardContents();
      Graphics.Blit(_rtBuffer[0], dest, material, pass);
      for (int index6 = 0; index6 < _rtBuffer.Length; ++index6)
      {
        AmplifyUtils.ReleaseTempRenderTarget(_rtBuffer[index6]);
        _rtBuffer[index6] = null;
      }
    }

    public void UpdateMatrixesForPass(
      Material material,
      Vector4[] offsets,
      Vector4[] weights,
      float glareIntensity,
      float rotation)
    {
      float num1 = Mathf.Cos(rotation);
      float num2 = Mathf.Sin(rotation);
      for (int index1 = 0; index1 < 16; ++index1)
      {
        int index2 = index1 >> 2;
        int row = index1 & 3;
        m_offsetsMat[index2][row, 0] = (float) (offsets[index1].x * (double) num1 - offsets[index1].y * (double) num2);
        m_offsetsMat[index2][row, 1] = (float) (offsets[index1].x * (double) num2 + offsets[index1].y * (double) num1);
        m_weigthsMat[index2][row, 0] = glareIntensity * weights[index1].x;
        m_weigthsMat[index2][row, 1] = glareIntensity * weights[index1].y;
        m_weigthsMat[index2][row, 2] = glareIntensity * weights[index1].z;
      }
      for (int index = 0; index < 4; ++index)
      {
        material.SetMatrix(AmplifyUtils.AnamorphicGlareOffsetsMatStr[index], m_offsetsMat[index]);
        material.SetMatrix(AmplifyUtils.AnamorphicGlareWeightsMatStr[index], m_weigthsMat[index]);
      }
    }

    public void OnRenderImage(
      Material material,
      RenderTexture source,
      RenderTexture dest,
      float cameraRot)
    {
      Graphics.Blit(Texture2D.blackTexture, dest);
      if (m_isDirty || m_currentWidth != source.width || m_currentHeight != source.height)
      {
        m_isDirty = false;
        m_currentWidth = source.width;
        m_currentHeight = source.height;
        bool flag = false;
        GlareDefData glareDefData;
        if (m_currentGlareType == GlareLibType.Custom)
        {
          if (m_customGlareDef != null && m_customGlareDef.Length != 0)
          {
            glareDefData = m_customGlareDef[m_customGlareDefIdx];
            flag = true;
          }
          else
            glareDefData = m_glareDefArr[0];
        }
        else
          glareDefData = m_glareDefArr[m_currentGlareIdx];
        m_amplifyGlareCache.GlareDef = glareDefData;
        float width = source.width;
        float height = source.height;
        StarDefData starDefData = flag ? glareDefData.CustomStarData : m_starDefArr[(int) glareDefData.StarType];
        m_amplifyGlareCache.StarDef = starDefData;
        int num1 = m_glareMaxPassCount < starDefData.PassCount ? m_glareMaxPassCount : starDefData.PassCount;
        m_amplifyGlareCache.CurrentPassCount = num1;
        float num2 = glareDefData.StarInclination + starDefData.Inclination;
        for (int index1 = 0; index1 < m_glareMaxPassCount; ++index1)
        {
          float t = (index1 + 1) / (float) m_glareMaxPassCount;
          for (int index2 = 0; index2 < 8; ++index2)
          {
            Color b = _overallTint * Color.Lerp(m_cromaticAberrationGrad.Evaluate(index2 / 7f), m_whiteReference, t);
            m_amplifyGlareCache.CromaticAberrationMat[index1, index2] = Color.Lerp(m_whiteReference, b, glareDefData.ChromaticAberration);
          }
        }
        m_amplifyGlareCache.TotalRT = starDefData.StarlinesCount * num1;
        for (int index = 0; index < m_amplifyGlareCache.TotalRT; ++index)
          _rtBuffer[index] = AmplifyUtils.GetTempRenderTarget(source.width, source.height);
        int index3 = 0;
        for (int index4 = 0; index4 < starDefData.StarlinesCount; ++index4)
        {
          StarLineData starLineData = starDefData.StarLinesArr[index4];
          float f = num2 + starLineData.Inclination;
          float num3 = Mathf.Sin(f);
          float num4 = Mathf.Cos(f);
          Vector2 vector2 = new Vector2();
          vector2.x = (float) (num4 / (double) width * (starLineData.SampleLength * (double) m_overallStreakScale));
          vector2.y = (float) (num3 / (double) height * (starLineData.SampleLength * (double) m_overallStreakScale));
          float num5 = (float) ((m_aTanFoV + 0.10000000149011612) * 280.0 / (width + (double) height) * 1.2000000476837158);
          for (int index5 = 0; index5 < num1; ++index5)
          {
            for (int index6 = 0; index6 < 8; ++index6)
            {
              float num6 = Mathf.Pow(starLineData.Attenuation, num5 * index6);
              m_amplifyGlareCache.Starlines[index4].Passes[index5].Weights[index6] = m_amplifyGlareCache.CromaticAberrationMat[num1 - 1 - index5, index6] * num6 * (index5 + 1f) * 0.5f;
              m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets[index6].x = vector2.x * index6;
              m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets[index6].y = vector2.y * index6;
              if (Mathf.Abs(m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets[index6].x) >= 0.89999997615814209 || Mathf.Abs(m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets[index6].y) >= 0.89999997615814209)
              {
                m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets[index6].x = 0.0f;
                m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets[index6].y = 0.0f;
                m_amplifyGlareCache.Starlines[index4].Passes[index5].Weights[index6] *= 0.0f;
              }
            }
            for (int index7 = 8; index7 < 16; ++index7)
            {
              m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets[index7] = -m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets[index7 - 8];
              m_amplifyGlareCache.Starlines[index4].Passes[index5].Weights[index7] = m_amplifyGlareCache.Starlines[index4].Passes[index5].Weights[index7 - 8];
            }
            UpdateMatrixesForPass(material, m_amplifyGlareCache.Starlines[index4].Passes[index5].Offsets, m_amplifyGlareCache.Starlines[index4].Passes[index5].Weights, m_intensity, starDefData.CameraRotInfluence * cameraRot);
            if (index5 == 0)
              Graphics.Blit(source, _rtBuffer[index3], material, 2);
            else
              Graphics.Blit(_rtBuffer[index3 - 1], _rtBuffer[index3], material, 2);
            ++index3;
            vector2 *= m_perPassDisplacement;
            num5 *= m_perPassDisplacement;
          }
        }
        m_amplifyGlareCache.AverageWeight = Vector4.one / starDefData.StarlinesCount;
        for (int index8 = 0; index8 < starDefData.StarlinesCount; ++index8)
        {
          material.SetVector(AmplifyUtils.AnamorphicGlareWeightsStr[index8], m_amplifyGlareCache.AverageWeight);
          int index9 = (index8 + 1) * num1 - 1;
          material.SetTexture(AmplifyUtils.AnamorphicRTS[index8], _rtBuffer[index9]);
        }
        int pass = 19 + starDefData.StarlinesCount - 1;
        dest.DiscardContents();
        Graphics.Blit(_rtBuffer[0], dest, material, pass);
        for (int index10 = 0; index10 < _rtBuffer.Length; ++index10)
        {
          AmplifyUtils.ReleaseTempRenderTarget(_rtBuffer[index10]);
          _rtBuffer[index10] = null;
        }
      }
      else
        OnRenderFromCache(source, dest, material, m_intensity, cameraRot);
    }

    public GlareLibType CurrentGlare
    {
      get => m_currentGlareType;
      set
      {
        if (m_currentGlareType == value)
          return;
        m_currentGlareType = value;
        m_currentGlareIdx = (int) value;
        m_isDirty = true;
      }
    }

    public int GlareMaxPassCount
    {
      get => m_glareMaxPassCount;
      set
      {
        m_glareMaxPassCount = value;
        m_isDirty = true;
      }
    }

    public float PerPassDisplacement
    {
      get => m_perPassDisplacement;
      set
      {
        m_perPassDisplacement = value;
        m_isDirty = true;
      }
    }

    public float Intensity
    {
      get => m_intensity;
      set
      {
        m_intensity = value < 0.0 ? 0.0f : value;
        m_isDirty = true;
      }
    }

    public Color OverallTint
    {
      get => _overallTint;
      set
      {
        _overallTint = value;
        m_isDirty = true;
      }
    }

    public bool ApplyLensGlare
    {
      get => m_applyGlare;
      set => m_applyGlare = value;
    }

    public Gradient CromaticColorGradient
    {
      get => m_cromaticAberrationGrad;
      set
      {
        m_cromaticAberrationGrad = value;
        m_isDirty = true;
      }
    }

    public float OverallStreakScale
    {
      get => m_overallStreakScale;
      set
      {
        m_overallStreakScale = value;
        m_isDirty = true;
      }
    }

    public GlareDefData[] CustomGlareDef
    {
      get => m_customGlareDef;
      set => m_customGlareDef = value;
    }

    public int CustomGlareDefIdx
    {
      get => m_customGlareDefIdx;
      set => m_customGlareDefIdx = value;
    }

    public int CustomGlareDefAmount
    {
      get => m_customGlareDefAmount;
      set
      {
        if (value == m_customGlareDefAmount)
          return;
        if (value == 0)
        {
          m_customGlareDef = null;
          m_customGlareDefIdx = 0;
          m_customGlareDefAmount = 0;
        }
        else
        {
          GlareDefData[] glareDefDataArray = new GlareDefData[value];
          for (int index = 0; index < value; ++index)
            glareDefDataArray[index] = index >= m_customGlareDefAmount ? new GlareDefData() : m_customGlareDef[index];
          m_customGlareDefIdx = Mathf.Clamp(m_customGlareDefIdx, 0, value - 1);
          m_customGlareDef = glareDefDataArray;
          m_customGlareDefAmount = value;
        }
      }
    }
  }
}
