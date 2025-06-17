using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Time;

namespace PLVirtualMachine.Dynamic
{
  public class DynamicEventBody : IDependedEventRef, ISerializeStateSave, IDynamicLoadSerializable
  {
    private EEventUpdateMode eventUpdateMode;
    private VMPartCondition eventCondition;
    private VMParameter eventParameter;
    private GameTime eventTime;
    private bool repeated;
    private List<GameTimer> checkRaisingTimePointEvents = [];
    private List<VMParameter> checkRaisingParams = [];
    private int memConditionResult = -1;
    private IGameMode gameTimeContext;
    private VMEntity parentEntity;
    private bool forceCheck;
    private OnEventBodyRise fnOnBodyRise;
    private INamed parent;
    private List<ulong> saveLoadingCheckRaisingObjectsID = [];

    public DynamicEventBody(
      VMPartCondition condition,
      INamed parent,
      IGameMode gameTimeContext,
      OnEventBodyRise onBodyRise,
      bool repeated,
      VMEntity parentEntity = null)
    {
      eventCondition = condition;
      eventParameter = null;
      eventTime = null;
      this.parent = parent;
      this.gameTimeContext = gameTimeContext;
      this.parentEntity = parentEntity;
      fnOnBodyRise = onBodyRise;
      this.repeated = repeated;
      Init();
    }

    public DynamicEventBody(
      VMParameter parameter,
      INamed parent,
      IGameMode gameTimeContext,
      OnEventBodyRise onBodyRise,
      bool repeated,
      VMEntity parentEntity = null)
    {
      eventParameter = parameter;
      eventCondition = null;
      eventTime = null;
      this.parent = parent;
      this.gameTimeContext = gameTimeContext;
      this.parentEntity = parentEntity;
      fnOnBodyRise = onBodyRise;
      this.repeated = repeated;
      Init();
    }

    public DynamicEventBody(
      GameTime time,
      INamed parent,
      IGameMode gameTimeContext,
      OnEventBodyRise onBodyRise,
      bool repeated,
      VMEntity parentEntity = null)
    {
      eventTime = time;
      eventParameter = null;
      eventCondition = null;
      this.parent = parent;
      this.gameTimeContext = gameTimeContext;
      this.parentEntity = parentEntity;
      fnOnBodyRise = onBodyRise;
      this.repeated = false;
      Init();
    }

    public EEventRaisingType RaisingType
    {
      get
      {
        if (eventCondition != null)
          return EEventRaisingType.EVENT_RAISING_TYPE_CONDITION;
        if (eventParameter != null)
          return EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE;
        if (eventTime != null)
          return EEventRaisingType.EVENT_RAISING_TYPE_TIME;
        Logger.AddError(string.Format("Invalid dynamic event body at {0}!", parent.Name));
        return EEventRaisingType.EVENT_RAISING_TYPE_CONDITION;
      }
    }

    public EEventUpdateMode EventUpdateMode => eventUpdateMode;

    public bool NeedUpdate()
    {
      if (eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_ALWAYS)
        return true;
      if (!forceCheck && memConditionResult >= 0)
        return false;
      forceCheck = false;
      return true;
    }

    public void Think()
    {
      if (eventCondition != null)
      {
        bool conditionResult = CalculateConditionResult();
        bool flag = false;
        if (memConditionResult > -1)
          flag = memConditionResult > 0;
        if (flag != conditionResult)
          fnOnBodyRise(conditionResult, EEventRaisingMode.ERM_ADD_TO_QUEUE);
        else if (conditionResult && forceCheck)
          fnOnBodyRise(conditionResult, EEventRaisingMode.ERM_ADD_TO_QUEUE);
        memConditionResult = conditionResult ? 1 : 0;
      }
      forceCheck = false;
    }

    public bool CalculateConditionResult()
    {
      DynamicFSM dynamicObjContext = VirtualMachine.Instance.GameRootFsm;
      if (parentEntity != null)
        dynamicObjContext = parentEntity.GetFSM();
      return eventCondition != null && ExpressionUtility.CalculateConditionResult(eventCondition, dynamicObjContext, parent);
    }

    public void ClearSubscribtions()
    {
      for (int index = 0; index < checkRaisingTimePointEvents.Count; ++index)
        checkRaisingTimePointEvents[index].OnEvent -= OnOutTimer;
      checkRaisingTimePointEvents.Clear();
      for (int index = 0; index < checkRaisingParams.Count; ++index)
        DynamicParameterUtility.RemoveDependedEvent(checkRaisingParams[index].BaseGuid, this);
      checkRaisingParams.Clear();
    }

    public void OnParamUpdate(bool bValueChange, DynamicParameter dynParamInstance)
    {
      if (parentEntity != null && !parentEntity.Instantiated)
        return;
      if (RaisingType == EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE)
      {
        fnOnBodyRise(bValueChange, DynamicFSM.EventProcessingMode);
      }
      else
      {
        if (!bValueChange)
          return;
        Think();
      }
    }

