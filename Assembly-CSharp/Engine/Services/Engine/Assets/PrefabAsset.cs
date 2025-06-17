using AssetDatabases;
using Inspectors;
using UnityEngine;

namespace Engine.Services.Engine.Assets
{
  public class PrefabAsset(string path) : IAsset 
  {
    private IAsyncLoad async = AssetDatabaseService.Instance.LoadAsync<GameObject>(path);
    private bool isDone;
    [Inspected]
    private string path = path;

    [Inspected]
    public bool IsError { get; private set; }

    [Inspected]
    public bool IsLoaded { get; private set; }

    [Inspected]
    public bool IsDisposed { get; set; }

    [Inspected]
    public bool IsReadyToDispose { get; set; }

    public GameObject Prefab { get; private set; }

    public void Update()
    {
      if (IsError)
        return;
      if (!isDone)
      {
        if (async == null)
        {
          IsError = true;
          return;
        }
        isDone = async.IsDone;
        if (!isDone)
          return;
      }
      if (!IsLoaded)
      {
        IsLoaded = true;
        if (!IsDisposed)
          Prefab = (GameObject) async.Asset;
      }
      if (!IsDisposed)
        return;
      Prefab = null;
      IsReadyToDispose = true;
    }

    public void Dispose(string reason) => IsDisposed = true;

    public bool IsValid => Prefab != null;

    public string Path => Prefab.GetFullName();
  }
}
