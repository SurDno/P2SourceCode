using System;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;

namespace Engine.Source.Debugs;

[Initialisable]
public static class InputGroupDebug {
	private static string name = "[Input]";
	private static KeyCode key = KeyCode.R;
	private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
	private static KeyCode commonKey = KeyCode.F1;
	private static KeyCode joystickKey = KeyCode.F2;
	private static Color headerColor = Color.green;
	private static Color bodyColor = Color.white;
	private static Color bodyColor2 = Color.yellow;
	private static KeyCode[] keys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
	private static Color trueColor = Color.white;
	private static Color falseColor = ColorPreset.LightGray;
	private static BoolPlayerProperty commonVisible;
	private static BoolPlayerProperty joystickVisible;

	[Initialise]
	private static void Initialise() {
		InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action)(() => {
			GroupDebugService.RegisterGroup(name, key, modifficators, Update);
			commonVisible = BoolPlayerProperty.Create(() => commonVisible);
			joystickVisible = BoolPlayerProperty.Create(() => joystickVisible);
		});
	}

	private static void Update() {
		if (InputUtility.IsKeyDown(commonKey, KeyModifficator.Control))
			commonVisible.Value = !commonVisible;
		if (InputUtility.IsKeyDown(joystickKey, KeyModifficator.Control))
			joystickVisible.Value = !joystickVisible;
		var text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
		ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
		var text2 = "  Common " + (commonVisible ? "True" : (object)"False") + " [Control + " + commonKey + "]";
		ServiceLocator.GetService<GizmoService>().DrawText(text2, commonVisible ? trueColor : falseColor);
		var text3 = "  Joystick " + (joystickVisible ? "True" : (object)"False") + " [Control + " + joystickKey + "]";
		ServiceLocator.GetService<GizmoService>().DrawText(text3, joystickVisible ? trueColor : falseColor);
		if (commonVisible)
			DrawCommon();
		if (!joystickVisible)
			return;
		DrawJoystick();
	}

	private static void DrawCommon() {
		var str1 = "\nMouse present : " + Input.mousePresent + "\n";
		var axisName1 = "MouseX";
		var str2 = str1 + axisName1 + " : " + Input.GetAxisRaw(axisName1) + "\n";
		var axisName2 = "MouseY";
		var str3 = str2 + axisName2 + " : " + Input.GetAxisRaw(axisName2) + "\n";
		var axisName3 = "MouseWheel";
		var text1 = str3 + axisName3 + " : " + Input.GetAxisRaw(axisName3) + "\n";
		for (var index = 1; index <= 28; ++index) {
			var axisName4 = "JoystickAxis" + index;
			text1 = text1 + axisName4 + " : " + Input.GetAxisRaw(axisName4) + "\n";
		}

		ServiceLocator.GetService<GizmoService>().DrawText(text1, bodyColor);
		var text2 = "";
		if (Input.anyKey) {
			text2 = "Pressed : \n";
			foreach (var key in keys)
				if (Input.GetKey(key))
					text2 = text2 + key + "\n";
		}

		ServiceLocator.GetService<GizmoService>().DrawText(text2, bodyColor);
	}

	private static void DrawJoystick() {
		var text1 = "\nJoystick present : " + InputService.Instance.JoystickPresent + "\n";
		var joystickNames = Input.GetJoystickNames();
		if (joystickNames.Length != 0) {
			text1 += "\n";
			for (var index = 0; index < joystickNames.Length; ++index)
				text1 = text1 + "Joystick name : " + joystickNames[index] + "\n";
		}

		ServiceLocator.GetService<GizmoService>().DrawText(text1, bodyColor);
		var layout = InputService.Instance.Layout;
		if (layout == null)
			return;
		ServiceLocator.GetService<GizmoService>().DrawText("Layout : " + layout.Name, headerColor);
		ServiceLocator.GetService<GizmoService>().DrawText("\nAxes : ", headerColor);
		foreach (var ax in layout.Axes) {
			var name = ax.Name;
			var axis = InputService.Instance.GetAxis(name);
			var text2 = name + " : " + axis;
			ServiceLocator.GetService<GizmoService>().DrawText(text2, axis != 0.0 ? bodyColor2 : bodyColor);
		}

		ServiceLocator.GetService<GizmoService>().DrawText("\nButtons : ", headerColor);
		foreach (var axesToButton in layout.AxesToButtons) {
			var name = axesToButton.Name;
			var button1 = InputService.Instance.GetButton(name, false);
			var button2 = InputService.Instance.GetButton(name, true);
			var text3 = name + " : " + button1 + " , hold : " + button2;
			ServiceLocator.GetService<GizmoService>().DrawText(text3, button1 ? bodyColor2 : bodyColor);
		}

		foreach (var keysToButton in layout.KeysToButtons) {
			var name = keysToButton.Name;
			var button3 = InputService.Instance.GetButton(name, false);
			var button4 = InputService.Instance.GetButton(name, true);
			var text4 = name + " : " + button3 + " , hold : " + button4;
			ServiceLocator.GetService<GizmoService>().DrawText(text4, button3 ? bodyColor2 : bodyColor);
		}
	}
}