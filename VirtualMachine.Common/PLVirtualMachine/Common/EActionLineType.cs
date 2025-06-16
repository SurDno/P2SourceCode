using System.ComponentModel;

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
