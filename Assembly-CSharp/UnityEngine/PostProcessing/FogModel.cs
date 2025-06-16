using System;

namespace UnityEngine.PostProcessing
{
  [Serializable]
  public class FogModel : PostProcessingModel
  {
    [SerializeField]
    private FogModel.Settings m_Settings = FogModel.Settings.defaultSettings;

    public FogModel.Settings settings
    {
      get => this.m_Settings;
      set => this.m_Settings = value;
    }

    public override void Reset() => this.m_Settings = FogModel.Settings.defaultSettings;

    [Serializable]
    public struct Settings
    {
      [Tooltip("Should the fog affect the skybox?")]
      public bool excludeSkybox;

      public static FogModel.Settings defaultSettings
      {
        get
        {
          return new FogModel.Settings()
          {
            excludeSkybox = true
          };
        }
      }
    }
  }
}
