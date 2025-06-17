using System;

namespace PLVirtualMachine.Common.VMDebug
{
  public class ParamValueInfo(ulong paramGuid, Type paramType, object paramValue) {
    public ulong ParamGuid = paramGuid;
    public Type ParamValueType = paramType;
    public object ParamValue = paramValue;
  }
}
