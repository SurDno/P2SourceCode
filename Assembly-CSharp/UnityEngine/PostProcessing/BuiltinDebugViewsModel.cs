﻿using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class BuiltinDebugViewsModel : PostProcessingModel
  {
    [SerializeField]
    private Settings m_Settings = Settings.defaultSettings;

    public Settings settings
    {
      get => m_Settings;
      set => m_Settings = value;
    }

    public bool willInterrupt => !IsModeActive(Mode.None) && !IsModeActive(Mode.EyeAdaptation) && !IsModeActive(Mode.PreGradingLog) && !IsModeActive(Mode.LogLut) && !IsModeActive(Mode.UserLut);

    public override void Reset() => settings = Settings.defaultSettings;

    public bool IsModeActive(Mode mode) => m_Settings.mode == mode;

    [Serializable]
    public struct DepthSettings
    {
      [Range(0.0f, 1f)]
      [Tooltip("Scales the camera far plane before displaying the depth map.")]
      public float scale;

      public static DepthSettings defaultSettings =>
        new() {
          scale = 1f
        };
    }

    [Serializable]
    public struct MotionVectorsSettings
    {
      [Range(0.0f, 1f)]
      [Tooltip("Opacity of the source render.")]
      public float sourceOpacity;
      [Range(0.0f, 1f)]
      [Tooltip("Opacity of the per-pixel motion vector colors.")]
      public float motionImageOpacity;
      [Min(0.0f)]
      [Tooltip("Because motion vectors are mainly very small vectors, you can use this setting to make them more visible.")]
      public float motionImageAmplitude;
      [Range(0.0f, 1f)]
      [Tooltip("Opacity for the motion vector arrows.")]
      public float motionVectorsOpacity;
      [Range(8f, 64f)]
      [Tooltip("The arrow density on screen.")]
      public int motionVectorsResolution;
      [Min(0.0f)]
      [Tooltip("Tweaks the arrows length.")]
      public float motionVectorsAmplitude;

      public static MotionVectorsSettings defaultSettings =>
        new() {
          sourceOpacity = 1f,
          motionImageOpacity = 0.0f,
          motionImageAmplitude = 16f,
          motionVectorsOpacity = 1f,
          motionVectorsResolution = 24,
          motionVectorsAmplitude = 64f
        };
    }

    public enum Mode
    {
      None,
      Depth,
      Normals,
      MotionVectors,
      AmbientOcclusion,
      EyeAdaptation,
      FocusPlane,
      PreGradingLog,
      LogLut,
      UserLut,
    }

    [Serializable]
    public struct Settings
    {
      public Mode mode;
      public DepthSettings depth;
      public MotionVectorsSettings motionVectors;

      public static Settings defaultSettings =>
        new() {
          mode = Mode.None,
          depth = DepthSettings.defaultSettings,
          motionVectors = MotionVectorsSettings.defaultSettings
        };
    }
  }
}
