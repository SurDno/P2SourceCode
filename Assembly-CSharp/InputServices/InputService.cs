using System;
using System.Collections.Generic;
using Cofe.Utility;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputServices;

public class InputService : IUpdatable {
	private static InputService instance;
	private Dictionary<string, AxisBind> axes = new();
	private Dictionary<string, DateTime> buttons = new();
	private HashSet<string> buttonsPressed = new();
	private HashSet<string> holdButtonsPressed = new();
	private JoystickLayout layout;
	private TimeSpan holdDelay;
	private string currentJoystick;
	private float delayTime = 2f;
	private float currentTime;
	private bool _joystickUsed;

	public static InputService Instance {
		get {
			if (instance == null) {
				instance = new InputService();
				instance.Initialise();
			}

			return instance;
		}
	}

	public event Action OnSessionStateChanged;

	public void ChangeGameSession() {
		var sessionStateChanged = OnSessionStateChanged;
		if (sessionStateChanged == null)
			return;
		sessionStateChanged();
	}

	public event Action<bool> onJoystickUsedChanged;

	[Inspected]
	public bool JoystickUsed {
		get => _joystickUsed;
		set {
			if (_joystickUsed == value)
				return;
			_joystickUsed = value;
			var instance = CursorService.Instance;
			var position = instance.Position;
			var vector2 = (value ? Vector2.zero : new Vector2(Screen.width, Screen.height) * 0.5f) - position;
			instance.Move(vector2.x, vector2.y);
			var currentInputModule = EventSystem.current.currentInputModule;
			currentInputModule.DeactivateModule();
			currentInputModule.ActivateModule();
			instance.Visible = !value && instance.Free;
			var joystickUsedChanged = onJoystickUsedChanged;
			if (joystickUsedChanged != null)
				joystickUsedChanged(value);
		}
	}

	[Inspected] public bool JoystickPresent => GetJoystickName() != "";

	private string GetJoystickName() {
		foreach (var joystickName in Input.GetJoystickNames())
			if (joystickName != "")
				return joystickName;
		return "";
	}

	[Inspected] public JoystickLayout Layout => layout;

	public float GetAxis(string name) {
		AxisBind axisBind;
		if (axes.TryGetValue(name, out axisBind)) {
			var num = Input.GetAxisRaw(axisBind.Axis);
			if (axisBind.Normalize)
				num = (float)(num * 0.5 + 0.5);
			if (num > (double)axisBind.Dead)
				return (float)((num - (double)axisBind.Dead) / (1.0 - axisBind.Dead));
			if (num < -(double)axisBind.Dead)
				return (float)((num + (double)axisBind.Dead) / (1.0 - axisBind.Dead));
		}

		return 0.0f;
	}

	public bool GetButton(string name, bool hold) {
		DateTime dateTime;
		if (!buttons.TryGetValue(name, out dateTime))
			return false;
		return !hold || dateTime == DateTime.MinValue;
	}

	public bool GetButtonDown(string name, bool hold) {
		return hold ? holdButtonsPressed.Contains(name) : buttonsPressed.Contains(name);
	}

	public float GetHoldProgress(string name) {
		DateTime dateTime;
		return buttons.TryGetValue(name, out dateTime) && dateTime > DateTime.MinValue
			? (float)(DateTime.UtcNow - dateTime).TotalSeconds / (float)holdDelay.TotalSeconds
			: 0.0f;
	}

	private void Initialise() {
		holdDelay = TimeSpan.FromSeconds(ExternalSettingsInstance<ExternalInputSettings>.Instance.HoldDelay);
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
	}

	public void ComputeUpdate() {
		if (!ExternalSettingsInstance<ExternalInputSettings>.Instance.UseJoystick)
			return;
		CheckJoystick();
		if (layout == null)
			return;
		ComputeButtons();
		ComputeHoldButtons();
	}

	private void CheckJoystick() {
		var realtimeSinceStartup = Time.realtimeSinceStartup;
		if (currentTime + (double)delayTime > realtimeSinceStartup)
			return;
		currentTime = realtimeSinceStartup;
		var joystickName = GetJoystickName();
		if (!(joystickName != currentJoystick))
			return;
		currentJoystick = joystickName;
		InitialiseJoystick();
	}

	private void ComputeButtons() {
		buttonsPressed.Clear();
		var flag = false;
		foreach (var axesToButton in layout.AxesToButtons) {
			var axis = GetAxis(axesToButton.Axis);
			if (axesToButton.Inverse)
				axis *= -1f;
			if (buttons.ContainsKey(axesToButton.Name)) {
				if (axis < (double)axesToButton.Min) {
					buttons.Remove(axesToButton.Name);
					flag = true;
				}
			} else if (axis > (double)axesToButton.Max) {
				buttons.Add(axesToButton.Name, DateTime.UtcNow);
				buttonsPressed.Add(axesToButton.Name);
				flag = true;
			}
		}

		foreach (var keysToButton in layout.KeysToButtons) {
			var key = Input.GetKey((KeyCode)keysToButton.KeyCode);
			if (buttons.ContainsKey(keysToButton.Name)) {
				if (!key) {
					buttons.Remove(keysToButton.Name);
					flag = true;
				}
			} else if (key) {
				buttons.Add(keysToButton.Name, DateTime.UtcNow);
				buttonsPressed.Add(keysToButton.Name);
				flag = true;
			}
		}

		foreach (var ax in layout.Axes) {
			var num = Input.GetAxisRaw(ax.Axis);
			if (ax.Normalize)
				num = (float)(num * 0.5 + 0.5);
			if (num > (double)ax.Dead || num < -(double)ax.Dead) {
				flag = true;
				break;
			}
		}

		if (!flag)
			return;
		JoystickUsed = true;
	}

	private void ComputeHoldButtons() {
		holdButtonsPressed.Clear();
		label_1:
		foreach (var button in buttons) {
			var dateTime = button.Value;
			if (!(dateTime == DateTime.MinValue) && DateTime.UtcNow - dateTime > holdDelay) {
				holdButtonsPressed.Add(button.Key);
				buttons[button.Key] = DateTime.MinValue;
				goto label_1;
			}
		}
	}

	private void InitialiseJoystick() {
		layout = null;
		axes.Clear();
		if (this.currentJoystick.IsNullOrEmpty())
			return;
		var currentJoystick = this.currentJoystick;
		layout = currentJoystick.IndexOf("xbox", StringComparison.InvariantCultureIgnoreCase) == -1
			? !(currentJoystick == "Wireless Controller")
				? ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Layouts[2]
				: ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Layouts[1]
			: ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Layouts[0];
		foreach (var ax in layout.Axes)
			axes[ax.Name] = ax;
	}
}