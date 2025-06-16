// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.DebugUtility
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common;
using PLVirtualMachine.Common.VMDebug;
using PLVirtualMachine.Debug;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine
{
  public static class DebugUtility
  {
    private static Dictionary<string, DynamicParameter> changedParamsMemoryDict = new Dictionary<string, DynamicParameter>();
    private static VMDebugger debugger = new VMDebugger();

    public static VMDebugger Debugger => DebugUtility.debugger;

    public static bool IsDebug { get; set; } = true;

    private static bool IsDebugging
    {
      get
      {
        return DebugUtility.IsDebug && !DebugUtility.Debugger.NeedUpdateHierarchy && DebugUtility.Debugger.ControllerWorkMode != 0;
      }
    }

    public static void Init()
    {
      if (!DebugUtility.IsDebug)
        return;
      DebugUtility.debugger.Init();
    }

    public static void Clear()
    {
      DebugUtility.Debugger.Clear();
      DebugUtility.changedParamsMemoryDict.Clear();
    }

    private static void OnDebugParamValueChangedInner(DynamicParameter param, object value)
    {
      if (DebugUtility.Debugger.ControllerWorkMode == EDebugIPCApplicationWorkMode.IPC_APPLICATION_WORK_MODE_DEBUG)
      {
        DebugUtility.Debugger.OnParamValueChange((VMParameter) param.StaticObject, value, param.DynamicGuid);
      }
      else
      {
        string key = param.DynamicGuid.ToString() + param.StaticGuid.ToString();
        if (DebugUtility.changedParamsMemoryDict.ContainsKey(key))
          return;
        DebugUtility.changedParamsMemoryDict.Add(key, param);
      }
    }

    private static void SendChangedParamsInfoToEditor()
    {
      foreach (KeyValuePair<string, DynamicParameter> keyValuePair in DebugUtility.changedParamsMemoryDict)
      {
        DynamicParameter dynamicParameter = keyValuePair.Value;
        if (dynamicParameter.Entity.Instantiated)
          DebugUtility.Debugger.OnParamValueChange((VMParameter) dynamicParameter.StaticObject, dynamicParameter.Value, dynamicParameter.DynamicGuid);
      }
      DebugUtility.changedParamsMemoryDict.Clear();
    }

    public static void Update()
    {
      if (!DebugUtility.IsDebugging)
        return;
      if (DebugUtility.changedParamsMemoryDict.Count > 0)
        DebugUtility.SendChangedParamsInfoToEditor();
      else
        DebugUtility.debugger.OnTick();
    }

    public static void OnAddObject(DynamicFSM fsm)
    {
      if (!DebugUtility.IsDebug)
        return;
      DebugUtility.Debugger.OnAddObject(fsm);
    }

    public static void OnStateExec(DynamicFSM fsm, IState state)
    {
      if (!DebugUtility.IsDebug)
        return;
      DebugUtility.Debugger.OnStateExec(fsm, state);
    }

    public static void OnDebugParamValueChanged(DynamicParameter par, object value)
    {
      if (!DebugUtility.IsDebug || value != null && typeof (ICommonList).IsAssignableFrom(value.GetType()))
        return;
      DebugUtility.OnDebugParamValueChangedInner(par, value);
    }
  }
}
