// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.FunctionInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Common.EngineAPI;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.GameLogic
{
  public class FunctionInfo
  {
    public List<APIParamInfo> Params { get; } = new List<APIParamInfo>();

    public APIParamInfo OutputParam { get; set; }
  }
}
