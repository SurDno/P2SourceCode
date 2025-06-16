using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Services;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Time;

public static class GameTimeManager {
	private static Dictionary<string, GameTimeContext> gameTimeContexts = new();
	private static GameTimeContext currentContext;
	private static bool isGameEventsInited;
	private static VMCharacter mainContextPlayingCharacter;
	private static VMEntity currentPlayCharacterEntity;

	public static void Init() {
		currentContext = new GameTimeContext();
		AddGameTimeContext(currentContext);
	}

	public static void Start() {
		foreach (var gameTimeContext in gameTimeContexts)
			if (gameTimeContext.Value.IsMain)
				SetCurrentGameTimeContext(gameTimeContext.Key);
	}

	public static void Update(TimeSpan delta) {
		if (!isGameEventsInited)
			InitGameEvents();
		if (currentContext == null) {
			Logger.AddError("Current game time context not defined!!!");
			SetDefaultContext();
		}

		if (currentContext == null)
			return;
		var totalSeconds = delta.TotalSeconds;
		if (totalSeconds < 0.0)
			Logger.AddError(string.Format("Negative timespan interval {0} received at update from engine", delta));
		else {
			GameTimeContext.UpdateMode = true;
			currentContext.Update(totalSeconds);
			GameTimeContext.UpdateMode = false;
			UpdateGameTime();
		}
	}

	public static void ClearContexts() {
		foreach (var gameTimeContext in gameTimeContexts)
			gameTimeContext.Value.Clear();
		currentContext = null;
	}

	public static void Clear() {
		ClearContexts();
		gameTimeContexts.Clear();
		isGameEventsInited = false;
		mainContextPlayingCharacter = null;
		currentPlayCharacterEntity = null;
		GameTimer.CurrTimerSerialNumber = 0;
	}

	public static VMCharacter MainContextPlayingCharacter => mainContextPlayingCharacter;

	public static Dictionary<string, GameTimeContext> GameTimeContexts => gameTimeContexts;

	public static GameTimeContext CurrentGameTimeContext => currentContext;

	public static void SetCurrentGameTimeContext(string sContextName) {
		if (!gameTimeContexts.ContainsKey(sContextName))
			Logger.AddError(string.Format("Game time context with name {0} not found", sContextName));
		else {
			SynhronizeSolarTime();
			currentContext = gameTimeContexts[sContextName];
			var playCharacterEntity = GetContextPlayCharacterEntity(currentContext);
			if (playCharacterEntity != null)
				MakePlayCharacterEntity(playCharacterEntity);
			else
				Logger.AddWarning(string.Format("Play character entity for game context {0} not defined",
					sContextName));
			SetEngineSolarTime();
			SetEngineGameTime();
			VMGameComponent.Instance.OnGameModeChange();
		}
	}

	public static void SetCurrentGameTime(GameTime newTime, bool bForceEvents = true) {
		DoSetCurrentGameTime(newTime, bForceEvents);
		SetEngineGameTime();
	}

	public static void AddTime(GameTime addingTime) {
		DoAddGameTime(addingTime);
		var gameTime =
			new GameTime((ulong)Math.Round(ServiceLocator.GetService<ITimeService>().SolarTime.TotalSeconds));
		currentContext.SolarTime = new GameTime(gameTime.TotalSeconds + addingTime.TotalSeconds);
		SetEngineGameTime();
		SetEngineSolarTime();
	}

	public static GameTime GetCurrentGameTime() {
		if (currentContext == null) {
			Logger.AddError(string.Format(
				"Cannot get current game time at {0}: Current game time context not defined!!!",
				DynamicFSM.CurrentStateInfo));
			SetDefaultContext();
		}

		return currentContext != null ? currentContext.GameTime : new GameTime();
	}

