using Engine.Source.Commons;
using Engine.Source.Settings;
using Engine.Source.Settings.External;

namespace Engine.Source.Difficulties;

public static class DifficultyUtility {
	public static void SetPresetValues(string presetName) {
		foreach (var preset in ExternalSettingsInstance<ExternalDifficultySettings>.Instance.Presets)
			if (preset.Name == presetName) {
				SetPresetValues(preset);
				break;
			}
	}

	public static void SetPresetValues(DifficultyPresetData preset) {
		var instance = InstanceByRequest<DifficultySettings>.Instance;
		foreach (var difficultyPresetItemData in preset.Items) {
			var valueItem = instance.GetValueItem(difficultyPresetItemData.Name);
			if (valueItem != null)
				valueItem.Value = difficultyPresetItemData.Value;
		}

		instance.Apply();
	}
}