using System.ComponentModel;

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
