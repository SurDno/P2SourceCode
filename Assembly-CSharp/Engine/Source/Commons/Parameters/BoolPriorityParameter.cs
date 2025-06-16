using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Parameters;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class BoolPriorityParameter : PriorityParameter<bool> {
	protected override bool Compare(bool a, bool b) {
		return a == b;
	}
}