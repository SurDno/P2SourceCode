// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.VMDebug.ReciveIpcMessage
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common.VMDebug
{
  public class ReciveIpcMessage
  {
    private EDebugIPCMessageType messageType;
    private Guid debugObjectEngineGuid;
    private string debugObjectUniName;
    private ulong stateGuid;
    private List<ulong> staticGuidsList = new List<ulong>();
    private List<ulong> paramsChanged = new List<ulong>();
    private List<object> paramsValues = new List<object>();
    private string lastError = "";

    public EDebugIPCMessageType MessageType => this.messageType;

    public Guid DebugObjectEngGuid
    {
      get => this.debugObjectEngineGuid;
      set => this.debugObjectEngineGuid = value;
    }

    public string DebugObjectUniName
    {
      get => this.debugObjectUniName;
      set => this.debugObjectUniName = value;
    }

    public ulong StateGuid
    {
      get => this.stateGuid;
      set => this.stateGuid = value;
    }

    public List<ulong> RaisedEvents => this.staticGuidsList;

    public List<ulong> ChangedParams => this.paramsChanged;

    public List<object> ChangedParamValues => this.paramsValues;

    public void Deserialize(byte[] data, int length)
    {
      try
      {
        if (length < 4)
        {
          this.lastError = string.Format("Cannot read ipc message: invalid data length");
        }
        else
        {
          int offset = 0;
          this.messageType = (EDebugIPCMessageType) IpcMessageUtility.DeserializeInt(data, ref offset);
          if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_INTO || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_OVER || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT)
          {
            if (length <= offset + 4)
            {
              this.lastError = string.Format("Cannot read ipc message: invalid data length");
              return;
            }
            this.debugObjectEngineGuid = IpcMessageUtility.DeserializeGuid(data, ref offset);
          }
          if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_UPDATE_OBJECTS_END)
            this.stateGuid = IpcMessageUtility.DeserializeUInt64(data, ref offset);
          else if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE)
          {
            if (length < offset + 8)
            {
              this.lastError = string.Format("Cannot read ipc message: invalid object engine guid data");
              return;
            }
            this.stateGuid = IpcMessageUtility.DeserializeUInt64(data, ref offset);
          }
          if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE)
          {
            if (length < offset + 4)
            {
              this.lastError = string.Format("Cannot read ipc message: invalid raised events count data");
              return;
            }
            this.staticGuidsList = IpcMessageUtility.DeSerializeUInt64List(data, ref offset);
          }
          if (this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE || this.messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE)
          {
            if (length < offset + 4)
            {
              this.lastError = string.Format("Cannot read ipc message: invalid changed params count data");
              return;
            }
            int num1 = IpcMessageUtility.DeserializeInt(data, ref offset);
            for (int index = 0; index < num1; ++index)
            {
              ulong num2 = IpcMessageUtility.DeserializeUInt64(data, ref offset);
              try
              {
                Type valueType = PLVirtualMachine.Common.Data.StringSerializer.ReadTypeByString(IpcMessageUtility.DeserializeString(data, ref offset));
                if (!((Type) null != valueType))
                  return;
                this.paramsValues.Add(IpcMessageUtility.DeserializeValue(data, ref offset, valueType));
                this.paramsChanged.Add(num2);
              }
              catch (Exception ex)
              {
                return;
              }
            }
          }
          if (this.messageType != EDebugIPCMessageType.IPC_MESSAGE_REQUEST_RAISE_EVENT)
            return;
          this.debugObjectUniName = IpcMessageUtility.DeserializeString(data, ref offset);
          this.stateGuid = IpcMessageUtility.DeserializeUInt64(data, ref offset);
        }
      }
      catch (Exception ex)
      {
        this.lastError = string.Format("Ipc message deserialize error: {0}", (object) ex.ToString());
      }
    }

    public string LastError => this.lastError;

    public bool IsValid => this.lastError.Length == 0;
  }
}
