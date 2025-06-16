// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.DynamicTalkingFSM
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.FSM;
using PLVirtualMachine.Objects;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
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
      this.speakingComponent = (VMSpeaking) entity.GetComponentByName(nameof (Speaking));
      if (entity == null)
        Logger.AddWarning(string.Format("Cannot init talking fsm: speaking component not found in {0}", (object) templateObj.Name));
      this.speechAuthorsList.Add(this.Entity);
      if (this.graphManager != null)
        return;
      Logger.AddError(string.Format("Invalid talking fsm creation at {0}, graph manager not created!", (object) templateObj.Name));
    }

    public override FSMGraphManager CreateGraphManager()
    {
      return (FSMGraphManager) new FSMTalkingManager((DynamicFSM) this);
    }

    public FSMTalkingManager TalkingManager => (FSMTalkingManager) this.graphManager;

    public override void StateSave(IDataWriter writer) => base.StateSave(writer);

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "CanceledSpeechGuidsList")
          this.TalkingManager.LoadFromXML((XmlElement) xmlNode.ChildNodes[i]);
        else if (xmlNode.ChildNodes[i].Name == "CanceledAnswerGuidsList")
          this.TalkingManager.LoadFromXML((XmlElement) xmlNode.ChildNodes[i]);
      }
    }

    public VMSpeaking Speaking => this.speakingComponent;

    public VMTalkingGraph CurrentTalkingGraph
    {
      get
      {
        return this.CurrentState != null && this.CurrentState.Parent != null && typeof (VMTalkingGraph) == this.CurrentState.Parent.GetType() ? (VMTalkingGraph) this.CurrentState.Parent : (VMTalkingGraph) null;
      }
    }

    public override void Think()
    {
      base.Think();
      DynamicFSM.SetCurrentDebugFSM((DynamicFSM) this);
      if (this.Active)
        this.CheckContextTalkingAvailable();
      DynamicFSM.SetCurrentDebugFSM((DynamicFSM) null);
    }

    public override void OnStart()
    {
      base.OnStart();
      this.beginTalkingEvent = this.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_BEGIN_TALKING, typeof (VMSpeaking), true));
      if (this.beginTalkingEvent == null)
        Logger.AddError(string.Format("Begin talking event not created in {0} FSM", (object) this.FSMStaticObject.Name));
      else
        this.beginTalkingEvent.Subscribe((DynamicFSM) this);
    }

    public override void OnProcessEvent(RaisedEventInfo evntInfo)
    {
      if (evntInfo == null || evntInfo.Instance == null)
        return;
      DynamicEvent instance = evntInfo.Instance;
      if (this.CurrentTalkingGraph != null)
      {
        if (!(instance.Name == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking))) || evntInfo.Messages.Count <= 0)
          return;
        this.TalkingManager.ProcessSpeechReply(Convert.ToUInt64(evntInfo.Messages[0].Value));
      }
      else
      {
        base.OnProcessEvent(evntInfo);
        if (!this.Speaking.IsActiveTalking)
          return;
        IFiniteStateMachine activeTalking = this.Speaking.ActiveTalking;
        if (this.CheckTalkingActual(activeTalking))
        {
          this.TalkingManager.StartTalking(this.Speaking.ActiveTalking);
        }
        else
        {
          this.Speaking.CancelTalking();
          Logger.AddError(string.Format("Talking {0} won't cause any speeches by inner logic. Talking will be close to avoid 'Chicken Bug' at {1}", (object) activeTalking.Name, (object) DynamicFSM.CurrentStateInfo));
        }
      }
    }

    public void OnChangeState(IState state)
    {
      if (!this.Active)
        return;
      try
      {
        if (((IState) state.Parent).IsProcedure)
          return;
        this.curentTalkingEventLinks.Clear();
        if (state != null && state.Parent != null)
        {
          for (int index = 0; index < state.OutputLinks.Count; ++index)
          {
            VMEventLink outputLink = (VMEventLink) state.OutputLinks[index];
            if (outputLink.Event != null && outputLink.Event.EventInstance != null && outputLink.Event.EventInstance.Name == VMSpeaking.BeginTalkingEventName)
              this.curentTalkingEventLinks.Add((ILink) outputLink);
          }
          FiniteStateMachine parent = (FiniteStateMachine) state.Parent;
          for (int index = 0; index < parent.EnterLinks.Count; ++index)
          {
            VMEventLink enterLink = (VMEventLink) parent.EnterLinks[index];
            if (!enterLink.IsInitial() && enterLink.Event != null && enterLink.Event.EventInstance != null && enterLink.Event.EventInstance.Name == VMSpeaking.BeginTalkingEventName)
              this.curentTalkingEventLinks.Add((ILink) enterLink);
          }
        }
        this.CheckContextTalkingAvailable();
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cnange state processing error {0}!", (object) ex.Message));
      }
    }

    protected bool CheckTalkingActual(IFiniteStateMachine talkingGraph, int depth = 0)
    {
      if (talkingGraph == null)
        return false;
      if (depth > 10)
      {
        Logger.AddError(string.Format("Unusual talking content error in talking {0}: probably invalid talking data loaded", (object) talkingGraph.Name));
        return false;
      }
      return talkingGraph.Abstract ? this.CheckTalkingActual((IFiniteStateMachine) this.Speaking.CurrentTalking.State, depth + 1) : this.TalkingManager.CheckTalkingStateActual(talkingGraph.InitState).Key;
    }

    public static bool IsTalking => FSMTalkingManager.IsTalking;

    public void CheckContextTalkingAvailable()
    {
      if (!this.Entity.Instantiated)
      {
        string str = "none";
        if (this.CurrentState != null)
          str = this.CurrentState.Name;
        Logger.AddError(string.Format("Attempt to removed object fsm processing at {0}, state {1}", (object) this.FSMStaticObject.Name, (object) str));
      }
      else
      {
        if (this.Speaking == null || this.Speaking.CurrentTalking != null && this.Speaking.CurrentTalking.State != null)
          return;
        bool isTalkingAvailable = false;
        for (int index = 0; index < this.curentTalkingEventLinks.Count; ++index)
        {
          isTalkingAvailable = this.TalkingManager.CheckTalkingStateActual(((VMEventLink) this.curentTalkingEventLinks[index]).DestState).Key;
          if (isTalkingAvailable)
            break;
        }
        this.Speaking.SetContextTalkingAvailable(isTalkingAvailable);
      }
    }

    public override void AfterSaveLoading()
    {
      base.AfterSaveLoading();
      this.speakingComponent = (VMSpeaking) this.Entity.GetComponentByName("Speaking");
      if (this.Entity == null)
        Logger.AddWarning(string.Format("Cannot init talking fsm: speaking component not found in {0}", (object) this.FSMStaticObject.Name));
      this.speechAuthorsList.Add(this.Entity);
      this.beginTalkingEvent = this.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_BEGIN_TALKING, typeof (VMSpeaking), true));
      if (this.beginTalkingEvent == null)
        Logger.AddError(string.Format("Begin talking event not created in {0} FSM", (object) this.FSMStaticObject.Name));
      else
        this.beginTalkingEvent.Subscribe((DynamicFSM) this);
    }
  }
}
