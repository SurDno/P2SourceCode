using Engine.Common.Generator;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a ^= b] : float", MenuItem = "a ^= b/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextFloatValuePowAssignment : EffectContextValueAssignment<float>
  {
    protected override float Compute(float a, float b) => Mathf.Pow(a, b);

    public override string ValueView
    {
      get
      {
        return (this.a != null ? this.a.ValueView : "null") + " ^= " + (this.b != null ? this.b.ValueView : "null");
      }
    }

    public override string TypeView
    {
      get
      {
        return (this.a != null ? this.a.TypeView : "null") + " ^= " + (this.b != null ? this.b.TypeView : "null");
      }
    }
  }
}
