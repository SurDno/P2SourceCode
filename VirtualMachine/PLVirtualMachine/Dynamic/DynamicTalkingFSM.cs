using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.FSM;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic
{
  public class DynamicTalkingFSM : DynamicFSM
  {
    private VMSpeaking speakingComponent;
    private DynamicEvent beginTalkingEvent;
    private List<VMEntity> speechAuthorsList = new List<VMEntity>();
    private List<ILink> curentTalkingEventLinks = new List<ILink>();

    public DynamicTalkingFSM(VMEntity entity, VMLogicObject templateObj)
      : base(entity, templateObj)
    {
      speakingComponent = (VMSpeaking) entity.GetComponentByName(nameof (Speaking));
      if (entity == null)
        Logger.AddWarning(string.Format("Cannot init talking fsm: speaking component not found in {0}", templateObj.Name));
      speechAuthorsList.Add(Entity);
      if (graphManager != null)
        return;
      Logger.AddError(string.Format("Invalid talking fsm creation at {0}, graph manager not created!", templateObj.Name));
    }

    public override FSMGraphManager CreateGraphManager()
    {
      return new FSMTalkingManager(this);
    }

    public FSMTalkingManager TalkingManager => (FSMTalkingManager) graphManager;

    public override void StateSave(IDataWriter writer) => base.StateSave(writer);

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "CanceledSpeechGuidsList")
          TalkingManager.LoadFromXML((XmlElement) xmlNode.ChildNodes[i]);
        else if (xmlNode.ChildNodes[i].Name == "CanceledAnswerGuidsList")
          TalkingManager.LoadFromXML((XmlElement) xmlNode.ChildNodes[i]);
      }
    }

    public VMSpeaking Speaking => speakingComponent;

    public VMTalkingGraph CurrentTalkingGraph
    {
      get
      {
        return CurrentState != null && CurrentState.Parent != null && typeof (VMTalkingGraph) == CurrentState.Parent.GetType() ? (VMTalkingGraph) CurrentState.Parent : null;
      }
    }

    public override void Think()
    {
      base.Think();
      SetCurrentDebugFSM(this);
      if (Active)
        CheckContextTalkingAvailable();
      SetCurrentDebugFSM(null);
    }

    public override void OnStart()
    {
      base.OnStart();
      beginTalkingEvent = GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_BEGIN_TALKING, typeof (VMSpeaking), true));
      if (beginTalkingEvent == null)
        Logger.AddError(string.Format("Begin talking event not created in {0} FSM", FSMStaticObject.Name));
      else
        beginTalkingEvent.Subscribe(this);
    }

    public override void OnProcessEvent(RaisedEventInfo evntInfo)
    {
      if (evntInfo == null || evntInfo.Instance == null)
        return;
      DynamicEvent instance = evntInfo.Instance;
      if (CurrentTalkingGraph != null)
      {
        if (!(instance.Name == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking))) || evntInfo.Messages.Count <= 0)
          return;
        TalkingManager.ProcessSpeechReply(Convert.ToUInt64(evntInfo.Messages[0].Value));
      }
      else
      {
        base.OnProcessEvent(evntInfo);
        if (!Speaking.IsActiveTalking)
          return;
        IFiniteStateMachine activeTalking = Speaking.ActiveTalking;
        if (CheckTalkingActual(activeTalking))
        {
          TalkingManager.StartTalking(Speaking.ActiveTalking);
        }
        else
        {
          Speaking.CancelTalking();
          Logger.AddError(string.Format("Talking {0} won't cause any speeches by inner logic. Talking will be close to avoid 'Chicken Bug' at {1}", activeTalking.Name, CurrentStateInfo));
        }
      }
    }

    public void OnChangeState(IState state)
    {
      if (!Active)
        return;
      try
      {
        if (((IState) state.Parent).IsProcedure)
          return;
        curentTalkingEventLinks.Clear();
        if (state != null && state.Parent != null)
        {
          for (int index = 0; index < state.OutputLinks.Count; ++index)
          {
            VMEventLink outputLink = (VMEventLink) state.OutputLinks[index];
            if (outputLink.Event != null && outputLink.Event.EventInstance != null && outputLink.Event.EventInstance.Name == VMSpeaking.BeginTalkingEventName)
              curentTalkingEventLinks.Add(outputLink);
          }
          FiniteStateMachine parent = (FiniteStateMachine) state.Parent;
          for (int index = 0; index < parent.EnterLinks.Count; ++index)
          {
            VMEventLink enterLink = (VMEventLink) parent.EnterLinks[index];
            if (!enterLink.IsInitial() && enterLink.Event != null && enterLink.Event.EventInstance != null && enterLink.Event.EventInstance.Name == VMSpeaking.BeginTalkingEventName)
              curentTalkingEventLinks.Add(enterLink);
          }
        }
        CheckContextTalkingAvailable();
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cnange state processing error {0}!", ex.Message));
      }
    }

    protected bool CheckTalkingActual(IFiniteStateMachine talkingGraph, int depth = 0)
    {
      if (talkingGraph == null)
        return false;
      if (depth > 10)
      {
        Logger.AddError(string.Format("Unusual talking content error in talking {0}: probably invalid talking data loaded", talkingGraph.Name));
        return false;
      }
      return talkingGraph.Abstract ? CheckTalkingActual((IFiniteStateMachine) Speaking.CurrentTalking.State, depth + 1) : TalkingManager.CheckTalkingStateActual(talkingGraph.InitState).Key;
    }

    public static bool IsTalking => FSMTalkingManager.IsTalking;

    public void CheckContextTalkingAvailable()
    {
      if (!Entity.Instantiated)
      {
        string str = "none";
        if (CurrentState != null)
          str = CurrentState.Name;
        Logger.AddError(string.Format("Attempt to removed object fsm processing at {0}, state {1}", FSMStaticObject.Name, str));
      }
      else
      {
        if (Speaking == null || Speaking.CurrentTalking != null && Speaking.CurrentTalking.State != null)
          return;
        bool isTalkingAvailable = false;
        for (int index = 0; index < curentTalkingEventLinks.Count; ++index)
        {
          isTalkingAvailable = TalkingManager.CheckTalkingStateActual(((VMEventLink) curentTalkingEventLinks[index]).DestState).Key;
          if (isTalkingAvailable)
            break;
        }
        Speaking.SetContextTalkingAvailable(isTalkingAvailable);
      }
    }

    public override void AfterSaveLoading()
    {
      base.AfterSaveLoading();
      speakingComponent = (VMSpeaking) Entity.GetComponentByName("Speaking");
      if (Entity == null)
        Logger.AddWarning(string.Format("Cannot init talking fsm: speaking component not found in {0}", FSMStaticObject.Name));
      speechAuthorsList.Add(Entity);
      beginTalkingEvent = GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_BEGIN_TALKING, typeof (VMSpeaking), true));
      if (beginTalkingEvent == null)
        Logger.AddError(string.Format("Begin talking event not created in {0} FSM", FSMStaticObject.Name));
      else
        beginTalkingEvent.Subscribe(this);
    }
  }
}
