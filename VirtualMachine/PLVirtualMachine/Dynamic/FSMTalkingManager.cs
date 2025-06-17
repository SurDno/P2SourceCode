using System;
using System.Collections.Generic;
using System.Xml;
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

namespace PLVirtualMachine.Dynamic
{
  public class FSMTalkingManager(DynamicFSM fsm) : FSMGraphManager(fsm) {
    private DynamicEvent speechReplyEvent;
    private List<KeyValuePair<ulong, bool>> currSpeechReplyTextGuids = [];
    private List<ulong> canceledSpeechGuids = [];
    private List<ulong> canceledAnswerGuids = [];
    private bool talking;
    private static bool talkingMode;

    public static bool IsTalking => talkingMode;

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.SaveList(writer, "CanceledSpeechGuidsList", canceledSpeechGuids);
      SaveManagerUtility.SaveList(writer, "CanceledAnswerGuidsList", canceledAnswerGuids);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      if (xmlNode.Name == "CanceledSpeechGuidsList")
      {
        VMSaveLoadManager.LoadList(xmlNode, canceledSpeechGuids);
      }
      else
      {
        if (!(xmlNode.Name == "CanceledAnswerGuidsList"))
          return;
        VMSaveLoadManager.LoadList(xmlNode, canceledAnswerGuids);
      }
    }

