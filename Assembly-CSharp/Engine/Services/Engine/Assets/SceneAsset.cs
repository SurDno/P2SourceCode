// Decompiled with JetBrains decompiler
// Type: Engine.Services.Engine.Assets.SceneAsset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using Cofe.Utility;
using Engine.Common;
using Inspectors;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
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
      if (!this.initialized)
      {
        if (!SceneController.CanLoad)
          return;
        if (this.IsDisposed)
        {
          this.IsReadyToDispose = true;
          return;
        }
        string path = AssetDatabaseService.Instance.GetPath(this.reference.Id);
        if (path.IsNullOrEmpty())
        {
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Scene not found : ").Append((object) this.reference.Id).Append(" , context : ").Append(this.context));
          this.IsError = true;
          return;
        }
        Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Begin async load scene : ").Append(path).Append(" , context : ").Append(this.context));
        this.async = AssetDatabaseService.Instance.LoadSceneAsync(path);
        this.initialized = true;
      }
      if (this.async == null)
      {
        this.IsError = true;
      }
      else
      {
        if (!this.async.IsDone)
          return;
        if (!this.IsLoaded)
        {
          this.IsLoaded = true;
          this.Scene = (Scene) this.async.Asset;
        }
        if (!this.IsDisposed || !this.Scene.IsValid())
          return;
        Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Unload scene : ").Append(this.Scene.path).Append(" , context : ").Append(this.context).Append(" , reason : ").Append(this.reason));
        SceneManager.UnloadSceneAsync(this.Scene);
        this.Scene = new Scene();
        this.IsReadyToDispose = true;
      }
    }

    public void Dispose(string reason)
    {
      this.reason = reason;
      this.IsDisposed = true;
    }

    public bool IsValid => this.Scene.IsValid();

    public string Path => this.Scene.path;
  }
}
