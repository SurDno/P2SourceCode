// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.APIMethodInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI
{
  public class APIMethodInfo
  {
    public readonly string MethodName;
    public APIParamInfo ReturnParam;
    public readonly List<APIParamInfo> InputParams = new List<APIParamInfo>();

    public APIMethodInfo(string methodName) => this.MethodName = methodName;
  }
}
