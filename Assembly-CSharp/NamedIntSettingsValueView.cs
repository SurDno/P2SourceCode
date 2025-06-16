public class NamedIntSettingsValueView : SettingsValueView<int>
{
  private bool changeEventDisabled;
  private string[] names;
  private int visibleValue;

  public override int VisibleValue
  {
    get => visibleValue;
    set => SetVisibleValue(value);
  }

  private void Awake()
  {
    slider.onValueChanged.AddListener(OnValueChanged);
  }

  public override void ApplyVisibleValue() => SettingsValue.Value = (int) slider.value;

  private void OnValueChanged(float value)
  {
    if (changeEventDisabled)
      return;
    int visibleValue = this.visibleValue;
    SetVisibleValue((int) value);
    if (this.visibleValue == visibleValue)
      return;
    FireVisibleValueChangeEvent();
  }

  public void SetValueNames(string[] names)
  {
    this.names = names;
    slider.minValue = 0.0f;
    slider.maxValue = names != null ? names.Length - 1 : 0.0f;
  }

  public override void RevertVisibleValue() => SetVisibleValue(SettingsValue.Value);

  private void SetVisibleValue(int value)
  {
    changeEventDisabled = true;
    visibleValue = value;
    slider.value = visibleValue;
    UpdateName();
    changeEventDisabled = false;
  }

  private void UpdateName()
  {
    if (names == null)
      return;
    valueText.Signature = names[visibleValue];
  }

  public override void IncrementValue()
  {
    ++visibleValue;
    visibleValue = visibleValue > (double) slider.maxValue ? (int) slider.maxValue : visibleValue;
    OnValueChanged(visibleValue);
    FireVisibleValueChangeEvent();
  }

  public override void DecrementValue()
  {
    --visibleValue;
    visibleValue = visibleValue < (double) slider.minValue ? (int) slider.minValue : visibleValue;
    OnValueChanged(visibleValue);
    FireVisibleValueChangeEvent();
  }
}
