using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Comparers;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Dynamic;

namespace PLVirtualMachine.Time
{
  public class GameTimeContext : ISerializeStateSave, IDynamicLoadSerializable
  {
    private bool isMain = true;
    private string contextName = "common";
    private float solarTimeSpeedFactor = 1f;
    private float gameTimeSpeedFactor = 1f;
    private GameTime gameTime = new GameTime();
    private GameTime gameDayTime = new GameTime();
    private GameTime solarTime = new GameTime();
    private int currProcTimerIndex = -1;
    private List<GameTimer> gameTimerList = new List<GameTimer>();
    private Stack<int> freeTimersIndexesStack = new Stack<int>();
    private Dictionary<ulong, int> loadingTimersIdIndexesDict = new Dictionary<ulong, int>(UlongComparer.Instance);
    private SortedList<ulong, int> timerPriorityList = new SortedList<ulong, int>();
    private IGameMode gameMode;
    private static bool isUpdateMode;
    public static bool TIMERS_LIST_PROCESSIN_ON;

    public GameTimeContext()
    {
    }

    public GameTimeContext(IGameMode gameMode)
    {
      if (gameMode == null)
      {
        Logger.AddError(" Invalid game time context creation: game mode not defined! : " + DynamicFSM.CurrentStateInfo);
      }
      else
      {
        if (gameMode.StartGameTime == null)
          Logger.AddError(" Invalid game time context creation: start time not defined! : " + DynamicFSM.CurrentStateInfo);
        this.gameMode = gameMode;
        Init();
      }
    }

    public IGameMode StaticGameMode => gameMode;

    public bool IsMain => isMain;

    public GameTime GameTime
    {
      get => gameTime;
      set
      {
        if (value == null)
        {
          Logger.AddError("Invalid game time value received at game time context " + Name);
        }
        else
        {
          gameTime = new GameTime(value);
          gameDayTime.CopyFrom(gameTime);
          gameDayTime.Days = 0;
        }
      }
    }

    public GameTime GameDayTime => gameDayTime;

    public GameTime SolarTime
    {
      get => solarTime;
      set => solarTime = value;
    }

    public float GameTimeSpeed
    {
      get => gameTimeSpeedFactor;
      set => gameTimeSpeedFactor = value;
    }

    public float SolarTimeSpeed
    {
      get => solarTimeSpeedFactor;
      set => solarTimeSpeedFactor = value;
    }

    public string Name => contextName;

    public void Update(double fDtime, bool bForceEvents = true)
    {
      gameTime.Process(fDtime);
      ServiceLocator.GetService<ITimeService>();
      TIMERS_LIST_PROCESSIN_ON = true;
      foreach (KeyValuePair<ulong, int> timerPriority in timerPriorityList)
      {
        int index = timerPriority.Value;
        if (gameTimerList[index].Active)
        {
          if (!bForceEvents)
            gameTimerList[index].EventsCutTime = gameTime;
          if (gameTimerList[index].GTType == EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE)
            gameTimerList[index].Check(gameTime);
          else
            gameTimerList[index].Check(fDtime, bForceEvents);
        }
      }
      TIMERS_LIST_PROCESSIN_ON = false;
      gameDayTime.CopyFrom(gameTime);
      gameDayTime.Days = 0;
      currProcTimerIndex = -1;
    }

    public GameTimer StartTimer(
      EGameTimerType timerType,
      Guid initiatorFSMGuid,
      ulong stateId,
      GameTime targetTime,
      bool bIsRepeat)
    {
      if (freeTimersIndexesStack.Count > 0)
      {
        int index = freeTimersIndexesStack.Pop();
        if (index < 0 || index >= gameTimerList.Count)
        {
          Logger.AddError(string.Format("Invalid free timer index {0}, timers list count = {1} at {2}", index, gameTimerList.Count, DynamicFSM.CurrentStateInfo));
        }
        else
        {
          if (currProcTimerIndex < 0 || index < currProcTimerIndex)
          {
            gameTimerList[index].Start(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat);
            return gameTimerList[index];
          }
          freeTimersIndexesStack.Push(index);
        }
      }
      int count = gameTimerList.Count;
      GameTimer gameTimer = new GameTimer(count);
      gameTimer.Start(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat);
      gameTimerList.Add(gameTimer);
      OnNewTimer(count);
      int dynamicFsmObjectsCount = VirtualMachine.Instance.DynamicFSMObjectsCount;
      if (VirtualMachine.Instance.IsLoaded && gameTimerList.Count > dynamicFsmObjectsCount * 4)
        Logger.AddError("Warning: on start game timer too many timers created!!! at " + DynamicFSM.CurrentStateInfo);
      return gameTimer;
    }

    public void StopTimer(ulong timerID)
    {
      int timerIndexById = GetTimerIndexByID(timerID);
      if (timerIndexById < 0 || timerIndexById >= gameTimerList.Count)
      {
        Logger.AddWarning(string.Format("Invalid stopping game timer index {0} at {1}", timerIndexById, DynamicFSM.CurrentStateInfo));
      }
      else
      {
        if ((long) gameTimerList[timerIndexById].TimerGuid != (long) timerID)
          return;
        gameTimerList[timerIndexById].Stop();
        if (freeTimersIndexesStack.Contains(timerIndexById))
          return;
        freeTimersIndexesStack.Push(timerIndexById);
      }
    }

