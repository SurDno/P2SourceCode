using System.Collections;
using System.Collections.Generic;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SequenceMemoryStrategy : IMemoryStrategy
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected List<IMemoryStrategy> items = [];

    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      foreach (IMemoryStrategy item in items)
        yield return item.Compute(context);
    }
  }
}
