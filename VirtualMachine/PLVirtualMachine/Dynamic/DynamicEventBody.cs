// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.DynamicEventBody
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class DynamicEventBody : IDependedEventRef, ISerializeStateSave, IDynamicLoadSerializable
  {
    private EEventUpdateMode eventUpdateMode;
    private VMPartCondition eventCondition;
    private VMParameter eventParameter;
    private GameTime eventTime;
    private bool repeated;
    private List<GameTimer> checkRaisingTimePointEvents = new List<GameTimer>();
    private List<VMParameter> checkRaisingParams = new List<VMParameter>();
    private int memConditionResult = -1;
    private IGameMode gameTimeContext;
    private VMEntity parentEntity;
    private bool forceCheck;
    private DynamicEventBody.OnEventBodyRise fnOnBodyRise;
    private INamed parent;
    private List<ulong> saveLoadingCheckRaisingObjectsID = new List<ulong>();

    public DynamicEventBody(
      VMPartCondition condition,
      INamed parent,
      IGameMode gameTimeContext,
      DynamicEventBody.OnEventBodyRise onBodyRise,
      bool repeated,
      VMEntity parentEntity = null)
    {
      this.eventCondition = condition;
      this.eventParameter = (VMParameter) null;
      this.eventTime = (GameTime) null;
      this.parent = parent;
      this.gameTimeContext = gameTimeContext;
      this.parentEntity = parentEntity;
      this.fnOnBodyRise = onBodyRise;
      this.repeated = repeated;
      this.Init();
    }

    public DynamicEventBody(
      VMParameter parameter,
      INamed parent,
      IGameMode gameTimeContext,
      DynamicEventBody.OnEventBodyRise onBodyRise,
      bool repeated,
      VMEntity parentEntity = null)
    {
      this.eventParameter = parameter;
      this.eventCondition = (VMPartCondition) null;
      this.eventTime = (GameTime) null;
      this.parent = parent;
      this.gameTimeContext = gameTimeContext;
      this.parentEntity = parentEntity;
      this.fnOnBodyRise = onBodyRise;
      this.repeated = repeated;
      this.Init();
    }

    public DynamicEventBody(
      GameTime time,
      INamed parent,
      IGameMode gameTimeContext,
      DynamicEventBody.OnEventBodyRise onBodyRise,
      bool repeated,
      VMEntity parentEntity = null)
    {
      this.eventTime = time;
      this.eventParameter = (VMParameter) null;
      this.eventCondition = (VMPartCondition) null;
      this.parent = parent;
      this.gameTimeContext = gameTimeContext;
      this.parentEntity = parentEntity;
      this.fnOnBodyRise = onBodyRise;
      this.repeated = false;
      this.Init();
    }

    public EEventRaisingType RaisingType
    {
      get
      {
        if (this.eventCondition != null)
          return EEventRaisingType.EVENT_RAISING_TYPE_CONDITION;
        if (this.eventParameter != null)
          return EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE;
        if (this.eventTime != null)
          return EEventRaisingType.EVENT_RAISING_TYPE_TIME;
        Logger.AddError(string.Format("Invalid dynamic event body at {0}!", (object) this.parent.Name));
        return EEventRaisingType.EVENT_RAISING_TYPE_CONDITION;
      }
    }

    public EEventUpdateMode EventUpdateMode => this.eventUpdateMode;

    public bool NeedUpdate()
    {
      if (this.eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_ALWAYS)
        return true;
      if (!this.forceCheck && this.memConditionResult >= 0)
        return false;
      this.forceCheck = false;
      return true;
    }

    public void Think()
    {
      if (this.eventCondition != null)
      {
        bool conditionResult = this.CalculateConditionResult();
        bool flag = false;
        if (this.memConditionResult > -1)
          flag = this.memConditionResult > 0;
        if (flag != conditionResult)
          this.fnOnBodyRise((object) conditionResult, EEventRaisingMode.ERM_ADD_TO_QUEUE);
        else if (conditionResult && this.forceCheck)
          this.fnOnBodyRise((object) conditionResult, EEventRaisingMode.ERM_ADD_TO_QUEUE);
        this.memConditionResult = conditionResult ? 1 : 0;
      }
      this.forceCheck = false;
    }

    public bool CalculateConditionResult()
    {
      DynamicFSM dynamicObjContext = VirtualMachine.Instance.GameRootFsm;
      if (this.parentEntity != null)
        dynamicObjContext = this.parentEntity.GetFSM();
      return this.eventCondition != null && ExpressionUtility.CalculateConditionResult((ICondition) this.eventCondition, (IDynamicGameObjectContext) dynamicObjContext, this.parent);
    }

    public void ClearSubscribtions()
    {
      for (int index = 0; index < this.checkRaisingTimePointEvents.Count; ++index)
        this.checkRaisingTimePointEvents[index].OnEvent -= new Action<GameTimer>(this.OnOutTimer);
      this.checkRaisingTimePointEvents.Clear();
      for (int index = 0; index < this.checkRaisingParams.Count; ++index)
        DynamicParameterUtility.RemoveDependedEvent(this.checkRaisingParams[index].BaseGuid, (IDependedEventRef) this);
      this.checkRaisingParams.Clear();
    }

    public void OnParamUpdate(bool bValueChange, DynamicParameter dynParamInstance)
    {
      if (this.parentEntity != null && !this.parentEntity.Instantiated)
        return;
      if (this.RaisingType == EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE)
      {
        this.fnOnBodyRise((object) bValueChange, DynamicFSM.EventProcessingMode);
      }
      else
      {
        if (!bValueChange)
          return;
        this.Think();
      }
    }

    public void OnOutTimer(GameTimer gtEvent)
    {
      if (this.RaisingType == EEventRaisingType.EVENT_RAISING_TYPE_TIME)
      {
        this.fnOnBodyRise((object) gtEvent.TargetTime, EEventRaisingMode.ERM_ADD_TO_QUEUE);
      }
      else
      {
        this.forceCheck = true;
        this.Think();
        if (this.checkRaisingTimePointEvents.Count == 0)
        {
          if (this.eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT)
            this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_NEVER;
          else if (this.eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_COMBINE)
            this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_PARAM_CHANGE;
        }
      }
      if (this.repeated)
        return;
      this.checkRaisingTimePointEvents.Remove(gtEvent);
      gtEvent.OnEvent -= new Action<GameTimer>(this.OnOutTimer);
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "CurrentConditionResult", this.memConditionResult);
      SaveManagerUtility.SaveEnum<EEventUpdateMode>(writer, "EventUpdateMode", this.eventUpdateMode);
      if (this.RaisingType != EEventRaisingType.EVENT_RAISING_TYPE_CONDITION && this.RaisingType != EEventRaisingType.EVENT_RAISING_TYPE_TIME)
        return;
      SaveManagerUtility.SaveList(writer, "DependedObjectsInfo", this.checkRaisingTimePointEvents.Select<GameTimer, ulong>((Func<GameTimer, ulong>) (o => o.TimerGuid)));
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      this.saveLoadingCheckRaisingObjectsID.Clear();
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode1.Name == "CurrentConditionResult")
          this.memConditionResult = VMSaveLoadManager.ReadInt((XmlNode) childNode1);
        else if (childNode1.Name == "EventUpdateMode")
          this.eventUpdateMode = VMSaveLoadManager.ReadEnum<EEventUpdateMode>((XmlNode) childNode1);
        else if (childNode1.Name == "DependedObjectsInfo")
        {
          try
          {
            foreach (XmlNode childNode2 in childNode1.ChildNodes)
              this.saveLoadingCheckRaisingObjectsID.Add(VMSaveLoadManager.ReadUlong(childNode2));
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("SaveLoad error: cannot load raising timers guids list from node {0}  in event {1}, error: {2}", (object) childNode1.InnerText, (object) this.parent.Name, (object) ex.ToString()));
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
            Logger.AddError(string.Format("SaveLoad error at {0} event afterloading: event game time context with name {1} not found", (object) this.parent.Name, (object) this.gameTimeContext.Name));
            return;
          }
        }
        int count = this.checkRaisingTimePointEvents.Count;
        if (count > 0)
        {
          for (int index = count - 1; index >= 0; --index)
          {
            GameTimer raisingTimePointEvent = this.checkRaisingTimePointEvents[index];
            if (raisingTimePointEvent.IsElapsed(gameTimeContext.GameTime))
            {
              gameTimeContext.StopTimer(raisingTimePointEvent.TimerGuid);
              this.checkRaisingTimePointEvents.RemoveAt(index);
            }
          }
        }
        if (this.RaisingType != EEventRaisingType.EVENT_RAISING_TYPE_CONDITION || this.eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_NEVER)
          return;
        this.memConditionResult = this.CalculateConditionResult() ? 1 : 0;
        if (this.eventUpdateMode == EEventUpdateMode.EVENT_UPDATE_MODE_ALWAYS)
          return;
        if (this.checkRaisingTimePointEvents.Count > 0 && this.checkRaisingParams.Count > 0)
          this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_COMBINE;
        else if (this.checkRaisingTimePointEvents.Count > 0)
          this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT;
        else
          this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_PARAM_CHANGE;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("SaveLoad error at {0} event afterloading, error: {1}", (object) this.parent.Name, (object) ex.ToString()));
      }
    }

    private void Init()
    {
      try
      {
        this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_ALWAYS;
        if (this.RaisingType == EEventRaisingType.EVENT_RAISING_TYPE_CONDITION)
        {
          if (this.eventCondition == null)
            Logger.AddError(string.Format("Invalid dynamic event body at {0}!", (object) this.parent.Name));
          else if (this.eventCondition.IsConstant())
          {
            this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_NEVER;
          }
          else
          {
            foreach (GameTime raisingTimePoint in this.eventCondition.GetCheckRaisingTimePoints())
            {
              GameTimer gameTimer = GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE, this.parentEntity.EngineGuid, 0UL, raisingTimePoint, this.repeated, this.gameTimeContext.Name);
              gameTimer.OnEvent += new Action<GameTimer>(this.OnOutTimer);
              this.checkRaisingTimePointEvents.Add(gameTimer);
            }
            if (this.checkRaisingTimePointEvents.Count > 0)
              this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT;
            this.checkRaisingParams.Clear();
            this.checkRaisingParams.AddRange((IEnumerable<VMParameter>) this.eventCondition.GetCheckRaisingParams());
            if (this.eventCondition.GetCheckRaisingFunctions().Count > this.checkRaisingTimePointEvents.Count)
            {
              this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_ALWAYS;
            }
            else
            {
              if (this.checkRaisingParams.Count <= 0)
                return;
              this.eventUpdateMode = this.eventUpdateMode != EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT ? EEventUpdateMode.EVENT_UPDATE_MODE_PARAM_CHANGE : EEventUpdateMode.EVENT_UPDATE_MODE_COMBINE;
              for (int index = 0; index < this.checkRaisingParams.Count; ++index)
                DynamicParameterUtility.AddDependedEvent(this.checkRaisingParams[index].BaseGuid, (IDependedEventRef) this);
            }
          }
        }
        else if (this.RaisingType == EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE)
        {
          DynamicParameterUtility.AddDependedEvent(this.eventParameter.BaseGuid, (IDependedEventRef) this);
          this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_PARAM_CHANGE;
        }
        else
        {
          if (this.RaisingType != EEventRaisingType.EVENT_RAISING_TYPE_TIME)
            return;
          GameTimer gameTimer = GameTimeManager.StartTimer(EGameTimerType.GAME_TIMER_TYPE_ABSOLUTE, this.parentEntity.EngineGuid, 0UL, this.eventTime, this.repeated, this.gameTimeContext.Name);
          gameTimer.OnEvent += new Action<GameTimer>(this.OnOutTimer);
          this.checkRaisingTimePointEvents.Add(gameTimer);
          this.eventUpdateMode = EEventUpdateMode.EVENT_UPDATE_MODE_OUTER_EVENT;
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot create event body at {0}, error: {1}", (object) this.parent.Name, (object) ex.ToString()));
      }
    }

    public delegate void OnEventBodyRise(object newValue, EEventRaisingMode eventRaisingMode);
  }
}
