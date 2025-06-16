using AssetDatabases;
using Inspectors;
using UnityEngine;

namespace Engine.Services.Engine.Assets
{
  public class PrefabAsset : IAsset
  {
    private IAsyncLoad async;
    private bool isDone;
    [Inspected]
    private string path;

    [Inspected]
    public bool IsError { get; private set; }

    [Inspected]
    public bool IsLoaded { get; private set; }

    [Inspected]
    public bool IsDisposed { get; set; }

    [Inspected]
    public bool IsReadyToDispose { get; set; }

    public GameObject Prefab { get; private set; }

    public PrefabAsset(string path)
    {
      this.path = path;
      this.async = AssetDatabaseService.Instance.LoadAsync<GameObject>(path);
    }

    public void Update()
    {
      if (this.IsError)
        return;
      if (!this.isDone)
      {
        if (this.async == null)
        {
          this.IsError = true;
          return;
        }
        this.isDone = this.async.IsDone;
        if (!this.isDone)
          return;
      }
      if (!this.IsLoaded)
      {
        this.IsLoaded = true;
        if (!this.IsDisposed)
          this.Prefab = (GameObject) this.async.Asset;
      }
      if (!this.IsDisposed)
        return;
      this.Prefab = (GameObject) null;
      this.IsReadyToDispose = true;
    }

    public void Dispose(string reason) => this.IsDisposed = true;

    public bool IsValid => (Object) this.Prefab != (Object) null;

    public string Path => this.Prefab.GetFullName();
  }
}
