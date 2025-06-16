using Engine.Impl.UI.Menu.Main;
using UnityEngine;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (InitialiseEngineProgressService))]
  public class InitialiseEngineProgressService
  {
    public int Progress { get; set; }

    public int Count { get; private set; }

    public void Begin(int count)
    {
      Progress = 0;
      Count = count;
      LoadWindow.Instance.Progress = ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
      LoadWindow.Instance.ShowProgress = true;
    }

    public void Update(string title, string info)
    {
      float maxLoaderProgress = ScriptableObjectInstance<GameSettingsData>.Instance.MaxLoaderProgress;
      LoadWindow.Instance.Progress = Mathf.Clamp01((float) ((Progress + 1) / (double) Count * (1.0 - maxLoaderProgress)) + maxLoaderProgress);
    }

    public void End()
    {
      LoadWindow.Instance.ShowProgress = false;
      LoadWindow.Instance.Progress = 0.0f;
    }
  }
}
