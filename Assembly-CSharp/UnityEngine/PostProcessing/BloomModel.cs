﻿using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class BloomModel : PostProcessingModel
  {
    [SerializeField]
    private Settings m_Settings = Settings.defaultSettings;

    public Settings settings
    {
      get => m_Settings;
      set => m_Settings = value;
    }

    public override void Reset() => m_Settings = Settings.defaultSettings;

    [Serializable]
    public struct BloomSettings
    {
      [Min(0.0f)]
      [Tooltip("Strength of the bloom filter.")]
      public float intensity;
      [Min(0.0f)]
      [Tooltip("Filters out pixels under this level of brightness.")]
      public float threshold;
      [Range(0.0f, 1f)]
      [Tooltip("Makes transition between under/over-threshold gradual (0 = hard threshold, 1 = soft threshold).")]
      public float softKnee;
      [Range(1f, 7f)]
      [Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
      public float radius;
      [Tooltip("Reduces flashing noise with an additional filter.")]
      public bool antiFlicker;

      public float thresholdLinear
      {
        set => threshold = Mathf.LinearToGammaSpace(value);
        get => Mathf.GammaToLinearSpace(threshold);
      }

      public static BloomSettings defaultSettings =>
        new() {
          intensity = 0.5f,
          threshold = 1.1f,
          softKnee = 0.5f,
          radius = 4f,
          antiFlicker = false
        };
    }

    [Serializable]
    public struct LensDirtSettings
    {
      [Tooltip("Dirtiness texture to add smudges or dust to the lens.")]
      public Texture texture;
      [Min(0.0f)]
      [Tooltip("Amount of lens dirtiness.")]
      public float intensity;

      public static LensDirtSettings defaultSettings =>
        new() {
          texture = null,
          intensity = 3f
        };
    }

    [Serializable]
    public struct Settings
    {
      public BloomSettings bloom;
      public LensDirtSettings lensDirt;

      public static Settings defaultSettings =>
        new() {
          bloom = BloomSettings.defaultSettings,
          lensDirt = LensDirtSettings.defaultSettings
        };
    }
  }
}
