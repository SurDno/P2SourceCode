// Decompiled with JetBrains decompiler
// Type: NamedIntSettingsValueView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine.Events;

#nullable disable
public class NamedIntSettingsValueView : SettingsValueView<int>
{
  private bool changeEventDisabled;
  private string[] names;
  private int visibleValue;

  public override int VisibleValue
  {
    get => this.visibleValue;
    set => this.SetVisibleValue(value);
  }

  private void Awake()
  {
    this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
  }

  public override void ApplyVisibleValue() => this.SettingsValue.Value = (int) this.slider.value;

  private void OnValueChanged(float value)
  {
    if (this.changeEventDisabled)
      return;
    int visibleValue = this.visibleValue;
    this.SetVisibleValue((int) value);
    if (this.visibleValue == visibleValue)
      return;
    this.FireVisibleValueChangeEvent();
  }

  public void SetValueNames(string[] names)
  {
    this.names = names;
    this.slider.minValue = 0.0f;
    this.slider.maxValue = names != null ? (float) (names.Length - 1) : 0.0f;
  }

  public override void RevertVisibleValue() => this.SetVisibleValue(this.SettingsValue.Value);

  private void SetVisibleValue(int value)
  {
    this.changeEventDisabled = true;
    this.visibleValue = value;
    this.slider.value = (float) this.visibleValue;
    this.UpdateName();
    this.changeEventDisabled = false;
  }

  private void UpdateName()
  {
    if (this.names == null)
      return;
    this.valueText.Signature = this.names[this.visibleValue];
  }

  public override void IncrementValue()
  {
    ++this.visibleValue;
    this.visibleValue = (double) this.visibleValue > (double) this.slider.maxValue ? (int) this.slider.maxValue : this.visibleValue;
    this.OnValueChanged((float) this.visibleValue);
    this.FireVisibleValueChangeEvent();
  }

  public override void DecrementValue()
  {
    --this.visibleValue;
    this.visibleValue = (double) this.visibleValue < (double) this.slider.minValue ? (int) this.slider.minValue : this.visibleValue;
    this.OnValueChanged((float) this.visibleValue);
    this.FireVisibleValueChangeEvent();
  }
}
