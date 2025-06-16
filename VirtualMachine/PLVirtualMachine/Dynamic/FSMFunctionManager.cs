using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.GameLogic;

namespace PLVirtualMachine.Dynamic;

public class FSMFunctionManager {
	private DynamicFSM fsm;
	private Dictionary<int, VMFunction> contextFunctions;
	public static double GetContextFunctionTimeMaxRT;

	public FSMFunctionManager(DynamicFSM fsm) {
		this.fsm = fsm;
		try {
			if (fsm.FSMStaticObject.DirectEngineCreated)
				LoadFunctionsFromEngineDirect();
			else
				LoadFunctions();
		} catch (Exception ex) {
			Logger.AddError(ex.ToString());
		}
	}

	public void Clear() {
		contextFunctions.Clear();
	}

	public BaseFunction GetContextFunction(string functionName) {
		VMFunction contextFunction = null;
		contextFunctions.TryGetValue(functionName.GetHashCode(), out contextFunction);
		return contextFunction;
	}

	private void LoadFunctions() {
		if (fsm.FSMStaticObject.Functions == null)
			return;
		contextFunctions = new Dictionary<int, VMFunction>(fsm.FSMStaticObject.Functions.Count);
		foreach (var function in fsm.FSMStaticObject.Functions) {
			var vmFunction = new VMFunction(function, fsm);
			contextFunctions.Add(vmFunction.Name.GetHashCode(), vmFunction);
		}
	}

	private void LoadFunctionsFromEngineDirect() {
		contextFunctions = new Dictionary<int, VMFunction>();
		foreach (var component in fsm.Entity.Components) {
			var functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
			if (functionalComponentByName == null)
				Logger.AddError(string.Format("Component with name {0} not found in virtual machine api",
					component.Name));
			else {
				var type = component.GetType();
				for (var index = 0; index < functionalComponentByName.Methods.Count; ++index) {
					var vmFunction = new VMFunction(functionalComponentByName.Methods[index], component.Name, type,
						fsm);
					contextFunctions.Add(vmFunction.Name.GetHashCode(), vmFunction);
				}
			}
		}
	}
}