using System.Collections.Generic;
using Engine.Source.Services.Inputs;
using UnityEngine;

namespace Engine.Source.Components.Interactable
{
  public static class InteractUtility
  {
    private static List<GameActionType> interactAction;
    public static readonly ActionInfo[] DebugActions = [
      new() {
        Action = GameActionType.DebugAction1,
        Key = KeyCode.F1
      },
      new() {
        Action = GameActionType.DebugAction2,
        Key = KeyCode.F2
      },
      new() {
        Action = GameActionType.DebugAction3,
        Key = KeyCode.F3
      },
      new() {
        Action = GameActionType.DebugAction4,
        Key = KeyCode.F4
      },
      new() {
        Action = GameActionType.DebugAction5,
        Key = KeyCode.F5
      },
      new() {
        Action = GameActionType.DebugAction6,
        Key = KeyCode.F6
      },
      new() {
        Action = GameActionType.DebugAction7,
        Key = KeyCode.F7
      },
      new() {
        Action = GameActionType.DebugAction8,
        Key = KeyCode.F8
      },
      new() {
        Action = GameActionType.DebugAction9,
        Key = KeyCode.F9
      }
    ];

    public static IEnumerable<GameActionType> InteractActions
    {
      get
      {
        if (interactAction == null)
        {
          interactAction = [];
          foreach (ActionGroup group in JoystickLayoutSwitcher.Instance.Groups)
          {
            if (group.Interact)
            {
              foreach (GameActionType action in group.Actions)
              {
                interactAction.Remove(action);
                interactAction.Add(action);
              }
            }
          }
          foreach (ActionInfo debugAction in DebugActions)
          {
            interactAction.Remove(debugAction.Action);
            interactAction.Add(debugAction.Action);
          }
        }
        return interactAction;
      }
    }
  }
}
