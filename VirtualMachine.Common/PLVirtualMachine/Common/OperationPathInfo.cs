using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common;

public class OperationPathInfo : IVMStringSerializable {
	private List<CommonVariable> operationRootsList = new();
	public const string OperationPathRootName = "Pathologic";

	public OperationPathInfo(string data) {
		Read(data);
	}

	public List<CommonVariable> RootInfoList => operationRootsList;

	public void Read(string data) {
		switch (data) {
			case null:
				Logger.AddError(string.Format("Attempt to read null operation path info at {0}",
					EngineAPIManager.Instance.CurrentFSMStateInfo));
				break;
			case "":
				break;
			case "0":
				break;
			default:
				if (data.Contains("&ROOT&PATH&VAR")) {
					var separator = new string[1] {
						"&ROOT&PATH&VAR"
					};
					ReadPathList(data.Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
					break;
				}

				ReadPathList(data);
				break;
		}
	}

	public string Write() {
		Logger.AddError("Not allowed serialization data struct in virtual machine!");
		return string.Empty;
	}

	private void ReadPathList(string data) {
		operationRootsList.Clear();
		var separator = new string[1] { "END&PATH" };
		foreach (var operationRootsListInfo in data.Split(separator, StringSplitOptions.RemoveEmptyEntries))
			if ("" != operationRootsListInfo)
				operationRootsList.Add(ReadContextParamValue(operationRootsListInfo));
	}

	private CommonVariable ReadContextParamValue(string operationRootsListInfo) {
		var str = "";
		var variableData = "";
		if (operationRootsListInfo.Contains("CONTEXT&PARAM")) {
			var separator = new string[1] {
				"CONTEXT&PARAM"
			};
			var strArray = operationRootsListInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (strArray.Length > 1) {
				str = strArray[0];
				variableData = strArray[1];
			} else if (strArray.Length == 1) {
				str = strArray[0];
				variableData = strArray[0];
			}
		} else
			str = operationRootsListInfo;

		var rootVarInfo = new CommonVariable();
		rootVarInfo.Initialise(str, variableData);
		if (!IsValidRootInfo(rootVarInfo) && str.Contains("/") && "" != IStaticDataContainer.StaticDataContainer
			    .GameRoot.GetHierarchyGuidByHierarchyPath(str).Write()) {
			rootVarInfo = new CommonVariable();
			rootVarInfo.Initialise(str, "");
		}

		return rootVarInfo;
	}

	private bool IsValidRootInfo(CommonVariable rootVarInfo) {
		IContext gameRoot = IStaticDataContainer.StaticDataContainer.GameRoot;
		rootVarInfo.Bind(gameRoot);
		return rootVarInfo.IsBinded && rootVarInfo.VariableContext != null;
	}
}