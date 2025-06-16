using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class CreateProjectileEntityEffect : IEffect {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected Typed<IEntity> template;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ProjectileSpawnPlaceEnum spawnPlace = ProjectileSpawnPlaceEnum.None;

	public string Name => GetType().Name;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public ParameterEffectQueueEnum Queue => queue;

	public bool Prepare(float currentRealTime, float currentGameTime) {
		var entity = ServiceLocator.GetService<IFactory>().Instantiate(template.Value);
		((Entity)entity).DontSave = true;
		entity.GetComponent<ParentComponent>()?.SetParent(Target);
		ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Objects);
		var component1 = entity.GetComponent<LocationItemComponent>();
		if (component1 != null)
			component1.Location = Target?.GetComponent<LocationItemComponent>()?.Location;
		var transform = ((IEntityView)Target).GameObject.transform;
		var component2 = ((IEntityView)Target).GameObject.GetComponent<NPCWeaponService>();
		if (component2 != null && spawnPlace == ProjectileSpawnPlaceEnum.Bomb && component2.BombParent != null)
			transform = component2.BombParent.transform;
		if (component2 != null && spawnPlace == ProjectileSpawnPlaceEnum.Samopal && component2.SamopalParent != null)
			transform = component2.SamopalParent.transform;
		if (transform != null) {
			((IEntityView)entity).Position = transform.position;
			((IEntityView)entity).Rotation = transform.rotation;
		}

		return true;
	}

	public bool Compute(float currentRealTime, float currentGameTime) {
		return false;
	}

	public void Cleanup() { }
}