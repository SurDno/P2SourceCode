using System;
using System.Collections.Generic;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class GroupDebugService
  {
    private static string name = "[Help]";
    private static KeyCode key = KeyCode.F1;
    private static KeyModifficator modifficators = KeyModifficator.None;
    private static Color headerColor = Color.yellow;
    private static Color trueColor = Color.white;
    private static Color falseColor = ColorPreset.LightGray;
    private static bool visible;
    private static List<GroupInfo> groups = new List<GroupInfo>();
    private static List<Action> handlers = new List<Action>();

    public static bool IsGroupVisible(string name)
    {
      foreach (GroupInfo group in groups)
      {
        if (group.Name == name)
          return group.Visible;
      }
      return false;
    }

    public static void RegisterGroup(Action handler) => handlers.Add(handler);

    public static void RegisterGroup(
      string name,
      KeyCode key,
      KeyModifficator modifficators,
      Action handler)
    {
      groups.Add(new GroupInfo {
        Name = name,
        HotKey = key,
        Modifficators = modifficators,
        Handler = handler,
        Visible = new BoolPlayerProperty(name)
      });
      groups.Sort((a, b) => a.HotKey.CompareTo((object) b.HotKey));
    }

    [Initialise]
    private static void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(new UpdatableProxy((Action) (() => Update())));
    }

    private static void Update()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsDebug)
        return;
      foreach (Action handler in handlers)
        handler();
      if (InputUtility.IsKeyDown(key))
        visible = !visible;
      string text1 = name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
      foreach (GroupInfo group in groups)
      {
        if (InputUtility.IsKeyDown(group.HotKey, group.Modifficators))
          group.Visible.Value = !group.Visible;
        if (visible)
        {
          string text2 = "  " + group.Name + " (" + InputUtility.GetHotKeyText(group.HotKey, group.Modifficators) + ")";
          ServiceLocator.GetService<GizmoService>().DrawText(text2, group.Visible ? trueColor : falseColor);
        }
        else if (group.Visible)
          group.Handler();
      }
    }

    public class GroupInfo
    {
      public string Name;
      public KeyCode HotKey;
      public KeyModifficator Modifficators;
      public Action Handler;
      public BoolPlayerProperty Visible;
    }
  }
}
