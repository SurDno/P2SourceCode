using System;
using System.Collections.Generic;
using Engine.Common.Types;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common.VMDebug;

public class EngineObjectCreationInfo {
	public EDataType ObjectType;
	public string EngineTemplateInfo;
	public string Name;
	public string EngineData;
	public string EnginePath;
	public bool Static;
	public List<ComponentInfo> Components = new();

	public EngineObjectCreationInfo(
		EDataType objectType,
		Guid templateGuid,
		string name,
		string typeStr,
		string path) {
		ObjectType = objectType;
		EngineTemplateInfo = GuidUtility.GetGuidString(templateGuid);
		Name = name;
		Static = false;
		EngineData = typeStr;
		EnginePath = path;
	}

	public EngineObjectCreationInfo(
		EDataType objectType,
		Guid engineTemplateGuid,
		string engineData,
		string enginePath,
		string name,
		bool bStatic) {
		ObjectType = objectType;
		EngineTemplateInfo = GuidUtility.GetGuidString(engineTemplateGuid);
		EngineData = engineData;
		EnginePath = enginePath;
		Name = name;
		Static = bStatic;
	}
}