using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[min(a, b)] : float", MenuItem = "min(a, b)/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MinFloatOperation : BinaryOperation<float>
  {
    protected override float Compute(float a, float b) => a < (double) b ? a : b;

    public override string ValueView
    {
      get
      {
        return "min(" + (a != null ? a.ValueView : "null") + ", " + (b != null ? b.ValueView : "null") + ")";
      }
    }
  }
}
