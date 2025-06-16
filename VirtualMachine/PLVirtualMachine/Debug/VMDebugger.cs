using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.VMDebug;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.FSM;
using PLVirtualMachine.GameLogic;

namespace PLVirtualMachine.Debug
{
  public class VMDebugger : DebugServer
  {
    private bool needUpdateHierarchy;
    private Dictionary<Guid, DebugFSMInfo> fsmObjects = new Dictionary<Guid, DebugFSMInfo>(GuidComparer.Instance);
    private List<Guid> objectsSendedToClient = new List<Guid>();
    private object fsmDebugObjectsLocker = new object();
    private object raisingEventsLocker = new object();
    private Dictionary<ulong, Guid> debuggingObjectsByStaticFSM = new Dictionary<ulong, Guid>(UlongComparer.Instance);
    private HashSet<Guid> currentDebuggingObjects = new HashSet<Guid>(GuidComparer.Instance);
    private Queue<DebugRaisingEventInfo> debugRaisingEventQueue = new Queue<DebugRaisingEventInfo>();

    public void OnTick()
    {
      if (ControllerWorkMode == EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY)
        return;
      UpdateDebugInfoFromClient();
      ProcessDebug();
      ProcessErrors();
    }

    protected override void SetWorkMode(EDebugIPCApplicationWorkMode workMode)
    {
      switch (workMode)
      {
        case EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY:
          if (ControllerWorkMode != EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_PLAY)
          {
            objectsSendedToClient.Clear();
            debuggingObjectsByStaticFSM.Clear();
            currentDebuggingObjects.Clear();
          }
          break;
        case EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG:
          lock (fsmDebugObjectsLocker)
          {
            foreach (KeyValuePair<Guid, DebugFSMInfo> fsmObject in fsmObjects)
            {
              DebugFSMInfo debugFsmInfo = fsmObject.Value;
              debugFsmInfo.CurrentPosition = debugFsmInfo.FSM.DebugCurrState;
              debugFsmInfo.CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_PASS;
              debugFsmInfo.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY;
            }
          }
          using (Dictionary<Guid, DebugFSMInfo>.Enumerator enumerator = fsmObjects.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              DebugFSMInfo debugFsmInfo = enumerator.Current.Value;
              if (!objectsSendedToClient.Contains(debugFsmInfo.FSM.Entity.EngineGuid))
                AddMessageToClient(new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT)
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
        lock (fsmDebugObjectsLocker)
        {
          if (!fsmObjects.ContainsKey(fsm.Entity.EngineGuid))
            fsmObjects.Add(fsm.Entity.EngineGuid, debugFsmInfo);
          else
            OnError(string.Format("Dynamic object {0} guid {1} dublication at debugger registration", fsm.Entity.Name, fsm.Entity.EngineGuid));
        }
        SendIpcMessage messageToClient = new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT);
        messageToClient.DebugObjectEngGuid = fsm.Entity.EngineGuid;
        messageToClient.StateGuid = stateGraph.BaseGuid;
        objectsSendedToClient.Add(fsm.Entity.EngineGuid);
        AddMessageToClient(messageToClient);
      }
      catch (Exception ex)
      {
        OnError(ex.ToString());
      }
    }

