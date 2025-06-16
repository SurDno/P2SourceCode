using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[max(a, b)] : float", MenuItem = "max(a, b)/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MaxFloatOperation : BinaryOperation<float>
  {
    protected override float Compute(float a, float b) => a > (double) b ? a : b;

    public override string ValueView
    {
      get
      {
        return "max(" + (a != null ? a.ValueView : "null") + ", " + (b != null ? b.ValueView : "null") + ")";
      }
    }
  }
}
