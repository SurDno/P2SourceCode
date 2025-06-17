using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class GameObjectUtility2
{
  private static List<Component> tmp = [];

  public static T FindObjectOfType<T>() where T : Object
  {
    for (int index = 0; index < SceneManager.sceneCount; ++index)
    {
      Scene sceneAt = SceneManager.GetSceneAt(index);
      if (sceneAt.isLoaded)
      {
        foreach (GameObject rootGameObject in sceneAt.GetRootGameObjects())
        {
          T componentInChildren = rootGameObject.GetComponentInChildren<T>(true);
          if (componentInChildren != null)
            return componentInChildren;
        }
      }
    }
    return default (T);
  }

  public static T GetComponentNonAlloc<T>(this Component component) where T : Component
  {
    return component.gameObject.GetComponentNonAlloc<T>();
  }

  public static T GetComponentNonAlloc<T>(this GameObject go) where T : Component
  {
    tmp.Clear();
    go.GetComponents(typeof (T), tmp);
    if (tmp.Count == 0)
      return default (T);
    T componentNonAlloc = (T) tmp[0];
    tmp.Clear();
    return componentNonAlloc;
  }

  public static GameObject GetByPath(string path)
  {
    string[] paths = path.Split(['/'], StringSplitOptions.RemoveEmptyEntries);
    if (paths.Length < 2)
      return null;
    Scene sceneByName = SceneManager.GetSceneByName(paths[0]);
    return !sceneByName.IsValid() ? null : GetChildGameObject(sceneByName.GetRootGameObjects(), paths, 1);
  }

  private static GameObject GetChildGameObject(
    IEnumerable<GameObject> gos,
    string[] paths,
    int pathIndex)
  {
    string path = paths[pathIndex];
    foreach (GameObject go in gos)
    {
      if (go.name == path)
        return pathIndex == paths.Length - 1 ? go : GetChildGameObject(go.GetChilds(), paths, pathIndex + 1);
    }
    return null;
  }

  public static IEnumerable<GameObject> GetChilds(this GameObject go)
  {
    if (go != null)
    {
      for (int index = 0; index < go.transform.childCount; ++index)
      {
        Transform child = go.transform.GetChild(index);
        yield return child.gameObject;
        child = null;
      }
    }
  }

  public static IEnumerable<GameObject> GetAllGameObjects(GameObject go)
  {
    yield return go;
    foreach (GameObject child2 in GetAllChildren(go))
      yield return child2;
  }

  private static IEnumerable<GameObject> GetAllChildren(GameObject go)
  {
    for (int index = 0; index < go.transform.childCount; ++index)
    {
      GameObject child = go.transform.GetChild(index).gameObject;
      yield return child;
      foreach (GameObject child2 in GetAllChildren(child))
        yield return child2;
      child = null;
    }
  }
}
