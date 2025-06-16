using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[a ^ b] : int", MenuItem = "a ^ b/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PowIntOperation : BinaryOperation<int>
  {
    protected override int Compute(int a, int b) => (int) Mathf.Pow((float) a, (float) b);

    protected override string OperatorView() => "^";
  }
}
