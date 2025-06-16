using Cofe.Utility;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Main;
using Engine.Services.Engine.Assets;
using Engine.Source.Behaviours.Yields;
using Engine.Source.Commons;
using Engine.Source.Otimizations;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Profiles;
using Engine.Source.Services.Saves;
using Engine.Source.Settings;
using Engine.Source.UI;
using Scripts.Data;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Engine.Source.Services
{
  public static class GameLauncherUtility
  {
    public static IEnumerator LoadGameRoute(string saveName)
    {
      GarbageCollector.Mode storedGCMode = GarbageCollector.GCMode;
      GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
      yield return (object) null;
      OptimizationUtility.ForceCollect();
      LoadGameProgressService loadProgress = ServiceLocator.GetService<LoadGameProgressService>();
      loadProgress.BeginLoadGame();
      Stopwatch sw = new Stopwatch();
      sw.Restart();
      bool newGame = saveName.IsNullOrEmpty();
      GameDataInfo data = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData();
      if (newGame)
      {
        LoadWindow.Instance.GameDay = data.LoadingWindowGameDay;
        LoadWindow.Instance.Mode = LoadWindowMode.StartGameData;
      }
      else
      {
        LoadWindow.Instance.GameDay = ProfilesUtility.GetGameDay(saveName);
        LoadWindow.Instance.Mode = LoadWindowMode.None;
      }
      LoadWindow.Instance.Show = true;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      TOD_Sky tod = TOD_Sky.Instance;
      if ((Object) tod != (Object) null)
        tod.transform.localEulerAngles = new Vector3(0.0f, data.SkyRotation, 0.0f);
      yield return (object) null;
      UIService uiService = ServiceLocator.GetService<UIService>();
      while ((Object) uiService.Active != (Object) null)
      {
        uiService.Pop();
        while (uiService.IsTransition)
          yield return (object) null;
      }
      yield return (object) null;
      GameServices.Initialize();
      yield return (object) null;
      SavesService savesService = ServiceLocator.GetService<SavesService>();
      if (newGame)
        yield return (object) savesService.Load();
      else
        yield return (object) savesService.Load(saveName);
      loadProgress.EndLoadGame();
      if (savesService.HasErrorLoading)
      {
        yield return (object) GameLauncherUtility.UnloadGameRoute();
        yield return (object) GameLauncherUtility.ShowStartWindow();
        IMessageWindow messageWindow = uiService.Push<IMessageWindow>();
        messageWindow.SetMessage(savesService.ErrorLoading);
      }
      else
      {
        InstanceByRequest<EngineApplication>.Instance.ViewEnabled = true;
        yield return (object) null;
        yield return (object) new WaitPlayer();
        loadProgress.PlayerReady();
        GarbageCollector.GCMode = storedGCMode;
        yield return (object) null;
        yield return (object) MemoryStrategy.Instance.Compute(MemoryStrategyContextEnum.StartGame);
        yield return (object) new WaitForSecondsRealtime(1f);
        InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
        loadProgress.LoadGameComplete();
        if (!newGame || data == null || data.HideLoadingWindow)
          LoadWindow.Instance.Show = false;
        ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.FirstPerson_Controlling;
        uiService.Push<IHudWindow>();
        while (uiService.IsTransition)
          yield return (object) null;
        ServiceLocator.GetService<LogicEventService>().FireCommonEvent("GameLaunchComplete");
        sw.Stop();
        UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("LoadGame, total elapsed : ").Append((object) sw.Elapsed));
      }
    }

    public static IEnumerator UnloadGameRoute()
    {
      LoadWindow.Instance.Mode = LoadWindowMode.None;
      LoadWindow.Instance.Show = true;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      AssetLoader assetLoader = ServiceLocator.GetService<AssetLoader>();
      while (!assetLoader.IsEmpty)
        yield return (object) null;
      InstanceByRequest<EngineApplication>.Instance.ViewEnabled = false;
      yield return (object) null;
      UIService uiService = ServiceLocator.GetService<UIService>();
      while ((Object) uiService.Active != (Object) null)
      {
        uiService.Pop();
        while (uiService.IsTransition)
          yield return (object) null;
      }
      SavesService savesService = ServiceLocator.GetService<SavesService>();
      savesService.Unload();
      yield return (object) null;
      GameServices.Terminate();
      yield return (object) null;
      while (SceneManager.sceneCount > 1)
        yield return (object) null;
      yield return (object) null;
      yield return (object) Resources.UnloadUnusedAssets();
      yield return (object) null;
      OptimizationUtility.ForceCollect();
      yield return (object) null;
    }

    public static IEnumerator ShowStartWindow()
    {
      MainMenuSetup.SetupMainMenuSettings();
      yield return (object) null;
      yield return (object) null;
      UIService uiService = ServiceLocator.GetService<UIService>();
      uiService.Push<IStartWindow>();
      while (uiService.IsTransition)
        yield return (object) null;
      InstanceByRequest<SoundGameSettings>.Instance.Apply();
      yield return (object) new WaitForSecondsRealtime(1f);
      LoadWindow.Instance.Show = false;
    }

    public static IEnumerator ShowStartGammaWindow()
    {
      MainMenuSetup.SetupMainMenuSettings();
      UIService uiService = ServiceLocator.GetService<UIService>();
      uiService.Push<IStartGammaSettingsWindow>();
      while (uiService.IsTransition)
        yield return (object) null;
      InstanceByRequest<SoundGameSettings>.Instance.Apply();
      LoadWindow.Instance.Show = false;
    }
  }
}
