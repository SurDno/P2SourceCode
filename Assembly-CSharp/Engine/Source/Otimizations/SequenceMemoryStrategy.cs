// Decompiled with JetBrains decompiler
// Type: Engine.Source.Otimizations.SequenceMemoryStrategy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections;
using System.Collections.Generic;

#nullable disable
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
