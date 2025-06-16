// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.ControlSettingsView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings;
using InputServices;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Menu.Main
{
  public class ControlSettingsView : SettingsView
  {
    private FloatSettingsValueView mouseSensitivityView;
    private BoolSettingsValueView mouseInvertView;
    private FloatSettingsValueView joystickSensitivityView;
    private BoolSettingsValueView joystickInvertView;
    private InputGameSetting inputGameSettings;

    protected override void Awake()
    {
      this.inputGameSettings = InstanceByRequest<InputGameSetting>.Instance;
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.ApplySettings();
      base.Awake();
    }

    private void ApplySettings()
    {
      this.mouseSensitivityView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.mouseSensitivityView.SetName("{UI.Menu.Main.Settings.Control.Mouse.Sensitivity}");
      this.mouseSensitivityView.SetMinValue(this.inputGameSettings.MouseSensitivity.MinValue);
      this.mouseSensitivityView.SetMaxValue(this.inputGameSettings.MouseSensitivity.MaxValue);
      this.mouseSensitivityView.SetValueNameFunction(new Func<float, string>(this.SensitivityName));
      this.mouseSensitivityView.SetSetting(this.inputGameSettings.MouseSensitivity);
      this.mouseSensitivityView.SetValueValidationFunction(new Func<float, float>(this.SensitivityRound), 0.1f);
      FloatSettingsValueView mouseSensitivityView = this.mouseSensitivityView;
      mouseSensitivityView.VisibleValueChangeEvent = mouseSensitivityView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnAutoValueChange<float>);
      this.mouseInvertView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.mouseInvertView.SetName("{UI.Menu.Main.Settings.Control.Mouse.Invert}");
      this.mouseInvertView.SetSetting(this.inputGameSettings.MouseInvert);
      BoolSettingsValueView mouseInvertView = this.mouseInvertView;
      mouseInvertView.VisibleValueChangeEvent = mouseInvertView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(this.OnAutoValueChange<bool>);
      this.joystickSensitivityView = UnityEngine.Object.Instantiate<FloatSettingsValueView>(this.floatValueViewPrefab, (Transform) this.layout.Content, false);
      this.joystickSensitivityView.SetName("{UI.Menu.Main.Settings.Control.Joystick.Sensitivity}");
      this.joystickSensitivityView.SetMinValue(this.inputGameSettings.JoystickSensitivity.MinValue);
      this.joystickSensitivityView.SetMaxValue(this.inputGameSettings.JoystickSensitivity.MaxValue);
      this.joystickSensitivityView.SetValueNameFunction(new Func<float, string>(this.SensitivityName));
      this.joystickSensitivityView.SetSetting(this.inputGameSettings.JoystickSensitivity);
      this.joystickSensitivityView.SetValueValidationFunction(new Func<float, float>(this.SensitivityRound), 0.1f);
      FloatSettingsValueView joystickSensitivityView = this.joystickSensitivityView;
      joystickSensitivityView.VisibleValueChangeEvent = joystickSensitivityView.VisibleValueChangeEvent + new Action<SettingsValueView<float>>(this.OnAutoValueChange<float>);
      this.joystickInvertView = UnityEngine.Object.Instantiate<BoolSettingsValueView>(this.boolValueViewPrefab, (Transform) this.layout.Content, false);
      this.joystickInvertView.SetName("{UI.Menu.Main.Settings.Control.Joystick.Invert}");
      this.joystickInvertView.SetSetting(this.inputGameSettings.JoystickInvert);
      BoolSettingsValueView joystickInvertView = this.joystickInvertView;
      joystickInvertView.VisibleValueChangeEvent = joystickInvertView.VisibleValueChangeEvent + new Action<SettingsValueView<bool>>(this.OnAutoValueChange<bool>);
    }

    private void OnAutoValueChange<T>(SettingsValueView<T> view)
    {
      view.ApplyVisibleValue();
      this.inputGameSettings.Apply();
    }

    protected override void OnButtonReset()
    {
      this.mouseSensitivityView.ResetValue();
      this.mouseInvertView.ResetValue();
      this.joystickSensitivityView.ResetValue();
      this.joystickInvertView.ResetValue();
      this.inputGameSettings.Apply();
      this.UpdateViews();
    }

    protected override void OnJoystick(bool isUsed)
    {
      base.OnJoystick(isUsed);
      bool joystickPresent = InputService.Instance.JoystickPresent;
      this.joystickSensitivityView.gameObject.SetActive(joystickPresent | isUsed);
      this.joystickInvertView.gameObject.SetActive(joystickPresent | isUsed);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.UpdateViews();
    }

    private void UpdateViews()
    {
      this.mouseSensitivityView.RevertVisibleValue();
      this.mouseInvertView.RevertVisibleValue();
      this.joystickSensitivityView.RevertVisibleValue();
      this.joystickInvertView.RevertVisibleValue();
    }

    private string SensitivityName(float value) => value.ToString("n1");

    private float SensitivityRound(float value) => SettingsViewUtility.RoundValue(value, 0.1f);
  }
}
