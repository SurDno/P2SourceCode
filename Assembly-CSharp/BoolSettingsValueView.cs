using System;
using UnityEngine;
using UnityEngine.UI;

public class BoolSettingsValueView : SettingsValueView<bool>
{
  [SerializeField]
  private Button button;
  private bool visibleValue;
  private bool changeEventDisabled;

  public override bool VisibleValue
  {
    get => visibleValue;
    set => SetVisibleValue(value);
  }

  private void Awake()
  {
    button.onClick.AddListener(OnClick);
    slider.onValueChanged.AddListener(OnValueChanged);
  }

  public override void ApplyVisibleValue() => SettingsValue.Value = visibleValue;

  private void OnClick()
  {
    SetVisibleValue(!visibleValue);
    FireVisibleValueChangeEvent();
  }

  private void OnValueChanged(float value)
  {
    if (changeEventDisabled)
      return;
    bool visibleValue = this.visibleValue;
    SetVisibleValue(Convert.ToBoolean(value));
    if (this.visibleValue == visibleValue)
      return;
    FireVisibleValueChangeEvent();
  }

  private void SetVisibleValue(bool value)
  {
    changeEventDisabled = true;
    visibleValue = value;
    slider.value = visibleValue ? 1f : 0.0f;
    valueText.Signature = visibleValue ? "{UI.Menu.Main.Settings.Bool.On}" : "{UI.Menu.Main.Settings.Bool.Off}";
    changeEventDisabled = false;
  }

  public override void RevertVisibleValue() => SetVisibleValue(SettingsValue.Value);

  public override void IncrementValue()
  {
    OnValueChanged(1f);
    ApplyVisibleValue();
  }

  public override void DecrementValue()
  {
    OnValueChanged(0.0f);
    ApplyVisibleValue();
  }
}
