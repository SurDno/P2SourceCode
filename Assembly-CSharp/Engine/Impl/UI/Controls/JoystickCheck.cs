using System;
using InputServices;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class JoystickCheck : MonoBehaviour {
	[SerializeField] private HideableView view;
	private Action<bool> onJoystickAction;

	private void OnDisable() {
		InputService.Instance.onJoystickUsedChanged -= onJoystickAction;
	}

	private void OnEnable() {
		var instance = InputService.Instance;
		view.Visible = instance.JoystickUsed;
		if (onJoystickAction == null)
			onJoystickAction = OnJoystick;
		instance.onJoystickUsedChanged += onJoystickAction;
	}

	private void OnJoystick(bool joystick) {
		view.Visible = joystick;
	}
}