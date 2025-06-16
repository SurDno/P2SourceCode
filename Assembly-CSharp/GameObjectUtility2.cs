using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameObjectUtility2
{
  private static List<Component> tmp = new List<Component>();

  public static T FindObjectOfType<T>() where T : UnityEngine.Object
  {
    for (int index = 0; index < SceneManager.sceneCount; ++index)
    {
      Scene sceneAt = SceneManager.GetSceneAt(index);
      if (sceneAt.isLoaded)
      {
        foreach (GameObject rootGameObject in sceneAt.GetRootGameObjects())
        {
          T componentInChildren = rootGameObject.GetComponentInChildren<T>(true);
          if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
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
    GameObjectUtility2.tmp.Clear();
    go.GetComponents(typeof (T), GameObjectUtility2.tmp);
    if (GameObjectUtility2.tmp.Count == 0)
      return default (T);
    T componentNonAlloc = (T) GameObjectUtility2.tmp[0];
    GameObjectUtility2.tmp.Clear();
    return componentNonAlloc;
  }

  public static GameObject GetByPath(string path)
  {
    string[] paths = path.Split(new char[1]{ '/' }, StringSplitOptions.RemoveEmptyEntries);
    if (paths.Length < 2)
      return (GameObject) null;
    Scene sceneByName = SceneManager.GetSceneByName(paths[0]);
    return !sceneByName.IsValid() ? (GameObject) null : GameObjectUtility2.GetChildGameObject((IEnumerable<GameObject>) sceneByName.GetRootGameObjects(), paths, 1);
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
        return pathIndex == paths.Length - 1 ? go : GameObjectUtility2.GetChildGameObject(go.GetChilds(), paths, pathIndex + 1);
    }
    return (GameObject) null;
  }

  public static IEnumerable<GameObject> GetChilds(this GameObject go)
  {
    if ((UnityEngine.Object) go != (UnityEngine.Object) null)
    {
      for (int index = 0; index < go.transform.childCount; ++index)
      {
        Transform child = go.transform.GetChild(index);
        yield return child.gameObject;
        child = (Transform) null;
      }
    }
  }

  public static IEnumerable<GameObject> GetAllGameObjects(GameObject go)
  {
    yield return go;
    foreach (GameObject child2 in GameObjectUtility2.GetAllChildren(go))
      yield return child2;
  }

  private static IEnumerable<GameObject> GetAllChildren(GameObject go)
  {
    for (int index = 0; index < go.transform.childCount; ++index)
    {
      GameObject child = go.transform.GetChild(index).gameObject;
      yield return child;
      foreach (GameObject child2 in GameObjectUtility2.GetAllChildren(child))
        yield return child2;
      child = (GameObject) null;
    }
  }
}
