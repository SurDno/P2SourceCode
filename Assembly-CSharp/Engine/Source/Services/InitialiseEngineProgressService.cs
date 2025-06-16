using Engine.Impl.UI.Menu.Main;
using UnityEngine;

namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (InitialiseEngineProgressService)})]
  public class InitialiseEngineProgressService
  {
    public int Progress { get; set; }

    public int Count { get; private set; }

    public void Begin(int count)
    {
      this.Progress = 0;
      this.Count = count;
      LoadWindow.Instance.Progress = ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
      LoadWindow.Instance.ShowProgress = true;
    }

    public void Update(string title, string info)
    {
      float maxLoaderProgress = ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
      LoadWindow.Instance.Progress = Mathf.Clamp01((float) ((double) (this.Progress + 1) / (double) this.Count * (1.0 - (double) maxLoaderProgress)) + maxLoaderProgress);
    }

    public void End()
    {
      LoadWindow.Instance.ShowProgress = false;
      LoadWindow.Instance.Progress = 0.0f;
    }
  }
}
