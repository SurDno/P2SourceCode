// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.FSMTalkingManager
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.FSM;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class FSMTalkingManager : FSMGraphManager
  {
    private DynamicEvent speechReplyEvent;
    private List<KeyValuePair<ulong, bool>> currSpeechReplyTextGuids = new List<KeyValuePair<ulong, bool>>();
    private List<ulong> canceledSpeechGuids = new List<ulong>();
    private List<ulong> canceledAnswerGuids = new List<ulong>();
    private bool talking;
    private static bool talkingMode;

    public FSMTalkingManager(DynamicFSM fsm)
      : base(fsm)
    {
    }

    public static bool IsTalking => FSMTalkingManager.talkingMode;

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.SaveList(writer, "CanceledSpeechGuidsList", this.canceledSpeechGuids);
      SaveManagerUtility.SaveList(writer, "CanceledAnswerGuidsList", this.canceledAnswerGuids);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      if (xmlNode.Name == "CanceledSpeechGuidsList")
      {
        VMSaveLoadManager.LoadList(xmlNode, this.canceledSpeechGuids);
      }
      else
      {
        if (!(xmlNode.Name == "CanceledAnswerGuidsList"))
          return;
        VMSaveLoadManager.LoadList(xmlNode, this.canceledAnswerGuids);
      }
    }

    public void StartTalking(IFiniteStateMachine talkingGraph)
    {
      this.SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_LOCAL, true);
      this.talking = true;
      FSMTalkingManager.talkingMode = true;
      if (this.speechReplyEvent == null)
        this.speechReplyEvent = this.fsm.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking), true));
      if (this.speechReplyEvent == null)
        Logger.AddError(string.Format("Start talking: speech reply event not created in {0} FSM", (object) this.TalkingFSM.FSMStaticObject.Name));
      else
        this.speechReplyEvent.Subscribe(this.fsm);
      ITalkingGraph actualTalking = this.TalkingFSM.Speaking.GetActualTalking(talkingGraph);
      if (actualTalking == null)
      {
        Logger.AddError(string.Format("Invalid start talking in {0}, cannot define actual talking at", (object) this.TalkingFSM.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
        this.SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_MAIN);
      }
      else
        this.MoveIntoTalking(actualTalking);
    }

    public void StopTalking()
    {
      if (this.TalkingFSM.Speaking.ActiveTalking != null)
      {
        VMEventLink afterExitLink = ((VMState) this.TalkingFSM.Speaking.ActiveTalking).GetAfterExitLink();
        if (afterExitLink != null && afterExitLink.DestState != null && typeof (IFiniteStateMachine).IsAssignableFrom(afterExitLink.DestState.GetType()))
        {
          IFiniteStateMachine destState = (IFiniteStateMachine) afterExitLink.DestState;
          if (destState.GraphType == EGraphType.GRAPH_TYPE_TALKING && this.CheckTalkingStateActual(afterExitLink.DestState).Key)
          {
            ITalkingGraph actualTalking = this.TalkingFSM.Speaking.GetActualTalking(destState);
            if (actualTalking != null)
            {
              this.TalkingFSM.Speaking.ActiveTalking = destState;
              this.PopState();
              this.MoveIntoTalking(actualTalking);
              return;
            }
          }
        }
      }
      this.talking = false;
      FSMTalkingManager.talkingMode = false;
      if (this.speechReplyEvent == null)
        Logger.AddError(string.Format("Stop talking: speech reply event not created in {0} FSM", (object) this.fsm.FSMStaticObject.Name));
      else
        this.speechReplyEvent.DeSubscribe(this.fsm);
      this.TalkingFSM.Speaking.ExitTalking();
      this.SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_MAIN);
    }

    protected override void OnChangeState(IState state) => this.TalkingFSM.OnChangeState(state);

    protected override void MoveIntoState(IState newState, int iDestEntryPoint, bool bNextLevel = false)
    {
      if (newState == null)
        Logger.AddError(string.Format("New state not defined in object {0}!", (object) this.fsm.FSMStaticObject.Name));
      else if (iDestEntryPoint < 0 || iDestEntryPoint >= newState.EntryPoints.Count)
        Logger.AddError(string.Format("Invalid entry point index at move to {0} state in {1} object", (object) newState.Name, (object) this.fsm.StaticGuid));
      else if (typeof (IBranch).IsAssignableFrom(newState.GetType()))
        this.ProcessBranch((IBranch) newState);
      else if (this.CurrentState != null && (long) newState.BaseGuid == (long) this.CurrentState.BaseGuid)
      {
        this.ProcessState(newState, iDestEntryPoint);
      }
      else
      {
        IState currentState = this.CurrentState;
        bool flag1 = false;
        bool flag2 = typeof (ISpeech).IsAssignableFrom(newState.GetType());
        if (!bNextLevel && (!flag2 ? 0 : (this.CurrentState.Parent.IsEqual((IObject) ((VMBaseObject) newState).Parent) ? 1 : 0)) == 0 && !flag1)
          this.PopState();
        if (flag2)
        {
          if (!this.ProcessSpeech((ISpeech) newState))
            this.StopTalking();
          iDestEntryPoint = 0;
        }
        if (!flag1)
          this.PushState(currentState, newState);
        if (flag2)
          return;
        this.ProcessState(newState, iDestEntryPoint);
      }
    }

    public override void ProcessMoveToState(
      IState newState,
      IEventLink inputLink,
      int iDestEntryPoint = 0)
    {
      if (newState == null)
        Logger.AddError(string.Format("State for moving to not defined in {0} !!!", (object) this.TalkingFSM.FSMStaticObject.Name));
      else if (((VMBaseObject) newState).GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH)
      {
        if (((FiniteStateMachine) newState).GraphType == EGraphType.GRAPH_TYPE_TALKING)
        {
          ITalkingGraph actualTalking = this.TalkingFSM.Speaking.GetActualTalking((IFiniteStateMachine) newState);
          if (actualTalking != null)
          {
            if (!this.TalkingFSM.Speaking.IsTalkingOnlyOncePassed((IFiniteStateMachine) actualTalking))
              this.TalkingFSM.Speaking.ActiveTalking = (IFiniteStateMachine) newState;
            this.ReturnToPreviousState();
          }
          else
          {
            VMEventLink afterExitLink = ((VMState) newState).GetAfterExitLink();
            if (afterExitLink != null)
              this.ProcessLink(afterExitLink);
            else
              this.ReturnToPreviousState();
          }
        }
        else
          this.MoveIntoSubGraph((FiniteStateMachine) newState, inputLink, iDestEntryPoint);
      }
      else
        this.MoveIntoState(newState, iDestEntryPoint, false);
    }

    private void MoveIntoTalking(ITalkingGraph talkingGraph)
    {
      this.PushState(this.CurrentState, (IState) talkingGraph);
      this.MoveIntoState(talkingGraph.InitState, 0, true);
    }

    public void ProcessSpeechReply(ulong replyTextID)
    {
      if (!typeof (ISpeech).IsAssignableFrom(this.CurrentState.GetType()))
        Logger.AddError(string.Format("Current FSM {0} isn't in speech state, cannot process reply", (object) this.fsm.StaticObject.BaseGuid));
      else if (replyTextID == 0UL)
      {
        this.ProcessSpeech((ISpeech) this.CurrentState);
      }
      else
      {
        ISpeechReply speechReply = (ISpeechReply) null;
        int num = -1;
        for (int index = 0; index < ((ISpeech) this.CurrentState).Replies.Count; ++index)
        {
          ISpeechReply reply = ((ISpeech) this.CurrentState).Replies[index];
          if ((long) reply.Text.BaseGuid == (long) replyTextID)
          {
            speechReply = reply;
            num = index;
            break;
          }
        }
        if (speechReply == null)
        {
          Logger.AddError(string.Format("Invalid reply in speech {0}: Replay id={1} not found in curr speech replyes, probably two replyes from one speech gui sended at the same time", (object) this.CurrentState.Name, (object) replyTextID));
          this.ProcessSpeech((ISpeech) this.CurrentState);
        }
        else
        {
          if (speechReply.OnlyOneReply)
            this.canceledAnswerGuids.Add(speechReply.BaseGuid);
          IActionLine actionLine = speechReply.ActionLine;
          if (actionLine != null)
            this.ProcessActionLine(actionLine, true);
          VMEventLink moveLink = (VMEventLink) null;
          for (int index = 0; index < ((VMState) this.CurrentState).OutputLinks.Count; ++index)
          {
            VMEventLink outputLink = (VMEventLink) ((VMState) this.CurrentState).OutputLinks[index];
            if (outputLink.SourceExitPoint == num)
            {
              moveLink = outputLink;
              break;
            }
          }
          if (moveLink == null)
            this.StopTalking();
          else if (moveLink.DestState == null)
            this.StopTalking();
          else
            this.ProcessLink(moveLink);
        }
      }
    }

    public bool ProcessSpeech(ISpeech speech)
    {
      if (this.OnStateIn((IState) speech))
        return true;
      VMEntity author1 = (VMEntity) null;
      if (this.fsm.FSMStaticObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_QUEST)
      {
        if (speech.Author != null)
        {
          IObjRef author2 = speech.Author;
          if (((VMObjRef) author2).Object != null)
          {
            if (((VMLogicObject) ((VMObjRef) author2).Object).Static)
            {
              author1 = WorldEntityUtility.GetDynamicObjectEntityByStaticGuid(author2.StaticInstance.BaseGuid);
            }
            else
            {
              DynamicParameter dynamicObjectParameter = this.fsm.GetDynamicObjectParameter(((VMSpeech) speech).SpeechAuthorObjGuid);
              if (dynamicObjectParameter != null && typeof (IObjRef).IsAssignableFrom(dynamicObjectParameter.Type.BaseType))
                author1 = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((IEngineInstanced) dynamicObjectParameter.Value).EngineGuid);
            }
          }
        }
      }
      else
        author1 = this.fsm.Entity;
      if (author1 == null)
      {
        Logger.AddError(string.Format("Speech author entity not found in speech {0}", (object) speech.Name));
        return false;
      }
      this.currSpeechReplyTextGuids.Clear();
      int num = -1;
      for (int index = 0; index < speech.Replies.Count; ++index)
      {
        ISpeechReply reply = speech.Replies[index];
        if (reply.IsDefault)
          num = index;
        bool flag1 = true;
        if (reply.EnableCondition != null)
          flag1 = ExpressionUtility.CalculateConditionResult(reply.EnableCondition, (IDynamicGameObjectContext) this.fsm);
        if (flag1 && !this.canceledAnswerGuids.Contains(reply.BaseGuid))
        {
          VMEventLink sourceExitPointIndex = ((VMState) speech).GetOutputLinkBySourceExitPointIndex(index);
          bool flag2 = false;
          if (sourceExitPointIndex != null)
            flag2 = sourceExitPointIndex.DestState != null;
          this.currSpeechReplyTextGuids.Add(new KeyValuePair<ulong, bool>(reply.Text.BaseGuid, flag2));
          if (reply.OnlyOnce)
            this.canceledAnswerGuids.Add(reply.BaseGuid);
        }
      }
      if (this.currSpeechReplyTextGuids.Count == 0 && num >= 0)
      {
        ISpeechReply reply = speech.Replies[num];
        VMEventLink sourceExitPointIndex = ((VMState) speech).GetOutputLinkBySourceExitPointIndex(num);
        bool flag = false;
        if (sourceExitPointIndex != null)
          flag = sourceExitPointIndex.DestState != null;
        this.currSpeechReplyTextGuids.Add(new KeyValuePair<ulong, bool>(reply.Text.BaseGuid, flag));
      }
      IGameString text = speech.Text;
      if (speech.TextParam != null)
      {
        if (((VMParameter) speech.TextParam).Parent != null)
        {
          if (typeof (VMLogicObject).IsAssignableFrom(((VMParameter) speech.TextParam).Parent.GetType()))
          {
            IParam contextParam = ((VMVariableService) IVariableService.Instance).GetDynamicContext((IContext) ((VMParameter) speech.TextParam).Parent, (IDynamicGameObjectContext) this.fsm).GetContextParam(((VMParameter) speech.TextParam).BaseGuid);
            if (contextParam != null)
            {
              if (typeof (ITextRef).IsAssignableFrom(contextParam.Type.BaseType) && contextParam.Value != null)
                text = ((ITextRef) contextParam.Value).Text;
              else
                Logger.AddError(string.Format("Invalid text param {0} value type in speech {1}", (object) contextParam.Name, (object) speech.Name));
            }
            else
              Logger.AddError(string.Format("Dynamic param with name {0} not found in object {1}", (object) speech.TextParam.Name, (object) speech.Name));
          }
          else
            Logger.AddError(string.Format("Text param {0} parent is invalid", (object) speech.TextParam.Name));
        }
        else
          Logger.AddError(string.Format("Text param {0} parent isn' t defined", (object) speech.TextParam.Name));
      }
      this.ProcessActionLine(speech.ActionLine);
      this.CheckLoopStackClear();
      if (!this.TalkingFSM.Speaking.MakePlayerSpeech(text.BaseGuid, this.currSpeechReplyTextGuids, (VMBaseEntity) author1))
        return false;
      if (speech.OnlyOnce)
        this.canceledSpeechGuids.Add(speech.BaseGuid);
      return true;
    }

    public KeyValuePair<bool, bool> CheckTalkingStateActual(IState talkingState, bool bFromBranch = false)
    {
      if (talkingState == null)
        return new KeyValuePair<bool, bool>(false, false);
      try
      {
        if (typeof (IFiniteStateMachine).IsAssignableFrom(talkingState.GetType()))
        {
          if (!((IFiniteStateMachine) talkingState).Abstract)
            return this.CheckTalkingStateActual(((IFiniteStateMachine) talkingState).InitState);
          return this.TalkingFSM.Speaking.CurrentTalking != null ? this.CheckTalkingStateActual(this.TalkingFSM.Speaking.CurrentTalking.State, bFromBranch) : new KeyValuePair<bool, bool>(false, false);
        }
        if (typeof (ISpeech).IsAssignableFrom(talkingState.GetType()))
        {
          VMSpeech vmSpeech = (VMSpeech) talkingState;
          FSMGraphManager.debugCurrentState = (IGraphObject) vmSpeech;
          if (this.TalkingFSM.Speaking.IsTalkingOnlyOncePassed((IFiniteStateMachine) vmSpeech.Parent))
            return new KeyValuePair<bool, bool>(false, false);
          bool flag = false;
          if (vmSpeech.OnlyOnce && this.canceledSpeechGuids.Contains(vmSpeech.BaseGuid))
            flag = true;
          if (!flag)
            return new KeyValuePair<bool, bool>(true, false);
          if (bFromBranch)
            return new KeyValuePair<bool, bool>(false, true);
          if (vmSpeech.OutputLinks.Count <= 0)
            return new KeyValuePair<bool, bool>(false, false);
          VMEventLink outputLink = (VMEventLink) vmSpeech.OutputLinks[0];
          if (outputLink != null)
          {
            if (outputLink.DestState != null)
              return this.CheckTalkingStateActual(outputLink.DestState);
          }
        }
        else if (typeof (IBranch).IsAssignableFrom(talkingState.GetType()))
        {
          VMBranch vmBranch = (VMBranch) talkingState;
          FSMGraphManager.debugCurrentState = (IGraphObject) vmBranch;
          if (vmBranch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH || vmBranch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH)
          {
            object secondValue = (object) 0;
            int iSrcPntIndex = -1;
            if (vmBranch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH)
              secondValue = (object) float.MaxValue;
            else if (vmBranch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH)
              secondValue = (object) float.MinValue;
            for (int exitPntIndex = 0; exitPntIndex < vmBranch.GetExitPointsCount(); ++exitPntIndex)
            {
              VMPartCondition branchCondition = (VMPartCondition) vmBranch.GetBranchCondition(exitPntIndex);
              if (branchCondition != null)
              {
                VMExpression firstExpression = (VMExpression) branchCondition.FirstExpression;
                if (firstExpression != null)
                {
                  object expressionResult = ExpressionUtility.CalculateExpressionResult((IExpression) firstExpression, (IDynamicGameObjectContext) this.fsm);
                  if (vmBranch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH)
                  {
                    if (VMTypeMathUtility.IsValueLess(expressionResult, secondValue))
                    {
                      iSrcPntIndex = exitPntIndex;
                      secondValue = expressionResult;
                    }
                  }
                  else if (vmBranch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH && VMTypeMathUtility.IsValueLarger(expressionResult, secondValue))
                  {
                    iSrcPntIndex = exitPntIndex;
                    secondValue = expressionResult;
                  }
                }
                else
                  Logger.AddError(string.Format("MinMax branch {0} expression not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
              }
            }
            if (iSrcPntIndex >= 0)
            {
              VMEventLink sourceExitPointIndex = vmBranch.GetOutputLinkBySourceExitPointIndex(iSrcPntIndex);
              if (sourceExitPointIndex == null)
                return new KeyValuePair<bool, bool>(false, false);
              if (sourceExitPointIndex.DestState == null)
                return new KeyValuePair<bool, bool>(false, false);
              KeyValuePair<bool, bool> keyValuePair = this.CheckTalkingStateActual(sourceExitPointIndex.DestState, true);
              if (keyValuePair.Key)
                return keyValuePair;
              if (!keyValuePair.Value)
                return keyValuePair;
            }
          }
          else
          {
            for (int index = 0; index < vmBranch.GetExitPointsCount(); ++index)
            {
              ICondition branchCondition = vmBranch.GetBranchCondition(index);
              if (index >= vmBranch.GetExitPointsCount() - 1 || ExpressionUtility.CalculateConditionResult(branchCondition, (IDynamicGameObjectContext) this.fsm))
              {
                VMEventLink sourceExitPointIndex = vmBranch.GetOutputLinkBySourceExitPointIndex(index);
                if (sourceExitPointIndex == null)
                  return new KeyValuePair<bool, bool>(false, false);
                if (sourceExitPointIndex.DestState == null)
                  return new KeyValuePair<bool, bool>(false, false);
                KeyValuePair<bool, bool> keyValuePair = this.CheckTalkingStateActual(sourceExitPointIndex.DestState, true);
                if (keyValuePair.Key || !keyValuePair.Value)
                  return keyValuePair;
              }
            }
          }
        }
        else
        {
          VMEventLink afterExitLink = ((VMState) talkingState).GetAfterExitLink();
          if (afterExitLink != null)
          {
            if (afterExitLink.DestState != null)
              return this.CheckTalkingStateActual(afterExitLink.DestState);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Checking talking state actual error at {0}, error: {1}", (object) this.TalkingFSM.FSMStaticObject.Name, (object) ex.ToString()));
      }
      return new KeyValuePair<bool, bool>(false, false);
    }

    protected override bool IsThisTalking => this.talking;

    protected override bool IgnoreBranchCase(IState newState)
    {
      bool flag = false;
      VMTalkingGraph vmTalkingGraph = this.TalkingFSM.CurrentTalkingGraph;
      if (vmTalkingGraph == null && this.CurrentState != null && typeof (VMTalkingGraph) == this.CurrentState.GetType())
        vmTalkingGraph = (VMTalkingGraph) this.CurrentState;
      if (vmTalkingGraph != null && newState != null && typeof (VMSpeech) == newState.GetType())
        flag = this.canceledSpeechGuids.Contains(((VMBaseObject) newState).BaseGuid);
      return flag;
    }

    protected override void ReturnToOuterGraph(VMEventLink prevLink = null)
    {
      if (this.IsThisTalking)
        this.StopTalking();
      else
        base.ReturnToOuterGraph(prevLink);
    }

    private DynamicTalkingFSM TalkingFSM => (DynamicTalkingFSM) this.fsm;
  }
}
