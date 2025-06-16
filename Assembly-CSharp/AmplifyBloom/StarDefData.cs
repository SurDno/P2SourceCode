using System;

namespace AmplifyBloom
{
  [Serializable]
  public class StarDefData
  {
    [SerializeField]
    private StarLibType m_starType = StarLibType.Cross;
    [SerializeField]
    private string m_starName = string.Empty;
    [SerializeField]
    private int m_starlinesCount = 2;
    [SerializeField]
    private int m_passCount = 4;
    [SerializeField]
    private float m_sampleLength = 1f;
    [SerializeField]
    private float m_attenuation = 0.85f;
    [SerializeField]
    private float m_inclination;
    [SerializeField]
    private float m_rotation;
    [SerializeField]
    private StarLineData[] m_starLinesArr;
    [SerializeField]
    private float m_customIncrement = 90f;
    [SerializeField]
    private float m_longAttenuation;

    public StarDefData()
    {
    }

    public void Destroy() => m_starLinesArr = null;

    public StarDefData(
      StarLibType starType,
      string starName,
      int starLinesCount,
      int passCount,
      float sampleLength,
      float attenuation,
      float inclination,
      float rotation,
      float longAttenuation = 0.0f,
      float customIncrement = -1f)
    {
      m_starType = starType;
      m_starName = starName;
      m_passCount = passCount;
      m_sampleLength = sampleLength;
      m_attenuation = attenuation;
      m_starlinesCount = starLinesCount;
      m_inclination = inclination;
      m_rotation = rotation;
      m_customIncrement = customIncrement;
      m_longAttenuation = longAttenuation;
      CalculateStarData();
    }

    public void CalculateStarData()
    {
      if (m_starlinesCount == 0)
        return;
      m_starLinesArr = new StarLineData[m_starlinesCount];
      float num = (m_customIncrement > 0.0 ? m_customIncrement : 180f / m_starlinesCount) * ((float) Math.PI / 180f);
      for (int index = 0; index < m_starlinesCount; ++index)
      {
        m_starLinesArr[index] = new StarLineData();
        m_starLinesArr[index].PassCount = m_passCount;
        m_starLinesArr[index].SampleLength = m_sampleLength;
        m_starLinesArr[index].Attenuation = m_longAttenuation <= 0.0 ? m_attenuation : (index % 2 == 0 ? m_longAttenuation : m_attenuation);
        m_starLinesArr[index].Inclination = num * index;
      }
    }

    public StarLibType StarType
    {
      get => m_starType;
      set => m_starType = value;
    }

    public string StarName
    {
      get => m_starName;
      set => m_starName = value;
    }

    public int StarlinesCount
    {
      get => m_starlinesCount;
      set
      {
        m_starlinesCount = value;
        CalculateStarData();
      }
    }

    public int PassCount
    {
      get => m_passCount;
      set
      {
        m_passCount = value;
        CalculateStarData();
      }
    }

    public float SampleLength
    {
      get => m_sampleLength;
      set
      {
        m_sampleLength = value;
        CalculateStarData();
      }
    }

    public float Attenuation
    {
      get => m_attenuation;
      set
      {
        m_attenuation = value;
        CalculateStarData();
      }
    }

    public float Inclination
    {
      get => m_inclination;
      set
      {
        m_inclination = value;
        CalculateStarData();
      }
    }

    public float CameraRotInfluence
    {
      get => m_rotation;
      set => m_rotation = value;
    }

    public StarLineData[] StarLinesArr => m_starLinesArr;

    public float CustomIncrement
    {
      get => m_customIncrement;
      set
      {
        m_customIncrement = value;
        CalculateStarData();
      }
    }

    public float LongAttenuation
    {
      get => m_longAttenuation;
      set
      {
        m_longAttenuation = value;
        CalculateStarData();
      }
    }
  }
}
