using System;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class ScenesGroupDebug
  {
    private static string name = "[Scenes]";
    private static KeyCode key = KeyCode.U;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = ColorPreset.Orange;
    private static Color bodyColor = Color.white;
    private static Color notValidColor = Color.red;
    private static Color loadingColor = Color.yellow;
    private static Color loadedColor = Color.green;

    [Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(name, key, modifficators, Update));
    }

    private static void Update()
    {
      string text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
      ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
      int sceneCount = SceneManager.sceneCount;
      string text2 = "  Scene count : " + sceneCount;
      ServiceLocator.GetService<GizmoService>().DrawText(text2, bodyColor);
      for (int index = 0; index < sceneCount; ++index)
      {
        Scene sceneAt = SceneManager.GetSceneAt(index);
        if (sceneAt.IsValid())
        {
          if (sceneAt.isLoaded)
          {
            string text3 = "  state : complete , scene : " + sceneAt.name;
            ServiceLocator.GetService<GizmoService>().DrawText(text3, loadedColor);
          }
          else
          {
            string text4 = "  state : loading , scene : " + sceneAt.name;
            ServiceLocator.GetService<GizmoService>().DrawText(text4, loadingColor);
          }
        }
        else
        {
          string text5 = "  state : not valid , scene : " + sceneAt.name;
          ServiceLocator.GetService<GizmoService>().DrawText(text5, notValidColor);
        }
      }
    }
  }
}
