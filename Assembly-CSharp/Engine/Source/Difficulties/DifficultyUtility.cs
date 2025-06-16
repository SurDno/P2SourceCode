// Decompiled with JetBrains decompiler
// Type: Engine.Source.Difficulties.DifficultyUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings;
using Engine.Source.Settings.External;

#nullable disable
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
          DifficultyUtility.SetPresetValues(preset);
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
