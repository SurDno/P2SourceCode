// Decompiled with JetBrains decompiler
// Type: UnityFactory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public static class UnityFactory
{
  private static Dictionary<string, GameObject> groups = new Dictionary<string, GameObject>();

  private static GameObject CreateGroup(string group)
  {
    GameObject group1 = new GameObject(group);
    group1.AddComponent<EngineGameObjectContainer>();
    return group1;
  }

  public static GameObject GetOrCreateGroup(string group)
  {
    GameObject group1;
    if (!UnityFactory.groups.TryGetValue(group, out group1))
    {
      group1 = UnityFactory.CreateGroup(group);
      UnityFactory.groups.Add(group, group1);
    }
    return group1;
  }

  public static GameObject Create(string group, string name)
  {
    if (group.IsNullOrEmpty())
      return new GameObject(name);
    GameObject group1 = UnityFactory.GetOrCreateGroup(group);
    GameObject gameObject = new GameObject(name);
    gameObject.transform.SetParent(group1.transform);
    return gameObject;
  }

  public static T Instantiate<T>(GameObject prefab, string group) where T : MonoBehaviour
  {
    return UnityFactory.InstantiateObject<GameObject>(prefab, group).GetComponent<T>();
  }

  public static GameObject Instantiate(GameObject prefab, string group)
  {
    return UnityFactory.InstantiateObject<GameObject>(prefab, group);
  }

  public static T Instantiate<T>(T prefab, string group) where T : MonoBehaviour
  {
    return UnityFactory.InstantiateObject<T>(prefab, group);
  }

  private static T InstantiateObject<T>(T prefab, string group) where T : Object
  {
    if (!Application.isEditor || group.IsNullOrEmpty())
      return Object.Instantiate<T>(prefab);
    GameObject group1 = UnityFactory.GetOrCreateGroup(group);
    return Object.Instantiate<T>(prefab, group1.transform);
  }

  public static void Destroy(GameObject go) => Object.Destroy((Object) go);

  public static void Destroy(MonoBehaviour mono) => Object.Destroy((Object) mono.gameObject);
}
