using Engine.Common.Generator;

namespace Engine.Source.Commons.Parameters
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad, Type = typeof (PriorityItem<bool>))]
  public class BoolPriorityItem : PriorityItem<bool>
  {
  }
}
