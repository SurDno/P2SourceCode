using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a = b] : DetectType", MenuItem = "a = b/DetectType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextDetectTypeValueAssignment : EffectContextValueAssignment<DetectType>
  {
    protected override DetectType Compute(DetectType a, DetectType b) => b;

    public override string ValueView => (a != null ? a.ValueView : "null") + " = " + (b != null ? b.ValueView : "null");

    public override string TypeView => (a != null ? a.TypeView : "null") + " = " + (b != null ? b.TypeView : "null");
  }
}
