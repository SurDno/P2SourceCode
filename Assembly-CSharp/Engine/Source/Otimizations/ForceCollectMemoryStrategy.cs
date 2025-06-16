using System.Collections;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Inspectors;
using UnityEngine.Scripting;

namespace Engine.Source.Otimizations;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ForceCollectMemoryStrategy : IMemoryStrategy {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected bool disableGC;

	public IEnumerator Compute(MemoryStrategyContextEnum context) {
		if (disableGC) {
			GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
			yield return null;
		}

		OptimizationUtility.ForceCollect();
		if (disableGC) {
			yield return null;
			GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
		}
	}
}