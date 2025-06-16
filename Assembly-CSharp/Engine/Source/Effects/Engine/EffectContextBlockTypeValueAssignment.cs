using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine;

[TypeName(TypeName = "[a = b] : BlockType", MenuItem = "a = b/BlockType")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EffectContextBlockTypeValueAssignment : EffectContextValueAssignment<BlockTypeEnum> {
	protected override BlockTypeEnum Compute(BlockTypeEnum a, BlockTypeEnum b) {
		return b;
	}

	public override string ValueView => (a != null ? a.ValueView : "null") + " = " + (b != null ? b.ValueView : "null");

	public override string TypeView => (a != null ? a.TypeView : "null") + " = " + (b != null ? b.TypeView : "null");
}