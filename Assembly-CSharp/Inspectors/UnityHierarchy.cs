using System.Collections.Generic;

namespace Inspectors
{
  public class UnityHierarchy
  {
    private static UnityHierarchy instance = new UnityHierarchy();

    public static UnityHierarchy Instance => instance;

    [Inspected]
    public IEnumerable<SceneInfo> Scenes
    {
      get
      {
        int count = SceneManager.sceneCount;
        for (int index = 0; index < count; ++index)
          yield return new SceneInfo(SceneManager.GetSceneAt(index));
      }
    }

    public class SceneInfo
    {
      private Scene scene;

      public SceneInfo(Scene scene) => this.scene = scene;

      [Inspected(Header = true)]
      private string Name => scene.IsValid() ? scene.name : "null";

      [Inspected]
      private IEnumerable<GameObjectInfo> Childs
      {
        get
        {
          if (scene.IsValid())
          {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; ++index)
              yield return new GameObjectInfo(roots[index]);
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
          return (Object) transform != (Object) null ? transform.localPosition : Vector3.zero;
        }
      }

      [Inspected]
      private Quaternion Rotation
      {
        get
        {
          return (Object) transform != (Object) null ? transform.localRotation : Quaternion.identity;
        }
      }

      [Inspected]
      private Vector3 Scale
      {
        get => (Object) transform != (Object) null ? transform.localScale : Vector3.one;
      }
    }

    public class GameObjectInfo
    {
      private GameObject gameObject;

      public GameObjectInfo(GameObject gameObject) => this.gameObject = gameObject;

      [Inspected(Header = true)]
      private string Name => (Object) gameObject != (Object) null ? gameObject.name : "";

      [Inspected(Mutable = true)]
      public bool Enable
      {
        get => (Object) gameObject != (Object) null && gameObject.activeSelf;
        set
        {
          if (!((Object) gameObject != (Object) null))
            return;
          gameObject.SetActive(value);
        }
      }

      [Inspected]
      public string Tag => (Object) gameObject != (Object) null ? gameObject.tag : "";

      [Inspected]
      public string Layer
      {
        get
        {
          return (Object) gameObject != (Object) null ? LayerMask.LayerToName(gameObject.layer) : "";
        }
      }

      [Inspected]
      private IEnumerable<object> Components
      {
        get
        {
          if ((Object) gameObject != (Object) null)
          {
            Component[] components = gameObject.GetComponents<Component>();
            Component[] componentArray = components;
            for (int index = 0; index < componentArray.Length; ++index)
            {
              Component component = componentArray[index];
              Transform transform = component as Transform;
              if ((Object) transform != (Object) null)
                yield return new TransformInfo(transform);
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
      private IEnumerable<GameObjectInfo> Childs
      {
        get
        {
          if ((Object) gameObject != (Object) null)
          {
            int count = gameObject.transform.childCount;
            for (int index = 0; index < count; ++index)
              yield return new GameObjectInfo(gameObject.transform.GetChild(index).gameObject);
          }
        }
      }
    }
  }
}
