using Engine.Source.Commons;
using Engine.Source.Settings;
using Engine.Source.Settings.External;

namespace Engine.Source.Difficulties
{
  public static class DifficultyUtility
  {
    public static void SetPresetValues(string presetName)
    {
      foreach (DifficultyPresetData preset in ExternalSettingsInstance<ExternalDifficultySettings>.Instance.Presets)
      {
        if (preset.Name == presetName)
        {
          SetPresetValues(preset);
          break;
        }
      }
    }

    public static void SetPresetValues(DifficultyPresetData preset)
    {
      DifficultySettings instance = InstanceByRequest<DifficultySettings>.Instance;
      foreach (DifficultyPresetItemData difficultyPresetItemData in preset.Items)
      {
        IValue<float> valueItem = instance.GetValueItem(difficultyPresetItemData.Name);
        if (valueItem != null)
          valueItem.Value = difficultyPresetItemData.Value;
      }
      instance.Apply();
    }
  }
}
