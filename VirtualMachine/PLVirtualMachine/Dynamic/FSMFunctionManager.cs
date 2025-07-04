﻿using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.GameLogic;

namespace PLVirtualMachine.Dynamic
{
  public class FSMFunctionManager
  {
    private DynamicFSM fsm;
    private Dictionary<int, VMFunction> contextFunctions;
    public static double GetContextFunctionTimeMaxRT;

    public FSMFunctionManager(DynamicFSM fsm)
    {
      this.fsm = fsm;
      try
      {
        if (fsm.FSMStaticObject.DirectEngineCreated)
          LoadFunctionsFromEngineDirect();
        else
          LoadFunctions();
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString());
      }
    }

    public void Clear() => contextFunctions.Clear();

    public BaseFunction GetContextFunction(string functionName)
    {
      contextFunctions.TryGetValue(functionName.GetHashCode(), out VMFunction contextFunction);
      return contextFunction;
    }

    private void LoadFunctions()
    {
      if (fsm.FSMStaticObject.Functions == null)
        return;
      contextFunctions = new Dictionary<int, VMFunction>(fsm.FSMStaticObject.Functions.Count);
      foreach (BaseFunction function in fsm.FSMStaticObject.Functions)
      {
        VMFunction vmFunction = new VMFunction(function, fsm);
        contextFunctions.Add(vmFunction.Name.GetHashCode(), vmFunction);
      }
    }

    private void LoadFunctionsFromEngineDirect()
    {
      contextFunctions = new Dictionary<int, VMFunction>();
      foreach (VMComponent component in fsm.Entity.Components)
      {
        ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
        if (functionalComponentByName == null)
        {
          Logger.AddError(string.Format("Component with name {0} not found in virtual machine api", component.Name));
        }
        else
        {
          Type type = component.GetType();
          for (int index = 0; index < functionalComponentByName.Methods.Count; ++index)
          {
            VMFunction vmFunction = new VMFunction(functionalComponentByName.Methods[index], component.Name, type, fsm);
            contextFunctions.Add(vmFunction.Name.GetHashCode(), vmFunction);
          }
        }
      }
    }
  }
}
