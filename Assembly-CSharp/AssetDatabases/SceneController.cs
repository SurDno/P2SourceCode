using System;
using System.Collections.Generic;
using System.Reflection;
using Engine.Source.Settings.External;

namespace AssetDatabases
{
  public static class SceneController
  {
    private static List<State> states = new List<State>();
    private static List<Scene> scenes = new List<Scene>();

    public static bool CanLoad
    {
      get
      {
        return !Disabled && (states.Count == 0 || ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MultipleSceneLoader);
      }
    }

    public static bool Disabled { get; set; }

    static SceneController()
    {
      SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(MethodBase.GetCurrentMethod().Name).Append(" , scene : ").Append(scene.name));
      scenes.Add(scene);
      UpdateScenes();
    }

    private static void UpdateScenes()
    {
      for (int index1 = 0; index1 < scenes.Count; ++index1)
      {
        Scene scene = scenes[index1];
        string path = scene.path;
        for (int index2 = 0; index2 < states.Count; ++index2)
        {
          State state = states[index2];
          if (state.Path == path)
          {
            states.RemoveAt(index2);
            state.Action(scene);
            scenes.RemoveAt(index1);
            return;
          }
        }
      }
    }

    public static void AddHandler(string path, Action<Scene> action)
    {
      State state = new State {
        Path = path,
        Action = action
      };
      states.Add(state);
      UpdateScenes();
    }

    public class State
    {
      public string Path;
      public Action<Scene> Action;
    }
  }
}
