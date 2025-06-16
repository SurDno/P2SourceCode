using UnityEngine;

public static class SettingsViewUtility {
	public static string GammaValueName(float value) {
		return RoundValue(4.4f / Mathf.Pow(4f, value), 0.05f).ToString("F2");
	}

	public static float GammaValueValidation(float value) {
		return RoundValue(value, 0.025f);
	}

	public static string PercentValueName(float value) {
		return value * 100f + " %";
	}

	public static float RoundValue(float value, float step) {
		return Mathf.Round(value / step) * step;
	}

	public static float RoundValueTo5(float value) {
		return RoundValue(value, 5f);
	}
}