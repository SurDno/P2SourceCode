using Engine.Common.Generator;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a ^= b] : int", MenuItem = "a ^= b/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextIntValuePowAssignment : EffectContextValueAssignment<int>
  {
    protected override int Compute(int a, int b) => (int) Mathf.Pow((float) a, (float) b);

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
