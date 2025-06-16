using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common;

namespace PLVirtualMachine.Dynamic;

public class LoopInfo {
	private ICommonList loopList;
	private int currentIndex;
	private object currentListElement;
	private Dictionary<string, object> loopLocalVariableValuesDict;
	private string loopIndexVariableName = "";
	private string loopListElementVariableName = "";

	public LoopInfo(IContextElement ownerContextElement, ICommonList loopList = null) {
		currentIndex = 0;
		this.loopList = loopList;
		var contextVariables = ownerContextElement.LocalContextVariables;
		if (contextVariables.Count == 1)
			loopIndexVariableName = contextVariables[0].Name;
		else if (contextVariables.Count == 2) {
			loopListElementVariableName = contextVariables[0].Name;
			loopIndexVariableName = contextVariables[1].Name;
		} else
			Logger.AddError(string.Format("Invalid action loop guid={0} context variables count : {1}",
				ownerContextElement.BaseGuid, contextVariables.Count));
	}

	public void RegistrLoopLocalVarsDict(
		Dictionary<string, object> loopLocalVariableValuesDict) {
		this.loopLocalVariableValuesDict = loopLocalVariableValuesDict;
	}

	public int CurrentLoopIndex {
		get => currentIndex;
		set {
			currentIndex = value;
			if (loopLocalVariableValuesDict == null)
				Logger.AddError("Loop local variables dictionary not regitered in loop info !!!");
			else {
				loopLocalVariableValuesDict[loopIndexVariableName] = currentIndex;
				if (loopList == null)
					return;
				currentListElement = loopList.GetObject(currentIndex);
				loopLocalVariableValuesDict[loopListElementVariableName] = currentListElement;
			}
		}
	}
}