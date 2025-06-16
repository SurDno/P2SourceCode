using Engine.Common.Commons;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a = b] : StammKind", MenuItem = "a = b/StammKind")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextStammKindValueAssignment : EffectContextValueAssignment<StammKind>
  {
    protected override StammKind Compute(StammKind a, StammKind b) => b;

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
