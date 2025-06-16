// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EActionLineType
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
namespace PLVirtualMachine.Common
{
  public enum EActionLineType
  {
    [Description("Simple action line")] ACTION_LINE_TYPE_COMMON,
    [Description("Loop action line")] ACTION_LINE_TYPE_LOOP,
    [Description("Inventory group operations")] ACTION_LINE_TYPE_INVENTORY,
    [Description("Market group operations")] ACTION_LINE_TYPE_MARKET,
    [Description("Doors group operations")] ACTION_LINE_TYPE_GATE_SYSTEM,
    [Description("Custom group operations")] ACTION_LINE_TYPE_CUSTOM_GROUP,
  }
}