	public static GameTime GetCurrentGameDayTime() {
		if (currentContext == null) {
			Logger.AddError(string.Format(
				"Cannot get current game time at {0}: Current game time context not defined!!!",
				DynamicFSM.CurrentStateInfo));
			SetDefaultContext();
		}

		return currentContext != null ? currentContext.GameDayTime : new GameTime();
	}

	public static void SetCurrentGameTimeSpeed(float fTimeSpeedFactor) {
		if (currentContext == null) {
			Logger.AddError(string.Format(
				"Cannot set current game time speed at {0}: Current game time context not defined!!!",
				DynamicFSM.CurrentStateInfo));
			SetDefaultContext();
		}

		currentContext.GameTimeSpeed = fTimeSpeedFactor;
		SetEngineGameTime();
	}

	public static void SetCurrentSolarTime(GameTime solarTime) {
		if (currentContext == null) {
			Logger.AddError(string.Format(
				"Cannot set current solar time at {0}: Current game time context not defined!!!",
				DynamicFSM.CurrentStateInfo));
			SetDefaultContext();
		}

		currentContext.SolarTime = solarTime;
		SetEngineSolarTime();
	}

	public static void SetCurrentSolarTimeSpeed(float fTimeSpeedFactor) {
		if (currentContext == null) {
			Logger.AddError(string.Format(
				"Cannot set current solar time speed at {0}: Current game time context not defined!!!",
				DynamicFSM.CurrentStateInfo));
			SetDefaultContext();
		}

		SynhronizeSolarTime();
		currentContext.SolarTimeSpeed = fTimeSpeedFactor;
		SetEngineSolarTime();
	}

	public static void MakePlayCharacterEntity(VMEntity playCharacterEntity) {
		if (currentPlayCharacterEntity != null) {
			if (playCharacterEntity == currentPlayCharacterEntity)
				return;
			currentPlayCharacterEntity.Enabled = false;
		}

		currentPlayCharacterEntity = playCharacterEntity;
		currentPlayCharacterEntity.Enabled = true;
	}

	public static VMEntity CurrentPlayCharacterEntity => currentPlayCharacterEntity;

	public static void CreateGameTimeContext(IGameMode gameMode) {
		var context = new GameTimeContext(gameMode);
		if (gameMode.IsMain) {
			if (gameMode.PlayCharacterVariable != null) {
				var characterVariable = gameMode.PlayCharacterVariable;
				characterVariable.Bind(IStaticDataContainer.StaticDataContainer.GameRoot, new VMType(typeof(IObjRef)));
				if (characterVariable.IsBinded) {
					if (characterVariable.Variable != null &&
					    typeof(IObjRef).IsAssignableFrom(characterVariable.Variable.GetType())) {
						var variable = (VMObjRef)characterVariable.Variable;
						if (variable != null) {
							if (variable.Object != null) {
								if (typeof(VMCharacter) == variable.Object.GetType())
									mainContextPlayingCharacter = (VMCharacter)variable.Object;
								else
									Logger.AddError(
										"Main game context play character must be static character object !!!");
							} else
								Logger.AddError("Main game context play character must be static character object !!!");
						} else
							Logger.AddError("Main game context play character must be static character object !!!");
					}
				} else
					Logger.AddError("Main game context play character must be static character object !!!");
			} else
				Logger.AddError("Main game context play character not defined!!!");
		}

		AddGameTimeContext(context);
	}

	public static GameTimer StartTimer(
		EGameTimerType timerType,
		Guid initiatorFSMGuid,
		ulong stateId,
		GameTime targetTime,
		bool bIsRepeat,
		string sContextName) {
		if (sContextName == "") {
			if (currentContext != null)
				return currentContext.StartTimer(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat);
			Logger.AddError(string.Format("Cannot create timer at {0}: Current game time context not defined!!!",
				DynamicFSM.CurrentStateInfo));
			SetDefaultContext();
			return null;
		}

		if (gameTimeContexts.ContainsKey(sContextName))
			return gameTimeContexts[sContextName]
				.StartTimer(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat);
		Logger.AddError(string.Format("Cannot create timer at {0}: Game time context with name {1} not found",
			DynamicFSM.CurrentStateInfo, sContextName));
		return StartTimer(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat, "");
	}

