using Engine.Common.Generator;

namespace Engine.Source.Commons.Parameters;

[GenerateProxy(
	TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
	TypeEnum.StateLoad, Type = typeof(PriorityContainer<bool>))]
public class BoolPriorityContainer : PriorityContainer<bool> {
	protected override bool IsDefault(bool value) {
		return !value;
	}
}