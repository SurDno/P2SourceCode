using Engine.Source.Services.Inputs;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Components.Interactable
{
  public static class InteractUtility
  {
    private static List<GameActionType> interactAction;
    public static readonly ActionInfo[] DebugActions = new ActionInfo[9]
    {
      new ActionInfo()
      {
        Action = GameActionType.DebugAction1,
        Key = KeyCode.F1
      },
      new ActionInfo()
      {
        Action = GameActionType.DebugAction2,
        Key = KeyCode.F2
      },
      new ActionInfo()
      {
        Action = GameActionType.DebugAction3,
        Key = KeyCode.F3
      },
      new ActionInfo()
      {
        Action = GameActionType.DebugAction4,
        Key = KeyCode.F4
      },
      new ActionInfo()
      {
        Action = GameActionType.DebugAction5,
        Key = KeyCode.F5
      },
      new ActionInfo()
      {
        Action = GameActionType.DebugAction6,
        Key = KeyCode.F6
      },
      new ActionInfo()
      {
        Action = GameActionType.DebugAction7,
        Key = KeyCode.F7
      },
      new ActionInfo()
      {
        Action = GameActionType.DebugAction8,
        Key = KeyCode.F8
      },
      new ActionInfo()
      {
        Action = GameActionType.DebugAction9,
        Key = KeyCode.F9
      }
    };

    public static IEnumerable<GameActionType> InteractActions
    {
      get
      {
        if (InteractUtility.interactAction == null)
        {
          InteractUtility.interactAction = new List<GameActionType>();
          foreach (ActionGroup group in JoystickLayoutSwitcher.Instance.Groups)
          {
            if (group.Interact)
            {
              foreach (GameActionType action in group.Actions)
              {
                InteractUtility.interactAction.Remove(action);
                InteractUtility.interactAction.Add(action);
              }
            }
          }
          foreach (ActionInfo debugAction in InteractUtility.DebugActions)
          {
            InteractUtility.interactAction.Remove(debugAction.Action);
            InteractUtility.interactAction.Add(debugAction.Action);
          }
        }
        return (IEnumerable<GameActionType>) InteractUtility.interactAction;
      }
    }
  }
}
