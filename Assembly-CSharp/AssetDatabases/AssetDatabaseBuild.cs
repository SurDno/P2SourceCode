// Decompiled with JetBrains decompiler
// Type: AssetDatabases.AssetDatabaseBuild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common.Comparers;
using Scripts.AssetDatabaseService;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
namespace AssetDatabases
{
  public class AssetDatabaseBuild : IAssetDatabase
  {
    private Dictionary<Guid, string> paths = new Dictionary<Guid, string>((IEqualityComparer<Guid>) GuidComparer.Instance);
    private Dictionary<string, Guid> ids = new Dictionary<string, Guid>();

    public void RegisterAssets()
    {
      AssetDatabaseMapData assetDatabaseMapData = AssetDatabaseUtility.LoadFromFile<AssetDatabaseMapData>(PlatformUtility.GetPath("Data/Database/AssetDatabaseData.xml"));
      int count = assetDatabaseMapData.Items.Count;
      this.paths = new Dictionary<Guid, string>(count, (IEqualityComparer<Guid>) GuidComparer.Instance);
      this.ids = new Dictionary<string, Guid>(count);
      foreach (AssetDatabaseMapItemData databaseMapItemData in assetDatabaseMapData.Items)
      {
        Guid key = new Guid(databaseMapItemData.Id);
        this.paths.Add(key, databaseMapItemData.Name);
        this.ids.Add(databaseMapItemData.Name, key);
      }
    }

    public int GetAllAssetPathsCount() => this.paths.Count;

    public IEnumerable<string> GetAllAssetPaths() => (IEnumerable<string>) this.paths.Values;

    public Guid GetId(string path)
    {
      Guid id;
      this.ids.TryGetValue(path, out id);
      return id;
    }

    public string GetPath(Guid id)
    {
      string str;
      return this.paths.TryGetValue(id, out str) ? str : "";
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
            return (IAsyncLoad) new AsyncLoadFromResources(operation);
        }
      }
      Debug.LogError((object) (MethodBase.GetCurrentMethod().Name + " wrong async operator : " + path));
      return (IAsyncLoad) null;
    }

    public IAsyncLoad LoadSceneAsync(string path)
    {
      SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
      return (IAsyncLoad) new AsyncLoadScene(path);
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
