﻿using System;
using System.Collections.Generic;
using Cofe.Serializations.Converters;
using Engine.Source.Settings;
using Engine.Source.Settings.External;

namespace Engine.Source.Difficulties
{
  public class DifficultySettings : SettingsInstanceByRequest<DifficultySettings>
  {
    public IValue<bool> OriginalExperience = new BoolValue(nameof (OriginalExperience), true);
    private Dictionary<string, IValue<float>> values = new();
    private static readonly char separator = '|';
    private static readonly char[] separators = [
      separator
    ];
    private const string settingsName = "DifficultySettings";

    public DifficultySettings() => Load();

    public IValue<float> GetValueItem(string name)
    {
      values.TryGetValue(name, out IValue<float> valueItem);
      return valueItem;
    }

    public float GetValue(string name)
    {
      return values.TryGetValue(name, out IValue<float> obj) ? obj.Value : 1f;
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      Save();
    }

    private void Load()
    {
      ExternalDifficultySettings instance = ExternalSettingsInstance<ExternalDifficultySettings>.Instance;
      DifficultyPresetData difficultyPresetData = null;
      foreach (DifficultyPresetData preset in instance.Presets)
      {
        if (preset.Name == "Default")
        {
          difficultyPresetData = preset;
          break;
        }
      }
      foreach (DifficultyItemData difficultyItemData in instance.Items)
      {
        float num = 1f;
        foreach (DifficultyPresetItemData difficultyPresetItemData in difficultyPresetData.Items)
        {
          if (difficultyPresetItemData.Name == difficultyItemData.Name)
          {
            num = difficultyPresetItemData.Value;
            break;
          }
        }
        values.Add(difficultyItemData.Name, new DifficultyValue {
          Value = num,
          DefaultValue = num,
          MinValue = difficultyItemData.Min,
          MaxValue = difficultyItemData.Max
        });
      }
      string[] strArray = PlayerSettings.Instance.GetString(nameof (DifficultySettings)).Split(separators, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; index += 2)
      {
        if (values.TryGetValue(strArray[index], out IValue<float> obj))
        {
          float num = DefaultConverter.ParseFloat(strArray[index + 1]);
          obj.Value = num;
        }
      }
    }

    private void Save()
    {
      string str1 = "";
      foreach (KeyValuePair<string, IValue<float>> keyValuePair in values)
      {
        str1 += keyValuePair.Key;
        string str2 = str1;
        char separator = DifficultySettings.separator;
        string str3 = separator.ToString();
        str1 = str2 + str3;
        str1 += (string) (object) keyValuePair.Value.Value;
        string str4 = str1;
        separator = DifficultySettings.separator;
        string str5 = separator.ToString();
        str1 = str4 + str5;
      }
      PlayerSettings.Instance.SetString(nameof (DifficultySettings), str1);
      PlayerSettings.Instance.Save();
    }
  }
}
