using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a -= b] : int", MenuItem = "a -= b/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextIntValueSubtractionAssignment : EffectContextValueAssignment<int>
  {
    protected override int Compute(int a, int b) => a - b;

    public override string ValueView
    {
      get
      {
        return (this.a != null ? this.a.ValueView : "null") + " -= " + (this.b != null ? this.b.ValueView : "null");
      }
    }

    public override string TypeView
    {
      get
      {
        return (this.a != null ? this.a.TypeView : "null") + " -= " + (this.b != null ? this.b.TypeView : "null");
      }
    }
  }
}