    private int GetTimerIndexByID(ulong timerID)
    {
      if (timerID == 0UL)
        return gameTimerList.Count - 1;
      return loadingTimersIdIndexesDict.ContainsKey(timerID) ? loadingTimersIdIndexesDict[timerID] : (int) (timerID % uint.MaxValue);
    }

    public void RevertEventsCutTime(GameTime newTime)
    {
      for (int index = 0; index < gameTimerList.Count; ++index)
      {
        if (newTime.TotalValue < gameTimerList[index].EventsCutTime.TotalValue)
          gameTimerList[index].EventsCutTime = newTime;
      }
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      loadingTimersIdIndexesDict.Clear();
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "TimeContextName")
          contextName = childNode.InnerText;
        else if (childNode.Name == "CurrentGameTime")
          gameTime.LoadFromXML(childNode);
        else if (childNode.Name == "CurrentSolarTime")
          solarTime.LoadFromXML(childNode);
        else if (childNode.Name == "CurrentGameSpeed")
          gameTimeSpeedFactor = VMSaveLoadManager.ReadFloat(childNode);
        else if (childNode.Name == "CurrentSolarSpeed")
          solarTimeSpeedFactor = VMSaveLoadManager.ReadFloat(childNode);
        else if (childNode.Name == "GameTimerList")
          LoadTimers(childNode);
      }
      gameDayTime.CopyFrom(gameTime);
      gameDayTime.Days = 0;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "TimeContextName", contextName);
      SaveManagerUtility.SaveDynamicSerializable(writer, "CurrentGameTime", gameTime);
      SaveManagerUtility.SaveDynamicSerializable(writer, "CurrentSolarTime", solarTime);
      SaveManagerUtility.Save(writer, "CurrentGameSpeed", gameTimeSpeedFactor);
      SaveManagerUtility.Save(writer, "CurrentSolarSpeed", solarTimeSpeedFactor);
      SaveTimers(writer);
    }

    private void SaveTimers(IDataWriter writer)
    {
      List<GameTimer> gameTimerList = new List<GameTimer>();
      for (int index = 0; index < this.gameTimerList.Count; ++index)
      {
        if (this.gameTimerList[index].GTType != EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE && this.gameTimerList[index].Active)
          gameTimerList.Add(this.gameTimerList[index]);
      }
      SaveManagerUtility.SaveDynamicSerializableList(writer, "GameTimerList", gameTimerList);
    }

    private void LoadTimers(XmlElement xmlNode)
    {
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        GameTimer gameTimer = new GameTimer();
        gameTimer.LoadFromXML(childNode);
        if (gameTimer.GTType != EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE && gameTimer.Active)
        {
          gameTimerList.Add(gameTimer);
          OnNewTimer(gameTimerList.Count - 1);
          if (loadingTimersIdIndexesDict.ContainsKey(gameTimer.TimerGuid))
            Logger.AddError(string.Format("SaveLoad error: loading timer id {0} dublication", gameTimer.TimerGuid));
          else
            loadingTimersIdIndexesDict.Add(gameTimer.TimerGuid, gameTimerList.Count - 1);
        }
      }
    }

    public void Clear()
    {
      timerPriorityList.Clear();
      Init();
    }

    public static bool UpdateMode
    {
      get => isUpdateMode;
      set => isUpdateMode = value;
    }

    public void OnModify()
    {
    }

    private void Init()
    {
      contextName = gameMode.Name;
      isMain = contextName == "common";
      gameTime = gameMode.StartGameTime == null ? new GameTime() : new GameTime(gameMode.StartGameTime);
      solarTime = gameMode.StartSolarTime == null ? new GameTime() : new GameTime(gameMode.StartSolarTime);
      gameDayTime = new GameTime(gameTime);
      gameDayTime.Days = 0;
      gameTimeSpeedFactor = gameMode.GameTimeSpeed;
      solarTimeSpeedFactor = gameMode.SolarTimeSpeed;
      gameTimerList.Clear();
      freeTimersIndexesStack.Clear();
      if (!IsMain)
        return;
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      if (gameMode.StartSolarTime != null)
      {
        service.SolarTime = new TimeSpan(solarTime.Days, solarTime.Hours, solarTime.Minutes, solarTime.Seconds);
        service.SolarTimeFactor = solarTimeSpeedFactor;
      }
      if (gameMode.StartGameTime == null)
        return;
      service.GameTime = new TimeSpan(gameTime.Days, gameTime.Hours, gameTime.Minutes, gameTime.Seconds);
      service.GameTimeFactor = gameTimeSpeedFactor;
    }

    private void OnNewTimer(int timerIndex)
    {
      if (TIMERS_LIST_PROCESSIN_ON)
        Logger.AddError("Invalid timer creation: timers are being processed now at " + gameMode.Name + " game mode! Probably incorrect non-queue event rising");
      else
        timerPriorityList.Add(gameTimerList[timerIndex].CalculateTimerPriority() + (ulong) timerIndex, timerIndex);
    }
  }
}
