using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[a and b] : bool", MenuItem = "a and b/bool")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AndBoolOperation : ConditionLogicalOperation
  {
    protected override bool Compute(bool a, bool b) => a & b;

    protected override string OperatorView() => "&&";
  }
}
