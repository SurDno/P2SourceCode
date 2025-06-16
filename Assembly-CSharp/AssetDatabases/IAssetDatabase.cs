using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace AssetDatabases
{
  public interface IAssetDatabase
  {
    void RegisterAssets();

    IEnumerable<string> GetAllAssetPaths();

    int GetAllAssetPathsCount();

    string GetPath(Guid id);

    Guid GetId(string path);

    T Load<T>(string path) where T : Object;

    Object[] LoadAll(string path);

    IAsyncLoad LoadAsync<T>(string path) where T : Object;

    IAsyncLoad LoadSceneAsync(string path);

    void Unload(Object obj);
  }
}
