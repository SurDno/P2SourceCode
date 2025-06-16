using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a = b] : bool", MenuItem = "a = b/bool")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextBoolValueAssignment : EffectContextValueAssignment<bool>
  {
    protected override bool Compute(bool a, bool b) => b;

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
