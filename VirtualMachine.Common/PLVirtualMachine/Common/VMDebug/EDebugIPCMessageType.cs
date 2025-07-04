﻿namespace PLVirtualMachine.Common.VMDebug
{
  public enum EDebugIPCMessageType
  {
    IPC_MESSAGE_NONE,
    IPC_MESSAGE_REQUEST_SET_DEBUG_OBJECT,
    IPC_MESSAGE_REQUEST_SET_BREAKPOINT,
    IPC_MESSAGE_REQUEST_STEP_INTO,
    IPC_MESSAGE_REQUEST_STEP_OVER,
    IPC_MESSAGE_REQUEST_STEP_CONTINUE,
    IPC_MESSAGE_REQUEST_REMOVE_BREAKPOINT,
    IPC_MESSAGE_REQUEST_STOP_DEBUG,
    IPC_MESSAGE_REQUEST_SET_DEBUG_CAMERA,
    IPC_MESSAGE_REQUEST_RAISE_EVENT,
    IPC_MESSAGE_REQUEST_SERVER_EVENT,
    IPC_MESSAGE_REQUEST_COUNT,
    IPC_MESSAGE_RESPONSE_SET_DEBUG_OBJECT,
    IPC_MESSAGE_RESPONSE_SET_BREAKPOINT,
    IPC_MESSAGE_RESPONSE_STEP_INTO,
    IPC_MESSAGE_RESPONSE_STEP_OVER,
    IPC_MESSAGE_RESPONSE_STEP_CONTINUE,
    IPC_MESSAGE_RESPONSE_REMOVE_BREAKPOINT,
    IPC_MESSAGE_RESPONSE_STOP_DEBUG,
    IPC_MESSAGE_RESPONSE_SET_DEBUG_CAMERA,
    IPC_MESSAGE_RESPONSE_RAISE_EVENT,
    IPC_MESSAGE_SERVER_EVENT_CREATE_ENGINE_OBJECT,
    IPC_MESSAGE_SERVER_EVENT_ADD_DEBUG_OBJECT,
    IPC_MESSAGE_SERVER_EVENT_REMOVE_DEBUG_OBJECT,
    IPC_MESSAGE_SERVER_EVENT_INPUT_STATE,
    IPC_MESSAGE_SERVER_EVENT_OUTPUT_STATE,
    IPC_MESSAGE_SERVER_EVENT_PARAM_VALUE_CHANGE,
    IPC_MESSAGE_SERVER_EVENT_UPDATE_OBJECTS_END,
    IPC_MESSAGE_COUNT,
  }
}
