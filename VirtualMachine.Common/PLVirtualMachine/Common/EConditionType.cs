// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EConditionType
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
namespace PLVirtualMachine.Common
{
  public enum EConditionType
  {
    [Description("TRUE")] CONDITION_TYPE_CONST_TRUE,
    [Description("FALSE")] CONDITION_TYPE_CONST_FALSE,
    [Description("LESS")] CONDITION_TYPE_VALUE_LESS,
    [Description("LESS_EQUAL")] CONDITION_TYPE_VALUE_LESS_EQUAL,
    [Description("LARGER")] CONDITION_TYPE_VALUE_LARGER,
    [Description("LARGER_EQUAL")] CONDITION_TYPE_VALUE_LARGER_EQUAL,
    [Description("EQUAL")] CONDITION_TYPE_VALUE_EQUAL,
    [Description("NOT_EQUAL")] CONDITION_TYPE_VALUE_NOT_EQUAL,
    [Description("EXPRESSION")] CONDITION_TYPE_VALUE_EXPRESSION,
  }
}
