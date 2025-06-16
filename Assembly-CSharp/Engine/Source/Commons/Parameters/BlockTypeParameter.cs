using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Parameters;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad | TypeEnum.NeedSave)]
public class BlockTypeParameter : Parameter<BlockTypeEnum> {
	protected override bool Compare(BlockTypeEnum a, BlockTypeEnum b) {
		return a == b;
	}
}