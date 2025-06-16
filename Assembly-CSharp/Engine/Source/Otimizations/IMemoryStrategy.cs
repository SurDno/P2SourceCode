using System.Collections;

namespace Engine.Source.Otimizations;

public interface IMemoryStrategy {
	IEnumerator Compute(MemoryStrategyContextEnum context);
}