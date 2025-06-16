using Engine.Source.Commons;
using Engine.Source.Settings;
using InputServices;

namespace Engine.Impl.UI.Menu.Main;

public class ControlSettingsView : SettingsView {
	private FloatSettingsValueView mouseSensitivityView;
	private BoolSettingsValueView mouseInvertView;
	private FloatSettingsValueView joystickSensitivityView;
	private BoolSettingsValueView joystickInvertView;
	private InputGameSetting inputGameSettings;

	protected override void Awake() {
		inputGameSettings = InstanceByRequest<InputGameSetting>.Instance;
		layout = Instantiate(listLayoutPrefab, transform, false);
		ApplySettings();
		base.Awake();
	}

	private void ApplySettings() {
		this.mouseSensitivityView = Instantiate(floatValueViewPrefab, layout.Content, false);
		this.mouseSensitivityView.SetName("{UI.Menu.Main.Settings.Control.Mouse.Sensitivity}");
		this.mouseSensitivityView.SetMinValue(inputGameSettings.MouseSensitivity.MinValue);
		this.mouseSensitivityView.SetMaxValue(inputGameSettings.MouseSensitivity.MaxValue);
		this.mouseSensitivityView.SetValueNameFunction(SensitivityName);
		this.mouseSensitivityView.SetSetting(inputGameSettings.MouseSensitivity);
		this.mouseSensitivityView.SetValueValidationFunction(SensitivityRound, 0.1f);
		var mouseSensitivityView = this.mouseSensitivityView;
		mouseSensitivityView.VisibleValueChangeEvent = mouseSensitivityView.VisibleValueChangeEvent + OnAutoValueChange;
		this.mouseInvertView = Instantiate(boolValueViewPrefab, layout.Content, false);
		this.mouseInvertView.SetName("{UI.Menu.Main.Settings.Control.Mouse.Invert}");
		this.mouseInvertView.SetSetting(inputGameSettings.MouseInvert);
		var mouseInvertView = this.mouseInvertView;
		mouseInvertView.VisibleValueChangeEvent = mouseInvertView.VisibleValueChangeEvent + OnAutoValueChange;
		this.joystickSensitivityView = Instantiate(floatValueViewPrefab, layout.Content, false);
		this.joystickSensitivityView.SetName("{UI.Menu.Main.Settings.Control.Joystick.Sensitivity}");
		this.joystickSensitivityView.SetMinValue(inputGameSettings.JoystickSensitivity.MinValue);
		this.joystickSensitivityView.SetMaxValue(inputGameSettings.JoystickSensitivity.MaxValue);
		this.joystickSensitivityView.SetValueNameFunction(SensitivityName);
		this.joystickSensitivityView.SetSetting(inputGameSettings.JoystickSensitivity);
		this.joystickSensitivityView.SetValueValidationFunction(SensitivityRound, 0.1f);
		var joystickSensitivityView = this.joystickSensitivityView;
		joystickSensitivityView.VisibleValueChangeEvent =
			joystickSensitivityView.VisibleValueChangeEvent + OnAutoValueChange;
		this.joystickInvertView = Instantiate(boolValueViewPrefab, layout.Content, false);
		this.joystickInvertView.SetName("{UI.Menu.Main.Settings.Control.Joystick.Invert}");
		this.joystickInvertView.SetSetting(inputGameSettings.JoystickInvert);
		var joystickInvertView = this.joystickInvertView;
		joystickInvertView.VisibleValueChangeEvent = joystickInvertView.VisibleValueChangeEvent + OnAutoValueChange;
	}

	private void OnAutoValueChange<T>(SettingsValueView<T> view) {
		view.ApplyVisibleValue();
		inputGameSettings.Apply();
	}

	protected override void OnButtonReset() {
		mouseSensitivityView.ResetValue();
		mouseInvertView.ResetValue();
		joystickSensitivityView.ResetValue();
		joystickInvertView.ResetValue();
		inputGameSettings.Apply();
		UpdateViews();
	}

	protected override void OnJoystick(bool isUsed) {
		base.OnJoystick(isUsed);
		var joystickPresent = InputService.Instance.JoystickPresent;
		joystickSensitivityView.gameObject.SetActive(joystickPresent | isUsed);
		joystickInvertView.gameObject.SetActive(joystickPresent | isUsed);
	}

	protected override void OnEnable() {
		base.OnEnable();
		UpdateViews();
	}

	private void UpdateViews() {
		mouseSensitivityView.RevertVisibleValue();
		mouseInvertView.RevertVisibleValue();
		joystickSensitivityView.RevertVisibleValue();
		joystickInvertView.RevertVisibleValue();
	}

	private string SensitivityName(float value) {
		return value.ToString("n1");
	}

	private float SensitivityRound(float value) {
		return SettingsViewUtility.RoundValue(value, 0.1f);
	}
}