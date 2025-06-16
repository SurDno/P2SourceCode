using System.ComponentModel;

namespace PLVirtualMachine.Common
{
  public enum FormulaOperation
  {
    [Description(" ")] FORMULA_OP_NONE,
    [Description("+")] FORMULA_OP_PLUS,
    [Description("-")] FORMULA_OP_MINUS,
    [Description("*")] FORMULA_OP_MULTIPLY,
    [Description("/")] FORMULA_OP_DIVIDE,
    [Description("%")] FORMULA_OP_RDIVIDE,
    [Description(" ^ ")] FORMULA_OP_POWER,
    [Description("Log")] FORMULA_OP_LOG,
    [Description("Log10")] FORMULA_OP_LOG10,
    [Description("Exp")] FORMULA_OP_EXP,
    [Description("Sin")] FORMULA_OP_SIN,
    [Description("Cos")] FORMULA_OP_COS,
  }
}
