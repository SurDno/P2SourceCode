// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.GameLauncher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Source.Commons;
using Engine.Source.Settings;
using System.Collections;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (GameLauncher)})]
  public class GameLauncher
  {
    private bool busy;

    public void StartGame()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(this.StartGameRoute());
    }

    public void RestartGame()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(this.RestartGameRoute((string) null));
    }

    public void RestartGameWithSave(string saveName)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(this.RestartGameRoute(saveName));
    }

    public void StartNewGame()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(this.LoadGameRoute((string) null));
    }

    public void StartGameWithSave(string saveName)
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(this.LoadGameRoute(saveName));
    }

    public void ExitToMainMenu()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(this.GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      CoroutineService.Instance.Route(this.ExitToMainMenuRoute());
    }

    private IEnumerator LoadGameRoute(string saveName)
    {
      if (this.busy)
      {
        Debug.LogError((object) (this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + " Launcher is busy"));
      }
      else
      {
        this.busy = true;
        yield return (object) GameLauncherUtility.LoadGameRoute(saveName);
        this.busy = false;
      }
    }

    private IEnumerator ExitToMainMenuRoute()
    {
      if (this.busy)
      {
        Debug.LogError((object) (this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + " Launcher is busy"));
      }
      else
      {
        this.busy = true;
        yield return (object) GameLauncherUtility.UnloadGameRoute();
        yield return (object) GameLauncherUtility.ShowStartWindow();
        this.busy = false;
      }
    }

    private IEnumerator RestartGameRoute(string saveName)
    {
      if (this.busy)
      {
        Debug.LogError((object) (this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + " Launcher is busy"));
      }
      else
      {
        this.busy = true;
        yield return (object) GameLauncherUtility.UnloadGameRoute();
        yield return (object) GameLauncherUtility.LoadGameRoute(saveName);
        this.busy = false;
      }
    }

    private IEnumerator StartGameRoute()
    {
      if (this.busy)
      {
        Debug.LogError((object) (this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name + " Launcher is busy"));
      }
      else
      {
        this.busy = true;
        if (InstanceByRequest<CommonSettings>.Instance.NotFirstStart.Value)
        {
          yield return (object) GameLauncherUtility.ShowStartWindow();
          this.busy = false;
        }
        else
        {
          yield return (object) GameLauncherUtility.ShowStartGammaWindow();
          InstanceByRequest<CommonSettings>.Instance.NotFirstStart.Value = true;
          this.busy = false;
        }
      }
    }
  }
}
