using System;
using AssetDatabases;
using Cofe.Utility;
using Object = UnityEngine.Object;

namespace Engine.Source.Connections
{
  public static class UnityAssetUtility
  {
    public static T GetValue<T>(Guid id) where T : Object
    {
      string path = AssetDatabaseService.Instance.GetPath(id);
      return path.IsNullOrEmpty() ? default (T) : AssetDatabaseService.Instance.Load<T>(path);
    }

    public static T GetValue<T>(Guid id, string name) where T : Object
    {
      string path = AssetDatabaseService.Instance.GetPath(id);
      if (path.IsNullOrEmpty())
        return default (T);
      if (name.IsNullOrEmpty())
        return AssetDatabaseService.Instance.Load<T>(path);
      foreach (Object @object in AssetDatabaseService.Instance.LoadAll(path))
      {
        if (@object.name == name)
        {
          T obj = @object as T;
          if (obj != null)
            return obj;
        }
      }
      return default (T);
    }
  }
}
