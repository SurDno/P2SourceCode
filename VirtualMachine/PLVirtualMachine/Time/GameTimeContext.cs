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
using System;
using System.Collections.Generic;
using System.Xml;

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
    private Dictionary<ulong, int> loadingTimersIdIndexesDict = new Dictionary<ulong, int>((IEqualityComparer<ulong>) UlongComparer.Instance);
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
        this.Init();
      }
    }

    public IGameMode StaticGameMode => this.gameMode;

    public bool IsMain => this.isMain;

    public GameTime GameTime
    {
      get => this.gameTime;
      set
      {
        if (value == null)
        {
          Logger.AddError("Invalid game time value received at game time context " + this.Name);
        }
        else
        {
          this.gameTime = new GameTime(value);
          this.gameDayTime.CopyFrom((object) this.gameTime);
          this.gameDayTime.Days = (ushort) 0;
        }
      }
    }

    public GameTime GameDayTime => this.gameDayTime;

    public GameTime SolarTime
    {
      get => this.solarTime;
      set => this.solarTime = value;
    }

    public float GameTimeSpeed
    {
      get => this.gameTimeSpeedFactor;
      set => this.gameTimeSpeedFactor = value;
    }

    public float SolarTimeSpeed
    {
      get => this.solarTimeSpeedFactor;
      set => this.solarTimeSpeedFactor = value;
    }

    public string Name => this.contextName;

    public void Update(double fDtime, bool bForceEvents = true)
    {
      this.gameTime.Process(fDtime);
      ServiceLocator.GetService<ITimeService>();
      GameTimeContext.TIMERS_LIST_PROCESSIN_ON = true;
      foreach (KeyValuePair<ulong, int> timerPriority in this.timerPriorityList)
      {
        int index = timerPriority.Value;
        if (this.gameTimerList[index].Active)
        {
          if (!bForceEvents)
            this.gameTimerList[index].EventsCutTime = this.gameTime;
          if (this.gameTimerList[index].GTType == EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE)
            this.gameTimerList[index].Check(this.gameTime);
          else
            this.gameTimerList[index].Check(fDtime, bForceEvents);
        }
      }
      GameTimeContext.TIMERS_LIST_PROCESSIN_ON = false;
      this.gameDayTime.CopyFrom((object) this.gameTime);
      this.gameDayTime.Days = (ushort) 0;
      this.currProcTimerIndex = -1;
    }

    public GameTimer StartTimer(
      EGameTimerType timerType,
      Guid initiatorFSMGuid,
      ulong stateId,
      GameTime targetTime,
      bool bIsRepeat)
    {
      if (this.freeTimersIndexesStack.Count > 0)
      {
        int index = this.freeTimersIndexesStack.Pop();
        if (index < 0 || index >= this.gameTimerList.Count)
        {
          Logger.AddError(string.Format("Invalid free timer index {0}, timers list count = {1} at {2}", (object) index, (object) this.gameTimerList.Count, (object) DynamicFSM.CurrentStateInfo));
        }
        else
        {
          if (this.currProcTimerIndex < 0 || index < this.currProcTimerIndex)
          {
            this.gameTimerList[index].Start(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat);
            return this.gameTimerList[index];
          }
          this.freeTimersIndexesStack.Push(index);
        }
      }
      int count = this.gameTimerList.Count;
      GameTimer gameTimer = new GameTimer(count);
      gameTimer.Start(timerType, initiatorFSMGuid, stateId, targetTime, bIsRepeat);
      this.gameTimerList.Add(gameTimer);
      this.OnNewTimer(count);
      int dynamicFsmObjectsCount = VirtualMachine.Instance.DynamicFSMObjectsCount;
      if (VirtualMachine.Instance.IsLoaded && this.gameTimerList.Count > dynamicFsmObjectsCount * 4)
        Logger.AddError("Warning: on start game timer too many timers created!!! at " + DynamicFSM.CurrentStateInfo);
      return gameTimer;
    }

    public void StopTimer(ulong timerID)
    {
      int timerIndexById = this.GetTimerIndexByID(timerID);
      if (timerIndexById < 0 || timerIndexById >= this.gameTimerList.Count)
      {
        Logger.AddWarning(string.Format("Invalid stopping game timer index {0} at {1}", (object) timerIndexById, (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        if ((long) this.gameTimerList[timerIndexById].TimerGuid != (long) timerID)
          return;
        this.gameTimerList[timerIndexById].Stop();
        if (this.freeTimersIndexesStack.Contains(timerIndexById))
          return;
        this.freeTimersIndexesStack.Push(timerIndexById);
      }
    }

    private int GetTimerIndexByID(ulong timerID)
    {
      if (timerID == 0UL)
        return this.gameTimerList.Count - 1;
      return this.loadingTimersIdIndexesDict.ContainsKey(timerID) ? this.loadingTimersIdIndexesDict[timerID] : (int) (timerID % (ulong) uint.MaxValue);
    }

    public void RevertEventsCutTime(GameTime newTime)
    {
      for (int index = 0; index < this.gameTimerList.Count; ++index)
      {
        if (newTime.TotalValue < this.gameTimerList[index].EventsCutTime.TotalValue)
          this.gameTimerList[index].EventsCutTime = newTime;
      }
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      this.loadingTimersIdIndexesDict.Clear();
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "TimeContextName")
          this.contextName = childNode.InnerText;
        else if (childNode.Name == "CurrentGameTime")
          this.gameTime.LoadFromXML(childNode);
        else if (childNode.Name == "CurrentSolarTime")
          this.solarTime.LoadFromXML(childNode);
        else if (childNode.Name == "CurrentGameSpeed")
          this.gameTimeSpeedFactor = VMSaveLoadManager.ReadFloat((XmlNode) childNode);
        else if (childNode.Name == "CurrentSolarSpeed")
          this.solarTimeSpeedFactor = VMSaveLoadManager.ReadFloat((XmlNode) childNode);
        else if (childNode.Name == "GameTimerList")
          this.LoadTimers(childNode);
      }
      this.gameDayTime.CopyFrom((object) this.gameTime);
      this.gameDayTime.Days = (ushort) 0;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "TimeContextName", this.contextName);
      SaveManagerUtility.SaveDynamicSerializable(writer, "CurrentGameTime", (ISerializeStateSave) this.gameTime);
      SaveManagerUtility.SaveDynamicSerializable(writer, "CurrentSolarTime", (ISerializeStateSave) this.solarTime);
      SaveManagerUtility.Save(writer, "CurrentGameSpeed", this.gameTimeSpeedFactor);
      SaveManagerUtility.Save(writer, "CurrentSolarSpeed", this.solarTimeSpeedFactor);
      this.SaveTimers(writer);
    }

    private void SaveTimers(IDataWriter writer)
    {
      List<GameTimer> gameTimerList = new List<GameTimer>();
      for (int index = 0; index < this.gameTimerList.Count; ++index)
      {
        if (this.gameTimerList[index].GTType != EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE && this.gameTimerList[index].Active)
          gameTimerList.Add(this.gameTimerList[index]);
      }
      SaveManagerUtility.SaveDynamicSerializableList<GameTimer>(writer, "GameTimerList", gameTimerList);
    }

    private void LoadTimers(XmlElement xmlNode)
    {
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        GameTimer gameTimer = new GameTimer();
        gameTimer.LoadFromXML(childNode);
        if (gameTimer.GTType != EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE && gameTimer.Active)
        {
          this.gameTimerList.Add(gameTimer);
          this.OnNewTimer(this.gameTimerList.Count - 1);
          if (this.loadingTimersIdIndexesDict.ContainsKey(gameTimer.TimerGuid))
            Logger.AddError(string.Format("SaveLoad error: loading timer id {0} dublication", (object) gameTimer.TimerGuid));
          else
            this.loadingTimersIdIndexesDict.Add(gameTimer.TimerGuid, this.gameTimerList.Count - 1);
        }
      }
    }

    public void Clear()
    {
      this.timerPriorityList.Clear();
      this.Init();
    }

    public static bool UpdateMode
    {
      get => GameTimeContext.isUpdateMode;
      set => GameTimeContext.isUpdateMode = value;
    }

    public void OnModify()
    {
    }

    private void Init()
    {
      this.contextName = this.gameMode.Name;
      this.isMain = this.contextName == "common";
      this.gameTime = this.gameMode.StartGameTime == null ? new GameTime() : new GameTime(this.gameMode.StartGameTime);
      this.solarTime = this.gameMode.StartSolarTime == null ? new GameTime() : new GameTime(this.gameMode.StartSolarTime);
      this.gameDayTime = new GameTime(this.gameTime);
      this.gameDayTime.Days = (ushort) 0;
      this.gameTimeSpeedFactor = this.gameMode.GameTimeSpeed;
      this.solarTimeSpeedFactor = this.gameMode.SolarTimeSpeed;
      this.gameTimerList.Clear();
      this.freeTimersIndexesStack.Clear();
      if (!this.IsMain)
        return;
      ITimeService service = ServiceLocator.GetService<ITimeService>();
      if (this.gameMode.StartSolarTime != null)
      {
        service.SolarTime = new TimeSpan((int) this.solarTime.Days, (int) this.solarTime.Hours, (int) this.solarTime.Minutes, (int) this.solarTime.Seconds);
        service.SolarTimeFactor = this.solarTimeSpeedFactor;
      }
      if (this.gameMode.StartGameTime == null)
        return;
      service.GameTime = new TimeSpan((int) this.gameTime.Days, (int) this.gameTime.Hours, (int) this.gameTime.Minutes, (int) this.gameTime.Seconds);
      service.GameTimeFactor = this.gameTimeSpeedFactor;
    }

    private void OnNewTimer(int timerIndex)
    {
      if (GameTimeContext.TIMERS_LIST_PROCESSIN_ON)
        Logger.AddError("Invalid timer creation: timers are being processed now at " + this.gameMode.Name + " game mode! Probably incorrect non-queue event rising");
      else
        this.timerPriorityList.Add(this.gameTimerList[timerIndex].CalculateTimerPriority() + (ulong) timerIndex, timerIndex);
    }
  }
}
