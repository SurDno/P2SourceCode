using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Dynamic;
using System;
using System.Xml;

namespace PLVirtualMachine.Time
{
  public class GameTimer : ISerializeStateSave, IDynamicLoadSerializable
  {
    private GameTime targetTime = new GameTime();
    private GameTime currentTime = new GameTime();
    private bool isRepeat;
    private bool isActive;
    private Guid fsmObjGuid;
    private ulong stateGuid;
    private int timerIndex;
    private EGameTimerType gtType;
    private double remainder;
    private int timerSerial;
    private GameTime eventsCutTime = new GameTime();
    public static int CurrTimerSerialNumber;

    public GameTimer()
    {
    }

    public GameTimer(int index) => this.timerIndex = index;

    public event Action<GameTimer> OnEvent;

    public void Start(
      EGameTimerType timerType,
      Guid initiatorFSMGuid,
      ulong stateId,
      GameTime targetTime,
      bool bRepeat)
    {
      this.gtType = timerType;
      this.targetTime = targetTime;
      this.currentTime.Init();
      this.eventsCutTime = new GameTime(this.currentTime);
      this.isRepeat = bRepeat && timerType != 0;
      this.fsmObjGuid = initiatorFSMGuid;
      this.isActive = true;
      this.timerSerial = GameTimer.CreateTimerSerial();
      if (timerType == EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE)
        return;
      Logger.AddWarning(string.Format("Test timer started, timer id={0}, hours={1}, minutes={2}, seconds={3} at {4}", (object) this.TimerGuid, (object) targetTime.Hours, (object) targetTime.Minutes, (object) targetTime.Seconds, (object) DynamicFSM.CurrentStateInfo));
    }

