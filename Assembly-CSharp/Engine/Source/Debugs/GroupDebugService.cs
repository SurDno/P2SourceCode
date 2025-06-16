using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    private static List<GroupDebugService.GroupInfo> groups = new List<GroupDebugService.GroupInfo>();
    private static List<Action> handlers = new List<Action>();

    public static bool IsGroupVisible(string name)
    {
      foreach (GroupDebugService.GroupInfo group in GroupDebugService.groups)
      {
        if (group.Name == name)
          return (bool) group.Visible;
      }
      return false;
    }

    public static void RegisterGroup(Action handler) => GroupDebugService.handlers.Add(handler);

    public static void RegisterGroup(
      string name,
      KeyCode key,
      KeyModifficator modifficators,
      Action handler)
    {
      GroupDebugService.groups.Add(new GroupDebugService.GroupInfo()
      {
        Name = name,
        HotKey = key,
        Modifficators = modifficators,
        Handler = handler,
        Visible = new BoolPlayerProperty(name)
      });
      GroupDebugService.groups.Sort((Comparison<GroupDebugService.GroupInfo>) ((a, b) => a.HotKey.CompareTo((object) b.HotKey)));
    }

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) new UpdatableProxy((Action) (() => GroupDebugService.Update())));
    }

    private static void Update()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsDebug)
        return;
      foreach (Action handler in GroupDebugService.handlers)
        handler();
      if (InputUtility.IsKeyDown(GroupDebugService.key))
        GroupDebugService.visible = !GroupDebugService.visible;
      string text1 = GroupDebugService.name + " (" + InputUtility.GetHotKeyText(GroupDebugService.key, GroupDebugService.modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, GroupDebugService.headerColor);
      foreach (GroupDebugService.GroupInfo group in GroupDebugService.groups)
      {
        if (InputUtility.IsKeyDown(group.HotKey, group.Modifficators))
          group.Visible.Value = !(bool) group.Visible;
        if (GroupDebugService.visible)
        {
          string text2 = "  " + group.Name + " (" + InputUtility.GetHotKeyText(group.HotKey, group.Modifficators) + ")";
          ServiceLocator.GetService<GizmoService>().DrawText(text2, (bool) group.Visible ? GroupDebugService.trueColor : GroupDebugService.falseColor);
        }
        else if ((bool) group.Visible)
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
