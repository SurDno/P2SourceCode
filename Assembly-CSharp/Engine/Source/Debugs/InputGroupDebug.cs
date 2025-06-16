using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using System;
using System.Linq.Expressions;
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

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() =>
      {
        GroupDebugService.RegisterGroup(InputGroupDebug.name, InputGroupDebug.key, InputGroupDebug.modifficators, new Action(InputGroupDebug.Update));
        InputGroupDebug.commonVisible = BoolPlayerProperty.Create<BoolPlayerProperty>((Expression<Func<BoolPlayerProperty>>) (() => InputGroupDebug.commonVisible));
        InputGroupDebug.joystickVisible = BoolPlayerProperty.Create<BoolPlayerProperty>((Expression<Func<BoolPlayerProperty>>) (() => InputGroupDebug.joystickVisible));
      });
    }

    private static void Update()
    {
      if (InputUtility.IsKeyDown(InputGroupDebug.commonKey, KeyModifficator.Control))
        InputGroupDebug.commonVisible.Value = !(bool) InputGroupDebug.commonVisible;
      if (InputUtility.IsKeyDown(InputGroupDebug.joystickKey, KeyModifficator.Control))
        InputGroupDebug.joystickVisible.Value = !(bool) InputGroupDebug.joystickVisible;
      string text1 = "\n" + InputGroupDebug.name + " (" + InputUtility.GetHotKeyText(InputGroupDebug.key, InputGroupDebug.modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, InputGroupDebug.headerColor);
      string text2 = "  Common " + ((bool) InputGroupDebug.commonVisible ? (object) "True" : (object) "False") + " [Control + " + (object) InputGroupDebug.commonKey + "]";
      ServiceLocator.GetService<GizmoService>().DrawText(text2, (bool) InputGroupDebug.commonVisible ? InputGroupDebug.trueColor : InputGroupDebug.falseColor);
      string text3 = "  Joystick " + ((bool) InputGroupDebug.joystickVisible ? (object) "True" : (object) "False") + " [Control + " + (object) InputGroupDebug.joystickKey + "]";
      ServiceLocator.GetService<GizmoService>().DrawText(text3, (bool) InputGroupDebug.joystickVisible ? InputGroupDebug.trueColor : InputGroupDebug.falseColor);
      if ((bool) InputGroupDebug.commonVisible)
        InputGroupDebug.DrawCommon();
      if (!(bool) InputGroupDebug.joystickVisible)
        return;
      InputGroupDebug.DrawJoystick();
    }

    private static void DrawCommon()
    {
      string str1 = "\nMouse present : " + Input.mousePresent.ToString() + "\n";
      string axisName1 = "MouseX";
      string str2 = str1 + axisName1 + " : " + (object) Input.GetAxisRaw(axisName1) + "\n";
      string axisName2 = "MouseY";
      string str3 = str2 + axisName2 + " : " + (object) Input.GetAxisRaw(axisName2) + "\n";
      string axisName3 = "MouseWheel";
      string text1 = str3 + axisName3 + " : " + (object) Input.GetAxisRaw(axisName3) + "\n";
      for (int index = 1; index <= 28; ++index)
      {
        string axisName4 = "JoystickAxis" + (object) index;
        text1 = text1 + axisName4 + " : " + (object) Input.GetAxisRaw(axisName4) + "\n";
      }
      ServiceLocator.GetService<GizmoService>().DrawText(text1, InputGroupDebug.bodyColor);
      string text2 = "";
      if (Input.anyKey)
      {
        text2 = "Pressed : \n";
        foreach (KeyCode key in InputGroupDebug.keys)
        {
          if (Input.GetKey(key))
            text2 = text2 + (object) key + "\n";
        }
      }
      ServiceLocator.GetService<GizmoService>().DrawText(text2, InputGroupDebug.bodyColor);
    }

    private static void DrawJoystick()
    {
      string text1 = "\nJoystick present : " + InputService.Instance.JoystickPresent.ToString() + "\n";
      string[] joystickNames = Input.GetJoystickNames();
      if (joystickNames.Length != 0)
      {
        text1 += "\n";
        for (int index = 0; index < joystickNames.Length; ++index)
          text1 = text1 + "Joystick name : " + joystickNames[index] + "\n";
      }
      ServiceLocator.GetService<GizmoService>().DrawText(text1, InputGroupDebug.bodyColor);
      JoystickLayout layout = InputService.Instance.Layout;
      if (layout == null)
        return;
      ServiceLocator.GetService<GizmoService>().DrawText("Layout : " + layout.Name, InputGroupDebug.headerColor);
      ServiceLocator.GetService<GizmoService>().DrawText("\nAxes : ", InputGroupDebug.headerColor);
      foreach (AxisBind ax in layout.Axes)
      {
        string name = ax.Name;
        float axis = InputService.Instance.GetAxis(name);
        string text2 = name + " : " + (object) axis;
        ServiceLocator.GetService<GizmoService>().DrawText(text2, (double) axis != 0.0 ? InputGroupDebug.bodyColor2 : InputGroupDebug.bodyColor);
      }
      ServiceLocator.GetService<GizmoService>().DrawText("\nButtons : ", InputGroupDebug.headerColor);
      foreach (AxisToButton axesToButton in layout.AxesToButtons)
      {
        string name = axesToButton.Name;
        bool button1 = InputService.Instance.GetButton(name, false);
        bool button2 = InputService.Instance.GetButton(name, true);
        string text3 = name + " : " + button1.ToString() + " , hold : " + button2.ToString();
        ServiceLocator.GetService<GizmoService>().DrawText(text3, button1 ? InputGroupDebug.bodyColor2 : InputGroupDebug.bodyColor);
      }
      foreach (KeyToButton keysToButton in layout.KeysToButtons)
      {
        string name = keysToButton.Name;
        bool button3 = InputService.Instance.GetButton(name, false);
        bool button4 = InputService.Instance.GetButton(name, true);
        string text4 = name + " : " + button3.ToString() + " , hold : " + button4.ToString();
        ServiceLocator.GetService<GizmoService>().DrawText(text4, button3 ? InputGroupDebug.bodyColor2 : InputGroupDebug.bodyColor);
      }
    }
  }
}
