using System.Collections;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Otimizations;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EmptyMemoryStrategy : IMemoryStrategy {
	public IEnumerator Compute(MemoryStrategyContextEnum context) {
		yield break;
	}
}