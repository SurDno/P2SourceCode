using Engine.Common.Components.Gate;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class LockStatePriorityParameter : PriorityParameter<LockState>
  {
    protected override bool Compare(LockState a, LockState b) => a == b;
  }
}
