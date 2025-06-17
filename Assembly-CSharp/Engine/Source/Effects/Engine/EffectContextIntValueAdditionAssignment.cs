using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a += b] : int", MenuItem = "a += b/int")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextIntValueAdditionAssignment : EffectContextValueAssignment<int>
  {
    protected override int Compute(int a, int b) => a + b;

    public override string ValueView => (a != null ? a.ValueView : "null") + " += " + (b != null ? b.ValueView : "null");

    public override string TypeView => (a != null ? a.TypeView : "null") + " += " + (b != null ? b.TypeView : "null");
  }
}
