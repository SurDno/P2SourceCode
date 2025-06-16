// Decompiled with JetBrains decompiler
// Type: ScriptableObjectInstance`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using UnityEngine;

#nullable disable
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
