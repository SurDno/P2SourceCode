using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace Engine.Source.Commons.Parameters;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad | TypeEnum.NeedSave)]
public class IntParameter : MinMaxParameter<int> {
	protected override bool Compare(int a, int b) {
		return a == b;
	}

	protected override void Correct() {
		Value = Mathf.Clamp(value, minValue, maxValue);
	}
}