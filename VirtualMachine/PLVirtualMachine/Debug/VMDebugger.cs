using Cofe.Loggers;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.VMDebug;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.FSM;
using PLVirtualMachine.GameLogic;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Debug
{
  public class VMDebugger : DebugServer
  {
    private bool needUpdateHierarchy;
    private Dictionary<Guid, DebugFSMInfo> fsmObjects = new Dictionary<Guid, DebugFSMInfo>((IEqualityComparer<Guid>) GuidComparer.Instance);
    private List<Guid> objectsSendedToClient = new List<Guid>();
    private object fsmDebugObjectsLocker = new object();
    private object raisingEventsLocker = new object();
    private Dictionary<ulong, Guid> debuggingObjectsByStaticFSM = new Dictionary<ulong, Guid>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private HashSet<Guid> currentDebuggingObjects = new HashSet<Guid>((IEqualityComparer<Guid>) GuidComparer.Instance);
    private Queue<DebugRaisingEventInfo> debugRaisingEventQueue = new Queue<DebugRaisingEventInfo>();

    public void OnTick()
    {
      if (this.ControllerWorkMode == EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY)
        return;
      this.UpdateDebugInfoFromClient();
      this.ProcessDebug();
      this.ProcessErrors();
    }

    protected override void SetWorkMode(EDebugIPCApplicationWorkMode workMode)
    {
      switch (workMode)
      {
        case EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY:
          if (this.ControllerWorkMode != EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY)
          {
            this.objectsSendedToClient.Clear();
            this.debuggingObjectsByStaticFSM.Clear();
            this.currentDebuggingObjects.Clear();
            break;
          }
          break;
        case EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG:
          lock (this.fsmDebugObjectsLocker)
          {
            foreach (KeyValuePair<Guid, DebugFSMInfo> fsmObject in this.fsmObjects)
            {
              DebugFSMInfo debugFsmInfo = fsmObject.Value;
              debugFsmInfo.CurrentPosition = debugFsmInfo.FSM.DebugCurrState;
              debugFsmInfo.CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_PASS;
              debugFsmInfo.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY;
            }
          }
          using (Dictionary<Guid, DebugFSMInfo>.Enumerator enumerator = this.fsmObjects.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              DebugFSMInfo debugFsmInfo = enumerator.Current.Value;
              if (!this.objectsSendedToClient.Contains(debugFsmInfo.FSM.Entity.EngineGuid))
                this.AddMessageToClient(new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT)
                {
                  DebugObjectEngGuid = debugFsmInfo.FSM.Entity.EngineGuid,
                  StateGuid = debugFsmInfo.FSM.FSMStaticObject.StateGraph.BaseGuid
                });
            }
            break;
          }
      }
      base.SetWorkMode(workMode);
    }

    public void OnAddObject(DynamicFSM fsm)
    {
      IFiniteStateMachine stateGraph = fsm.FSMStaticObject.StateGraph;
      if (stateGraph == null)
        return;
      try
      {
        DebugFSMInfo debugFsmInfo = new DebugFSMInfo(fsm);
        lock (this.fsmDebugObjectsLocker)
        {
          if (!this.fsmObjects.ContainsKey(fsm.Entity.EngineGuid))
            this.fsmObjects.Add(fsm.Entity.EngineGuid, debugFsmInfo);
          else
            this.OnError(string.Format("Dynamic object {0} guid {1} dublication at debugger registration", (object) fsm.Entity.Name, (object) fsm.Entity.EngineGuid));
        }
        SendIpcMessage messageToClient = new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT);
        messageToClient.DebugObjectEngGuid = fsm.Entity.EngineGuid;
        messageToClient.StateGuid = stateGraph.BaseGuid;
        this.objectsSendedToClient.Add(fsm.Entity.EngineGuid);
        this.AddMessageToClient(messageToClient);
      }
      catch (Exception ex)
      {
        this.OnError(ex.ToString());
      }
    }

    public bool OnStateInput(DynamicFSM fsm, IState state)
    {
      Guid engineGuid = fsm.Entity.EngineGuid;
      bool flag1 = false;
      lock (this.fsmDebugObjectsLocker)
      {
        if (this.fsmObjects.ContainsKey(engineGuid))
        {
          DebugFSMInfo fsmObject = this.fsmObjects[engineGuid];
          if (fsmObject.CurrentStatus == EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY)
          {
            IFiniteStateMachine parent = (IFiniteStateMachine) ((VMBaseObject) state).Parent;
            int num1 = this.IsAllDebugging(parent.BaseGuid) ? 1 : 0;
            bool flag2 = this.IsObjectUnderDebug(parent.BaseGuid, engineGuid);
            int num2 = this.IsObjectUnderDebug(parent.BaseGuid, engineGuid, true) ? 1 : 0;
            fsmObject.CurrentPosition = state;
            fsmObject.CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_PASS;
            if (num2 != 0)
              this.AddInputStateMessageToClient(engineGuid, state.BaseGuid);
            int num3 = flag2 ? 1 : 0;
            if ((num1 | num3) != 0)
            {
              if (fsmObject.Breakpoints.ContainsKey(state.BaseGuid))
              {
                fsmObject.CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_STAY;
                fsmObject.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_WAIT;
                flag1 = true;
              }
            }
          }
          else if (fsmObject.CurrentStatus == EDebugFSMStatus.DEBUG_FSM_STATUS_WAIT)
          {
            this.OnError(string.Format("Invalid state input: fsm {0} currently stepbystep blocked", (object) fsm.Entity.Name));
          }
          else
          {
            if (fsmObject.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_INTO)
            {
              if (fsmObject.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_OVER)
                goto label_16;
            }
            fsmObject.CurrentPosition = state;
            fsmObject.CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_STAY;
            fsmObject.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_WAIT;
            this.AddInputStateMessageToClient(engineGuid, state.BaseGuid);
            flag1 = true;
          }
        }
      }
label_16:
      return flag1;
    }

    public void OnParamValueChange(VMParameter param, object newValue, Guid objGuid)
    {
      if (newValue == null)
        return;
      object paramVal = newValue;
      Type type = paramVal.GetType();
      if (typeof (ICommonList).IsAssignableFrom(type))
        return;
      if (typeof (IRef).IsAssignableFrom(type))
        paramVal = (object) ((INamed) newValue).Name;
      else if (!type.IsValueType && !type.IsEnum && !(type == typeof (string)) && !(type == typeof (bool)) && !(type == typeof (GameTime)))
        return;
      if (!this.currentDebuggingObjects.Contains(objGuid))
        return;
      SendIpcMessage messageToClient = new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE);
      messageToClient.AddParamChange(param.BaseGuid, paramVal);
      messageToClient.DebugObjectEngGuid = objGuid;
      this.AddMessageToClient(messageToClient);
    }

    public bool OnStateExec(DynamicFSM fsm, IState state) => false;

    public bool NeedUpdateHierarchy
    {
      get => this.needUpdateHierarchy;
      set => this.needUpdateHierarchy = value;
    }

    public void Clear()
    {
      this.fsmObjects.Clear();
      this.debuggingObjectsByStaticFSM.Clear();
      this.currentDebuggingObjects.Clear();
      this.objectsSendedToClient.Clear();
      this.ClearMessages();
      if (this.ControllerWorkMode != EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG)
        return;
      this.AddMessageToClient(new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_STOP_DEBUG));
    }

    private void UpdateDebugInfoFromClient()
    {
      while (true)
      {
        ReciveIpcMessage reciveIpcMessage;
        do
        {
          reciveIpcMessage = this.PopMessageFromClient();
          if (reciveIpcMessage != null)
          {
            if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT)
              this.SetDebugObject(reciveIpcMessage.StateGuid, reciveIpcMessage.DebugObjectEngGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT)
              this.SetBreakpoint(reciveIpcMessage.DebugObjectEngGuid, reciveIpcMessage.StateGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT)
              this.RemoveBreakpoint(reciveIpcMessage.DebugObjectEngGuid, reciveIpcMessage.StateGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_INTO)
              this.StepInto(reciveIpcMessage.DebugObjectEngGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_OVER)
              this.StepOver(reciveIpcMessage.DebugObjectEngGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_CONTINUE)
              this.StepContinue(reciveIpcMessage.DebugObjectEngGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA)
              this.SetDebugCamera(reciveIpcMessage.DebugObjectEngGuid);
          }
          else
            goto label_17;
        }
        while (reciveIpcMessage.MessageType != EDebugIPCMessageType.IPC_MESSAGE_REQUEST_RAISE_EVENT);
        this.AddRaisingEvent(reciveIpcMessage.DebugObjectUniName, reciveIpcMessage.StateGuid);
      }
