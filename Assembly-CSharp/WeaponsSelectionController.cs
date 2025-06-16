using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Engine.Source.Utility;
using InputServices;
using System;
using System.Collections;
using UnityEngine;

public class WeaponsSelectionController : MonoBehaviour
{
  [SerializeField]
  private HUDWeaponsView _consoleView;
  [SerializeField]
  private HUDWeaponsView _pcView;
  private HUDWeaponsView _currentView;
  private bool _isConsole;
  private int _selectedIndex = -1;
  private float _sensativity;
  private Coroutine _waitCoroutine;
  [SerializeField]
  private GameObject DPadNavigationHint;
  [SerializeField]
  private GameObject RStickNavigationHint;
  private bool wasWeaponChanged = false;

  private void OnEnable()
  {
    InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
    this.OnJoystick(InputService.Instance.JoystickUsed);
    this._sensativity = ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity;
    JoystickLayoutSwitcher.Instance.OnLayoutChanged += new Action<JoystickLayoutSwitcher.KeyLayouts>(this.OnLayoutChanged);
    if (JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Two || JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Three)
    {
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon1, new GameActionHandle(this.OnShowSelector), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon2, new GameActionHandle(this.OnShowSelector), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon3, new GameActionHandle(this.OnShowSelector), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon4, new GameActionHandle(this.OnShowSelector), true);
      if (JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Three)
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Unholster, new GameActionHandle(this.OnBumper), true);
    }
    else if (JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.One)
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Unholster, new GameActionHandle(this.OnBumper), true);
    this.wasWeaponChanged = false;
  }

  private void OnLayoutChanged(JoystickLayoutSwitcher.KeyLayouts newLayout)
  {
    this.OnDisable();
    if (this._waitCoroutine == null)
      return;
    this.StopCoroutine(this._waitCoroutine);
  }

  private bool OnBumper(GameActionType type, bool down)
  {
    if (this._isConsole)
    {
      if (down && PlayerUtility.IsPlayerCanControlling)
      {
        if (JoystickLayoutSwitcher.Instance.CurrentLayout != JoystickLayoutSwitcher.KeyLayouts.Three)
        {
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickLeft, new GameActionHandle(this.OnStickMove), true);
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickRight, new GameActionHandle(this.OnStickMove), true);
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, new GameActionHandle(this.OnStickMove), true);
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, new GameActionHandle(this.OnStickMove), true);
        }
        if (this._waitCoroutine != null)
          this.StopCoroutine(this._waitCoroutine);
        this._currentView.Show();
        if (!this._currentView.Attacker.IsUnholstered)
        {
          this._selectedIndex = this._currentView.Attacker.currentWeapon;
          this._currentView.AssignCurrentItem();
          this._currentView.Attacker.ToggleCurrentWeapon();
          this.wasWeaponChanged = true;
        }
        else
          this.wasWeaponChanged = false;
        ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity = 0.0f;
        return true;
      }
      if (!down && PlayerUtility.IsPlayerCanControlling)
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, new GameActionHandle(this.OnStickMove));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, new GameActionHandle(this.OnStickMove));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, new GameActionHandle(this.OnStickMove));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, new GameActionHandle(this.OnStickMove));
        if (this._selectedIndex == this._currentView.Attacker.currentWeapon && !this.wasWeaponChanged)
        {
          this._currentView.Attacker.ToggleCurrentWeapon();
          this.wasWeaponChanged = false;
        }
        this._waitCoroutine = this.StartCoroutine(this.WaitBeforeHide(1f));
        ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity = this._sensativity;
        return true;
      }
    }
    return false;
  }

  private bool OnShowSelector(GameActionType type, bool down)
  {
    if (!PlayerUtility.IsPlayerCanControlling || !InputService.Instance.JoystickUsed)
      return false;
    if (this._waitCoroutine != null)
      this.StopCoroutine(this._waitCoroutine);
    if (down)
    {
      this._currentView.Show();
      if (!this._currentView.Attacker.IsUnholstered)
      {
        this._selectedIndex = this._currentView.Attacker.currentWeapon;
        this._currentView.AssignCurrentItem();
      }
      return this.OnStickMove(type, down);
    }
    this._waitCoroutine = this.StartCoroutine(this.WaitBeforeHide(1f));
    return true;
  }

  private bool OnStickMove(GameActionType type, bool down)
  {
    if (down)
    {
      AttackerPlayerComponent.Slots slot = AttackerPlayerComponent.Slots.WeaponNone;
      switch (type)
      {
        case GameActionType.Weapon1:
        case GameActionType.RStickUp:
          slot = AttackerPlayerComponent.Slots.WeaponHands;
          break;
        case GameActionType.Weapon2:
        case GameActionType.RStickLeft:
          slot = AttackerPlayerComponent.Slots.WeaponPrimary;
          break;
        case GameActionType.Weapon3:
        case GameActionType.RStickRight:
          slot = AttackerPlayerComponent.Slots.WeaponSecondary;
          break;
        case GameActionType.Weapon4:
        case GameActionType.RStickDown:
          slot = AttackerPlayerComponent.Slots.WeaponLamp;
          break;
      }
      if (slot != AttackerPlayerComponent.Slots.WeaponNone)
      {
        this._currentView.Attacker.SetWeaponByIndex(slot);
        this._selectedIndex = this._currentView.Attacker.currentWeapon;
        this.wasWeaponChanged = true;
      }
    }
    return true;
  }

  private void OnDisable()
  {
    InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Unholster, new GameActionHandle(this.OnBumper));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon1, new GameActionHandle(this.OnShowSelector));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon2, new GameActionHandle(this.OnShowSelector));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon3, new GameActionHandle(this.OnShowSelector));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon4, new GameActionHandle(this.OnShowSelector));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, new GameActionHandle(this.OnStickMove));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, new GameActionHandle(this.OnStickMove));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, new GameActionHandle(this.OnStickMove));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, new GameActionHandle(this.OnStickMove));
    ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity = this._sensativity;
    JoystickLayoutSwitcher.Instance.OnLayoutChanged -= new Action<JoystickLayoutSwitcher.KeyLayouts>(this.OnLayoutChanged);
  }

  private void OnJoystick(bool isUsed)
  {
    this._currentView = isUsed ? this._consoleView : this._pcView;
    this._pcView.gameObject.SetActive(!isUsed);
    this._consoleView.gameObject.SetActive(isUsed);
    this._isConsole = isUsed;
    this.DPadNavigationHint.SetActive(isUsed && (JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Two || JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Three));
    this.RStickNavigationHint.SetActive(isUsed && JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.One);
  }

  private IEnumerator WaitBeforeHide(float seconds)
  {
    while ((double) seconds > 0.0)
    {
      seconds -= Time.deltaTime;
      yield return (object) null;
    }
    if ((UnityEngine.Object) this._currentView != (UnityEngine.Object) null)
    {
      this._currentView.Hide();
      this.wasWeaponChanged = false;
    }
    this._waitCoroutine = (Coroutine) null;
  }
}
