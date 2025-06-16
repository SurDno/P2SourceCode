using System;
using System.Collections.Generic;
using Cofe.Serializations.Converters;
using Engine.Source.Settings;
using Engine.Source.Settings.External;

namespace Engine.Source.Difficulties;

public class DifficultySettings : SettingsInstanceByRequest<DifficultySettings> {
	public IValue<bool> OriginalExperience = new BoolValue(nameof(OriginalExperience), true);
	private Dictionary<string, IValue<float>> values = new();
	private static readonly char separator = '|';

	private static readonly char[] separators = new char[1] {
		separator
	};

	private const string settingsName = "DifficultySettings";

	public DifficultySettings() {
		Load();
	}

	public IValue<float> GetValueItem(string name) {
		IValue<float> valueItem;
		values.TryGetValue(name, out valueItem);
		return valueItem;
	}

	public float GetValue(string name) {
		IValue<float> obj;
		return values.TryGetValue(name, out obj) ? obj.Value : 1f;
	}

	protected override void OnInvalidate() {
		base.OnInvalidate();
		Save();
	}

	private void Load() {
		var instance = ExternalSettingsInstance<ExternalDifficultySettings>.Instance;
		DifficultyPresetData difficultyPresetData = null;
		foreach (var preset in instance.Presets)
			if (preset.Name == "Default") {
				difficultyPresetData = preset;
				break;
			}

		foreach (var difficultyItemData in instance.Items) {
			var num = 1f;
			foreach (var difficultyPresetItemData in difficultyPresetData.Items)
				if (difficultyPresetItemData.Name == difficultyItemData.Name) {
					num = difficultyPresetItemData.Value;
					break;
				}

			values.Add(difficultyItemData.Name, new DifficultyValue {
				Value = num,
				DefaultValue = num,
				MinValue = difficultyItemData.Min,
				MaxValue = difficultyItemData.Max
			});
		}

		var strArray = PlayerSettings.Instance.GetString(nameof(DifficultySettings))
			.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		for (var index = 0; index < strArray.Length; index += 2) {
			IValue<float> obj;
			if (values.TryGetValue(strArray[index], out obj)) {
				var num = DefaultConverter.ParseFloat(strArray[index + 1]);
				obj.Value = num;
			}
		}
	}

	private void Save() {
		var str1 = "";
		foreach (var keyValuePair in values) {
			str1 += keyValuePair.Key;
			var str2 = str1;
			var separator = DifficultySettings.separator;
			var str3 = separator.ToString();
			str1 = str2 + str3;
			str1 += (string)(object)keyValuePair.Value.Value;
			var str4 = str1;
			separator = DifficultySettings.separator;
			var str5 = separator.ToString();
			str1 = str4 + str5;
		}

		PlayerSettings.Instance.SetString(nameof(DifficultySettings), str1);
		PlayerSettings.Instance.Save();
	}
}