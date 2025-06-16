using System;
using Engine.Behaviours.Localization;

public class FloatSettingsValueView : SettingsValueView<float>
{
  private float visibleValue;
  private bool changeEventDisabled;
  private Func<float, float> valueValidationFunction;
  private Func<float, string> valueNameFunction;
  private float _step;

  public override float VisibleValue
  {
    get => visibleValue;
    set => SetVisibleValue(value);
  }

  public override void ApplyVisibleValue() => SettingsValue.Value = visibleValue;

  private void Awake()
  {
    slider.onValueChanged.AddListener(OnValueChanged);
  }

  private void OnValueChanged(float value)
  {
    if (changeEventDisabled)
      return;
    float visibleValue = this.visibleValue;
    SetVisibleValue(value);
    if (this.visibleValue == (double) visibleValue)
      return;
    FireVisibleValueChangeEvent();
  }

  public override void RevertVisibleValue() => SetVisibleValue(SettingsValue.Value);

  public void SetValueNameFunction(Func<float, string> valueNameFunction)
  {
    this.valueNameFunction = valueNameFunction;
  }

  public void SetValueValidationFunction(Func<float, float> valueValidationFunction, float step = 1f)
  {
    this.valueValidationFunction = valueValidationFunction;
    _step = step;
  }

  public void SetMaxValue(float maxValue) => slider.maxValue = maxValue;

  public void SetMinValue(float minValue) => slider.minValue = minValue;

  private void SetVisibleValue(float value)
  {
    changeEventDisabled = true;
    visibleValue = ValidateValue(value);
    slider.value = visibleValue;
    UpdateName();
    changeEventDisabled = false;
  }

  private void UpdateName()
  {
    Localizer valueText = this.valueText;
    Func<float, string> valueNameFunction = this.valueNameFunction;
    string str = valueNameFunction != null ? valueNameFunction(visibleValue) : null;
    valueText.Signature = str;
  }

  private float ValidateValue(float value)
  {
    return valueValidationFunction == null ? value : valueValidationFunction(value);
  }

  public override void IncrementValue()
  {
    visibleValue += _step;
    visibleValue = visibleValue > (double) slider.maxValue ? slider.maxValue : visibleValue;
    OnValueChanged(visibleValue);
    FireVisibleValueChangeEvent();
  }

  public override void DecrementValue()
  {
    visibleValue -= _step;
    visibleValue = visibleValue < (double) slider.minValue ? slider.minValue : visibleValue;
    OnValueChanged(visibleValue);
    FireVisibleValueChangeEvent();
  }
}
