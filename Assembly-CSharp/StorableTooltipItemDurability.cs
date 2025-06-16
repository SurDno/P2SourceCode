using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

[Factory(typeof(IStorableTooltipComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class StorableTooltipItemDurability : IStorableTooltipComponent {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool isEnabled = true;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected StorableTooltipNameEnum name;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum parameter;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool isFood;

	[Inspected] public bool IsEnabled => isEnabled;

	public StorableTooltipInfo GetInfo(IEntity owner) {
		var info = new StorableTooltipInfo();
		info.Name = name;
		if (owner != null) {
			var component = owner.GetComponent<ParametersComponent>();
			if (component != null) {
				var byName = component.GetByName<float>(parameter);
				CountParams(info, byName.Value);
			}
		}

		return info;
	}

	private void CountParams(StorableTooltipInfo info, float value) {
		if (value > 0.89999997615814209) {
			info.Value = isFood ? "{StorableTooltip.FoodExcellent}" : "{StorableTooltip.Excellent}";
			info.Color = Color.blue;
		} else if (value > 0.699999988079071) {
			info.Value = isFood ? "{StorableTooltip.FoodGood}" : "{StorableTooltip.Good}";
			info.Color = Color.green;
		} else if (value > 0.30000001192092896) {
			info.Value = isFood ? "{StorableTooltip.FoodAverage}" : "{StorableTooltip.Average}";
			info.Color = Color.yellow;
		} else if (value > 0.0) {
			info.Value = isFood ? "{StorableTooltip.FoodBad}" : "{StorableTooltip.Bad}";
			info.Color = Color.red;
		} else {
			info.Value = isFood ? "{StorableTooltip.FoodBroken}" : "{StorableTooltip.Broken}";
			info.Color = Color.red;
		}
	}
}