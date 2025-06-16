using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Inspectors;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AllocMemoryStrategy : IMemoryStrategy
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected long maxMemory;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected long minMemory;

    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      long used = Profiler.GetMonoUsedSizeLong();
      long max = Profiler.GetMonoHeapSizeLong();
      if (max == 0L || used == 0L)
      {
        Debug.Log((object) "Wrong memory info");
      }
      else
      {
        Debug.Log((object) ObjectInfoUtility.GetStream().Append("Memory info , used : ").GetMemoryText(used).Append(" , max : ").GetMemoryText(max).Append(" , min : ").GetMemoryText(this.minMemory));
        long has = max - used;
        if (has >= this.minMemory)
        {
          Debug.Log((object) ObjectInfoUtility.GetStream().Append("Memory enough , has : ").GetMemoryText(has).Append(" , need : ").GetMemoryText(this.minMemory));
        }
        else
        {
          OptimizationUtility.Alloc(this.maxMemory);
          yield break;
        }
      }
    }
  }
}
