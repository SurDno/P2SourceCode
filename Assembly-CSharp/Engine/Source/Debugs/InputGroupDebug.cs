using System;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class InputGroupDebug
  {
    private static string name = "[Input]";
    private static KeyCode key = KeyCode.R;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static KeyCode commonKey = KeyCode.F1;
    private static KeyCode joystickKey = KeyCode.F2;
    private static Color headerColor = Color.green;
    private static Color bodyColor = Color.white;
    private static Color bodyColor2 = Color.yellow;
    private static KeyCode[] keys = (KeyCode[]) Enum.GetValues(typeof (KeyCode));
    private static Color trueColor = Color.white;
    private static Color falseColor = ColorPreset.LightGray;
    private static BoolPlayerProperty commonVisible;
    private static BoolPlayerProperty joystickVisible;

    [Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() =>
      {
        GroupDebugService.RegisterGroup(name, key, modifficators, Update);
        commonVisible = BoolPlayerProperty.Create(() => commonVisible);
        joystickVisible = BoolPlayerProperty.Create(() => joystickVisible);
      });
    }

    private static void Update()
    {
      if (InputUtility.IsKeyDown(commonKey, KeyModifficator.Control))
        commonVisible.Value = !commonVisible;
      if (InputUtility.IsKeyDown(joystickKey, KeyModifficator.Control))
        joystickVisible.Value = !joystickVisible;
      string text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
      string text2 = "  Common " + (commonVisible ? "True" : (object) "False") + " [Control + " + commonKey + "]";
      ServiceLocator.GetService<GizmoService>().DrawText(text2, commonVisible ? trueColor : falseColor);
      string text3 = "  Joystick " + (joystickVisible ? "True" : (object) "False") + " [Control + " + joystickKey + "]";
      ServiceLocator.GetService<GizmoService>().DrawText(text3, joystickVisible ? trueColor : falseColor);
      if (commonVisible)
        DrawCommon();
      if (!joystickVisible)
        return;
      DrawJoystick();
    }

    private static void DrawCommon()
    {
      string str1 = "\nMouse present : " + Input.mousePresent + "\n";
      string axisName1 = "MouseX";
      string str2 = str1 + axisName1 + " : " + Input.GetAxisRaw(axisName1) + "\n";
      string axisName2 = "MouseY";
      string str3 = str2 + axisName2 + " : " + Input.GetAxisRaw(axisName2) + "\n";
      string axisName3 = "MouseWheel";
      string text1 = str3 + axisName3 + " : " + Input.GetAxisRaw(axisName3) + "\n";
      for (int index = 1; index <= 28; ++index)
      {
        string axisName4 = "JoystickAxis" + index;
        text1 = text1 + axisName4 + " : " + Input.GetAxisRaw(axisName4) + "\n";
      }
      ServiceLocator.GetService<GizmoService>().DrawText(text1, bodyColor);
      string text2 = "";
      if (Input.anyKey)
      {
        text2 = "Pressed : \n";
        foreach (KeyCode key in keys)
        {
          if (Input.GetKey(key))
            text2 = text2 + key + "\n";
        }
      }
      ServiceLocator.GetService<GizmoService>().DrawText(text2, bodyColor);
    }

    private static void DrawJoystick()
    {
      string text1 = "\nJoystick present : " + InputService.Instance.JoystickPresent + "\n";
      string[] joystickNames = Input.GetJoystickNames();
      if (joystickNames.Length != 0)
      {
        text1 += "\n";
        for (int index = 0; index < joystickNames.Length; ++index)
          text1 = text1 + "Joystick name : " + joystickNames[index] + "\n";
      }
      ServiceLocator.GetService<GizmoService>().DrawText(text1, bodyColor);
      JoystickLayout layout = InputService.Instance.Layout;
      if (layout == null)
        return;
      ServiceLocator.GetService<GizmoService>().DrawText("Layout : " + layout.Name, headerColor);
      ServiceLocator.GetService<GizmoService>().DrawText("\nAxes : ", headerColor);
      foreach (AxisBind ax in layout.Axes)
      {
        string name = ax.Name;
        float axis = InputService.Instance.GetAxis(name);
        string text2 = name + " : " + axis;
        ServiceLocator.GetService<GizmoService>().DrawText(text2, axis != 0.0 ? bodyColor2 : bodyColor);
      }
      ServiceLocator.GetService<GizmoService>().DrawText("\nButtons : ", headerColor);
      foreach (AxisToButton axesToButton in layout.AxesToButtons)
      {
        string name = axesToButton.Name;
        bool button1 = InputService.Instance.GetButton(name, false);
        bool button2 = InputService.Instance.GetButton(name, true);
        string text3 = name + " : " + button1 + " , hold : " + button2;
        ServiceLocator.GetService<GizmoService>().DrawText(text3, button1 ? bodyColor2 : bodyColor);
      }
      foreach (KeyToButton keysToButton in layout.KeysToButtons)
      {
        string name = keysToButton.Name;
        bool button3 = InputService.Instance.GetButton(name, false);
        bool button4 = InputService.Instance.GetButton(name, true);
        string text4 = name + " : " + button3 + " , hold : " + button4;
        ServiceLocator.GetService<GizmoService>().DrawText(text4, button3 ? bodyColor2 : bodyColor);
      }
    }
  }
}
