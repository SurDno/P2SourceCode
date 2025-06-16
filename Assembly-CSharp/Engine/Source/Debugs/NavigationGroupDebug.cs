using System;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class NavigationGroupDebug
  {
    private static string name = "[Navigation]";
    private static KeyCode key = KeyCode.V;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = Color.magenta;
    private static Color bodyColor = Color.white;

    [Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(name, key, modifficators, Update));
    }

    private static void Update()
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null || player.IsDisposed)
        return;
      string text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
      INavigationComponent component1 = player.GetComponent<INavigationComponent>();
      string str = "  Region : " + (component1 == null || component1.Region == null ? "" : component1.Region.Owner.Name) + "\n  Area : " + (component1 != null ? component1.Area.ToString() : "");
      LocationItemComponent component2 = player.GetComponent<LocationItemComponent>();
      string text2 = str + "\n  Location : " + (component2 == null || component2.Location == null ? "" : component2.Location.Owner.Name) + "\n  Logic Location : " + (component2 == null || component2.LogicLocation == null ? "" : component2.LogicLocation.Owner.Name) + "\n  Logic Location Type : " + (component2 == null || component2.LogicLocation == null ? "" : ((LocationComponent) component2.LogicLocation).LocationType.ToString());
      ServiceLocator.GetService<GizmoService>().DrawText(text2, bodyColor);
    }
  }
}
