// Decompiled with JetBrains decompiler
// Type: BoolSettingsValueView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
public class BoolSettingsValueView : SettingsValueView<bool>
{
  [SerializeField]
  private Button button;
  private bool visibleValue;
  private bool changeEventDisabled;

  public override bool VisibleValue
  {
    get => this.visibleValue;
    set => this.SetVisibleValue(value);
  }

  private void Awake()
  {
    this.button.onClick.AddListener(new UnityAction(this.OnClick));
    this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
  }

  public override void ApplyVisibleValue() => this.SettingsValue.Value = this.visibleValue;

  private void OnClick()
  {
    this.SetVisibleValue(!this.visibleValue);
    this.FireVisibleValueChangeEvent();
  }

  private void OnValueChanged(float value)
  {
    if (this.changeEventDisabled)
      return;
    bool visibleValue = this.visibleValue;
    this.SetVisibleValue(Convert.ToBoolean(value));
    if (this.visibleValue == visibleValue)
      return;
    this.FireVisibleValueChangeEvent();
  }

  private void SetVisibleValue(bool value)
  {
    this.changeEventDisabled = true;
    this.visibleValue = value;
    this.slider.value = this.visibleValue ? 1f : 0.0f;
    this.valueText.Signature = this.visibleValue ? "{UI.Menu.Main.Settings.Bool.On}" : "{UI.Menu.Main.Settings.Bool.Off}";
    this.changeEventDisabled = false;
  }

  public override void RevertVisibleValue() => this.SetVisibleValue(this.SettingsValue.Value);

  public override void IncrementValue()
  {
    this.OnValueChanged(1f);
    this.ApplyVisibleValue();
  }

  public override void DecrementValue()
  {
    this.OnValueChanged(0.0f);
    this.ApplyVisibleValue();
  }
}
