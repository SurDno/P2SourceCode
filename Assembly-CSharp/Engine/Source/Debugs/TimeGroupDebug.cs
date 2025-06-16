// Decompiled with JetBrains decompiler
// Type: Engine.Source.Debugs.TimeGroupDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class TimeGroupDebug
  {
    private static string name = "[Time]";
    private static KeyCode key = KeyCode.T;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = Color.green;
    private static Color bodyColor = Color.white;

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(TimeGroupDebug.name, TimeGroupDebug.key, TimeGroupDebug.modifficators, new Action(TimeGroupDebug.Update)));
    }

    private static void Update()
    {
      TimeService service = ServiceLocator.GetService<TimeService>();
      if (InputUtility.IsKeyDown(KeyCode.Pause))
        InstanceByRequest<EngineApplication>.Instance.IsPaused = !InstanceByRequest<EngineApplication>.Instance.IsPaused;
      if (InputUtility.IsKeyDown(KeyCode.KeypadPlus))
      {
        int solarTimeFactor = (int) service.SolarTimeFactor;
        int num = solarTimeFactor != 0 ? solarTimeFactor * 2 : 1;
        service.SolarTimeFactor = (float) num;
      }
      if (InputUtility.IsKeyDown(KeyCode.KeypadMinus))
      {
        int solarTimeFactor = (int) service.SolarTimeFactor;
        int num = solarTimeFactor > 1 ? solarTimeFactor / 2 : 0;
        service.SolarTimeFactor = (float) num;
      }
      if (InputUtility.IsKeyDown(KeyCode.KeypadMultiply))
        service.SolarTimeFactor = service.DefaultTimeFactor;
      if (InputUtility.IsKeyDown(KeyCode.KeypadDivide))
        service.SolarTimeFactor = 0.0f;
      if (InputUtility.IsKeyDown(KeyCode.Keypad7))
        service.SolarTime += TimeSpan.FromHours(1.0);
      if (InputUtility.IsKeyDown(KeyCode.Keypad8))
        service.SolarTime = TimeSpan.Zero;
      if (InputUtility.IsKeyDown(KeyCode.Keypad9))
      {
        TimeSpan timeSpan = service.SolarTime - TimeSpan.FromHours(1.0);
        if (timeSpan < TimeSpan.Zero)
          timeSpan = TimeSpan.Zero;
        service.SolarTime = timeSpan;
      }
      if (InputUtility.IsKeyDown(KeyCode.Keypad4))
      {
        TimeSpan time = service.GameTime + TimeSpan.FromHours(1.0);
        service.SetGameTime(time);
      }
      if (InputUtility.IsKeyDown(KeyCode.Keypad5))
        service.SetGameTime(TimeSpan.Zero);
      if (InputUtility.IsKeyDown(KeyCode.Keypad6))
      {
        TimeSpan time = service.GameTime - TimeSpan.FromHours(1.0);
        if (time < TimeSpan.Zero)
          time = TimeSpan.Zero;
        service.SetGameTime(time);
      }
      if (InputUtility.IsKeyDown(KeyCode.Keypad1))
      {
        int gameTimeFactor = (int) service.GameTimeFactor;
        int num = gameTimeFactor != 0 ? gameTimeFactor * 2 : 1;
        service.GameTimeFactor = (float) num;
      }
      if (InputUtility.IsKeyDown(KeyCode.Keypad2))
      {
        int gameTimeFactor = (int) service.GameTimeFactor;
        int num = gameTimeFactor > 1 ? gameTimeFactor / 2 : 0;
        service.GameTimeFactor = (float) num;
      }
      if (InputUtility.IsKeyDown(KeyCode.Keypad3))
        service.GameTimeFactor = service.DefaultTimeFactor;
      if (InputUtility.IsKeyDown(KeyCode.Keypad0))
        service.GameTimeFactor = 0.0f;
      string text1 = "\n" + TimeGroupDebug.name + " (" + InputUtility.GetHotKeyText(TimeGroupDebug.key, TimeGroupDebug.modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, TimeGroupDebug.headerColor);
      string text2 = "  Is Game Paused : " + InstanceByRequest<EngineApplication>.Instance.IsPaused.ToString() + "     [hotkeys : pause pause]" + "\n  Solar Time : " + service.SolarTime.ToLongTimeString() + "     [hotkeys : add hour 7 , set 00:00 : 8 , remove hour : 9]" + "\n  Solar Time Day : " + (object) service.SolarTime.GetTimesOfDay() + "\n  Solar Time Factor : " + (object) service.SolarTimeFactor + "     [hotkeys : great + , less - ,  default * , stop /]" + "\n  Game Time : " + service.GameTime.ToLongTimeString() + "     [hotkeys : add hour 4 , set 00:00 : 5 , remove hour : 6]" + "\n  Game Time Day: " + (object) service.GameTime.GetTimesOfDay() + "\n  Game Time Factor : " + (object) service.GameTimeFactor + "     [hotkeys : great 1 , less 2 ,  default 3 , stop 0]" + "\n  Default Time Factor : " + (object) service.DefaultTimeFactor;
      ServiceLocator.GetService<GizmoService>().DrawText(text2, TimeGroupDebug.bodyColor);
    }
  }
}
