// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.VMDebug.EngineObjectCreationInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Types;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

#nullable disable
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
