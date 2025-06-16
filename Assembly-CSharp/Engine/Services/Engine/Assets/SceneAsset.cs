using AssetDatabases;
using Cofe.Utility;
using Engine.Common;
using Inspectors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Engine.Services.Engine.Assets
{
  public class SceneAsset : IAsset
  {
    private IAsyncLoad async;
    private bool initialized;
    private string context;
    private string reason;
    [Inspected]
    private IScene reference;

    [Inspected]
    public bool IsError { get; private set; }

    [Inspected]
    public bool IsLoaded { get; private set; }

    [Inspected]
    public bool IsDisposed { get; set; }

    [Inspected]
    public bool IsReadyToDispose { get; set; }

    [Inspected]
    public Scene Scene { get; private set; }

    public SceneAsset(IScene reference, string context)
    {
      this.reference = reference;
      this.context = context;
    }

    public void Update()
    {
      if (!initialized)
      {
        if (!SceneController.CanLoad)
          return;
        if (IsDisposed)
        {
          IsReadyToDispose = true;
          return;
        }
        string path = AssetDatabaseService.Instance.GetPath(reference.Id);
        if (path.IsNullOrEmpty())
        {
          Debug.Log(ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Scene not found : ").Append(reference.Id).Append(" , context : ").Append(context));
          IsError = true;
          return;
        }
        Debug.Log(ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Begin async load scene : ").Append(path).Append(" , context : ").Append(context));
        async = AssetDatabaseService.Instance.LoadSceneAsync(path);
        initialized = true;
      }
      if (async == null)
      {
        IsError = true;
      }
      else
      {
        if (!async.IsDone)
          return;
        if (!IsLoaded)
        {
          IsLoaded = true;
          Scene = (Scene) async.Asset;
        }
        if (!IsDisposed || !Scene.IsValid())
          return;
        Debug.Log(ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Unload scene : ").Append(Scene.path).Append(" , context : ").Append(context).Append(" , reason : ").Append(reason));
        SceneManager.UnloadSceneAsync(Scene);
        Scene = new Scene();
        IsReadyToDispose = true;
      }
    }

    public void Dispose(string reason)
    {
      this.reason = reason;
      IsDisposed = true;
    }

    public bool IsValid => Scene.IsValid();

    public string Path => Scene.path;
  }
}
