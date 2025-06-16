// Decompiled with JetBrains decompiler
// Type: Engine.Source.Difficulties.DifficultySettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Converters;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Difficulties
{
  public class DifficultySettings : SettingsInstanceByRequest<DifficultySettings>
  {
    public IValue<bool> OriginalExperience = (IValue<bool>) new BoolValue(nameof (OriginalExperience), true);
    private Dictionary<string, IValue<float>> values = new Dictionary<string, IValue<float>>();
    private static readonly char separator = '|';
    private static readonly char[] separators = new char[1]
    {
      DifficultySettings.separator
    };
    private const string settingsName = "DifficultySettings";

    public DifficultySettings() => this.Load();

    public IValue<float> GetValueItem(string name)
    {
      IValue<float> valueItem;
      this.values.TryGetValue(name, out valueItem);
      return valueItem;
    }

    public float GetValue(string name)
    {
      IValue<float> obj;
      return this.values.TryGetValue(name, out obj) ? obj.Value : 1f;
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      this.Save();
    }

    private void Load()
    {
      ExternalDifficultySettings instance = ExternalSettingsInstance<ExternalDifficultySettings>.Instance;
      DifficultyPresetData difficultyPresetData = (DifficultyPresetData) null;
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
        this.values.Add(difficultyItemData.Name, (IValue<float>) new DifficultyValue()
        {
          Value = num,
          DefaultValue = num,
          MinValue = difficultyItemData.Min,
          MaxValue = difficultyItemData.Max
        });
      }
      string[] strArray = PlayerSettings.Instance.GetString(nameof (DifficultySettings)).Split(DifficultySettings.separators, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; index += 2)
      {
        IValue<float> obj;
        if (this.values.TryGetValue(strArray[index], out obj))
        {
          float num = DefaultConverter.ParseFloat(strArray[index + 1]);
          obj.Value = num;
        }
      }
    }

    private void Save()
    {
      string str1 = "";
      foreach (KeyValuePair<string, IValue<float>> keyValuePair in this.values)
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
