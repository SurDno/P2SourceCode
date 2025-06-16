using Inspectors;
using UnityEngine;

namespace Engine.Source.Settings
{
  public class FloatValue : IValue<float>
  {
    [Inspected]
    private string name;
    [Inspected]
    private float defaultValue;
    [Inspected]
    private float minValue;
    [Inspected]
    private float maxValue;
    private float value;

    public FloatValue(string name, float defaultValue = 0.0f, float minValue = -3.40282347E+38f, float maxValue = 3.40282347E+38f)
    {
      this.name = name;
      this.defaultValue = defaultValue;
      this.minValue = minValue;
      this.maxValue = maxValue;
      value = PlayerSettings.Instance.GetFloat(name, defaultValue);
      value = Mathf.Clamp(value, minValue, maxValue);
    }

    [Inspected(Mutable = true)]
    public float Value
    {
      get => value;
      set
      {
        if (this.value == (double) value)
          return;
        this.value = value;
        this.value = Mathf.Clamp(this.value, minValue, maxValue);
        PlayerSettings.Instance.SetFloat(name, this.value);
        PlayerSettings.Instance.Save();
      }
    }

    public float DefaultValue => defaultValue;

    public float MinValue => minValue;

    public float MaxValue => maxValue;
  }
}