	public static void StopTimer(ulong timerId, string sContextName) {
		if (sContextName == "") {
			if (currentContext == null)
				Logger.AddError(string.Format("Cannot create timer at {0}: Current game time context not defined!!!",
					DynamicFSM.CurrentStateInfo));
			else
				currentContext.StopTimer(timerId);
		} else if (gameTimeContexts.ContainsKey(sContextName))
			gameTimeContexts[sContextName].StopTimer(timerId);
		else
			Logger.AddError(string.Format("Cannot stop timer at {0}: Game time context with name {1} not found",
				DynamicFSM.CurrentStateInfo, sContextName));
	}

	public static void StateSave(IDataWriter writer) {
		writer.Begin(nameof(GameTimeManager), null, true);
		var str = "";
		if (currentContext != null) {
			SynhronizeSolarTime();
			str = currentContext.Name;
		}

		SaveManagerUtility.Save(writer, "CurrentContextName", str);
		SaveManagerUtility.SaveDynamicSerializableList(writer, "GameTimeContextList", gameTimeContexts.Values);
		writer.End(nameof(GameTimeManager), true);
	}

	public static void LoadFromXML(XmlElement xmlNode) {
		currentContext = null;
		currentPlayCharacterEntity = null;
		var str = "";
		for (var i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1) {
			var childNode1 = (XmlElement)xmlNode.ChildNodes[i1];
			if (childNode1.Name == "CurrentContextName")
				str = childNode1.InnerText;
			else if (childNode1.Name == "GameTimeContextList")
				for (var i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2) {
					var childNode2 = (XmlElement)childNode1.ChildNodes[i2];
					var innerText = childNode2.FirstChild.InnerText;
					if (gameTimeContexts.ContainsKey(innerText))
						gameTimeContexts[innerText].LoadFromXML(childNode2);
					else {
						Logger.AddError(
							string.Format("SaveLoad warning: Unknown game time context with name {0} loaded",
								innerText));
						gameTimeContexts.Add(innerText, new GameTimeContext());
						gameTimeContexts[innerText].LoadFromXML(childNode1);
					}
				}
		}

		if ("" != str) {
			if (gameTimeContexts.ContainsKey(str))
				SetCurrentGameTimeContext(str);
			else
				Logger.AddError(
					string.Format("SaveLoad error: loading current game time context name {0} not registered", str));
		} else
			Logger.AddError("SaveLoad error: current game time context name not loaded");

		if (currentContext != null)
			return;
		SetDefaultContext();
	}

	private static void AddGameTimeContext(GameTimeContext context) {
		if (gameTimeContexts.ContainsKey(context.Name) && currentContext != null && currentContext.Name == context.Name)
			currentContext = context;
		gameTimeContexts[context.Name] = context;
	}

	private static void SetDefaultContext() {
		foreach (var gameTimeContext in gameTimeContexts)
			if (gameTimeContext.Value.IsMain) {
				currentContext = gameTimeContext.Value;
				return;
			}

		Logger.AddError("Fatal game time context system error: main game time context not found!!!");
	}

	private static void DoSetCurrentGameTime(GameTime newTime, bool bForceEvents) {
		if (currentContext == null) {
			Logger.AddError(string.Format("Cannot create timer at {0}: Current game time context not defined!!!",
				DynamicFSM.CurrentStateInfo));
			SetDefaultContext();
		}

		if (currentContext == null)
			return;
		var fDtime = newTime.TotalValue - currentContext.GameTime.TotalValue;
		if (fDtime > 0.0)
			currentContext.Update(fDtime, bForceEvents);
		else {
			currentContext.RevertEventsCutTime(newTime);
			currentContext.GameTime = newTime;
		}
	}

