using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class MotionBlurModel : PostProcessingModel
  {
    [SerializeField]
    private MotionBlurModel.Settings m_Settings = MotionBlurModel.Settings.defaultSettings;

    public MotionBlurModel.Settings settings
    {
      get => this.m_Settings;
      set => this.m_Settings = value;
    }

    public override void Reset() => this.m_Settings = MotionBlurModel.Settings.defaultSettings;

    [Serializable]
    public struct Settings
    {
      [Range(0.0f, 360f)]
      [Tooltip("The angle of rotary shutter relative to 30 fps. Larger values give longer exposure.")]
      public float shutterAngle;
      [Range(4f, 32f)]
      [Tooltip("The amount of sample points, which affects quality and performances.")]
      public int sampleCount;
      [Range(0.0f, 1f)]
      [Tooltip("The strength of multiple frame blending. The opacity of preceding frames are determined from this coefficient and time differences.")]
      public float frameBlending;

      public static MotionBlurModel.Settings defaultSettings
      {
        get
        {
          return new MotionBlurModel.Settings()
          {
            shutterAngle = 270f,
            sampleCount = 10,
            frameBlending = 0.0f
          };
        }
      }
    }
  }
}
