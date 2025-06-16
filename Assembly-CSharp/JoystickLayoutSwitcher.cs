using System;
using System.Collections.Generic;
using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings;
using Engine.Source.Settings.External;

public class JoystickLayoutSwitcher
{
  private static JoystickLayoutSwitcher _instance;
  private KeyLayouts _currentLayout = KeyLayouts.None;

  public event Action<KeyLayouts> OnLayoutChanged;

  public static JoystickLayoutSwitcher Instance
  {
    get
    {
      if (_instance == null)
        _instance = new JoystickLayoutSwitcher();
      return _instance;
    }
  }

  private JoystickLayoutSwitcher()
  {
    ApplyInputGameSettings();
    InstanceByRequest<InputGameSetting>.Instance.OnApply += ApplyInputGameSettings;
  }

  public void ChangeLayout(int newLayoutIndex)
  {
    CurrentLayout = (KeyLayouts) newLayoutIndex;
  }

  public int GetCurrentLayoutIndex() => (int) (CurrentLayout + 1);

  public KeyLayouts CurrentLayout
  {
    get => _currentLayout;
    private set
    {
      if (_currentLayout == value)
        return;
      _currentLayout = value;
      switch (_currentLayout)
      {
        case KeyLayouts.One:
          Groups = ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Groups_Set_1;
          break;
        case KeyLayouts.Two:
          Groups = ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Groups_Set_2;
          break;
        case KeyLayouts.Three:
          Groups = ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Groups_Set_3;
          break;
        default:
          return;
      }
      Action<KeyLayouts> onLayoutChanged = OnLayoutChanged;
      if (onLayoutChanged == null)
        return;
      onLayoutChanged(_currentLayout);
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
    ChangeLayout(InstanceByRequest<InputGameSetting>.Instance.JoystickLayout.Value);
  }

  public enum KeyLayouts
  {
    None = -1,
    One = 0,
    Two = 1,
    Three = 2,
  }
}
