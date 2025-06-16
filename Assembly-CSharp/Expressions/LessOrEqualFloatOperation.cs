using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[a <= b] : float", MenuItem = "a <= b/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LessOrEqualFloatOperation : ComparisonOperation<float>
  {
    protected override bool Compute(float a, float b) => a <= (double) b;

    protected override string OperatorView() => "<=";
  }
}
