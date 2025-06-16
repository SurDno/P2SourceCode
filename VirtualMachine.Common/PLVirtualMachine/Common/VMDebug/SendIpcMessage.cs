using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common.VMDebug
{
  public class SendIpcMessage
  {
    private EDebugIPCMessageType messageType;
    private Guid debugObjectEngineGuid;
    private string debugObjectUniName;
    private ulong stateGuid;
    private List<ulong> staticGuidsList = new List<ulong>();
    private List<ulong> paramsChanged = new List<ulong>();
    private List<object> paramsValues = new List<object>();
    private string lastError = "";

    public SendIpcMessage(EDebugIPCMessageType messageType) => this.messageType = messageType;

    public void CopyData(ReciveIpcMessage message)
    {
      this.debugObjectEngineGuid = message.DebugObjectEngGuid;
      this.stateGuid = message.StateGuid;
      this.staticGuidsList.Clear();
      for (int index = 0; index < message.RaisedEvents.Count; ++index)
        this.staticGuidsList.Add(message.RaisedEvents[index]);
      this.paramsChanged.Clear();
      for (int index = 0; index < message.ChangedParams.Count; ++index)
        this.AddParamChange(message.ChangedParams[index], message.ChangedParamValues[index]);
    }

    public Guid DebugObjectEngGuid
    {
      get => this.debugObjectEngineGuid;
      set => this.debugObjectEngineGuid = value;
    }

    public ulong StateGuid
    {
      get => this.stateGuid;
      set => this.stateGuid = value;
    }

    public void AddParamChange(ulong paramId, object paramVal)
    {
      this.paramsChanged.Add(paramId);
      this.paramsValues.Add(paramVal);
    }

    public byte[] Serialize()
    {
      try
      {
        List<byte> dDestBytesList = new List<byte>();
        IpcMessageUtility.SerializeInt((int) this.messageType, dDestBytesList);
        if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_INTO || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_OVER || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA)
          IpcMessageUtility.SerializeGuid(this.debugObjectEngineGuid, dDestBytesList);
        if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_UPDATE_OBJECTS_END)
          IpcMessageUtility.SerializeUInt64(this.stateGuid, dDestBytesList);
        else if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE)
          IpcMessageUtility.SerializeUInt64(this.stateGuid, dDestBytesList);
        if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE)
          IpcMessageUtility.SerializeUInt64List(this.staticGuidsList, dDestBytesList);
        if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE)
        {
          IpcMessageUtility.SerializeInt(this.paramsChanged.Count, dDestBytesList);
          for (int index = 0; index < this.paramsChanged.Count; ++index)
          {
            if (this.paramsValues[index] != null)
            {
              IpcMessageUtility.SerializeUInt64(this.paramsChanged[index], dDestBytesList);
              IpcMessageUtility.SerializeString(this.paramsValues[index].GetType().ToString(), dDestBytesList);
              IpcMessageUtility.SerializeValue(this.paramsValues[index], dDestBytesList);
            }
          }
        }
        if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_RAISE_EVENT)
        {
          IpcMessageUtility.SerializeString(this.debugObjectUniName, dDestBytesList);
          IpcMessageUtility.SerializeUInt64(this.stateGuid, dDestBytesList);
        }
        byte[] numArray = new byte[dDestBytesList.Count];
        for (int index = 0; index < dDestBytesList.Count; ++index)
          numArray[index] = dDestBytesList[index];
        return numArray;
      }
      catch (Exception ex)
      {
        this.lastError = string.Format("Ipc message serialize error: {0}", (object) ex.ToString());
        return (byte[]) null;
      }
    }

    public string LastError => this.lastError;

    public bool IsValid => this.lastError.Length == 0;
  }
}
