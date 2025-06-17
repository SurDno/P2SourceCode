using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class APIMethodInfo(string methodName) {
    public readonly string MethodName = methodName;
    public APIParamInfo ReturnParam;
    public readonly List<APIParamInfo> InputParams = [];
  }
}
