using UnityEngine;

public static class GameObjectExtensions
{
  public static GameObject GetSceneInstance(this GameObject @object)
  {
    return (Object) @object == (Object) null ? (GameObject) null : @object;
  }
}
