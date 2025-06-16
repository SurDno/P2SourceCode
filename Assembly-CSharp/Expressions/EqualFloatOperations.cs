using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[a == b] : float", MenuItem = "a == b/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EqualFloatOperations : ComparisonOperation<float>
  {
    protected override bool Compute(float left, float right) => left == (double) right;

    protected override string OperatorView() => "==";
  }
}
