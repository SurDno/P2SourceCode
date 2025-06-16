using Cofe.Serializations.Converters;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Engine.Behaviours
{
  public class SceneObjectContainer : MonoBehaviour
  {
    [SerializeField]
    [HideInInspector]
    private string id;
    [SerializeField]
    [HideInInspector]
    private List<SceneObjectContainer.Element> elements = new List<SceneObjectContainer.Element>();

    public GameObject GetGameObject(Guid id)
    {
      string str = DefaultConverter.ToString(id);
      foreach (SceneObjectContainer.Element element in this.elements)
      {
        if ((UnityEngine.Object) element.Item != (UnityEngine.Object) null && element.Id == str)
          return element.Item.gameObject;
      }
      return (GameObject) null;
    }

    public Guid GetId(GameObject go)
    {
      if ((UnityEngine.Object) go != (UnityEngine.Object) null)
      {
        foreach (SceneObjectContainer.Element element in this.elements)
        {
          if ((UnityEngine.Object) element.Item != (UnityEngine.Object) null && (UnityEngine.Object) element.Item.gameObject == (UnityEngine.Object) go)
            return DefaultConverter.ParseGuid(element.Id);
        }
      }
      return Guid.Empty;
    }

    public static SceneObjectContainer GetContainer(Scene scene)
    {
      foreach (GameObject rootGameObject in scene.GetRootGameObjects())
      {
        SceneObjectContainer component = rootGameObject.GetComponent<SceneObjectContainer>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          return component;
      }
      return (SceneObjectContainer) null;
    }

    [Serializable]
    public struct Element
    {
      [Inspected(Header = true, Mode = ExecuteMode.EditAndRuntime, Mutable = false)]
      public string Id;
      [Inspected(Mode = ExecuteMode.EditAndRuntime, Mutable = false)]
      public EngineGameObject Item;
    }
  }
}
