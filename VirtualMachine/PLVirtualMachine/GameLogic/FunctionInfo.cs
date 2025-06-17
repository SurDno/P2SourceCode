using System.Collections.Generic;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  public class FunctionInfo
  {
    public List<APIParamInfo> Params { get; } = [];

    public APIParamInfo OutputParam { get; set; }
  }
}
