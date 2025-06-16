// Decompiled with JetBrains decompiler
// Type: SettingsMenuHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using System;

#nullable disable
public class SettingsMenuHelper
{
  private static SettingsMenuHelper _instance;
  public bool isSelected;
  private bool isActive = false;

  private event Action<bool> _onStateSelected;

  public event Action<bool> OnStateSelected
  {
    add
    {
      value(this.isSelected);
      this._onStateSelected += value;
    }
    remove => this._onStateSelected -= value;
  }

  public static SettingsMenuHelper Instatnce
  {
    get
    {
      if (SettingsMenuHelper._instance == null)
        SettingsMenuHelper._instance = new SettingsMenuHelper();
      return SettingsMenuHelper._instance;
    }
  }

  private SettingsMenuHelper()
  {
    if (InputService.Instance.JoystickUsed)
      return;
    this.SetSelectedState();
  }

  private void OnJoystick(bool isUsed)
  {
    if (isUsed)
      return;
    this.SetSelectedState();
  }

  public void ShowSettings<T>() where T : class, IWindow
  {
    ServiceLocator.GetService<UIService>().Swap<T>();
  }

  public void SetSelectedState()
  {
    this.isSelected = true;
    Action<bool> onStateSelected = this._onStateSelected;
    if (onStateSelected != null)
      onStateSelected(true);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(this.OnBack), true);
  }

  private bool OnBack(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed)
      return false;
    if (down)
      this.SetChangingState();
    return down;
  }

  public void SetChangingState()
  {
    this.isSelected = false;
    Action<bool> onStateSelected = this._onStateSelected;
    if (onStateSelected != null)
      onStateSelected(false);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(this.OnBack));
  }

  public void Reset()
  {
    if (!InputService.Instance.JoystickUsed)
      return;
    this.SetChangingState();
  }

  public void Activate(bool active)
  {
    this.isActive = active;
    if (this.isActive)
    {
      if (!InputService.Instance.JoystickUsed)
        this.SetSelectedState();
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
    }
    else
    {
      this.SetChangingState();
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    }
  }
}
