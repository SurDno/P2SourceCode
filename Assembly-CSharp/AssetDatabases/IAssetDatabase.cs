using System;
using System.Collections.Generic;

namespace AssetDatabases
{
  public interface IAssetDatabase
  {
    void RegisterAssets();

    IEnumerable<string> GetAllAssetPaths();

    int GetAllAssetPathsCount();

    string GetPath(Guid id);

    Guid GetId(string path);

    T Load<T>(string path) where T : UnityEngine.Object;

    UnityEngine.Object[] LoadAll(string path);

    IAsyncLoad LoadAsync<T>(string path) where T : UnityEngine.Object;

    IAsyncLoad LoadSceneAsync(string path);

    void Unload(UnityEngine.Object obj);
  }
}
