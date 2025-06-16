using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using System.Collections.Generic;

namespace Engine.Source.Commons.Parameters
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad, Type = typeof (PriorityContainer<List<Typed<IEntity>>>))]
  public class ListTypedEntityPriorityContainer : PriorityContainer<List<Typed<IEntity>>>
  {
    protected override bool IsDefault(List<Typed<IEntity>> value)
    {
      return value == null || value.Count == 0;
    }
  }
}
