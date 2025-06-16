using System;
using System.Collections.Generic;

namespace AmplifyBloom
{
  [Serializable]
  public sealed class AmplifyBokeh : IAmplifyItem, ISerializationCallbackReceiver
  {
    private const int PerPassSampleCount = 8;
    [SerializeField]
    private bool m_isActive;
    [SerializeField]
    private bool m_applyOnBloomSource;
    [SerializeField]
    private float m_bokehSampleRadius = 0.5f;
    [SerializeField]
    private Vector4 m_bokehCameraProperties = new Vector4(0.05f, 0.018f, 1.34f, 0.18f);
    [SerializeField]
    private float m_offsetRotation;
    [SerializeField]
    private ApertureShape m_apertureShape = ApertureShape.Hexagon;
    private List<AmplifyBokehData> m_bokehOffsets;

    public AmplifyBokeh()
    {
      m_bokehOffsets = new List<AmplifyBokehData>();
      CreateBokehOffsets(ApertureShape.Hexagon);
    }

    public void Destroy()
    {
      for (int index = 0; index < m_bokehOffsets.Count; ++index)
        m_bokehOffsets[index].Destroy();
    }

    private void CreateBokehOffsets(ApertureShape shape)
    {
      m_bokehOffsets.Clear();
      switch (shape)
      {
        case ApertureShape.Square:
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation)));
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 90f)));
          break;
        case ApertureShape.Hexagon:
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation)));
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation - 75f)));
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 75f)));
          break;
        case ApertureShape.Octagon:
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation)));
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 65f)));
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 90f)));
          m_bokehOffsets.Add(new AmplifyBokehData(CalculateBokehSamples(8, m_offsetRotation + 115f)));
          break;
      }
    }

    private Vector4[] CalculateBokehSamples(int sampleCount, float angle)
    {
      Vector4[] bokehSamples = new Vector4[sampleCount];
      float f = (float) Math.PI / 180f * angle;
      float num = (float) Screen.width / (float) Screen.height;
      Vector4 b = new Vector4(m_bokehSampleRadius * Mathf.Cos(f), m_bokehSampleRadius * Mathf.Sin(f));
      b.x /= num;
      for (int index = 0; index < sampleCount; ++index)
      {
        float t = index / (sampleCount - 1f);
        bokehSamples[index] = Vector4.Lerp(-b, b, t);
      }
      return bokehSamples;
    }

    public void ApplyBokehFilter(RenderTexture source, Material material)
    {
      for (int index = 0; index < m_bokehOffsets.Count; ++index)
        m_bokehOffsets[index].BokehRenderTexture = AmplifyUtils.GetTempRenderTarget(source.width, source.height);
      material.SetVector(AmplifyUtils.BokehParamsId, m_bokehCameraProperties);
      for (int index1 = 0; index1 < m_bokehOffsets.Count; ++index1)
      {
        for (int index2 = 0; index2 < 8; ++index2)
          material.SetVector(AmplifyUtils.AnamorphicGlareWeightsStr[index2], m_bokehOffsets[index1].Offsets[index2]);
        Graphics.Blit((Texture) source, m_bokehOffsets[index1].BokehRenderTexture, material, 27);
      }
      for (int index = 0; index < m_bokehOffsets.Count - 1; ++index)
        material.SetTexture(AmplifyUtils.AnamorphicRTS[index], (Texture) m_bokehOffsets[index].BokehRenderTexture);
      source.DiscardContents();
      Graphics.Blit((Texture) m_bokehOffsets[m_bokehOffsets.Count - 1].BokehRenderTexture, source, material, 28 + (m_bokehOffsets.Count - 2));
      for (int index = 0; index < m_bokehOffsets.Count; ++index)
      {
        AmplifyUtils.ReleaseTempRenderTarget(m_bokehOffsets[index].BokehRenderTexture);
        m_bokehOffsets[index].BokehRenderTexture = (RenderTexture) null;
      }
    }

    public void OnAfterDeserialize() => CreateBokehOffsets(m_apertureShape);

    public void OnBeforeSerialize()
    {
    }

    public ApertureShape ApertureShape
    {
      get => m_apertureShape;
      set
      {
        if (m_apertureShape == value)
          return;
        m_apertureShape = value;
        CreateBokehOffsets(value);
      }
    }

    public bool ApplyBokeh
    {
      get => m_isActive;
      set => m_isActive = value;
    }

    public bool ApplyOnBloomSource
    {
      get => m_applyOnBloomSource;
      set => m_applyOnBloomSource = value;
    }

    public float BokehSampleRadius
    {
      get => m_bokehSampleRadius;
      set => m_bokehSampleRadius = value;
    }

    public float OffsetRotation
    {
      get => m_offsetRotation;
      set => m_offsetRotation = value;
    }

    public Vector4 BokehCameraProperties
    {
      get => m_bokehCameraProperties;
      set => m_bokehCameraProperties = value;
    }

    public float Aperture
    {
      get => m_bokehCameraProperties.x;
      set => m_bokehCameraProperties.x = value;
    }

    public float FocalLength
    {
      get => m_bokehCameraProperties.y;
      set => m_bokehCameraProperties.y = value;
    }

    public float FocalDistance
    {
      get => m_bokehCameraProperties.z;
      set => m_bokehCameraProperties.z = value;
    }

    public float MaxCoCDiameter
    {
      get => m_bokehCameraProperties.w;
      set => m_bokehCameraProperties.w = value;
    }
  }
}
