using PLVirtualMachine.Common.EngineAPI;
using System.Collections.Generic;

namespace PLVirtualMachine.GameLogic
{
  public class FunctionInfo
  {
    public List<APIParamInfo> Params { get; } = new List<APIParamInfo>();

    public APIParamInfo OutputParam { get; set; }
  }
}
