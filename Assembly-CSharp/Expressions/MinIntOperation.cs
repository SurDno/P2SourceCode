using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[min(a, b)] : int", MenuItem = "min(a, b)/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MinIntOperation : BinaryOperation<int>
  {
    protected override int Compute(int a, int b) => a < b ? a : b;

    public override string ValueView
    {
      get
      {
        return "min(" + (a != null ? a.ValueView : "null") + ", " + (b != null ? b.ValueView : "null") + ")";
      }
    }
  }
}
