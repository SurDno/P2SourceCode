using System;

namespace PLVirtualMachine.Common.VMDebug
{
  public class ParamValueInfo
  {
    public ulong ParamGuid;
    public Type ParamValueType;
    public object ParamValue;

    public ParamValueInfo(ulong paramGuid, Type paramType, object paramValue)
    {
      this.ParamGuid = paramGuid;
      this.ParamValueType = paramType;
      this.ParamValue = paramValue;
    }
  }
}
