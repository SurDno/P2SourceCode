using System.Reflection;
using Engine.Common.Services;
using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (LoadGameProgressService), typeof (ILoadProgress))]
  public class LoadGameProgressService : ILoadProgress
  {
    public void BeginLoadGame()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 0.0f;
      LoadWindow.Instance.ShowProgress = true;
    }

    public void OnBeforeLoadData()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 1f / 43f;
    }

    public void OnAfterLoadData()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 0.302325577f;
    }

    public void OnBuildHierarchy()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 0.372093022f;
    }

    public void OnLoadDataComplete()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 0.372093022f;
    }

    public void OnBeforeCreateHierarchy()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 0.372093022f;
    }

    public void OnAfterCreateHierarchy()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 0.558139563f;
    }

    public void OnLoadComplete()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 0.627907f;
    }

    public void EndLoadGame()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 0.6976744f;
    }

    public void PlayerReady()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.Progress = 1f;
    }

    public void LoadGameComplete()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (LoadGameProgressService)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name));
      LoadWindow.Instance.ShowProgress = false;
      LoadWindow.Instance.Progress = 0.0f;
    }
  }
}
