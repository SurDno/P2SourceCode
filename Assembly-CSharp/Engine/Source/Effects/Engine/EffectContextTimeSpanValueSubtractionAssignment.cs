using System;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a -= b] : Time", MenuItem = "a -= b/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextTimeSpanValueSubtractionAssignment : 
    EffectContextValueAssignment<TimeSpan>
  {
    protected override TimeSpan Compute(TimeSpan a, TimeSpan b) => a - b;

    public override string ValueView => (a != null ? a.ValueView : "null") + " -= " + (b != null ? b.ValueView : "null");

    public override string TypeView => (a != null ? a.TypeView : "null") + " -= " + (b != null ? b.TypeView : "null");
  }
}
