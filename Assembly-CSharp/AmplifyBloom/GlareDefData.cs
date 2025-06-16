using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class GlareDefData
  {
    public bool FoldoutValue = true;
    [SerializeField]
    private StarLibType m_starType = StarLibType.Cross;
    [SerializeField]
    private float m_starInclination = 0.0f;
    [SerializeField]
    private float m_chromaticAberration = 0.0f;
    [SerializeField]
    private StarDefData m_customStarData = (StarDefData) null;

    public GlareDefData() => this.m_customStarData = new StarDefData();

    public GlareDefData(StarLibType starType, float starInclination, float chromaticAberration)
    {
      this.m_starType = starType;
      this.m_starInclination = starInclination;
      this.m_chromaticAberration = chromaticAberration;
    }

    public StarLibType StarType
    {
      get => this.m_starType;
      set => this.m_starType = value;
    }

    public float StarInclination
    {
      get => this.m_starInclination;
      set => this.m_starInclination = value;
    }

    public float StarInclinationDeg
    {
      get => this.m_starInclination * 57.29578f;
      set => this.m_starInclination = value * ((float) Math.PI / 180f);
    }

    public float ChromaticAberration
    {
      get => this.m_chromaticAberration;
      set => this.m_chromaticAberration = value;
    }

    public StarDefData CustomStarData
    {
      get => this.m_customStarData;
      set => this.m_customStarData = value;
    }
  }
}