    public void StartTalking(IFiniteStateMachine talkingGraph)
    {
      SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_LOCAL, true);
      talking = true;
      talkingMode = true;
      if (speechReplyEvent == null)
        speechReplyEvent = fsm.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking), true));
      if (speechReplyEvent == null)
        Logger.AddError(string.Format("Start talking: speech reply event not created in {0} FSM", TalkingFSM.FSMStaticObject.Name));
      else
        speechReplyEvent.Subscribe(fsm);
      ITalkingGraph actualTalking = TalkingFSM.Speaking.GetActualTalking(talkingGraph);
      if (actualTalking == null)
      {
        Logger.AddError(string.Format("Invalid start talking in {0}, cannot define actual talking at", TalkingFSM.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
        SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_MAIN);
      }
      else
        MoveIntoTalking(actualTalking);
    }

    public void StopTalking()
    {
      if (TalkingFSM.Speaking.ActiveTalking != null)
      {
        VMEventLink afterExitLink = ((VMState) TalkingFSM.Speaking.ActiveTalking).GetAfterExitLink();
        if (afterExitLink != null && afterExitLink.DestState != null && typeof (IFiniteStateMachine).IsAssignableFrom(afterExitLink.DestState.GetType()))
        {
          IFiniteStateMachine destState = (IFiniteStateMachine) afterExitLink.DestState;
          if (destState.GraphType == EGraphType.GRAPH_TYPE_TALKING && CheckTalkingStateActual(afterExitLink.DestState).Key)
          {
            ITalkingGraph actualTalking = TalkingFSM.Speaking.GetActualTalking(destState);
            if (actualTalking != null)
            {
              TalkingFSM.Speaking.ActiveTalking = destState;
              PopState();
              MoveIntoTalking(actualTalking);
              return;
            }
          }
        }
      }
      talking = false;
      talkingMode = false;
      if (speechReplyEvent == null)
        Logger.AddError(string.Format("Stop talking: speech reply event not created in {0} FSM", fsm.FSMStaticObject.Name));
      else
        speechReplyEvent.DeSubscribe(fsm);
      TalkingFSM.Speaking.ExitTalking();
      SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_MAIN);
    }

    protected override void OnChangeState(IState state) => TalkingFSM.OnChangeState(state);

    protected override void MoveIntoState(IState newState, int iDestEntryPoint, bool bNextLevel = false)
    {
      if (newState == null)
        Logger.AddError(string.Format("New state not defined in object {0}!", fsm.FSMStaticObject.Name));
      else if (iDestEntryPoint < 0 || iDestEntryPoint >= newState.EntryPoints.Count)
        Logger.AddError(string.Format("Invalid entry point index at move to {0} state in {1} object", newState.Name, fsm.StaticGuid));
      else if (typeof (IBranch).IsAssignableFrom(newState.GetType()))
        ProcessBranch((IBranch) newState);
      else if (CurrentState != null && (long) newState.BaseGuid == (long) CurrentState.BaseGuid)
      {
        ProcessState(newState, iDestEntryPoint);
      }
      else
      {
        IState currentState = CurrentState;
        bool flag1 = false;
        bool flag2 = typeof (ISpeech).IsAssignableFrom(newState.GetType());
        if (!bNextLevel && (!flag2 ? 0 : (CurrentState.Parent.IsEqual(((VMBaseObject) newState).Parent) ? 1 : 0)) == 0 && !flag1)
          PopState();
        if (flag2)
        {
          if (!ProcessSpeech((ISpeech) newState))
            StopTalking();
          iDestEntryPoint = 0;
        }
        if (!flag1)
          PushState(currentState, newState);
        if (flag2)
          return;
        ProcessState(newState, iDestEntryPoint);
      }
    }

    public override void ProcessMoveToState(
      IState newState,
      IEventLink inputLink,
      int iDestEntryPoint = 0)
    {
      if (newState == null)
        Logger.AddError(string.Format("State for moving to not defined in {0} !!!", TalkingFSM.FSMStaticObject.Name));
      else if (((VMBaseObject) newState).GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH)
      {
        if (((FiniteStateMachine) newState).GraphType == EGraphType.GRAPH_TYPE_TALKING)
        {
          ITalkingGraph actualTalking = TalkingFSM.Speaking.GetActualTalking((IFiniteStateMachine) newState);
          if (actualTalking != null)
          {
            if (!TalkingFSM.Speaking.IsTalkingOnlyOncePassed(actualTalking))
              TalkingFSM.Speaking.ActiveTalking = (IFiniteStateMachine) newState;
            ReturnToPreviousState();
          }
          else
          {
            VMEventLink afterExitLink = ((VMState) newState).GetAfterExitLink();
            if (afterExitLink != null)
              ProcessLink(afterExitLink);
            else
              ReturnToPreviousState();
          }
        }
        else
          MoveIntoSubGraph((FiniteStateMachine) newState, inputLink, iDestEntryPoint);
      }
      else
        MoveIntoState(newState, iDestEntryPoint);
    }

    private void MoveIntoTalking(ITalkingGraph talkingGraph)
    {
      PushState(CurrentState, talkingGraph);
      MoveIntoState(talkingGraph.InitState, 0, true);
    }

    public void ProcessSpeechReply(ulong replyTextID)
    {
      if (!typeof (ISpeech).IsAssignableFrom(CurrentState.GetType()))
        Logger.AddError(string.Format("Current FSM {0} isn't in speech state, cannot process reply", fsm.StaticObject.BaseGuid));
      else if (replyTextID == 0UL)
      {
        ProcessSpeech((ISpeech) CurrentState);
      }
      else
      {
        ISpeechReply speechReply = null;
        int num = -1;
        for (int index = 0; index < ((ISpeech) CurrentState).Replies.Count; ++index)
        {
          ISpeechReply reply = ((ISpeech) CurrentState).Replies[index];
          if ((long) reply.Text.BaseGuid == (long) replyTextID)
          {
            speechReply = reply;
            num = index;
            break;
          }
        }
        if (speechReply == null)
        {
          Logger.AddError(string.Format("Invalid reply in speech {0}: Replay id={1} not found in curr speech replyes, probably two replyes from one speech gui sended at the same time", CurrentState.Name, replyTextID));
          ProcessSpeech((ISpeech) CurrentState);
        }
        else
        {
          if (speechReply.OnlyOneReply)
            canceledAnswerGuids.Add(speechReply.BaseGuid);
          IActionLine actionLine = speechReply.ActionLine;
          if (actionLine != null)
            ProcessActionLine(actionLine, true);
          VMEventLink moveLink = null;
          for (int index = 0; index < ((VMState) CurrentState).OutputLinks.Count; ++index)
          {
            VMEventLink outputLink = (VMEventLink) ((VMState) CurrentState).OutputLinks[index];
            if (outputLink.SourceExitPoint == num)
            {
              moveLink = outputLink;
              break;
            }
          }
          if (moveLink == null)
            StopTalking();
          else if (moveLink.DestState == null)
            StopTalking();
          else
            ProcessLink(moveLink);
        }
      }
    }

    public bool ProcessSpeech(ISpeech speech)
    {
      if (OnStateIn(speech))
        return true;
      VMEntity author1 = null;
      if (fsm.FSMStaticObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_QUEST)
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
              DynamicParameter dynamicObjectParameter = fsm.GetDynamicObjectParameter(((VMSpeech) speech).SpeechAuthorObjGuid);
              if (dynamicObjectParameter != null && typeof (IObjRef).IsAssignableFrom(dynamicObjectParameter.Type.BaseType))
                author1 = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((IEngineInstanced) dynamicObjectParameter.Value).EngineGuid);
            }
          }
        }
      }
      else
        author1 = fsm.Entity;
      if (author1 == null)
      {
        Logger.AddError(string.Format("Speech author entity not found in speech {0}", speech.Name));
        return false;
      }
      currSpeechReplyTextGuids.Clear();
      int num = -1;
      for (int index = 0; index < speech.Replies.Count; ++index)
      {
        ISpeechReply reply = speech.Replies[index];
        if (reply.IsDefault)
          num = index;
        bool flag1 = true;
        if (reply.EnableCondition != null)
          flag1 = ExpressionUtility.CalculateConditionResult(reply.EnableCondition, fsm);
        if (flag1 && !canceledAnswerGuids.Contains(reply.BaseGuid))
        {
          VMEventLink sourceExitPointIndex = ((VMState) speech).GetOutputLinkBySourceExitPointIndex(index);
          bool flag2 = false;
          if (sourceExitPointIndex != null)
            flag2 = sourceExitPointIndex.DestState != null;
          currSpeechReplyTextGuids.Add(new KeyValuePair<ulong, bool>(reply.Text.BaseGuid, flag2));
          if (reply.OnlyOnce)
            canceledAnswerGuids.Add(reply.BaseGuid);
        }
      }
      if (currSpeechReplyTextGuids.Count == 0 && num >= 0)
      {
        ISpeechReply reply = speech.Replies[num];
        VMEventLink sourceExitPointIndex = ((VMState) speech).GetOutputLinkBySourceExitPointIndex(num);
        bool flag = false;
        if (sourceExitPointIndex != null)
          flag = sourceExitPointIndex.DestState != null;
        currSpeechReplyTextGuids.Add(new KeyValuePair<ulong, bool>(reply.Text.BaseGuid, flag));
      }
      IGameString text = speech.Text;
      if (speech.TextParam != null)
      {
        if (((VMParameter) speech.TextParam).Parent != null)
        {
          if (typeof (VMLogicObject).IsAssignableFrom(((VMParameter) speech.TextParam).Parent.GetType()))
          {
            IParam contextParam = ((VMVariableService) IVariableService.Instance).GetDynamicContext((IContext) ((VMParameter) speech.TextParam).Parent, fsm).GetContextParam(((VMParameter) speech.TextParam).BaseGuid);
            if (contextParam != null)
            {
              if (typeof (ITextRef).IsAssignableFrom(contextParam.Type.BaseType) && contextParam.Value != null)
                text = ((ITextRef) contextParam.Value).Text;
              else
                Logger.AddError(string.Format("Invalid text param {0} value type in speech {1}", contextParam.Name, speech.Name));
            }
            else
              Logger.AddError(string.Format("Dynamic param with name {0} not found in object {1}", speech.TextParam.Name, speech.Name));
          }
          else
            Logger.AddError(string.Format("Text param {0} parent is invalid", speech.TextParam.Name));
        }
        else
          Logger.AddError(string.Format("Text param {0} parent isn' t defined", speech.TextParam.Name));
      }
      ProcessActionLine(speech.ActionLine);
      CheckLoopStackClear();
      if (!TalkingFSM.Speaking.MakePlayerSpeech(text.BaseGuid, currSpeechReplyTextGuids, author1))
        return false;
      if (speech.OnlyOnce)
        canceledSpeechGuids.Add(speech.BaseGuid);
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
            return CheckTalkingStateActual(((IFiniteStateMachine) talkingState).InitState);
          return TalkingFSM.Speaking.CurrentTalking != null ? CheckTalkingStateActual(TalkingFSM.Speaking.CurrentTalking.State, bFromBranch) : new KeyValuePair<bool, bool>(false, false);
        }
        if (typeof (ISpeech).IsAssignableFrom(talkingState.GetType()))
        {
          VMSpeech vmSpeech = (VMSpeech) talkingState;
          debugCurrentState = vmSpeech;
          if (TalkingFSM.Speaking.IsTalkingOnlyOncePassed((IFiniteStateMachine) vmSpeech.Parent))
            return new KeyValuePair<bool, bool>(false, false);
          bool flag = false;
          if (vmSpeech.OnlyOnce && canceledSpeechGuids.Contains(vmSpeech.BaseGuid))
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
              return CheckTalkingStateActual(outputLink.DestState);
          }
        }
        else if (typeof (IBranch).IsAssignableFrom(talkingState.GetType()))
        {
          VMBranch vmBranch = (VMBranch) talkingState;
          debugCurrentState = vmBranch;
          if (vmBranch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH || vmBranch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH)
          {
            object secondValue = 0;
            int iSrcPntIndex = -1;
            if (vmBranch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH)
              secondValue = float.MaxValue;
            else if (vmBranch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH)
              secondValue = float.MinValue;
            for (int exitPntIndex = 0; exitPntIndex < vmBranch.GetExitPointsCount(); ++exitPntIndex)
            {
              VMPartCondition branchCondition = (VMPartCondition) vmBranch.GetBranchCondition(exitPntIndex);
              if (branchCondition != null)
              {
                VMExpression firstExpression = (VMExpression) branchCondition.FirstExpression;
                if (firstExpression != null)
                {
                  object expressionResult = ExpressionUtility.CalculateExpressionResult(firstExpression, fsm);
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
                  Logger.AddError(string.Format("MinMax branch {0} expression not defined at {0}", DynamicFSM.CurrentStateInfo));
              }
            }
            if (iSrcPntIndex >= 0)
            {
              VMEventLink sourceExitPointIndex = vmBranch.GetOutputLinkBySourceExitPointIndex(iSrcPntIndex);
              if (sourceExitPointIndex == null)
                return new KeyValuePair<bool, bool>(false, false);
              if (sourceExitPointIndex.DestState == null)
                return new KeyValuePair<bool, bool>(false, false);
              KeyValuePair<bool, bool> keyValuePair = CheckTalkingStateActual(sourceExitPointIndex.DestState, true);
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
              if (index >= vmBranch.GetExitPointsCount() - 1 || ExpressionUtility.CalculateConditionResult(branchCondition, fsm))
              {
                VMEventLink sourceExitPointIndex = vmBranch.GetOutputLinkBySourceExitPointIndex(index);
                if (sourceExitPointIndex == null)
                  return new KeyValuePair<bool, bool>(false, false);
                if (sourceExitPointIndex.DestState == null)
                  return new KeyValuePair<bool, bool>(false, false);
                KeyValuePair<bool, bool> keyValuePair = CheckTalkingStateActual(sourceExitPointIndex.DestState, true);
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
              return CheckTalkingStateActual(afterExitLink.DestState);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Checking talking state actual error at {0}, error: {1}", TalkingFSM.FSMStaticObject.Name, ex));
      }
      return new KeyValuePair<bool, bool>(false, false);
    }

    protected override bool IsThisTalking => talking;

    protected override bool IgnoreBranchCase(IState newState)
    {
      bool flag = false;
      VMTalkingGraph vmTalkingGraph = TalkingFSM.CurrentTalkingGraph;
      if (vmTalkingGraph == null && CurrentState != null && typeof (VMTalkingGraph) == CurrentState.GetType())
        vmTalkingGraph = (VMTalkingGraph) CurrentState;
      if (vmTalkingGraph != null && newState != null && typeof (VMSpeech) == newState.GetType())
        flag = canceledSpeechGuids.Contains(((VMBaseObject) newState).BaseGuid);
      return flag;
    }

    protected override void ReturnToOuterGraph(VMEventLink prevLink = null)
    {
      if (IsThisTalking)
        StopTalking();
      else
        base.ReturnToOuterGraph(prevLink);
    }

    private DynamicTalkingFSM TalkingFSM => (DynamicTalkingFSM) fsm;
  }
}
