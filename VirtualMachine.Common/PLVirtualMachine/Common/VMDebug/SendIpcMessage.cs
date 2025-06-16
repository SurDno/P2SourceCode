using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common.VMDebug;

public class SendIpcMessage {
	private EDebugIPCMessageType messageType;
	private Guid debugObjectEngineGuid;
	private string debugObjectUniName;
	private ulong stateGuid;
	private List<ulong> staticGuidsList = new();
	private List<ulong> paramsChanged = new();
	private List<object> paramsValues = new();
	private string lastError = "";

	public SendIpcMessage(EDebugIPCMessageType messageType) {
		this.messageType = messageType;
	}

	public void CopyData(ReciveIpcMessage message) {
		debugObjectEngineGuid = message.DebugObjectEngGuid;
		stateGuid = message.StateGuid;
		staticGuidsList.Clear();
		for (var index = 0; index < message.RaisedEvents.Count; ++index)
			staticGuidsList.Add(message.RaisedEvents[index]);
		paramsChanged.Clear();
		for (var index = 0; index < message.ChangedParams.Count; ++index)
			AddParamChange(message.ChangedParams[index], message.ChangedParamValues[index]);
	}

	public Guid DebugObjectEngGuid {
		get => debugObjectEngineGuid;
		set => debugObjectEngineGuid = value;
	}

	public ulong StateGuid {
		get => stateGuid;
		set => stateGuid = value;
	}

	public void AddParamChange(ulong paramId, object paramVal) {
		paramsChanged.Add(paramId);
		paramsValues.Add(paramVal);
	}

	public byte[] Serialize() {
		try {
			var dDestBytesList = new List<byte>();
			IpcMessageUtility.SerializeInt((int)messageType, dDestBytesList);
			if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_BREAKPOINT ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_INTO ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_OVER ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA)
				IpcMessageUtility.SerializeGuid(debugObjectEngineGuid, dDestBytesList);
			if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_UPDATE_OBJECTS_END)
				IpcMessageUtility.SerializeUInt64(stateGuid, dDestBytesList);
			else if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_BREAKPOINT ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE ||
			         messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE)
				IpcMessageUtility.SerializeUInt64(stateGuid, dDestBytesList);
			if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE)
				IpcMessageUtility.SerializeUInt64List(staticGuidsList, dDestBytesList);
			if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE ||
			    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE) {
				IpcMessageUtility.SerializeInt(paramsChanged.Count, dDestBytesList);
				for (var index = 0; index < paramsChanged.Count; ++index)
					if (paramsValues[index] != null) {
						IpcMessageUtility.SerializeUInt64(paramsChanged[index], dDestBytesList);
						IpcMessageUtility.SerializeString(paramsValues[index].GetType().ToString(), dDestBytesList);
						IpcMessageUtility.SerializeValue(paramsValues[index], dDestBytesList);
					}
			}

			if (messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_RAISE_EVENT) {
				IpcMessageUtility.SerializeString(debugObjectUniName, dDestBytesList);
				IpcMessageUtility.SerializeUInt64(stateGuid, dDestBytesList);
			}

			var numArray = new byte[dDestBytesList.Count];
			for (var index = 0; index < dDestBytesList.Count; ++index)
				numArray[index] = dDestBytesList[index];
			return numArray;
		} catch (Exception ex) {
			lastError = string.Format("Ipc message serialize error: {0}", ex);
			return null;
		}
	}

	public string LastError => lastError;

	public bool IsValid => lastError.Length == 0;
}