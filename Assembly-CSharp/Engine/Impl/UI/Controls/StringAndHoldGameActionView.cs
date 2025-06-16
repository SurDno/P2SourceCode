using Engine.Source.Utility;
using InputServices;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class StringAndHoldGameActionView : GameActionViewBase {
	[SerializeField] private GameObject holdObject;
	[SerializeField] private KeyCodeStringView keyCodeStringView;

	private void OnEnable() {
		InputService.Instance.onJoystickUsedChanged += SetCodeView;
		ApplyValue(true);
	}

	private void OnDisable() {
		InputService.Instance.onJoystickUsedChanged -= SetCodeView;
	}

	protected override void ApplyValue(bool instant) {
		SetCodeView(InputService.Instance.JoystickUsed);
	}

	private void SetCodeView(bool joystick) {
		bool hold;
		keyCodeStringView.StringValue = InputUtility.GetHotKeyByAction(GetValue(), joystick, out hold);
		holdObject.SetActive(hold);
	}
}