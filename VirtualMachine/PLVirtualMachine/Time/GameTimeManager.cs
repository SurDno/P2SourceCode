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
using System;
using System.Collections.Generic;
using System.Xml;

namespace PLVirtualMachine.Time
{
  public static class GameTimeManager
  {
    private static Dictionary<string, GameTimeContext> gameTimeContexts = new Dictionary<string, GameTimeContext>();
    private static GameTimeContext currentContext;
    private static bool isGameEventsInited = false;
    private static VMCharacter mainContextPlayingCharacter = (VMCharacter) null;
    private static VMEntity currentPlayCharacterEntity;

    public static void Init()
    {
      GameTimeManager.currentContext = new GameTimeContext();
      GameTimeManager.AddGameTimeContext(GameTimeManager.currentContext);
    }

    public static void Start()
    {
      foreach (KeyValuePair<string, GameTimeContext> gameTimeContext in GameTimeManager.gameTimeContexts)
      {
        if (gameTimeContext.Value.IsMain)
          GameTimeManager.SetCurrentGameTimeContext(gameTimeContext.Key);
      }
    }

    public static void Update(TimeSpan delta)
    {
      if (!GameTimeManager.isGameEventsInited)
        GameTimeManager.InitGameEvents();
      if (GameTimeManager.currentContext == null)
      {
        Logger.AddError(string.Format("Current game time context not defined!!!"));
        GameTimeManager.SetDefaultContext();
      }
      if (GameTimeManager.currentContext == null)
        return;
      double totalSeconds = delta.TotalSeconds;
      if (totalSeconds < 0.0)
      {
        Logger.AddError(string.Format("Negative timespan interval {0} received at update from engine", (object) delta));
      }
      else
      {
        GameTimeContext.UpdateMode = true;
        GameTimeManager.currentContext.Update(totalSeconds);
        GameTimeContext.UpdateMode = false;
        GameTimeManager.UpdateGameTime();
      }
    }

    public static void ClearContexts()
    {
      foreach (KeyValuePair<string, GameTimeContext> gameTimeContext in GameTimeManager.gameTimeContexts)
        gameTimeContext.Value.Clear();
      GameTimeManager.currentContext = (GameTimeContext) null;
    }

    public static void Clear()
    {
      GameTimeManager.ClearContexts();
      GameTimeManager.gameTimeContexts.Clear();
      GameTimeManager.isGameEventsInited = false;
      GameTimeManager.mainContextPlayingCharacter = (VMCharacter) null;
      GameTimeManager.currentPlayCharacterEntity = (VMEntity) null;
      GameTimer.CurrTimerSerialNumber = 0;
    }

    public static VMCharacter MainContextPlayingCharacter
    {
      get => GameTimeManager.mainContextPlayingCharacter;
    }

    public static Dictionary<string, GameTimeContext> GameTimeContexts
    {
      get => GameTimeManager.gameTimeContexts;
    }

    public static GameTimeContext CurrentGameTimeContext => GameTimeManager.currentContext;

    public static void SetCurrentGameTimeContext(string sContextName)
    {
      if (!GameTimeManager.gameTimeContexts.ContainsKey(sContextName))
      {
        Logger.AddError(string.Format("Game time context with name {0} not found", (object) sContextName));
      }
      else
      {
        GameTimeManager.SynhronizeSolarTime();
        GameTimeManager.currentContext = GameTimeManager.gameTimeContexts[sContextName];
        VMEntity playCharacterEntity = GameTimeManager.GetContextPlayCharacterEntity(GameTimeManager.currentContext);
        if (playCharacterEntity != null)
          GameTimeManager.MakePlayCharacterEntity(playCharacterEntity);
        else
          Logger.AddWarning(string.Format("Play character entity for game context {0} not defined", (object) sContextName));
        GameTimeManager.SetEngineSolarTime();
        GameTimeManager.SetEngineGameTime();
        VMGameComponent.Instance.OnGameModeChange();
      }
    }

    public static void SetCurrentGameTime(GameTime newTime, bool bForceEvents = true)
    {
      GameTimeManager.DoSetCurrentGameTime(newTime, bForceEvents);
      GameTimeManager.SetEngineGameTime();
    }

    public static void AddTime(GameTime addingTime)
    {
      GameTimeManager.DoAddGameTime(addingTime);
      GameTime gameTime = new GameTime((ulong) Math.Round(ServiceLocator.GetService<ITimeService>().SolarTime.TotalSeconds));
      GameTimeManager.currentContext.SolarTime = new GameTime(gameTime.TotalSeconds + addingTime.TotalSeconds);
      GameTimeManager.SetEngineGameTime();
      GameTimeManager.SetEngineSolarTime();
    }

