// Decompiled with JetBrains decompiler
// Type: JoystickLayoutSwitcher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using System;
using System.Collections.Generic;

#nullable disable
public class JoystickLayoutSwitcher
{
  private static JoystickLayoutSwitcher _instance;
  private JoystickLayoutSwitcher.KeyLayouts _currentLayout = JoystickLayoutSwitcher.KeyLayouts.None;

  public event Action<JoystickLayoutSwitcher.KeyLayouts> OnLayoutChanged;

  public static JoystickLayoutSwitcher Instance
  {
    get
    {
      if (JoystickLayoutSwitcher._instance == null)
        JoystickLayoutSwitcher._instance = new JoystickLayoutSwitcher();
      return JoystickLayoutSwitcher._instance;
    }
  }

  private JoystickLayoutSwitcher()
  {
    this.ApplyInputGameSettings();
    InstanceByRequest<InputGameSetting>.Instance.OnApply += new Action(this.ApplyInputGameSettings);
  }

  public void ChangeLayout(int newLayoutIndex)
  {
    this.CurrentLayout = (JoystickLayoutSwitcher.KeyLayouts) newLayoutIndex;
  }

  public int GetCurrentLayoutIndex() => (int) (this.CurrentLayout + 1);

  public JoystickLayoutSwitcher.KeyLayouts CurrentLayout
  {
    get => this._currentLayout;
    private set
    {
      if (this._currentLayout == value)
        return;
      this._currentLayout = value;
      switch (this._currentLayout)
      {
        case JoystickLayoutSwitcher.KeyLayouts.One:
          this.Groups = ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Groups_Set_1;
          break;
        case JoystickLayoutSwitcher.KeyLayouts.Two:
          this.Groups = ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Groups_Set_2;
          break;
        case JoystickLayoutSwitcher.KeyLayouts.Three:
          this.Groups = ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Groups_Set_3;
          break;
        default:
          return;
      }
      Action<JoystickLayoutSwitcher.KeyLayouts> onLayoutChanged = this.OnLayoutChanged;
      if (onLayoutChanged == null)
        return;
      onLayoutChanged(this._currentLayout);
    }
  }

  public List<ActionGroup> Groups { get; private set; }

  public float JoystickLayoutValidation(float value) => SettingsViewUtility.RoundValue(value, 1f);

  public void OnAutoJoystickValueChange<T>(SettingsValueView<T> view)
  {
    InputGameSetting instance = InstanceByRequest<InputGameSetting>.Instance;
    view.ApplyVisibleValue();
    instance.Apply();
  }

  private void ApplyInputGameSettings()
  {
    this.ChangeLayout(InstanceByRequest<InputGameSetting>.Instance.JoystickLayout.Value);
  }

  public enum KeyLayouts
  {
    None = -1, // 0xFFFFFFFF
    One = 0,
    Two = 1,
    Three = 2,
  }
}
