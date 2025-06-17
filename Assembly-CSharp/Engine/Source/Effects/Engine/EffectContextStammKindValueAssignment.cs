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

    public override string ValueView => (a != null ? a.ValueView : "null") + " = " + (b != null ? b.ValueView : "null");

    public override string TypeView => (a != null ? a.TypeView : "null") + " = " + (b != null ? b.TypeView : "null");
  }
}
