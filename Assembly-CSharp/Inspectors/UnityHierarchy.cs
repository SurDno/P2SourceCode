using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inspectors
{
  public class UnityHierarchy
  {
    private static UnityHierarchy instance = new UnityHierarchy();

    public static UnityHierarchy Instance => UnityHierarchy.instance;

    [Inspected]
    public IEnumerable<UnityHierarchy.SceneInfo> Scenes
    {
      get
      {
        int count = SceneManager.sceneCount;
        for (int index = 0; index < count; ++index)
          yield return new UnityHierarchy.SceneInfo(SceneManager.GetSceneAt(index));
      }
    }

    public class SceneInfo
    {
      private Scene scene;

      public SceneInfo(Scene scene) => this.scene = scene;

      [Inspected(Header = true)]
      private string Name => this.scene.IsValid() ? this.scene.name : "null";

      [Inspected]
      private IEnumerable<UnityHierarchy.GameObjectInfo> Childs
      {
        get
        {
          if (this.scene.IsValid())
          {
            GameObject[] roots = this.scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; ++index)
              yield return new UnityHierarchy.GameObjectInfo(roots[index]);
            roots = (GameObject[]) null;
          }
        }
      }
    }

    public class TransformInfo
    {
      private Transform transform;

      public TransformInfo(Transform transform) => this.transform = transform;

      [Inspected]
      private Vector3 Position
      {
        get
        {
          return (Object) this.transform != (Object) null ? this.transform.localPosition : Vector3.zero;
        }
      }

      [Inspected]
      private Quaternion Rotation
      {
        get
        {
          return (Object) this.transform != (Object) null ? this.transform.localRotation : Quaternion.identity;
        }
      }

      [Inspected]
      private Vector3 Scale
      {
        get => (Object) this.transform != (Object) null ? this.transform.localScale : Vector3.one;
      }
    }

    public class GameObjectInfo
    {
      private GameObject gameObject;

      public GameObjectInfo(GameObject gameObject) => this.gameObject = gameObject;

      [Inspected(Header = true)]
      private string Name => (Object) this.gameObject != (Object) null ? this.gameObject.name : "";

      [Inspected(Mutable = true)]
      public bool Enable
      {
        get => (Object) this.gameObject != (Object) null && this.gameObject.activeSelf;
        set
        {
          if (!((Object) this.gameObject != (Object) null))
            return;
          this.gameObject.SetActive(value);
        }
      }

      [Inspected]
      public string Tag => (Object) this.gameObject != (Object) null ? this.gameObject.tag : "";

      [Inspected]
      public string Layer
      {
        get
        {
          return (Object) this.gameObject != (Object) null ? LayerMask.LayerToName(this.gameObject.layer) : "";
        }
      }

      [Inspected]
      private IEnumerable<object> Components
      {
        get
        {
          if ((Object) this.gameObject != (Object) null)
          {
            Component[] components = this.gameObject.GetComponents<Component>();
            Component[] componentArray = components;
            for (int index = 0; index < componentArray.Length; ++index)
            {
              Component component = componentArray[index];
              Transform transform = component as Transform;
              if ((Object) transform != (Object) null)
                yield return (object) new UnityHierarchy.TransformInfo(transform);
              else
                yield return (object) component;
              transform = (Transform) null;
              component = (Component) null;
            }
            componentArray = (Component[]) null;
            components = (Component[]) null;
          }
        }
      }

      [Inspected]
      private IEnumerable<UnityHierarchy.GameObjectInfo> Childs
      {
        get
        {
          if ((Object) this.gameObject != (Object) null)
          {
            int count = this.gameObject.transform.childCount;
            for (int index = 0; index < count; ++index)
              yield return new UnityHierarchy.GameObjectInfo(this.gameObject.transform.GetChild(index).gameObject);
          }
        }
      }
    }
  }
}
