using UnityEngine;

public static class SettingsViewUtility
{
  public static string GammaValueName(float value)
  {
    return SettingsViewUtility.RoundValue(4.4f / Mathf.Pow(4f, value), 0.05f).ToString("F2");
  }

  public static float GammaValueValidation(float value)
  {
    return SettingsViewUtility.RoundValue(value, 0.025f);
  }

  public static string PercentValueName(float value) => (value * 100f).ToString() + " %";

  public static float RoundValue(float value, float step) => Mathf.Round(value / step) * step;

  public static float RoundValueTo5(float value) => SettingsViewUtility.RoundValue(value, 5f);
}