    public void Stop()
    {
      this.isRepeat = false;
      this.isActive = false;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.SaveDynamicSerializable(writer, "TargetTime", (ISerializeStateSave) this.targetTime);
      SaveManagerUtility.SaveDynamicSerializable(writer, "CurrentTime", (ISerializeStateSave) this.currentTime);
      SaveManagerUtility.SaveDynamicSerializable(writer, "EventsCutTime", (ISerializeStateSave) this.eventsCutTime);
      SaveManagerUtility.Save(writer, "Repeat", this.isRepeat);
      SaveManagerUtility.Save(writer, "FsmObjGuid", this.fsmObjGuid);
      SaveManagerUtility.Save(writer, "Active", this.isActive);
      SaveManagerUtility.SaveEnum<EGameTimerType>(writer, "GTType", this.gtType);
      SaveManagerUtility.Save(writer, "StateGuid", this.stateGuid);
      SaveManagerUtility.Save(writer, "TimerSerial", this.timerSerial);
      SaveManagerUtility.Save(writer, "TimerIndex", this.timerIndex);
      SaveManagerUtility.Save(writer, "Remainder", this.remainder);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      try
      {
        this.targetTime = new GameTime();
        this.currentTime = new GameTime();
        this.eventsCutTime = new GameTime();
        for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
        {
          if (xmlNode.ChildNodes[i].Name == "TargetTime")
            VMSaveLoadManager.LoadDynamicSerializable((XmlElement) xmlNode.ChildNodes[i], (IDynamicLoadSerializable) this.targetTime);
          else if (xmlNode.ChildNodes[i].Name == "CurrentTime")
            VMSaveLoadManager.LoadDynamicSerializable((XmlElement) xmlNode.ChildNodes[i], (IDynamicLoadSerializable) this.currentTime);
          else if (xmlNode.ChildNodes[i].Name == "EventsCutTime")
            VMSaveLoadManager.LoadDynamicSerializable((XmlElement) xmlNode.ChildNodes[i], (IDynamicLoadSerializable) this.eventsCutTime);
          else if (xmlNode.ChildNodes[i].Name == "Repeat")
            this.isRepeat = VMSaveLoadManager.ReadBool(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "FsmObjGuid")
            this.fsmObjGuid = VMSaveLoadManager.ReadGuid(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "Active")
            this.isActive = VMSaveLoadManager.ReadBool(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "GTType")
            this.gtType = VMSaveLoadManager.ReadEnum<EGameTimerType>(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "StateGuid")
            this.stateGuid = VMSaveLoadManager.ReadUlong(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "TimerSerial")
          {
            this.timerSerial = VMSaveLoadManager.ReadInt(xmlNode.ChildNodes[i]);
            if (this.timerSerial > GameTimer.CurrTimerSerialNumber)
              GameTimer.CurrTimerSerialNumber = this.timerSerial;
          }
          else if (xmlNode.ChildNodes[i].Name == "TimerIndex")
            this.timerIndex = VMSaveLoadManager.ReadInt(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "Remainder")
            this.remainder = (double) VMSaveLoadManager.ReadFloat(xmlNode.ChildNodes[i]);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Saveload error: GameTimer loading error : {0}", (object) ex.ToString()));
      }
    }

    public bool IsRepeat => this.isRepeat;

    public EGameTimerType GTType => this.gtType;

    public Guid FSMGuid => this.fsmObjGuid;

    public bool Active => this.isActive;

    public ulong TimerGuid
    {
      get => (ulong) this.timerSerial * (ulong) uint.MaxValue + (ulong) this.timerIndex;
    }

    public object ResultMessageValue
    {
      get
      {
        if (this.GTType == EGameTimerType.GAME_TIMER_TYPE_RELATIVE_GLOBAL)
          return (object) this.TimerGuid;
        return this.GTType == EGameTimerType.GAME_TIMER_TYPE_RELATIVE_LOCAL ? (object) this.stateGuid : (object) 0;
      }
    }

    public bool Check(GameTime currTime, bool needForceEvent = true)
    {
      if (currTime.TotalValue - this.targetTime.TotalValue <= 0.0)
        return false;
      this.OnGameTime(currTime.TotalValue, needForceEvent);
      return true;
    }

    public bool IsElapsed(GameTime currTime) => currTime.TotalValue > this.targetTime.TotalValue;

    public bool Check(double deltaTime, bool needForceEvent = true)
    {
      this.currentTime.Process(deltaTime);
      return this.Check(this.currentTime, needForceEvent);
    }

    public GameTime TargetTime => this.targetTime;

    public GameTime EventsCutTime
    {
      get => this.eventsCutTime;
      set
      {
        if (value != null)
          this.eventsCutTime = new GameTime(value);
        else
          this.eventsCutTime = new GameTime();
      }
    }

    private void OnGameTime(double timeElapsed, bool needForceEvents = true)
    {
      int iCount = 1;
      if (((!this.IsRepeat ? 0 : (this.gtType != 0 ? 1 : 0)) & (needForceEvents ? 1 : 0)) != 0)
      {
        double totalValue = this.targetTime.TotalValue;
        double d = this.remainder + timeElapsed / totalValue;
        iCount = (int) Math.Floor(d);
        this.remainder = d - (double) iCount;
      }
      if (needForceEvents)
      {
        if (this.gtType == EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE && this.targetTime.TotalSeconds >= this.eventsCutTime.TotalSeconds)
        {
          Action<GameTimer> onEvent = this.OnEvent;
          if (onEvent != null)
            onEvent(this);
        }
        else if (this.gtType != EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE)
          VirtualMachine.Instance.GameRootFsm.RaiseTimerEvent(this, iCount);
      }
      if (this.IsRepeat)
        this.currentTime.Init();
      else
        this.Stop();
    }

    private static int CreateTimerSerial()
    {
      ++GameTimer.CurrTimerSerialNumber;
      return GameTimer.CurrTimerSerialNumber;
    }

    public ulong CalculateTimerPriority()
    {
      return this.GTType == EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE ? this.TargetTime.TotalSeconds * 1000000UL : (this.TargetTime.TotalSeconds - this.currentTime.TotalSeconds) * 1000000UL;
    }
  }
}
