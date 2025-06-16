// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.VMDebug.ParamValueInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System;

#nullable disable
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
