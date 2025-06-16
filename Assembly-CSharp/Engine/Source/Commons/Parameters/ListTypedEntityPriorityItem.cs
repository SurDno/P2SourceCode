using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;

namespace Engine.Source.Commons.Parameters
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad, Type = typeof (PriorityItem<List<Typed<IEntity>>>))]
  public class ListTypedEntityPriorityItem : PriorityItem<List<Typed<IEntity>>>
  {
  }
}
