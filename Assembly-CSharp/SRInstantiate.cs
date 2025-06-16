using UnityEngine;

public static class SRInstantiate
{
  public static T Instantiate<T>(T prefab) where T : Component => Object.Instantiate<T>(prefab);

  public static GameObject Instantiate(GameObject prefab) => Object.Instantiate<GameObject>(prefab);

  public static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
  {
    return Object.Instantiate<T>(prefab, position, rotation);
  }
}
