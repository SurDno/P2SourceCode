// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.FormulaOperation
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.ComponentModel;

#nullable disable
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
