using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Components;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class WindowTargetAbilityProjectile : IAbilityProjectile {
	public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets) {
		var effectsComponentList = new List<EffectsComponent>();
		if (ServiceLocator.GetService<UIService>().Active is ITargetInventoryWindow)
			effectsComponentList.Add((ServiceLocator.GetService<UIService>().Active as ITargetInventoryWindow)
				.GetUseTarget().GetComponent<EffectsComponent>());
		targets.Targets = effectsComponentList;
	}
}