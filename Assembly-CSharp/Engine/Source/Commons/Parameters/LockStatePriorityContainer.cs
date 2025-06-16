using Engine.Common.Components.Gate;
using Engine.Common.Generator;

namespace Engine.Source.Commons.Parameters;

[GenerateProxy(
	TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
	TypeEnum.StateLoad, Type = typeof(PriorityContainer<LockState>))]
public class LockStatePriorityContainer : PriorityContainer<LockState> {
	protected override bool IsDefault(LockState value) {
		return value == LockState.Unlocked;
	}
}