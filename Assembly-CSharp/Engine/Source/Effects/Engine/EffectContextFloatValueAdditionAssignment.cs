using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a += b] : float", MenuItem = "a += b/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextFloatValueAdditionAssignment : EffectContextValueAssignment<float>
  {
    protected override float Compute(float a, float b) => a + b;

    public override string ValueView
    {
      get
      {
        return (this.a != null ? this.a.ValueView : "null") + " += " + (this.b != null ? this.b.ValueView : "null");
      }
    }

    public override string TypeView
    {
      get
      {
        return (this.a != null ? this.a.TypeView : "null") + " += " + (this.b != null ? this.b.TypeView : "null");
      }
    }
  }
}
