using System;
using System.Diagnostics;

namespace AssetDatabases
{
  public class AsyncLoadScene : IAsyncLoad
  {
    private Scene scene;
    private string path;
    private Stopwatch stopwatch;

    public object Asset => (object) scene;

    public bool IsDone { get; private set; }

    public AsyncLoadScene(string path)
    {
      this.path = path;
      stopwatch = new Stopwatch();
      stopwatch.Restart();
      SceneController.AddHandler(path, OnSceneLoaded);
    }

    private void OnSceneLoaded(Scene scene)
    {
      this.scene = scene;
      IsDone = true;
      stopwatch.Stop();
      TimeSpan elapsed = stopwatch.Elapsed;
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Loader]").Append(" Scene loaded, path : ").Append(path).Append(" , elapsed : ").Append(elapsed));
    }
  }
}
