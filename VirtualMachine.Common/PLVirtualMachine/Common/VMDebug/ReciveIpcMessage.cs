using System;
using System.Collections.Generic;
using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common.VMDebug;

public class ReciveIpcMessage {
	private EDebugIPCMessageType messageType;
	private Guid debugObjectEngineGuid;
	private string debugObjectUniName;
	private ulong stateGuid;
	private List<ulong> staticGuidsList = new();
	private List<ulong> paramsChanged = new();
	private List<object> paramsValues = new();
	private string lastError = "";

	public EDebugIPCMessageType MessageType => messageType;

	public Guid DebugObjectEngGuid {
		get => debugObjectEngineGuid;
		set => debugObjectEngineGuid = value;
	}

	public string DebugObjectUniName {
		get => debugObjectUniName;
		set => debugObjectUniName = value;
	}

	public ulong StateGuid {
		get => stateGuid;
		set => stateGuid = value;
	}

	public List<ulong> RaisedEvents => staticGuidsList;

	public List<ulong> ChangedParams => paramsChanged;

	public List<object> ChangedParamValues => paramsValues;

	public void Deserialize(byte[] data, int length) {
		try {
			if (length < 4)
				lastError = "Cannot read ipc message: invalid data length";
			else {
				var offset = 0;
				messageType = (EDebugIPCMessageType)IpcMessageUtility.DeserializeInt(data, ref offset);
				if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_BREAKPOINT ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_INTO ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_STEP_OVER ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT) {
					if (length <= offset + 4) {
						lastError = "Cannot read ipc message: invalid data length";
						return;
					}

					debugObjectEngineGuid = IpcMessageUtility.DeserializeGuid(data, ref offset);
				}

				if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_UPDATE_OBJECTS_END)
					stateGuid = IpcMessageUtility.DeserializeUInt64(data, ref offset);
				else if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_SET_BREAKPOINT ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_SET_BREAKPOINT ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE ||
				         messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE) {
					if (length < offset + 8) {
						lastError = "Cannot read ipc message: invalid object engine guid data";
						return;
					}

					stateGuid = IpcMessageUtility.DeserializeUInt64(data, ref offset);
				}

				if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE) {
					if (length < offset + 4) {
						lastError = "Cannot read ipc message: invalid raised events count data";
						return;
					}

					staticGuidsList = IpcMessageUtility.DeSerializeUInt64List(data, ref offset);
				}

				if (messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE ||
				    messageType == EDebugIPCMessageType.IPC_MESSAGE_SERVER_EVENT_INPUT_STATE) {
					if (length < offset + 4) {
						lastError = "Cannot read ipc message: invalid changed params count data";
						return;
					}

					var num1 = IpcMessageUtility.DeserializeInt(data, ref offset);
					for (var index = 0; index < num1; ++index) {
						var num2 = IpcMessageUtility.DeserializeUInt64(data, ref offset);
						try {
							var valueType =
								StringSerializer.ReadTypeByString(
									IpcMessageUtility.DeserializeString(data, ref offset));
							if (!(null != valueType))
								return;
							paramsValues.Add(IpcMessageUtility.DeserializeValue(data, ref offset, valueType));
							paramsChanged.Add(num2);
						} catch (Exception ex) {
							return;
						}
					}
				}

				if (messageType != EDebugIPCMessageType.IPC_MESSAGE_REQUEST_RAISE_EVENT)
					return;
				debugObjectUniName = IpcMessageUtility.DeserializeString(data, ref offset);
				stateGuid = IpcMessageUtility.DeserializeUInt64(data, ref offset);
			}
		} catch (Exception ex) {
			lastError = string.Format("Ipc message deserialize error: {0}", ex);
		}
	}

	public string LastError => lastError;

	public bool IsValid => lastError.Length == 0;
}