using Engine.Common.Generator;
using Inspectors;
using System;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a -= b] : Time", MenuItem = "a -= b/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextTimeSpanValueSubtractionAssignment : 
    EffectContextValueAssignment<TimeSpan>
  {
    protected override TimeSpan Compute(TimeSpan a, TimeSpan b) => a - b;

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