label_17:;
    }

    private void ProcessDebug()
    {
      lock (this.fsmDebugObjectsLocker)
      {
        foreach (KeyValuePair<Guid, DebugFSMInfo> fsmObject in this.fsmObjects)
        {
          DebugFSMInfo debugFsmInfo = fsmObject.Value;
          if (debugFsmInfo.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY && debugFsmInfo.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_INTO && debugFsmInfo.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_OVER)
          {
            int currentStatus = (int) debugFsmInfo.CurrentStatus;
          }
        }
      }
      lock (this.raisingEventsLocker)
      {
        while (this.debugRaisingEventQueue.Count != 0)
        {
          DebugRaisingEventInfo raisingEventInfo;
          try
          {
            raisingEventInfo = this.debugRaisingEventQueue.Dequeue();
          }
          catch (Exception ex)
          {
            break;
          }
          VMEntity objectEntityByUniName = WorldEntityUtility.GetDynamicObjectEntityByUniName(raisingEventInfo.EventOwnerUniName);
          if (objectEntityByUniName == null)
          {
            this.OnError(string.Format("Cannot raise debug event: object with dynamic id={0} not found", (object) raisingEventInfo.EventOwnerUniName));
            break;
          }
          DynamicEvent eventByStaticGuid = objectEntityByUniName.GetFSM().GetEventByStaticGuid(raisingEventInfo.EventGuid);
          if (eventByStaticGuid == null)
          {
            this.OnError(string.Format("Cannot raise debug event on object {0}: event with id={1} not found", (object) objectEntityByUniName.EditorTemplate.Name, (object) raisingEventInfo.EventGuid));
            break;
          }
          List<EventMessage> raisingEventMessageList = new List<EventMessage>();
          eventByStaticGuid.Raise(raisingEventMessageList, EEventRaisingMode.ERM_ADD_TO_QUEUE, Guid.Empty);
          if (this.debugRaisingEventQueue.Count <= 0)
            break;
        }
      }
    }

    private void ProcessErrors()
    {
      List<string> lastErrors = this.LastErrors;
      this.ResetErrors();
      for (int index = 0; index < lastErrors.Count; ++index)
        Logger.AddError("VMDebugger error: " + lastErrors[index]);
    }

    private void SetDebugObject(ulong graphId, Guid setObGuid)
    {
      if (!this.debuggingObjectsByStaticFSM.ContainsKey(graphId))
        this.debuggingObjectsByStaticFSM.Add(graphId, Guid.Empty);
      this.debuggingObjectsByStaticFSM[graphId] = setObGuid;
      this.currentDebuggingObjects.Clear();
      foreach (KeyValuePair<ulong, Guid> keyValuePair in this.debuggingObjectsByStaticFSM)
        this.currentDebuggingObjects.Add(keyValuePair.Value);
      if (setObGuid == Guid.Empty)
        return;
      lock (this.fsmDebugObjectsLocker)
      {
        if (!this.fsmObjects.ContainsKey(setObGuid))
        {
          this.OnError(string.Format("Object id={0} not created in debugger", (object) setObGuid));
        }
        else
        {
          if (this.fsmObjects[setObGuid].CurrentPosition == null)
            return;
          if (this.fsmObjects[setObGuid].CurrentPosition.Parent == null)
          {
            this.OnError(string.Format("Current position state {0} not created in debugger", (object) this.fsmObjects[setObGuid].CurrentPosition.Name));
          }
          else
          {
            IFiniteStateMachine parent = (IFiniteStateMachine) this.fsmObjects[setObGuid].CurrentPosition.Parent;
            IEnumerable<DynamicParameter> fsmDynamicParams = this.fsmObjects[setObGuid].FSM.FSMDynamicParams;
            this.AddInputStateMessageToClient(setObGuid, this.fsmObjects[setObGuid].CurrentPosition.BaseGuid, fsmDynamicParams);
          }
        }
      }
    }

    private void SetBreakpoint(Guid objGuid, ulong stateId)
    {
      lock (this.fsmDebugObjectsLocker)
      {
        if (this.fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = this.fsmObjects[objGuid];
          IState stateByGuid = ((FiniteStateMachine) fsmObject.FSM.FSMStaticObject.StateGraph).GetStateByGuid(stateId, false);
          if (stateByGuid == null)
            this.OnError(string.Format("State id={0} not found in object {1} state graph", (object) stateId, (object) objGuid));
          else
            fsmObject.Breakpoints.Add(stateId, stateByGuid);
        }
        else
          this.OnError(string.Format("Object id={0} not created in debugger", (object) objGuid));
      }
    }

    private void RemoveBreakpoint(Guid objGuid, ulong stateId)
    {
      lock (this.fsmDebugObjectsLocker)
      {
        if (this.fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = this.fsmObjects[objGuid];
          if (!fsmObject.Breakpoints.ContainsKey(stateId))
            this.OnError(string.Format("Breakpoint state id={0} not found in object {1} debug fsm", (object) stateId, (object) objGuid));
          else
            fsmObject.Breakpoints.Remove(stateId);
        }
        else
          this.OnError(string.Format("Object id={0} not created in debugger", (object) objGuid));
      }
    }

    private void StepInto(Guid objGuid)
    {
      lock (this.fsmDebugObjectsLocker)
      {
        if (this.fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = this.fsmObjects[objGuid];
          if (fsmObject.CurrentPosition != null)
          {
            if (fsmObject.CurrentStatus == EDebugFSMStatus.DEBUG_FSM_STATUS_WAIT)
              fsmObject.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_INTO;
            else
              this.OnError(string.Format("Inconsistent step into command in object {0}", (object) objGuid));
          }
          else
            this.OnError(string.Format("Current debug state position isn't defined in object {0}", (object) objGuid));
        }
        else
          this.OnError(string.Format("Object id={0} not created in debugger", (object) objGuid));
      }
    }

    private void StepOver(Guid objGuid)
    {
      lock (this.fsmDebugObjectsLocker)
      {
        if (this.fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = this.fsmObjects[objGuid];
          if (fsmObject.CurrentPosition != null)
          {
            if (fsmObject.CurrentStatus == EDebugFSMStatus.DEBUG_FSM_STATUS_WAIT)
              fsmObject.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_OVER;
            else
              this.OnError(string.Format("Inconsistent step over command in object {0}", (object) objGuid));
          }
          else
            this.OnError(string.Format("Current debug state position isn't defined in object {0}", (object) objGuid));
        }
        else
          this.OnError(string.Format("Object id={0} not created in debugger", (object) objGuid));
      }
    }

    private void StepContinue(Guid objGuid)
    {
      lock (this.fsmDebugObjectsLocker)
      {
        if (this.fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = this.fsmObjects[objGuid];
          if (fsmObject.CurrentPosition != null)
          {
            if (fsmObject.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY)
              fsmObject.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_CONTINUE;
            else
              this.OnError(string.Format("Inconsistent continue command in object {0}", (object) objGuid));
          }
          else
            this.OnError(string.Format("Current debug state position isn't defined in object {0}", (object) objGuid));
        }
        else
          this.OnError(string.Format("Object id={0} not created in debugger", (object) objGuid));
      }
    }

    private void SetDebugCamera(Guid objGuid)
    {
    }

    private void AddRaisingEvent(string sObjUniName, ulong stateGuid)
    {
      DebugRaisingEventInfo raisingEventInfo = new DebugRaisingEventInfo(sObjUniName, stateGuid);
      lock (this.raisingEventsLocker)
        this.debugRaisingEventQueue.Enqueue(raisingEventInfo);
    }

    private void AddInputStateMessageToClient(
      Guid objGuid,
      ulong stateGuid,
      IEnumerable<DynamicParameter> currentParams = null)
    {
      SendIpcMessage messageToClient = new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE);
      messageToClient.DebugObjectEngGuid = objGuid;
      messageToClient.StateGuid = stateGuid;
      if (currentParams != null)
      {
        foreach (DynamicParameter currentParam in currentParams)
        {
          if ((!currentParam.Implicit || !currentParam.Name.Contains("_Self")) && (currentParam.Entity == null || !currentParam.Entity.IsDisposed))
          {
            object name = currentParam.Value;
            if (name != null)
            {
              Type type = name.GetType();
              if (!typeof (ICommonList).IsAssignableFrom(type))
              {
                if (typeof (IRef).IsAssignableFrom(type))
                  name = (object) ((INamed) name).Name;
                else if (!type.IsValueType && !type.IsEnum && !(type == typeof (string)) && !(type == typeof (bool)))
                  continue;
                messageToClient.AddParamChange(currentParam.StaticObject.BaseGuid, name);
              }
            }
          }
        }
      }
      this.AddMessageToClient(messageToClient);
    }

    private bool IsObjectUnderDebug(ulong staticFSMGuid, Guid objGuid, bool allGraphes = false)
    {
      if (this.debuggingObjectsByStaticFSM.ContainsKey(staticFSMGuid) && this.debuggingObjectsByStaticFSM[staticFSMGuid] == objGuid)
        return true;
      return allGraphes && this.debuggingObjectsByStaticFSM.ContainsValue(objGuid);
    }

    private bool IsAllDebugging(ulong staticFSMGuid)
    {
      return !this.debuggingObjectsByStaticFSM.ContainsKey(staticFSMGuid) || this.debuggingObjectsByStaticFSM[staticFSMGuid] == Guid.Empty;
    }
  }
}
