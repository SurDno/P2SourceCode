using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine;

[TypeName(TypeName = "[a = b] : bool", MenuItem = "a = b/bool")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EffectContextBoolValueAssignment : EffectContextValueAssignment<bool> {
	protected override bool Compute(bool a, bool b) {
		return b;
	}

	public override string ValueView => (a != null ? a.ValueView : "null") + " = " + (b != null ? b.ValueView : "null");

	public override string TypeView => (a != null ? a.TypeView : "null") + " = " + (b != null ? b.TypeView : "null");
}