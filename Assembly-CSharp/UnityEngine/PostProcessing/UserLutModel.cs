using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class UserLutModel : PostProcessingModel
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
    public struct Settings
    {
      [Tooltip("Custom lookup texture (strip format, e.g. 256x16).")]
      public Texture2D lut;
      [Range(0.0f, 1f)]
      [Tooltip("Blending factor.")]
      public float contribution;

      public static Settings defaultSettings
      {
        get
        {
          return new Settings {
            lut = (Texture2D) null,
            contribution = 1f
          };
        }
      }
    }
  }
}
