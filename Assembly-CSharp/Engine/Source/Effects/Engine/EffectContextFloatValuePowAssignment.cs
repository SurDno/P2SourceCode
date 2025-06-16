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
        return (a != null ? a.ValueView : "null") + " ^= " + (b != null ? b.ValueView : "null");
      }
    }

    public override string TypeView
    {
      get
      {
        return (a != null ? a.TypeView : "null") + " ^= " + (b != null ? b.TypeView : "null");
      }
    }
  }
}
