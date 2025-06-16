using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[a / b] : float", MenuItem = "a div b/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DivisionFloatOperation : BinaryOperation<float>
  {
    protected override float Compute(float a, float b) => a / b;

    protected override string OperatorView() => "/";
  }
}
