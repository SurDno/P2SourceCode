// Decompiled with JetBrains decompiler
// Type: AssetDatabases.AsyncLoadScene
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;

#nullable disable
namespace AssetDatabases
{
  public class AsyncLoadScene : IAsyncLoad
  {
    private Scene scene;
    private string path;
    private Stopwatch stopwatch;

    public object Asset => (object) this.scene;

    public bool IsDone { get; private set; }

    public AsyncLoadScene(string path)
    {
      this.path = path;
      this.stopwatch = new Stopwatch();
      this.stopwatch.Restart();
      SceneController.AddHandler(path, new Action<Scene>(this.OnSceneLoaded));
    }

    private void OnSceneLoaded(Scene scene)
    {
      this.scene = scene;
      this.IsDone = true;
      this.stopwatch.Stop();
      TimeSpan elapsed = this.stopwatch.Elapsed;
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Scene loaded, path : ").Append(this.path).Append(" , elapsed : ").Append((object) elapsed));
    }
  }
}
