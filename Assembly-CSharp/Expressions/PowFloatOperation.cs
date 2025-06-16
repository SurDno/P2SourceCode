using Engine.Common.Generator;
using Inspectors;
using UnityEngine;

namespace Expressions
{
  [TypeName(TypeName = "[a ^ b] : float", MenuItem = "a ^ b/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PowFloatOperation : BinaryOperation<float>
  {
    protected override float Compute(float a, float b) => Mathf.Pow(a, b);

    protected override string OperatorView() => "^";
  }
}
