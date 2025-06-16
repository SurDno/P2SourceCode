using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class APIMethodInfo
  {
    public readonly string MethodName;
    public APIParamInfo ReturnParam;
    public readonly List<APIParamInfo> InputParams = new List<APIParamInfo>();

    public APIMethodInfo(string methodName) => MethodName = methodName;
  }
}
