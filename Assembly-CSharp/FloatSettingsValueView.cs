using Engine.Behaviours.Localization;
using System;
using UnityEngine.Events;

public class FloatSettingsValueView : SettingsValueView<float>
{
  private float visibleValue;
  private bool changeEventDisabled;
  private Func<float, float> valueValidationFunction;
  private Func<float, string> valueNameFunction;
  private float _step;

  public override float VisibleValue
  {
    get => this.visibleValue;
    set => this.SetVisibleValue(value);
  }

  public override void ApplyVisibleValue() => this.SettingsValue.Value = this.visibleValue;

  private void Awake()
  {
    this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
  }

  private void OnValueChanged(float value)
  {
    if (this.changeEventDisabled)
      return;
    float visibleValue = this.visibleValue;
    this.SetVisibleValue(value);
    if ((double) this.visibleValue == (double) visibleValue)
      return;
    this.FireVisibleValueChangeEvent();
  }

  public override void RevertVisibleValue() => this.SetVisibleValue(this.SettingsValue.Value);

  public void SetValueNameFunction(Func<float, string> valueNameFunction)
  {
    this.valueNameFunction = valueNameFunction;
  }

  public void SetValueValidationFunction(Func<float, float> valueValidationFunction, float step = 1f)
  {
    this.valueValidationFunction = valueValidationFunction;
    this._step = step;
  }

  public void SetMaxValue(float maxValue) => this.slider.maxValue = maxValue;

  public void SetMinValue(float minValue) => this.slider.minValue = minValue;

  private void SetVisibleValue(float value)
  {
    this.changeEventDisabled = true;
    this.visibleValue = this.ValidateValue(value);
    this.slider.value = this.visibleValue;
    this.UpdateName();
    this.changeEventDisabled = false;
  }

  private void UpdateName()
  {
    Localizer valueText = this.valueText;
    Func<float, string> valueNameFunction = this.valueNameFunction;
    string str = valueNameFunction != null ? valueNameFunction(this.visibleValue) : (string) null;
    valueText.Signature = str;
  }

  private float ValidateValue(float value)
  {
    return this.valueValidationFunction == null ? value : this.valueValidationFunction(value);
  }

  public override void IncrementValue()
  {
    this.visibleValue += this._step;
    this.visibleValue = (double) this.visibleValue > (double) this.slider.maxValue ? this.slider.maxValue : this.visibleValue;
    this.OnValueChanged(this.visibleValue);
    this.FireVisibleValueChangeEvent();
  }

  public override void DecrementValue()
  {
    this.visibleValue -= this._step;
    this.visibleValue = (double) this.visibleValue < (double) this.slider.minValue ? this.slider.minValue : this.visibleValue;
    this.OnValueChanged(this.visibleValue);
    this.FireVisibleValueChangeEvent();
  }
}
