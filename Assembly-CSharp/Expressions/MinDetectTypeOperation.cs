using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[min(a, b)] : DetectType", MenuItem = "min(a, b)/DetectType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MinDetectTypeOperation : BinaryOperation<DetectType>
  {
    protected override DetectType Compute(DetectType a, DetectType b) => a < b ? a : b;

    public override string ValueView
    {
      get
      {
        return "min(" + (this.a != null ? this.a.ValueView : "null") + ", " + (this.b != null ? this.b.ValueView : "null") + ")";
      }
    }
  }
}
