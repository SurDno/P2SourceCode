﻿using System;
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
    private float m_starInclination;
    [SerializeField]
    private float m_chromaticAberration;
    [SerializeField]
    private StarDefData m_customStarData;

    public GlareDefData() => m_customStarData = new StarDefData();

    public GlareDefData(StarLibType starType, float starInclination, float chromaticAberration)
    {
      m_starType = starType;
      m_starInclination = starInclination;
      m_chromaticAberration = chromaticAberration;
    }

    public StarLibType StarType
    {
      get => m_starType;
      set => m_starType = value;
    }

    public float StarInclination
    {
      get => m_starInclination;
      set => m_starInclination = value;
    }

    public float StarInclinationDeg
    {
      get => m_starInclination * 57.29578f;
      set => m_starInclination = value * ((float) Math.PI / 180f);
    }

    public float ChromaticAberration
    {
      get => m_chromaticAberration;
      set => m_chromaticAberration = value;
    }

    public StarDefData CustomStarData
    {
      get => m_customStarData;
      set => m_customStarData = value;
    }
  }
}
