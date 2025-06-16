// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EMathOperationType
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
namespace PLVirtualMachine.Common
{
  public enum EMathOperationType
  {
    [Description("No& ")] ACTION_OPERATION_TYPE_NONE,
    [Description("Addition&+")] ACTION_OPERATION_TYPE_ADDICTION,
    [Description("Substraction&-")] ACTION_OPERATION_TYPE_SUBTRACTION,
    [Description("Multiply&*")] ACTION_OPERATION_TYPE_MULTIPLY,
    [Description("Division&:")] ACTION_OPERATION_TYPE_DIVISION,
  }
}
