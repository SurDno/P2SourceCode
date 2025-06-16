namespace SRF
{
  public static class SRFGameObjectExtensions
  {
    public static T GetIComponent<T>(this GameObject t) where T : class
    {
      return t.GetComponent(typeof (T)) as T;
    }

    public static T GetComponentOrAdd<T>(this GameObject obj) where T : Component
    {
      T componentOrAdd = obj.GetComponent<T>();
      if ((Object) componentOrAdd == (Object) null)
        componentOrAdd = obj.AddComponent<T>();
      return componentOrAdd;
    }

    public static void RemoveComponentIfExists<T>(this GameObject obj) where T : Component
    {
      T component = obj.GetComponent<T>();
      if (!((Object) component != (Object) null))
        return;
      Object.Destroy((Object) component);
    }

    public static void RemoveComponentsIfExists<T>(this GameObject obj) where T : Component
    {
      foreach (T component in obj.GetComponents<T>())
        Object.Destroy((Object) component);
    }

    public static bool EnableComponentIfExists<T>(this GameObject obj, bool enable = true) where T : MonoBehaviour
    {
      T component = obj.GetComponent<T>();
      if ((Object) component == (Object) null)
        return false;
      component.enabled = enable;
      return true;
    }

    public static void SetLayerRecursive(this GameObject o, int layer)
    {
      SetLayerInternal(o.transform, layer);
    }

    private static void SetLayerInternal(Transform t, int layer)
    {
      t.gameObject.layer = layer;
      foreach (Transform t1 in t)
        SetLayerInternal(t1, layer);
    }
  }
}
