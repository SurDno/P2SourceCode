using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[max(a, b)] : int", MenuItem = "max(a, b)/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MaxIntOperation : BinaryOperation<int>
  {
    protected override int Compute(int a, int b) => a > b ? a : b;

    public override string ValueView => "max(" + (a != null ? a.ValueView : "null") + ", " + (b != null ? b.ValueView : "null") + ")";
  }
}
