using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Effects.Values;
using Inspectors;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ShootEffect : IEffect {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy(Name = "Action")]
	[DataWriteProxy(Name = "Action")]
	[CopyableProxy()]
	[Inspected]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ShootEffectEnum actionType;

	public string Name => GetType().Name;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public ParameterEffectQueueEnum Queue => queue;

	public bool Prepare(float currentRealTime, float currentGameTime) {
		return true;
	}

	public bool Compute(float currentRealTime, float currentGameTime) {
		var component = Target.GetComponent<NpcControllerComponent>();
		if (component != null) {
			var controllerComponent1 = AbilityItem.Self.GetComponent<PlayerControllerComponent>() ?? AbilityItem.Self
				.GetComponent<ParentComponent>()?.GetRootParent()?.GetComponent<PlayerControllerComponent>();
			if (controllerComponent1 != null) {
				if (actionType == ShootEffectEnum.Shoot)
					controllerComponent1.ComputeShoot(component);
				if (actionType == ShootEffectEnum.Hit)
					controllerComponent1.ComputeHit(component);
				if (actionType == ShootEffectEnum.HitOther)
					controllerComponent1.ComputeHitAnotherNPC(component);
			} else {
				var controllerComponent2 = AbilityItem.Self.GetComponent<NpcControllerComponent>() ?? AbilityItem.Self
					.GetComponent<ParentComponent>()?.GetRootParent()?.GetComponent<NpcControllerComponent>();
				if (controllerComponent2 != null) {
					if (actionType == ShootEffectEnum.Shoot)
						controllerComponent2.ComputeShoot(component);
					if (actionType == ShootEffectEnum.Hit)
						controllerComponent2.ComputeHit(component);
				}
			}
		}

		return false;
	}

	public void Cleanup() { }
}