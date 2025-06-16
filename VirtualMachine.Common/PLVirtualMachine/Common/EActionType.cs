// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EActionType
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
namespace PLVirtualMachine.Common
{
  public enum EActionType
  {
    [Description("None")] ACTION_TYPE_NONE,
    [Description("Set param")] ACTION_TYPE_SET_PARAM,
    [Description("Set expression")] ACTION_TYPE_SET_EXPRESSION,
    [Description("Math operation")] ACTION_TYPE_MATH,
    [Description("Function call")] ACTION_TYPE_DO_FUNCTION,
    [Description("Event raising")] ACTION_TYPE_RAISE_EVENT,
  }
}
