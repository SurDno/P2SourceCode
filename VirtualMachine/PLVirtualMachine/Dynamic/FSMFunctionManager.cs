using Cofe.Loggers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.GameLogic;
using System;
using System.Collections.Generic;

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
          this.LoadFunctionsFromEngineDirect();
        else
          this.LoadFunctions();
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString());
      }
    }

    public void Clear() => this.contextFunctions.Clear();

    public BaseFunction GetContextFunction(string functionName)
    {
      VMFunction contextFunction = (VMFunction) null;
      this.contextFunctions.TryGetValue(functionName.GetHashCode(), out contextFunction);
      return (BaseFunction) contextFunction;
    }

    private void LoadFunctions()
    {
      if (this.fsm.FSMStaticObject.Functions == null)
        return;
      this.contextFunctions = new Dictionary<int, VMFunction>(this.fsm.FSMStaticObject.Functions.Count);
      foreach (BaseFunction function in this.fsm.FSMStaticObject.Functions)
      {
        VMFunction vmFunction = new VMFunction(function, this.fsm);
        this.contextFunctions.Add(vmFunction.Name.GetHashCode(), vmFunction);
      }
    }

    private void LoadFunctionsFromEngineDirect()
    {
      this.contextFunctions = new Dictionary<int, VMFunction>();
      foreach (VMComponent component in this.fsm.Entity.Components)
      {
        ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
        if (functionalComponentByName == null)
        {
          Logger.AddError(string.Format("Component with name {0} not found in virtual machine api", (object) component.Name));
        }
        else
        {
          Type type = component.GetType();
          for (int index = 0; index < functionalComponentByName.Methods.Count; ++index)
          {
            VMFunction vmFunction = new VMFunction(functionalComponentByName.Methods[index], component.Name, type, this.fsm);
            this.contextFunctions.Add(vmFunction.Name.GetHashCode(), vmFunction);
          }
        }
      }
    }
  }
}
