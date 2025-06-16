using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common;

[VMType("ObjectCombinationDataStruct")]
public class ObjectCombinationDataStruct : IVMStringSerializable {
	private List<ObjectCombinationElement> combinationElements = new();

	public int GetElementsCount() {
		return combinationElements.Count;
	}

	public ObjectCombinationElement GetCombinationElement(int elemInd) {
		return elemInd < 0 || elemInd >= combinationElements.Count ? null : combinationElements[elemInd];
	}

	public bool ContainsItem(IBlueprint item) {
		for (var index = 0; index < combinationElements.Count; ++index)
			if (combinationElements[index].ContainsItem(item))
				return true;
		return false;
	}

	public void Read(string data) {
		switch (data) {
			case null:
				Logger.AddError(string.Format("Attempt to read null object combination data at {0}",
					EngineAPIManager.Instance.CurrentFSMStateInfo));
				break;
			case "":
				break;
			default:
				if (data.Length < 12) {
					Logger.AddError(string.Format("Cannot convert {0} to object combination data struct at {1}", data,
						EngineAPIManager.Instance.CurrentFSMStateInfo));
					break;
				}

				var separator = new string[1] { "END&ELEM" };
				var strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				combinationElements.Clear();
				for (var index = 0; index < strArray.Length; ++index)
					combinationElements.Add(new ObjectCombinationElement(strArray[index]));
				break;
		}
	}

	public string Write() {
		Logger.AddError("Not allowed serialization data struct in virtual machine!");
		return string.Empty;
	}
}