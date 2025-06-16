using AssetDatabases;
using Cofe.Utility;
using System;

namespace Engine.Source.Connections
{
  public static class UnityAssetUtility
  {
    public static T GetValue<T>(Guid id) where T : UnityEngine.Object
    {
      string path = AssetDatabaseService.Instance.GetPath(id);
      return path.IsNullOrEmpty() ? default (T) : AssetDatabaseService.Instance.Load<T>(path);
    }

    public static T GetValue<T>(Guid id, string name) where T : UnityEngine.Object
    {
      string path = AssetDatabaseService.Instance.GetPath(id);
      if (path.IsNullOrEmpty())
        return default (T);
      if (name.IsNullOrEmpty())
        return AssetDatabaseService.Instance.Load<T>(path);
      foreach (UnityEngine.Object @object in AssetDatabaseService.Instance.LoadAll(path))
      {
        if (@object.name == name)
        {
          T obj = @object as T;
          if ((UnityEngine.Object) obj != (UnityEngine.Object) null)
            return obj;
        }
      }
      return default (T);
    }
  }
}
