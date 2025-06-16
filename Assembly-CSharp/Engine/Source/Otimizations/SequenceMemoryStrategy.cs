using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections;
using System.Collections.Generic;

namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SequenceMemoryStrategy : IMemoryStrategy
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected List<IMemoryStrategy> items = new List<IMemoryStrategy>();

    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      foreach (IMemoryStrategy item in this.items)
        yield return (object) item.Compute(context);
    }
  }
}