    public static GameTime GetCurrentGameTime()
    {
      if (GameTimeManager.currentContext == null)
      {
        Logger.AddError(string.Format("Cannot get current game time at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        GameTimeManager.SetDefaultContext();
      }
      return GameTimeManager.currentContext != null ? GameTimeManager.currentContext.GameTime : new GameTime();
    }

    public static GameTime GetCurrentGameDayTime()
    {
      if (GameTimeManager.currentContext == null)
      {
        Logger.AddError(string.Format("Cannot get current game time at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        GameTimeManager.SetDefaultContext();
      }
      return GameTimeManager.currentContext != null ? GameTimeManager.currentContext.GameDayTime : new GameTime();
    }

    public static void SetCurrentGameTimeSpeed(float fTimeSpeedFactor)
    {
      if (GameTimeManager.currentContext == null)
      {
        Logger.AddError(string.Format("Cannot set current game time speed at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        GameTimeManager.SetDefaultContext();
      }
      GameTimeManager.currentContext.GameTimeSpeed = fTimeSpeedFactor;
      GameTimeManager.SetEngineGameTime();
    }

    public static void SetCurrentSolarTime(GameTime solarTime)
    {
      if (GameTimeManager.currentContext == null)
      {
        Logger.AddError(string.Format("Cannot set current solar time at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        GameTimeManager.SetDefaultContext();
      }
      GameTimeManager.currentContext.SolarTime = solarTime;
      GameTimeManager.SetEngineSolarTime();
    }

    public static void SetCurrentSolarTimeSpeed(float fTimeSpeedFactor)
    {
      if (GameTimeManager.currentContext == null)
      {
        Logger.AddError(string.Format("Cannot set current solar time speed at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        GameTimeManager.SetDefaultContext();
      }
      GameTimeManager.SynhronizeSolarTime();
      GameTimeManager.currentContext.SolarTimeSpeed = fTimeSpeedFactor;
      GameTimeManager.SetEngineSolarTime();
    }

    public static void MakePlayCharacterEntity(VMEntity playCharacterEntity)
    {
      if (GameTimeManager.currentPlayCharacterEntity != null)
      {
        if (playCharacterEntity == GameTimeManager.currentPlayCharacterEntity)
          return;
        GameTimeManager.currentPlayCharacterEntity.Enabled = false;
      }
      GameTimeManager.currentPlayCharacterEntity = playCharacterEntity;
      GameTimeManager.currentPlayCharacterEntity.Enabled = true;
    }

    public static VMEntity CurrentPlayCharacterEntity => GameTimeManager.currentPlayCharacterEntity;

    public static void CreateGameTimeContext(IGameMode gameMode)
    {
      GameTimeContext context = new GameTimeContext(gameMode);
      if (gameMode.IsMain)
      {
        if (gameMode.PlayCharacterVariable != null)
        {
          CommonVariable characterVariable = gameMode.PlayCharacterVariable;
          characterVariable.Bind((IContext) IStaticDataContainer.StaticDataContainer.GameRoot, new VMType(typeof (IObjRef)));
          if (characterVariable.IsBinded)
          {
            if (characterVariable.Variable != null && typeof (IObjRef).IsAssignableFrom(characterVariable.Variable.GetType()))
            {
              VMObjRef variable = (VMObjRef) characterVariable.Variable;
              if (variable != null)
              {
                if (variable.Object != null)
                {
                  if (typeof (VMCharacter) == variable.Object.GetType())
                    GameTimeManager.mainContextPlayingCharacter = (VMCharacter) variable.Object;
                  else
                    Logger.AddError(string.Format("Main game context play character must be static character object !!!"));
                }
                else
                  Logger.AddError(string.Format("Main game context play character must be static character object !!!"));
              }
              else
                Logger.AddError(string.Format("Main game context play character must be static character object !!!"));
            }
          }
          else
            Logger.AddError(string.Format("Main game context play character must be static character object !!!"));
        }
        else
          Logger.AddError(string.Format("Main game context play character not defined!!!"));
      }
      GameTimeManager.AddGameTimeContext(context);
    }

    public static GameTimer StartTimer(
      EGameTimerType timerType,
      Guid initiatorFSMGuid,
      ulong stateId,
      GameTime targetTime,
      bool bIsRepeat,
      string sContextName)
    {
      if (sContextName == "")
      {
        if (GameTimeManager.currentContext != null)
          return GameTimeManager.currentContext.StartTimer(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat);
        Logger.AddError(string.Format("Cannot create timer at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        GameTimeManager.SetDefaultContext();
        return (GameTimer) null;
      }
      if (GameTimeManager.gameTimeContexts.ContainsKey(sContextName))
        return GameTimeManager.gameTimeContexts[sContextName].StartTimer(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat);
      Logger.AddError(string.Format("Cannot create timer at {0}: Game time context with name {1} not found", (object) DynamicFSM.CurrentStateInfo, (object) sContextName));
      return GameTimeManager.StartTimer(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat, "");
    }

    public static void StopTimer(ulong timerId, string sContextName)
    {
      if (sContextName == "")
      {
        if (GameTimeManager.currentContext == null)
          Logger.AddError(string.Format("Cannot create timer at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        else
          GameTimeManager.currentContext.StopTimer(timerId);
      }
      else if (GameTimeManager.gameTimeContexts.ContainsKey(sContextName))
        GameTimeManager.gameTimeContexts[sContextName].StopTimer(timerId);
      else
        Logger.AddError(string.Format("Cannot stop timer at {0}: Game time context with name {1} not found", (object) DynamicFSM.CurrentStateInfo, (object) sContextName));
    }

    public static void StateSave(IDataWriter writer)
    {
      writer.Begin(nameof (GameTimeManager), (Type) null, true);
      string str = "";
      if (GameTimeManager.currentContext != null)
      {
        GameTimeManager.SynhronizeSolarTime();
        str = GameTimeManager.currentContext.Name;
      }
      SaveManagerUtility.Save(writer, "CurrentContextName", str);
      SaveManagerUtility.SaveDynamicSerializableList<GameTimeContext>(writer, "GameTimeContextList", (IEnumerable<GameTimeContext>) GameTimeManager.gameTimeContexts.Values);
      writer.End(nameof (GameTimeManager), true);
    }

    public static void LoadFromXML(XmlElement xmlNode)
    {
      GameTimeManager.currentContext = (GameTimeContext) null;
      GameTimeManager.currentPlayCharacterEntity = (VMEntity) null;
      string str = "";
      for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i1];
        if (childNode1.Name == "CurrentContextName")
          str = childNode1.InnerText;
        else if (childNode1.Name == "GameTimeContextList")
        {
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            XmlElement childNode2 = (XmlElement) childNode1.ChildNodes[i2];
            string innerText = childNode2.FirstChild.InnerText;
            if (GameTimeManager.gameTimeContexts.ContainsKey(innerText))
            {
              GameTimeManager.gameTimeContexts[innerText].LoadFromXML(childNode2);
            }
            else
            {
              Logger.AddError(string.Format("SaveLoad warning: Unknown game time context with name {0} loaded", (object) innerText));
              GameTimeManager.gameTimeContexts.Add(innerText, new GameTimeContext());
              GameTimeManager.gameTimeContexts[innerText].LoadFromXML(childNode1);
            }
          }
        }
      }
      if ("" != str)
      {
        if (GameTimeManager.gameTimeContexts.ContainsKey(str))
          GameTimeManager.SetCurrentGameTimeContext(str);
        else
          Logger.AddError(string.Format("SaveLoad error: loading current game time context name {0} not registered", (object) str));
      }
      else
        Logger.AddError(string.Format("SaveLoad error: current game time context name not loaded"));
      if (GameTimeManager.currentContext != null)
        return;
      GameTimeManager.SetDefaultContext();
    }

    private static void AddGameTimeContext(GameTimeContext context)
    {
      if (GameTimeManager.gameTimeContexts.ContainsKey(context.Name) && GameTimeManager.currentContext != null && GameTimeManager.currentContext.Name == context.Name)
        GameTimeManager.currentContext = context;
      GameTimeManager.gameTimeContexts[context.Name] = context;
    }

    private static void SetDefaultContext()
    {
      foreach (KeyValuePair<string, GameTimeContext> gameTimeContext in GameTimeManager.gameTimeContexts)
      {
        if (gameTimeContext.Value.IsMain)
        {
          GameTimeManager.currentContext = gameTimeContext.Value;
          return;
        }
      }
      Logger.AddError(string.Format("Fatal game time context system error: main game time context not found!!!"));
    }

    private static void DoSetCurrentGameTime(GameTime newTime, bool bForceEvents)
    {
      if (GameTimeManager.currentContext == null)
      {
        Logger.AddError(string.Format("Cannot create timer at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        GameTimeManager.SetDefaultContext();
      }
      if (GameTimeManager.currentContext == null)
        return;
      double fDtime = newTime.TotalValue - GameTimeManager.currentContext.GameTime.TotalValue;
      if (fDtime > 0.0)
      {
        GameTimeManager.currentContext.Update(fDtime, bForceEvents);
      }
      else
      {
        GameTimeManager.currentContext.RevertEventsCutTime(newTime);
        GameTimeManager.currentContext.GameTime = newTime;
      }
    }

    private static void DoAddGameTime(GameTime addingTime)
    {
      if (GameTimeManager.currentContext == null)
      {
        Logger.AddError(string.Format("Cannot create timer at {0}: Current game time context not defined!!!", (object) DynamicFSM.CurrentStateInfo));
        GameTimeManager.SetDefaultContext();
      }
      if (GameTimeManager.currentContext == null)
        return;
      double totalValue = addingTime.TotalValue;
      if (totalValue <= 0.0)
        return;
      GameTimeManager.currentContext.Update(totalValue);
    }

    private static void UpdateGameTime()
    {
      TimeSpan timeSpan = new TimeSpan((int) GameTimeManager.currentContext.GameTime.Days, (int) (ushort) GameTimeManager.currentContext.GameTime.Hours, (int) (ushort) GameTimeManager.currentContext.GameTime.Minutes, (int) (ushort) GameTimeManager.currentContext.GameTime.Seconds);
      if (GameTimeManager.currentContext == null)
        return;
      ServiceLocator.GetService<ITimeService>().GameTime = timeSpan;
    }

    private static void SetEngineSolarTime()
    {
      if (GameTimeManager.currentContext == null)
        return;
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      service.SolarTime = new TimeSpan((int) GameTimeManager.currentContext.SolarTime.Days, (int) GameTimeManager.currentContext.SolarTime.Hours, (int) GameTimeManager.currentContext.SolarTime.Minutes, (int) GameTimeManager.currentContext.SolarTime.Seconds);
      service.SolarTimeFactor = GameTimeManager.currentContext.SolarTimeSpeed;
    }

    private static void SetEngineGameTime()
    {
      if (GameTimeManager.currentContext == null)
        return;
      TimeSpan timeSpan = new TimeSpan((int) GameTimeManager.currentContext.GameTime.Days, (int) (ushort) GameTimeManager.currentContext.GameTime.Hours, (int) (ushort) GameTimeManager.currentContext.GameTime.Minutes, (int) (ushort) GameTimeManager.currentContext.GameTime.Seconds);
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      service.GameTime = timeSpan;
      service.GameTimeFactor = GameTimeManager.currentContext.GameTimeSpeed;
    }

    private static void InitGameEvents()
    {
      ServiceLocator.GetService<ITimeService>().GameTimeChangedEvent += new Action<TimeSpan>(GameTimeManager.OnChangeGameTime);
      GameTimeManager.isGameEventsInited = true;
    }

    private static void OnChangeGameTime(TimeSpan newTime)
    {
      GameTime newTime1 = new GameTime((ulong) Math.Round(newTime.TotalSeconds));
      GameTime currentGameTime = GameTimeManager.GetCurrentGameTime();
      if (newTime1.TotalSeconds < currentGameTime.TotalSeconds)
        return;
      GameTimeManager.DoSetCurrentGameTime(newTime1, true);
      GameTimeManager.SetEngineGameTime();
      GameTimeManager.SynhronizeSolarTime();
    }

    private static void SynhronizeSolarTime()
    {
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      if (GameTimeManager.currentContext == null)
        return;
      GameTime gameTime = new GameTime((ulong) Math.Round(service.SolarTime.TotalSeconds));
      GameTimeManager.currentContext.SolarTime = gameTime;
    }

    private static VMEntity GetContextPlayCharacterEntity(GameTimeContext context)
    {
      CommonVariable characterVariable = context.StaticGameMode.PlayCharacterVariable;
      if (characterVariable == null)
        return GameTimeManager.currentPlayCharacterEntity;
      VMType vmType = new VMType(typeof (IObjRef));
      characterVariable.Bind((IContext) IStaticDataContainer.StaticDataContainer.GameRoot, vmType);
      if (VirtualMachine.Instance.GameRootEntity != null)
      {
        IDynamicGameObjectContext activeContext = (IDynamicGameObjectContext) VirtualMachine.Instance.GameRootEntity.GetFSM();
        if (VMEngineAPIManager.LastMethodExecInitiator != null)
          activeContext = VMEngineAPIManager.LastMethodExecInitiator;
        IObjRef dynamicVariableValue = (IObjRef) ((VMVariableService) IVariableService.Instance).GetDynamicVariableValue(characterVariable, vmType, activeContext);
        if (dynamicVariableValue != null)
          return dynamicVariableValue.EngineInstance != null ? (VMEntity) dynamicVariableValue.EngineInstance : WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(dynamicVariableValue.EngineGuid);
        Logger.AddError(string.Format("Cannot get game time context player entity: entity by variable {0} not found at {1}", (object) characterVariable.ToString(), (object) DynamicFSM.CurrentStateInfo));
      }
      return (VMEntity) null;
    }
  }
}
