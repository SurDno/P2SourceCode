using System.Collections;
using System.Reflection;
using Cofe.Utility;
using Engine.Source.Commons;
using Engine.Source.Settings;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (GameLauncher))]
  public class GameLauncher
  {
    private bool busy;

    public void StartGame()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(StartGameRoute());
    }

    public void RestartGame()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(RestartGameRoute(null));
    }

    public void RestartGameWithSave(string saveName)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(RestartGameRoute(saveName));
    }

    public void StartNewGame()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(LoadGameRoute(null));
    }

    public void StartGameWithSave(string saveName)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(LoadGameRoute(saveName));
    }

    public void ExitToMainMenu()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(ExitToMainMenuRoute());
    }

    private IEnumerator LoadGameRoute(string saveName)
    {
      if (busy)
      {
        Debug.LogError((object) (GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + " Launcher is busy"));
      }
      else
      {
        busy = true;
        yield return GameLauncherUtility.LoadGameRoute(saveName);
        busy = false;
      }
    }

    private IEnumerator ExitToMainMenuRoute()
    {
      if (busy)
      {
        Debug.LogError((object) (GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + " Launcher is busy"));
      }
      else
      {
        busy = true;
        yield return GameLauncherUtility.UnloadGameRoute();
        yield return GameLauncherUtility.ShowStartWindow();
        busy = false;
      }
    }

    private IEnumerator RestartGameRoute(string saveName)
    {
      if (busy)
      {
        Debug.LogError((object) (GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + " Launcher is busy"));
      }
      else
      {
        busy = true;
        yield return GameLauncherUtility.UnloadGameRoute();
        yield return GameLauncherUtility.LoadGameRoute(saveName);
        busy = false;
      }
    }

    private IEnumerator StartGameRoute()
    {
      if (busy)
      {
        Debug.LogError((object) (GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + " Launcher is busy"));
      }
      else
      {
        busy = true;
        if (InstanceByRequest<CommonSettings>.Instance.NotFirstStart.Value)
        {
          yield return GameLauncherUtility.ShowStartWindow();
          busy = false;
        }
        else
        {
          yield return GameLauncherUtility.ShowStartGammaWindow();
          InstanceByRequest<CommonSettings>.Instance.NotFirstStart.Value = true;
          busy = false;
        }
      }
    }
  }
}
