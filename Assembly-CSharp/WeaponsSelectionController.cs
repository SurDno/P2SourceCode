using System.Collections;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;

public class WeaponsSelectionController : MonoBehaviour {
	[SerializeField] private HUDWeaponsView _consoleView;
	[SerializeField] private HUDWeaponsView _pcView;
	private HUDWeaponsView _currentView;
	private bool _isConsole;
	private int _selectedIndex = -1;
	private float _sensativity;
	private Coroutine _waitCoroutine;
	[SerializeField] private GameObject DPadNavigationHint;
	[SerializeField] private GameObject RStickNavigationHint;
	private bool wasWeaponChanged;

	private void OnEnable() {
		InputService.Instance.onJoystickUsedChanged += OnJoystick;
		OnJoystick(InputService.Instance.JoystickUsed);
		_sensativity = ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity;
		JoystickLayoutSwitcher.Instance.OnLayoutChanged += OnLayoutChanged;
		if (JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Two ||
		    JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Three) {
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon1, OnShowSelector, true);
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon2, OnShowSelector, true);
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon3, OnShowSelector, true);
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Weapon4, OnShowSelector, true);
			if (JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.Three)
				ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Unholster, OnBumper, true);
		} else if (JoystickLayoutSwitcher.Instance.CurrentLayout == JoystickLayoutSwitcher.KeyLayouts.One)
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Unholster, OnBumper, true);

		wasWeaponChanged = false;
	}

	private void OnLayoutChanged(JoystickLayoutSwitcher.KeyLayouts newLayout) {
		OnDisable();
		if (_waitCoroutine == null)
			return;
		StopCoroutine(_waitCoroutine);
	}

	private bool OnBumper(GameActionType type, bool down) {
		if (_isConsole) {
			if (down && PlayerUtility.IsPlayerCanControlling) {
				if (JoystickLayoutSwitcher.Instance.CurrentLayout != JoystickLayoutSwitcher.KeyLayouts.Three) {
					ServiceLocator.GetService<GameActionService>()
						.AddListener(GameActionType.RStickLeft, OnStickMove, true);
					ServiceLocator.GetService<GameActionService>()
						.AddListener(GameActionType.RStickRight, OnStickMove, true);
					ServiceLocator.GetService<GameActionService>()
						.AddListener(GameActionType.RStickUp, OnStickMove, true);
					ServiceLocator.GetService<GameActionService>()
						.AddListener(GameActionType.RStickDown, OnStickMove, true);
				}

				if (_waitCoroutine != null)
					StopCoroutine(_waitCoroutine);
				_currentView.Show();
				if (!_currentView.Attacker.IsUnholstered) {
					_selectedIndex = _currentView.Attacker.currentWeapon;
					_currentView.AssignCurrentItem();
					_currentView.Attacker.ToggleCurrentWeapon();
					wasWeaponChanged = true;
				} else
					wasWeaponChanged = false;

				ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity = 0.0f;
				return true;
			}

			if (!down && PlayerUtility.IsPlayerCanControlling) {
				ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, OnStickMove);
				ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, OnStickMove);
				ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, OnStickMove);
				ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, OnStickMove);
				if (_selectedIndex == _currentView.Attacker.currentWeapon && !wasWeaponChanged) {
					_currentView.Attacker.ToggleCurrentWeapon();
					wasWeaponChanged = false;
				}

				_waitCoroutine = StartCoroutine(WaitBeforeHide(1f));
				ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity = _sensativity;
				return true;
			}
		}

		return false;
	}

	private bool OnShowSelector(GameActionType type, bool down) {
		if (!PlayerUtility.IsPlayerCanControlling || !InputService.Instance.JoystickUsed)
			return false;
		if (_waitCoroutine != null)
			StopCoroutine(_waitCoroutine);
		if (down) {
			_currentView.Show();
			if (!_currentView.Attacker.IsUnholstered) {
				_selectedIndex = _currentView.Attacker.currentWeapon;
				_currentView.AssignCurrentItem();
			}

			return OnStickMove(type, down);
		}

		_waitCoroutine = StartCoroutine(WaitBeforeHide(1f));
		return true;
	}

	private bool OnStickMove(GameActionType type, bool down) {
		if (down) {
			var slot = AttackerPlayerComponent.Slots.WeaponNone;
			switch (type) {
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

			if (slot != AttackerPlayerComponent.Slots.WeaponNone) {
				_currentView.Attacker.SetWeaponByIndex(slot);
				_selectedIndex = _currentView.Attacker.currentWeapon;
				wasWeaponChanged = true;
			}
		}

		return true;
	}

	private void OnDisable() {
		InputService.Instance.onJoystickUsedChanged -= OnJoystick;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Unholster, OnBumper);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon1, OnShowSelector);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon2, OnShowSelector);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon3, OnShowSelector);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Weapon4, OnShowSelector);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, OnStickMove);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, OnStickMove);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, OnStickMove);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, OnStickMove);
		ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity = _sensativity;
		JoystickLayoutSwitcher.Instance.OnLayoutChanged -= OnLayoutChanged;
	}

	private void OnJoystick(bool isUsed) {
		_currentView = isUsed ? _consoleView : _pcView;
		_pcView.gameObject.SetActive(!isUsed);
		_consoleView.gameObject.SetActive(isUsed);
		_isConsole = isUsed;
		DPadNavigationHint.SetActive(isUsed &&
		                             (JoystickLayoutSwitcher.Instance.CurrentLayout ==
		                              JoystickLayoutSwitcher.KeyLayouts.Two ||
		                              JoystickLayoutSwitcher.Instance.CurrentLayout ==
		                              JoystickLayoutSwitcher.KeyLayouts.Three));
		RStickNavigationHint.SetActive(isUsed &&
		                               JoystickLayoutSwitcher.Instance.CurrentLayout ==
		                               JoystickLayoutSwitcher.KeyLayouts.One);
	}

	private IEnumerator WaitBeforeHide(float seconds) {
		while (seconds > 0.0) {
			seconds -= Time.deltaTime;
			yield return null;
		}

		if (_currentView != null) {
			_currentView.Hide();
			wasWeaponChanged = false;
		}

		_waitCoroutine = null;
	}
}