using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Inspectors;
using System.Collections;
using UnityEngine.Scripting;

namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ForceCollectMemoryStrategy : IMemoryStrategy
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected bool disableGC;

    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      if (this.disableGC)
      {
        GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
        yield return (object) null;
      }
      OptimizationUtility.ForceCollect();
      if (this.disableGC)
      {
        yield return (object) null;
        GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
      }
    }
  }
}
