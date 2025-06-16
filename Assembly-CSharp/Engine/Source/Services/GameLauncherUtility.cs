using System.Collections;
using System.Diagnostics;
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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;

namespace Engine.Source.Services;

public static class GameLauncherUtility {
	public static IEnumerator LoadGameRoute(string saveName) {
		var storedGCMode = GarbageCollector.GCMode;
		GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
		yield return null;
		OptimizationUtility.ForceCollect();
		var loadProgress = ServiceLocator.GetService<LoadGameProgressService>();
		loadProgress.BeginLoadGame();
		var sw = new Stopwatch();
		sw.Restart();
		var newGame = saveName.IsNullOrEmpty();
		var data = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData();
		if (newGame) {
			LoadWindow.Instance.GameDay = data.LoadingWindowGameDay;
			LoadWindow.Instance.Mode = LoadWindowMode.StartGameData;
		} else {
			LoadWindow.Instance.GameDay = ProfilesUtility.GetGameDay(saveName);
			LoadWindow.Instance.Mode = LoadWindowMode.None;
		}

		LoadWindow.Instance.Show = true;
		InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
		var tod = TOD_Sky.Instance;
		if (tod != null)
			tod.transform.localEulerAngles = new Vector3(0.0f, data.SkyRotation, 0.0f);
		yield return null;
		var uiService = ServiceLocator.GetService<UIService>();
		while (uiService.Active != null) {
			uiService.Pop();
			while (uiService.IsTransition)
				yield return null;
		}

		yield return null;
		GameServices.Initialize();
		yield return null;
		var savesService = ServiceLocator.GetService<SavesService>();
		if (newGame)
			yield return savesService.Load();
		else
			yield return savesService.Load(saveName);
		loadProgress.EndLoadGame();
		if (savesService.HasErrorLoading) {
			yield return UnloadGameRoute();
			yield return ShowStartWindow();
			var messageWindow = uiService.Push<IMessageWindow>();
			messageWindow.SetMessage(savesService.ErrorLoading);
		} else {
			InstanceByRequest<EngineApplication>.Instance.ViewEnabled = true;
			yield return null;
			yield return new WaitPlayer();
			loadProgress.PlayerReady();
			GarbageCollector.GCMode = storedGCMode;
			yield return null;
			yield return MemoryStrategy.Instance.Compute(MemoryStrategyContextEnum.StartGame);
			yield return new WaitForSecondsRealtime(1f);
			InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
			loadProgress.LoadGameComplete();
			if (!newGame || data == null || data.HideLoadingWindow)
				LoadWindow.Instance.Show = false;
			ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.FirstPerson_Controlling;
			uiService.Push<IHudWindow>();
			while (uiService.IsTransition)
				yield return null;
			ServiceLocator.GetService<LogicEventService>().FireCommonEvent("GameLaunchComplete");
			sw.Stop();
			Debug.Log(ObjectInfoUtility.GetStream().Append("LoadGame, total elapsed : ").Append(sw.Elapsed));
		}
	}

	public static IEnumerator UnloadGameRoute() {
		LoadWindow.Instance.Mode = LoadWindowMode.None;
		LoadWindow.Instance.Show = true;
		InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
		var assetLoader = ServiceLocator.GetService<AssetLoader>();
		while (!assetLoader.IsEmpty)
			yield return null;
		InstanceByRequest<EngineApplication>.Instance.ViewEnabled = false;
		yield return null;
		var uiService = ServiceLocator.GetService<UIService>();
		while (uiService.Active != null) {
			uiService.Pop();
			while (uiService.IsTransition)
				yield return null;
		}

		var savesService = ServiceLocator.GetService<SavesService>();
		savesService.Unload();
		yield return null;
		GameServices.Terminate();
		yield return null;
		while (SceneManager.sceneCount > 1)
			yield return null;
		yield return null;
		yield return Resources.UnloadUnusedAssets();
		yield return null;
		OptimizationUtility.ForceCollect();
		yield return null;
	}

	public static IEnumerator ShowStartWindow() {
		MainMenuSetup.SetupMainMenuSettings();
		yield return null;
		yield return null;
		var uiService = ServiceLocator.GetService<UIService>();
		uiService.Push<IStartWindow>();
		while (uiService.IsTransition)
			yield return null;
		InstanceByRequest<SoundGameSettings>.Instance.Apply();
		yield return new WaitForSecondsRealtime(1f);
		LoadWindow.Instance.Show = false;
	}

	public static IEnumerator ShowStartGammaWindow() {
		MainMenuSetup.SetupMainMenuSettings();
		var uiService = ServiceLocator.GetService<UIService>();
		uiService.Push<IStartGammaSettingsWindow>();
		while (uiService.IsTransition)
			yield return null;
		InstanceByRequest<SoundGameSettings>.Instance.Apply();
		LoadWindow.Instance.Show = false;
	}
}