	private static void DoAddGameTime(GameTime addingTime) {
		if (currentContext == null) {
			Logger.AddError(string.Format("Cannot create timer at {0}: Current game time context not defined!!!",
				DynamicFSM.CurrentStateInfo));
			SetDefaultContext();
		}

		if (currentContext == null)
			return;
		var totalValue = addingTime.TotalValue;
		if (totalValue <= 0.0)
			return;
		currentContext.Update(totalValue);
	}

	private static void UpdateGameTime() {
		var timeSpan = new TimeSpan(currentContext.GameTime.Days, currentContext.GameTime.Hours,
			currentContext.GameTime.Minutes, currentContext.GameTime.Seconds);
		if (currentContext == null)
			return;
		ServiceLocator.GetService<ITimeService>().GameTime = timeSpan;
	}

	private static void SetEngineSolarTime() {
		if (currentContext == null)
			return;
		var service = ServiceLocator.GetService<ITimeService>();
		service.SolarTime = new TimeSpan(currentContext.SolarTime.Days, currentContext.SolarTime.Hours,
			currentContext.SolarTime.Minutes, currentContext.SolarTime.Seconds);
		service.SolarTimeFactor = currentContext.SolarTimeSpeed;
	}

	private static void SetEngineGameTime() {
		if (currentContext == null)
			return;
		var timeSpan = new TimeSpan(currentContext.GameTime.Days, currentContext.GameTime.Hours,
			currentContext.GameTime.Minutes, currentContext.GameTime.Seconds);
		var service = ServiceLocator.GetService<ITimeService>();
		service.GameTime = timeSpan;
		service.GameTimeFactor = currentContext.GameTimeSpeed;
	}

	private static void InitGameEvents() {
		ServiceLocator.GetService<ITimeService>().GameTimeChangedEvent += OnChangeGameTime;
		isGameEventsInited = true;
	}

	private static void OnChangeGameTime(TimeSpan newTime) {
		var newTime1 = new GameTime((ulong)Math.Round(newTime.TotalSeconds));
		var currentGameTime = GetCurrentGameTime();
		if (newTime1.TotalSeconds < currentGameTime.TotalSeconds)
			return;
		DoSetCurrentGameTime(newTime1, true);
		SetEngineGameTime();
		SynhronizeSolarTime();
	}

	private static void SynhronizeSolarTime() {
		var service = ServiceLocator.GetService<ITimeService>();
		if (currentContext == null)
			return;
		var gameTime = new GameTime((ulong)Math.Round(service.SolarTime.TotalSeconds));
		currentContext.SolarTime = gameTime;
	}

	private static VMEntity GetContextPlayCharacterEntity(GameTimeContext context) {
		var characterVariable = context.StaticGameMode.PlayCharacterVariable;
		if (characterVariable == null)
			return currentPlayCharacterEntity;
		var vmType = new VMType(typeof(IObjRef));
		characterVariable.Bind(IStaticDataContainer.StaticDataContainer.GameRoot, vmType);
		if (VirtualMachine.Instance.GameRootEntity != null) {
			IDynamicGameObjectContext activeContext = VirtualMachine.Instance.GameRootEntity.GetFSM();
			if (VMEngineAPIManager.LastMethodExecInitiator != null)
				activeContext = VMEngineAPIManager.LastMethodExecInitiator;
			var dynamicVariableValue =
				(IObjRef)((VMVariableService)IVariableService.Instance).GetDynamicVariableValue(characterVariable,
					vmType, activeContext);
			if (dynamicVariableValue != null)
				return dynamicVariableValue.EngineInstance != null
					? (VMEntity)dynamicVariableValue.EngineInstance
					: WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(dynamicVariableValue.EngineGuid);
			Logger.AddError(string.Format(
				"Cannot get game time context player entity: entity by variable {0} not found at {1}",
				characterVariable, DynamicFSM.CurrentStateInfo));
		}

		return null;
	}
}