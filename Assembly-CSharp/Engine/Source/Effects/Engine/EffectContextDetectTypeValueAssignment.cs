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

    public override string ValueView
    {
      get
      {
        return (this.a != null ? this.a.ValueView : "null") + " = " + (this.b != null ? this.b.ValueView : "null");
      }
    }

    public override string TypeView
    {
      get
      {
        return (this.a != null ? this.a.TypeView : "null") + " = " + (this.b != null ? this.b.TypeView : "null");
      }
    }
  }
}
