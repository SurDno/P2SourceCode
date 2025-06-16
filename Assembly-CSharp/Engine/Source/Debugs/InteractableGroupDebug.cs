using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Interactable;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using System;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class InteractableGroupDebug
  {
    private static string name = "[Interactable]";
    private static KeyCode key = KeyCode.O;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = Color.cyan;
    private static Color trueColor = Color.white;
    private static Color falseColor = ColorPreset.LightGray;

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(InteractableGroupDebug.name, InteractableGroupDebug.key, InteractableGroupDebug.modifficators, new Action(InteractableGroupDebug.Update)));
    }

    private static void Update()
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      PlayerInteractableComponent component = player.GetComponent<PlayerInteractableComponent>();
      if (component == null)
        return;
      string text1 = "\n" + InteractableGroupDebug.name + " (" + InputUtility.GetHotKeyText(InteractableGroupDebug.key, InteractableGroupDebug.modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, InteractableGroupDebug.headerColor);
      if (component.Interactable == null)
      {
        string text2 = "  Interactable not found";
        ServiceLocator.GetService<GizmoService>().DrawText(text2, InteractableGroupDebug.falseColor);
      }
      else
      {
        string text3 = "  Interactable found, count : " + (object) component.Interactable.Items.Count<InteractItem>();
        ServiceLocator.GetService<GizmoService>().DrawText(text3, InteractableGroupDebug.falseColor);
        foreach (InteractItem interactItem in component.Interactable.Items)
        {
          InteractItem item = interactItem;
          InteractItemInfo interactItemInfo = component.ValidateItems.FirstOrDefault<InteractItemInfo>((Func<InteractItemInfo, bool>) (o => o.Item.Type == item.Type));
          string text4 = "  " + (object) item.Type + " , validate : " + (interactItemInfo != null).ToString();
          if (interactItemInfo != null && interactItemInfo.Invalid)
            text4 += " , debug";
          ServiceLocator.GetService<GizmoService>().DrawText(text4, interactItemInfo != null ? InteractableGroupDebug.trueColor : InteractableGroupDebug.falseColor);
        }
      }
    }
  }
}
