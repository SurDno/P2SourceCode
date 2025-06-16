// Decompiled with JetBrains decompiler
// Type: Engine.Source.Otimizations.AllocMemoryStrategy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Inspectors;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
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
