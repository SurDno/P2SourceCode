using System.Collections;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.Profiling;

namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AllocMemoryStrategy : IMemoryStrategy
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected long maxMemory;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected long minMemory;

    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      long used = Profiler.GetMonoUsedSizeLong();
      long max = Profiler.GetMonoHeapSizeLong();
      if (max == 0L || used == 0L)
      {
        Debug.Log("Wrong memory info");
      }
      else
      {
        Debug.Log(ObjectInfoUtility.GetStream().Append("Memory info , used : ").GetMemoryText(used).Append(" , max : ").GetMemoryText(max).Append(" , min : ").GetMemoryText(minMemory));
        long has = max - used;
        if (has >= minMemory)
        {
          Debug.Log(ObjectInfoUtility.GetStream().Append("Memory enough , has : ").GetMemoryText(has).Append(" , need : ").GetMemoryText(minMemory));
        }
        else
        {
          OptimizationUtility.Alloc(maxMemory);
          yield break;
        }
      }
    }
  }
}
