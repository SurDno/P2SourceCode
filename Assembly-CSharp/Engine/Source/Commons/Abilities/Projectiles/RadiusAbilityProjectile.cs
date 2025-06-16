using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Projectiles;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class RadiusAbilityProjectile : IAbilityProjectile {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float radius;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected bool ignoreSelf;

	public float Radius => radius;

	public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets) {
		var selfEntity = self;
		if (ignoreSelf) {
			var component = self.GetComponent<ParentComponent>();
			if (component != null)
				selfEntity = component.GetRootParent();
		}

		targets.Targets.Clear();
		DetectorUtility.GetCandidats(ServiceLocator.GetService<DetectorService>().Detectablies,
			self.GetComponent<DetectorComponent>(), self.GetComponent<ILocationItemComponent>(), radius, target => {
				var component = target.Detectable.Owner.GetComponent<EffectsComponent>();
				if (component == null || (ignoreSelf && component.Owner == selfEntity))
					return;
				targets.Targets.Add(component);
			});
	}
}