using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using System;
using UnityEngine;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class ParametersGroupDebug
  {
    private static string name = "[Parameters]";
    private static KeyCode key = KeyCode.Q;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = Color.cyan;
    private static Color trueColor = Color.white;

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(ParametersGroupDebug.name, ParametersGroupDebug.key, ParametersGroupDebug.modifficators, new Action(ParametersGroupDebug.Update)));
    }

    private static void Update()
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      ParametersComponent component = player.GetComponent<ParametersComponent>();
      if (component == null)
        return;
      string text1 = "\n" + ParametersGroupDebug.name + " (" + InputUtility.GetHotKeyText(ParametersGroupDebug.key, ParametersGroupDebug.modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, ParametersGroupDebug.headerColor);
      string text2 = "";
      foreach (IParameter parameter in component.Parameters)
        text2 = text2 + "  " + (object) parameter.Name + " : " + parameter.ValueData + "\n";
      ServiceLocator.GetService<GizmoService>().DrawText(text2, ParametersGroupDebug.trueColor);
    }
  }
}
