// Decompiled with JetBrains decompiler
// Type: AssetDatabases.SceneController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Settings.External;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#nullable disable
namespace AssetDatabases
{
  public static class SceneController
  {
    private static List<SceneController.State> states = new List<SceneController.State>();
    private static List<Scene> scenes = new List<Scene>();

    public static bool CanLoad
    {
      get
      {
        return !SceneController.Disabled && (SceneController.states.Count == 0 || ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MultipleSceneLoader);
      }
    }

    public static bool Disabled { get; set; }

    static SceneController()
    {
      SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(SceneController.OnSceneLoaded);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(MethodBase.GetCurrentMethod().Name).Append(" , scene : ").Append(scene.name));
      SceneController.scenes.Add(scene);
      SceneController.UpdateScenes();
    }

    private static void UpdateScenes()
    {
      for (int index1 = 0; index1 < SceneController.scenes.Count; ++index1)
      {
        Scene scene = SceneController.scenes[index1];
        string path = scene.path;
        for (int index2 = 0; index2 < SceneController.states.Count; ++index2)
        {
          SceneController.State state = SceneController.states[index2];
          if (state.Path == path)
          {
            SceneController.states.RemoveAt(index2);
            state.Action(scene);
            SceneController.scenes.RemoveAt(index1);
            return;
          }
        }
      }
    }

    public static void AddHandler(string path, Action<Scene> action)
    {
      SceneController.State state = new SceneController.State()
      {
        Path = path,
        Action = action
      };
      SceneController.states.Add(state);
      SceneController.UpdateScenes();
    }

    public class State
    {
      public string Path;
      public Action<Scene> Action;
    }
  }
}
