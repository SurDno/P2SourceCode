using AssetDatabases;
using UnityEngine;

public class ScriptableObjectInstance<T> : ScriptableObject where T : ScriptableObject
{
  private static T instance;

  public static T Instance
  {
    get
    {
      if ((Object) ScriptableObjectInstance<T>.instance == (Object) null)
      {
        string path = "Assets/Data/Settings/Resources/" + typeof (T).Name + ".asset";
        ScriptableObjectInstanceAttribute[] customAttributes = (ScriptableObjectInstanceAttribute[]) typeof (T).GetCustomAttributes(typeof (ScriptableObjectInstanceAttribute), false);
        if (customAttributes.Length != 0 && customAttributes[0].Path != null)
          path = customAttributes[0].Path;
        ScriptableObjectInstance<T>.instance = AssetDatabaseService.Instance.Load<T>(path);
      }
      return ScriptableObjectInstance<T>.instance;
    }
  }
}
