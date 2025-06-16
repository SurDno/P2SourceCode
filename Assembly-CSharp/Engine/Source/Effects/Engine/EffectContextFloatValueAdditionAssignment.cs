using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine;

[TypeName(TypeName = "[a += b] : float", MenuItem = "a += b/float")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EffectContextFloatValueAdditionAssignment : EffectContextValueAssignment<float> {
	protected override float Compute(float a, float b) {
		return a + b;
	}

	public override string ValueView =>
		(a != null ? a.ValueView : "null") + " += " + (b != null ? b.ValueView : "null");

	public override string TypeView => (a != null ? a.TypeView : "null") + " += " + (b != null ? b.TypeView : "null");
}