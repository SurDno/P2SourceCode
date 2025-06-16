using System.Collections.Generic;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.VMDebug;
using PLVirtualMachine.Debug;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;

namespace PLVirtualMachine
{
  public static class DebugUtility
  {
    private static Dictionary<string, DynamicParameter> changedParamsMemoryDict = new Dictionary<string, DynamicParameter>();
    private static VMDebugger debugger = new VMDebugger();

    public static VMDebugger Debugger => debugger;

    public static bool IsDebug { get; set; } = true;

    private static bool IsDebugging
    {
      get
      {
        return IsDebug && !Debugger.NeedUpdateHierarchy && Debugger.ControllerWorkMode != 0;
      }
    }

    public static void Init()
    {
      if (!IsDebug)
        return;
      debugger.Init();
    }

    public static void Clear()
    {
      Debugger.Clear();
      changedParamsMemoryDict.Clear();
    }

    private static void OnDebugParamValueChangedInner(DynamicParameter param, object value)
    {
      if (Debugger.ControllerWorkMode == EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG)
      {
        Debugger.OnParamValueChange((VMParameter) param.StaticObject, value, param.DynamicGuid);
      }
      else
      {
        string key = param.DynamicGuid + param.StaticGuid.ToString();
        if (changedParamsMemoryDict.ContainsKey(key))
          return;
        changedParamsMemoryDict.Add(key, param);
      }
    }

    private static void SendChangedParamsInfoToEditor()
    {
      foreach (KeyValuePair<string, DynamicParameter> keyValuePair in changedParamsMemoryDict)
      {
        DynamicParameter dynamicParameter = keyValuePair.Value;
        if (dynamicParameter.Entity.Instantiated)
          Debugger.OnParamValueChange((VMParameter) dynamicParameter.StaticObject, dynamicParameter.Value, dynamicParameter.DynamicGuid);
      }
      changedParamsMemoryDict.Clear();
    }

    public static void Update()
    {
      if (!IsDebugging)
        return;
      if (changedParamsMemoryDict.Count > 0)
        SendChangedParamsInfoToEditor();
      else
        debugger.OnTick();
    }

    public static void OnAddObject(DynamicFSM fsm)
    {
      if (!IsDebug)
        return;
      Debugger.OnAddObject(fsm);
    }

    public static void OnStateExec(DynamicFSM fsm, IState state)
    {
      if (!IsDebug)
        return;
      Debugger.OnStateExec(fsm, state);
    }

    public static void OnDebugParamValueChanged(DynamicParameter par, object value)
    {
      if (!IsDebug || value != null && typeof (ICommonList).IsAssignableFrom(value.GetType()))
        return;
      OnDebugParamValueChangedInner(par, value);
    }
  }
}
