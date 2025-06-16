using System;
using System.Collections.Generic;
using System.Reflection;
using Cofe.Utility;
using Engine.Common.Comparers;
using Scripts.AssetDatabaseService;

namespace AssetDatabases
{
  public class AssetDatabaseBuild : IAssetDatabase
  {
    private Dictionary<Guid, string> paths = new Dictionary<Guid, string>(GuidComparer.Instance);
    private Dictionary<string, Guid> ids = new Dictionary<string, Guid>();

    public void RegisterAssets()
    {
      AssetDatabaseMapData assetDatabaseMapData = AssetDatabaseUtility.LoadFromFile<AssetDatabaseMapData>(PlatformUtility.GetPath("Data/Database/AssetDatabaseData.xml"));
      int count = assetDatabaseMapData.Items.Count;
      paths = new Dictionary<Guid, string>(count, GuidComparer.Instance);
      ids = new Dictionary<string, Guid>(count);
      foreach (AssetDatabaseMapItemData databaseMapItemData in assetDatabaseMapData.Items)
      {
        Guid key = new Guid(databaseMapItemData.Id);
        paths.Add(key, databaseMapItemData.Name);
        ids.Add(databaseMapItemData.Name, key);
      }
    }

    public int GetAllAssetPathsCount() => paths.Count;

    public IEnumerable<string> GetAllAssetPaths() => paths.Values;

    public Guid GetId(string path)
    {
      Guid id;
      ids.TryGetValue(path, out id);
      return id;
    }

    public string GetPath(Guid id)
    {
      string str;
      return paths.TryGetValue(id, out str) ? str : "";
    }

    public T Load<T>(string path) where T : UnityEngine.Object
    {
      if (!path.IsNullOrEmpty())
      {
        string resourcePath = AssetDatabaseUtility.ConvertToResourcePath(path);
        if (!resourcePath.IsNullOrEmpty())
          return Resources.Load<T>(resourcePath);
      }
      return default (T);
    }

    public UnityEngine.Object[] LoadAll(string path)
    {
      if (!path.IsNullOrEmpty())
      {
        string resourcePath = AssetDatabaseUtility.ConvertToResourcePath(path);
        if (!resourcePath.IsNullOrEmpty())
          return Resources.LoadAll(resourcePath);
      }
      return (UnityEngine.Object[]) null;
    }

    public IAsyncLoad LoadAsync<T>(string path) where T : UnityEngine.Object
    {
      if (!path.IsNullOrEmpty())
      {
        string resourcePath = AssetDatabaseUtility.ConvertToResourcePath(path);
        if (!resourcePath.IsNullOrEmpty())
        {
          ResourceRequest operation = Resources.LoadAsync<T>(resourcePath);
          if (operation != null)
            return new AsyncLoadFromResources(operation);
        }
      }
      Debug.LogError((object) (MethodBase.GetCurrentMethod().Name + " wrong async operator : " + path));
      return null;
    }

    public IAsyncLoad LoadSceneAsync(string path)
    {
      SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
      return new AsyncLoadScene(path);
    }

    public void Unload(UnityEngine.Object obj)
    {
      if (!(obj != (UnityEngine.Object) null))
        return;
      Resources.UnloadAsset(obj);
      obj = (UnityEngine.Object) null;
    }
  }
}