    public void OnOutTimer(GameTimer gtEvent)
    {
      if (RaisingType == EEventRaisingType.EVENT_RAISING_TYPE_TIME)
      {
        fnOnBodyRise(gtEvent.TargetTime, EEventRaisingMode.ERM_ADD_TO_QUEUE);
      }
      else
      {
        forceCheck = true;
        Think();
        if (checkRaisingTimePointEvents.Count == 0)
        {
          if (eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT)
            eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_NEVER;
          else if (eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_COMBINE)
            eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_PARAM_CHANGE;
        }
      }
      if (repeated)
        return;
      checkRaisingTimePointEvents.Remove(gtEvent);
      gtEvent.OnEvent -= OnOutTimer;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "CurrentConditionResult", memConditionResult);
      SaveManagerUtility.SaveEnum(writer, "EventUpdateMode", eventUpdateMode);
      if (RaisingType != EEventRaisingType.EVENT_RAISING_TYPE_CONDITION && RaisingType != EEventRaisingType.EVENT_RAISING_TYPE_TIME)
        return;
      SaveManagerUtility.SaveList(writer, "DependedObjectsInfo", checkRaisingTimePointEvents.Select(o => o.TimerGuid));
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      saveLoadingCheckRaisingObjectsID.Clear();
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode1.Name == "CurrentConditionResult")
          memConditionResult = VMSaveLoadManager.ReadInt(childNode1);
        else if (childNode1.Name == "EventUpdateMode")
          eventUpdateMode = VMSaveLoadManager.ReadEnum<EEventUpdateMode>(childNode1);
        else if (childNode1.Name == "DependedObjectsInfo")
        {
          try
          {
            foreach (XmlNode childNode2 in childNode1.ChildNodes)
              saveLoadingCheckRaisingObjectsID.Add(VMSaveLoadManager.ReadUlong(childNode2));
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("SaveLoad error: cannot load raising timers guids list from node {0}  in event {1}, error: {2}", childNode1.InnerText, parent.Name, ex));
          }
        }
      }
    }

    public void AfterSaveLoading()
    {
      try
      {
        GameTimeContext gameTimeContext = GameTimeManager.CurrentGameTimeContext;
        if (this.gameTimeContext != null && "" != this.gameTimeContext.Name)
        {
          if (GameTimeManager.GameTimeContexts.ContainsKey(this.gameTimeContext.Name))
          {
            gameTimeContext = GameTimeManager.GameTimeContexts[this.gameTimeContext.Name];
          }
          else
          {
            Logger.AddError(string.Format("SaveLoad error at {0} event afterloading: event game time context with name {1} not found", parent.Name, this.gameTimeContext.Name));
            return;
          }
        }
        int count = checkRaisingTimePointEvents.Count;
        if (count > 0)
        {
          for (int index = count - 1; index >= 0; --index)
          {
            GameTimer raisingTimePointEvent = checkRaisingTimePointEvents[index];
            if (raisingTimePointEvent.IsElapsed(gameTimeContext.GameTime))
            {
              gameTimeContext.StopTimer(raisingTimePointEvent.TimerGuid);
              checkRaisingTimePointEvents.RemoveAt(index);
            }
          }
        }
        if (RaisingType != EEventRaisingType.EVENT_RAISING_TYPE_CONDITION || eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_NEVER)
          return;
        memConditionResult = CalculateConditionResult() ? 1 : 0;
        if (eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_ALWAYS)
          return;
        if (checkRaisingTimePointEvents.Count > 0 && checkRaisingParams.Count > 0)
          eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_COMBINE;
        else if (checkRaisingTimePointEvents.Count > 0)
          eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT;
        else
          eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_PARAM_CHANGE;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("SaveLoad error at {0} event afterloading, error: {1}", parent.Name, ex));
      }
    }

    private void Init()
    {
      try
      {
        eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_ALWAYS;
        if (RaisingType == EEventRaisingType.EVENT_RAISING_TYPE_CONDITION)
        {
          if (eventCondition == null)
            Logger.AddError(string.Format("Invalid dynamic event body at {0}!", parent.Name));
          else if (eventCondition.IsConstant())
          {
            eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_NEVER;
          }
          else
          {
            foreach (GameTime raisingTimePoint in eventCondition.GetCheckRaisingTimePoints())
            {
              GameTimer gameTimer = GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE, parentEntity.EngineGuid, 0UL, raisingTimePoint, repeated, gameTimeContext.Name);
              gameTimer.OnEvent += OnOutTimer;
              checkRaisingTimePointEvents.Add(gameTimer);
            }
            if (checkRaisingTimePointEvents.Count > 0)
              eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT;
            checkRaisingParams.Clear();
            checkRaisingParams.AddRange(eventCondition.GetCheckRaisingParams());
            if (eventCondition.GetCheckRaisingFunctions().Count > checkRaisingTimePointEvents.Count)
            {
              eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_ALWAYS;
            }
            else
            {
              if (checkRaisingParams.Count <= 0)
                return;
              eventUpdateMode = eventUpdateMode != EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT ? EEventUpdateMode.EVENT_UPDATE_MODE_PARAM_CHANGE : EEventUpdateMode.EVENT_UPDATE_MODE_COMBINE;
              for (int index = 0; index < checkRaisingParams.Count; ++index)
                DynamicParameterUtility.AddDependedEvent(checkRaisingParams[index].BaseGuid, this);
            }
          }
        }
        else if (RaisingType == EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE)
        {
          DynamicParameterUtility.AddDependedEvent(eventParameter.BaseGuid, this);
          eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_PARAM_CHANGE;
        }
        else
        {
          if (RaisingType != EEventRaisingType.EVENT_RAISING_TYPE_TIME)
            return;
          GameTimer gameTimer = GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE, parentEntity.EngineGuid, 0UL, eventTime, repeated, gameTimeContext.Name);
          gameTimer.OnEvent += OnOutTimer;
          checkRaisingTimePointEvents.Add(gameTimer);
          eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT;
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot create event body at {0}, error: {1}", parent.Name, ex));
      }
    }

    public delegate void OnEventBodyRise(object newValue, EEventRaisingMode eventRaisingMode);
  }
}
