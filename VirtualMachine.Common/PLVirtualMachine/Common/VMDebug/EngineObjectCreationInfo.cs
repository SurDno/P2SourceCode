using Engine.Common.Types;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common.VMDebug
{
  public class EngineObjectCreationInfo
  {
    public EDataType ObjectType;
    public string EngineTemplateInfo;
    public string Name;
    public string EngineData;
    public string EnginePath;
    public bool Static;
    public List<ComponentInfo> Components = new List<ComponentInfo>();

    public EngineObjectCreationInfo(
      EDataType objectType,
      Guid templateGuid,
      string name,
      string typeStr,
      string path)
    {
      this.ObjectType = objectType;
      this.EngineTemplateInfo = GuidUtility.GetGuidString(templateGuid);
      this.Name = name;
      this.Static = false;
      this.EngineData = typeStr;
      this.EnginePath = path;
    }

    public EngineObjectCreationInfo(
      EDataType objectType,
      Guid engineTemplateGuid,
      string engineData,
      string enginePath,
      string name,
      bool bStatic)
    {
      this.ObjectType = objectType;
      this.EngineTemplateInfo = GuidUtility.GetGuidString(engineTemplateGuid);
      this.EngineData = engineData;
      this.EnginePath = enginePath;
      this.Name = name;
      this.Static = bStatic;
    }
  }
}
