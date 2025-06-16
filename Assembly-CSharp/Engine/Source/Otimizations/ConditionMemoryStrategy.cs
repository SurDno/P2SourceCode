using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections;
using System.Collections.Generic;

namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ConditionMemoryStrategy : IMemoryStrategy
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected List<MemoryStrategyContextEnum> contexts = new List<MemoryStrategyContextEnum>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected IMemoryStrategy item;

    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      if (this.contexts.Contains(context))
        yield return (object) this.item.Compute(context);
    }
  }
}
