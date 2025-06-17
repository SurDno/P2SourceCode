using System;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Dynamic;

namespace PLVirtualMachine.Time
{
  public class GameTimer : ISerializeStateSave, IDynamicLoadSerializable
  {
    private GameTime targetTime = new();
    private GameTime currentTime = new();
    private bool isRepeat;
    private bool isActive;
    private Guid fsmObjGuid;
    private ulong stateGuid;
    private int timerIndex;
    private EGameTimerType gtType;
    private double remainder;
    private int timerSerial;
    private GameTime eventsCutTime = new();
    public static int CurrTimerSerialNumber;

    public GameTimer()
    {
    }

    public GameTimer(int index) => timerIndex = index;

    public event Action<GameTimer> OnEvent;

    public void Start(
      EGameTimerType timerType,
      Guid initiatorFSMGuid,
      ulong stateId,
      GameTime targetTime,
      bool bRepeat)
    {
      gtType = timerType;
      this.targetTime = targetTime;
      currentTime.Init();
      eventsCutTime = new GameTime(currentTime);
      isRepeat = bRepeat && timerType != 0;
      fsmObjGuid = initiatorFSMGuid;
      isActive = true;
      timerSerial = CreateTimerSerial();
      if (timerType == EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE)
        return;
      Logger.AddWarning(string.Format("Test timer started, timer id={0}, hours={1}, minutes={2}, seconds={3} at {4}", TimerGuid, targetTime.Hours, targetTime.Minutes, targetTime.Seconds, DynamicFSM.CurrentStateInfo));
    }

    public void Stop()
    {
      isRepeat = false;
      isActive = false;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.SaveDynamicSerializable(writer, "TargetTime", targetTime);
      SaveManagerUtility.SaveDynamicSerializable(writer, "CurrentTime", currentTime);
      SaveManagerUtility.SaveDynamicSerializable(writer, "EventsCutTime", eventsCutTime);
      SaveManagerUtility.Save(writer, "Repeat", isRepeat);
      SaveManagerUtility.Save(writer, "FsmObjGuid", fsmObjGuid);
      SaveManagerUtility.Save(writer, "Active", isActive);
      SaveManagerUtility.SaveEnum(writer, "GTType", gtType);
      SaveManagerUtility.Save(writer, "StateGuid", stateGuid);
      SaveManagerUtility.Save(writer, "TimerSerial", timerSerial);
      SaveManagerUtility.Save(writer, "TimerIndex", timerIndex);
      SaveManagerUtility.Save(writer, "Remainder", remainder);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      try
      {
        targetTime = new GameTime();
        currentTime = new GameTime();
        eventsCutTime = new GameTime();
        for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
        {
          if (xmlNode.ChildNodes[i].Name == "TargetTime")
            VMSaveLoadManager.LoadDynamicSerializable((XmlElement) xmlNode.ChildNodes[i], targetTime);
          else if (xmlNode.ChildNodes[i].Name == "CurrentTime")
            VMSaveLoadManager.LoadDynamicSerializable((XmlElement) xmlNode.ChildNodes[i], currentTime);
          else if (xmlNode.ChildNodes[i].Name == "EventsCutTime")
            VMSaveLoadManager.LoadDynamicSerializable((XmlElement) xmlNode.ChildNodes[i], eventsCutTime);
          else if (xmlNode.ChildNodes[i].Name == "Repeat")
            isRepeat = VMSaveLoadManager.ReadBool(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "FsmObjGuid")
            fsmObjGuid = VMSaveLoadManager.ReadGuid(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "Active")
            isActive = VMSaveLoadManager.ReadBool(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "GTType")
            gtType = VMSaveLoadManager.ReadEnum<EGameTimerType>(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "StateGuid")
            stateGuid = VMSaveLoadManager.ReadUlong(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "TimerSerial")
          {
            timerSerial = VMSaveLoadManager.ReadInt(xmlNode.ChildNodes[i]);
            if (timerSerial > CurrTimerSerialNumber)
              CurrTimerSerialNumber = timerSerial;
          }
          else if (xmlNode.ChildNodes[i].Name == "TimerIndex")
            timerIndex = VMSaveLoadManager.ReadInt(xmlNode.ChildNodes[i]);
          else if (xmlNode.ChildNodes[i].Name == "Remainder")
            remainder = VMSaveLoadManager.ReadFloat(xmlNode.ChildNodes[i]);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Saveload error: GameTimer loading error : {0}", ex));
      }
    }

    public bool IsRepeat => isRepeat;

    public EGameTimerType GTType => gtType;

    public Guid FSMGuid => fsmObjGuid;

    public bool Active => isActive;

    public ulong TimerGuid => (ulong) timerSerial * uint.MaxValue + (ulong) timerIndex;

    public object ResultMessageValue
    {
      get
      {
        if (GTType == EGameTimerType.GAME_TIMER_TYPE_RELATIVE_GLOBAL)
          return TimerGuid;
        return GTType == EGameTimerType.GAME_TIMER_TYPE_RELATIVE_LOCAL ? stateGuid : (object) 0;
      }
    }

    public bool Check(GameTime currTime, bool needForceEvent = true)
    {
      if (currTime.TotalValue - targetTime.TotalValue <= 0.0)
        return false;
      OnGameTime(currTime.TotalValue, needForceEvent);
      return true;
    }

    public bool IsElapsed(GameTime currTime) => currTime.TotalValue > targetTime.TotalValue;

    public bool Check(double deltaTime, bool needForceEvent = true)
    {
      currentTime.Process(deltaTime);
      return Check(currentTime, needForceEvent);
    }

    public GameTime TargetTime => targetTime;

    public GameTime EventsCutTime
    {
      get => eventsCutTime;
      set
      {
        if (value != null)
          eventsCutTime = new GameTime(value);
        else
          eventsCutTime = new GameTime();
      }
    }

    private void OnGameTime(double timeElapsed, bool needForceEvents = true)
    {
      int iCount = 1;
      if (((!IsRepeat ? 0 : (gtType != 0 ? 1 : 0)) & (needForceEvents ? 1 : 0)) != 0)
      {
        double totalValue = targetTime.TotalValue;
        double d = remainder + timeElapsed / totalValue;
        iCount = (int) Math.Floor(d);
        remainder = d - iCount;
      }
      if (needForceEvents)
      {
        if (gtType == EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE && targetTime.TotalSeconds >= eventsCutTime.TotalSeconds)
        {
          Action<GameTimer> onEvent = OnEvent;
          if (onEvent != null)
            onEvent(this);
        }
        else if (gtType != EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE)
          VirtualMachine.Instance.GameRootFsm.RaiseTimerEvent(this, iCount);
      }
      if (IsRepeat)
        currentTime.Init();
      else
        Stop();
    }

    private static int CreateTimerSerial()
    {
      ++CurrTimerSerialNumber;
      return CurrTimerSerialNumber;
    }

    public ulong CalculateTimerPriority()
    {
      return GTType == EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE ? TargetTime.TotalSeconds * 1000000UL : (TargetTime.TotalSeconds - currentTime.TotalSeconds) * 1000000UL;
    }
  }
}