    public bool OnStateInput(DynamicFSM fsm, IState state)
    {
      Guid engineGuid = fsm.Entity.EngineGuid;
      bool flag1 = false;
      lock (fsmDebugObjectsLocker)
      {
        if (fsmObjects.ContainsKey(engineGuid))
        {
          DebugFSMInfo fsmObject = fsmObjects[engineGuid];
          if (fsmObject.CurrentStatus == EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY)
          {
            IFiniteStateMachine parent = (IFiniteStateMachine) ((VMBaseObject) state).Parent;
            int num1 = IsAllDebugging(parent.BaseGuid) ? 1 : 0;
            bool flag2 = IsObjectUnderDebug(parent.BaseGuid, engineGuid);
            int num2 = IsObjectUnderDebug(parent.BaseGuid, engineGuid, true) ? 1 : 0;
            fsmObject.CurrentPosition = state;
            fsmObject.CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_PASS;
            if (num2 != 0)
              AddInputStateMessageToClient(engineGuid, state.BaseGuid);
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
            OnError(string.Format("Invalid state input: fsm {0} currently stepbystep blocked", fsm.Entity.Name));
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
            AddInputStateMessageToClient(engineGuid, state.BaseGuid);
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
        paramVal = ((INamed) newValue).Name;
      else if (!type.IsValueType && !type.IsEnum && !(type == typeof (string)) && !(type == typeof (bool)) && !(type == typeof (GameTime)))
        return;
      if (!currentDebuggingObjects.Contains(objGuid))
        return;
      SendIpcMessage messageToClient = new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE);
      messageToClient.AddParamChange(param.BaseGuid, paramVal);
      messageToClient.DebugObjectEngGuid = objGuid;
      AddMessageToClient(messageToClient);
    }

    public bool OnStateExec(DynamicFSM fsm, IState state) => false;

    public bool NeedUpdateHierarchy
    {
      get => needUpdateHierarchy;
      set => needUpdateHierarchy = value;
    }

    public void Clear()
    {
      fsmObjects.Clear();
      debuggingObjectsByStaticFSM.Clear();
      currentDebuggingObjects.Clear();
      objectsSendedToClient.Clear();
      ClearMessages();
      if (ControllerWorkMode != EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG)
        return;
      AddMessageToClient(new SendIpcMessage(EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_STOP_DEBUG));
    }

    private void UpdateDebugInfoFromClient()
    {
      while (true)
      {
        ReciveIpcMessage reciveIpcMessage;
        do
        {
          reciveIpcMessage = PopMessageFromClient();
          if (reciveIpcMessage != null)
          {
            if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT)
              SetDebugObject(reciveIpcMessage.StateGuid, reciveIpcMessage.DebugObjectEngGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT)
              SetBreakpoint(reciveIpcMessage.DebugObjectEngGuid, reciveIpcMessage.StateGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT)
              RemoveBreakpoint(reciveIpcMessage.DebugObjectEngGuid, reciveIpcMessage.StateGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_INTO)
              StepInto(reciveIpcMessage.DebugObjectEngGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_OVER)
              StepOver(reciveIpcMessage.DebugObjectEngGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_CONTINUE)
              StepContinue(reciveIpcMessage.DebugObjectEngGuid);
            else if (reciveIpcMessage.MessageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA)
              SetDebugCamera(reciveIpcMessage.DebugObjectEngGuid);
          }
          else
            goto label_17;
        }
        while (reciveIpcMessage.MessageType != EDebugIPCMessageType.IPC_MESSAGE_REQUEST_RAISE_EVENT);
        AddRaisingEvent(reciveIpcMessage.DebugObjectUniName, reciveIpcMessage.StateGuid);
      }
label_17:;
    }

    private void ProcessDebug()
    {
      lock (fsmDebugObjectsLocker)
      {
        foreach (KeyValuePair<Guid, DebugFSMInfo> fsmObject in fsmObjects)
        {
          DebugFSMInfo debugFsmInfo = fsmObject.Value;
          if (debugFsmInfo.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY && debugFsmInfo.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_INTO && debugFsmInfo.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_OVER)
          {
            int currentStatus = (int) debugFsmInfo.CurrentStatus;
          }
        }
      }
      lock (raisingEventsLocker)
      {
        while (debugRaisingEventQueue.Count != 0)
        {
          DebugRaisingEventInfo raisingEventInfo;
          try
          {
            raisingEventInfo = debugRaisingEventQueue.Dequeue();
          }
          catch (Exception ex)
          {
            break;
          }
          VMEntity objectEntityByUniName = WorldEntityUtility.GetDynamicObjectEntityByUniName(raisingEventInfo.EventOwnerUniName);
          if (objectEntityByUniName == null)
          {
            OnError(string.Format("Cannot raise debug event: object with dynamic id={0} not found", raisingEventInfo.EventOwnerUniName));
            break;
          }
          DynamicEvent eventByStaticGuid = objectEntityByUniName.GetFSM().GetEventByStaticGuid(raisingEventInfo.EventGuid);
          if (eventByStaticGuid == null)
          {
            OnError(string.Format("Cannot raise debug event on object {0}: event with id={1} not found", objectEntityByUniName.EditorTemplate.Name, raisingEventInfo.EventGuid));
            break;
          }
          List<EventMessage> raisingEventMessageList = new List<EventMessage>();
          eventByStaticGuid.Raise(raisingEventMessageList, EEventRaisingMode.ERM_ADD_TO_QUEUE, Guid.Empty);
          if (debugRaisingEventQueue.Count <= 0)
            break;
        }
      }
    }

    private void ProcessErrors()
    {
      List<string> lastErrors = LastErrors;
      ResetErrors();
      for (int index = 0; index < lastErrors.Count; ++index)
        Logger.AddError("VMDebugger error: " + lastErrors[index]);
    }

    private void SetDebugObject(ulong graphId, Guid setObGuid)
    {
      if (!debuggingObjectsByStaticFSM.ContainsKey(graphId))
        debuggingObjectsByStaticFSM.Add(graphId, Guid.Empty);
      debuggingObjectsByStaticFSM[graphId] = setObGuid;
      currentDebuggingObjects.Clear();
      foreach (KeyValuePair<ulong, Guid> keyValuePair in debuggingObjectsByStaticFSM)
        currentDebuggingObjects.Add(keyValuePair.Value);
      if (setObGuid == Guid.Empty)
        return;
      lock (fsmDebugObjectsLocker)
      {
        if (!fsmObjects.ContainsKey(setObGuid))
        {
          OnError(string.Format("Object id={0} not created in debugger", setObGuid));
        }
        else
        {
          if (fsmObjects[setObGuid].CurrentPosition == null)
            return;
          if (fsmObjects[setObGuid].CurrentPosition.Parent == null)
          {
            OnError(string.Format("Current position state {0} not created in debugger", fsmObjects[setObGuid].CurrentPosition.Name));
          }
          else
          {
            IFiniteStateMachine parent = (IFiniteStateMachine) fsmObjects[setObGuid].CurrentPosition.Parent;
            IEnumerable<DynamicParameter> fsmDynamicParams = fsmObjects[setObGuid].FSM.FSMDynamicParams;
            AddInputStateMessageToClient(setObGuid, fsmObjects[setObGuid].CurrentPosition.BaseGuid, fsmDynamicParams);
          }
        }
      }
    }

    private void SetBreakpoint(Guid objGuid, ulong stateId)
    {
      lock (fsmDebugObjectsLocker)
      {
        if (fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = fsmObjects[objGuid];
          IState stateByGuid = ((FiniteStateMachine) fsmObject.FSM.FSMStaticObject.StateGraph).GetStateByGuid(stateId, false);
          if (stateByGuid == null)
            OnError(string.Format("State id={0} not found in object {1} state graph", stateId, objGuid));
          else
            fsmObject.Breakpoints.Add(stateId, stateByGuid);
        }
        else
          OnError(string.Format("Object id={0} not created in debugger", objGuid));
      }
    }

    private void RemoveBreakpoint(Guid objGuid, ulong stateId)
    {
      lock (fsmDebugObjectsLocker)
      {
        if (fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = fsmObjects[objGuid];
          if (!fsmObject.Breakpoints.ContainsKey(stateId))
            OnError(string.Format("Breakpoint state id={0} not found in object {1} debug fsm", stateId, objGuid));
          else
            fsmObject.Breakpoints.Remove(stateId);
        }
        else
          OnError(string.Format("Object id={0} not created in debugger", objGuid));
      }
    }

    private void StepInto(Guid objGuid)
    {
      lock (fsmDebugObjectsLocker)
      {
        if (fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = fsmObjects[objGuid];
          if (fsmObject.CurrentPosition != null)
          {
            if (fsmObject.CurrentStatus == EDebugFSMStatus.DEBUG_FSM_STATUS_WAIT)
              fsmObject.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_INTO;
            else
              OnError(string.Format("Inconsistent step into command in object {0}", objGuid));
          }
          else
            OnError(string.Format("Current debug state position isn't defined in object {0}", objGuid));
        }
        else
          OnError(string.Format("Object id={0} not created in debugger", objGuid));
      }
    }

    private void StepOver(Guid objGuid)
    {
      lock (fsmDebugObjectsLocker)
      {
        if (fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = fsmObjects[objGuid];
          if (fsmObject.CurrentPosition != null)
          {
            if (fsmObject.CurrentStatus == EDebugFSMStatus.DEBUG_FSM_STATUS_WAIT)
              fsmObject.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_STEP_OVER;
            else
              OnError(string.Format("Inconsistent step over command in object {0}", objGuid));
          }
          else
            OnError(string.Format("Current debug state position isn't defined in object {0}", objGuid));
        }
        else
          OnError(string.Format("Object id={0} not created in debugger", objGuid));
      }
    }

    private void StepContinue(Guid objGuid)
    {
      lock (fsmDebugObjectsLocker)
      {
        if (fsmObjects.ContainsKey(objGuid))
        {
          DebugFSMInfo fsmObject = fsmObjects[objGuid];
          if (fsmObject.CurrentPosition != null)
          {
            if (fsmObject.CurrentStatus != EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY)
              fsmObject.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_STEPBYSTEP_CONTINUE;
            else
              OnError(string.Format("Inconsistent continue command in object {0}", objGuid));
          }
          else
            OnError(string.Format("Current debug state position isn't defined in object {0}", objGuid));
        }
        else
          OnError(string.Format("Object id={0} not created in debugger", objGuid));
      }
    }

    private void SetDebugCamera(Guid objGuid)
    {
    }

    private void AddRaisingEvent(string sObjUniName, ulong stateGuid)
    {
      DebugRaisingEventInfo raisingEventInfo = new DebugRaisingEventInfo(sObjUniName, stateGuid);
      lock (raisingEventsLocker)
        debugRaisingEventQueue.Enqueue(raisingEventInfo);
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
                  name = ((INamed) name).Name;
                else if (!type.IsValueType && !type.IsEnum && !(type == typeof (string)) && !(type == typeof (bool)))
                  continue;
                messageToClient.AddParamChange(currentParam.StaticObject.BaseGuid, name);
              }
            }
          }
        }
      }
      AddMessageToClient(messageToClient);
    }

    private bool IsObjectUnderDebug(ulong staticFSMGuid, Guid objGuid, bool allGraphes = false)
    {
      if (debuggingObjectsByStaticFSM.ContainsKey(staticFSMGuid) && debuggingObjectsByStaticFSM[staticFSMGuid] == objGuid)
        return true;
      return allGraphes && debuggingObjectsByStaticFSM.ContainsValue(objGuid);
    }

    private bool IsAllDebugging(ulong staticFSMGuid)
    {
      return !debuggingObjectsByStaticFSM.ContainsKey(staticFSMGuid) || debuggingObjectsByStaticFSM[staticFSMGuid] == Guid.Empty;
    }
  }
}
