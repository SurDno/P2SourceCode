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

    public override string ValueView => "min(" + (a != null ? a.ValueView : "null") + ", " + (b != null ? b.ValueView : "null") + ")";
  }
}
