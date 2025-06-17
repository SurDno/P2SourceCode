using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[max(a, b)] : DetectType", MenuItem = "max(a, b)/DetectType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MaxDetectTypeOperation : BinaryOperation<DetectType>
  {
    protected override DetectType Compute(DetectType a, DetectType b) => a > b ? a : b;

    public override string ValueView => "max(" + (a != null ? a.ValueView : "null") + ", " + (b != null ? b.ValueView : "null") + ")";
  }
}
