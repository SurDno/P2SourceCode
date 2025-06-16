using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Projectiles;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class NPCRaycastAbilityProjectile : IAbilityProjectile {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float hitDistance = 50f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected int bulletsCount = 5;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float minimumDirectionScatter = 1f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float maximumDirectionScatter = 10f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float maximumAimingDeltaAngle = 45f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float bulletScatter = 10f;

	private static List<RaycastHit> hits = new();

	public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets) {
		var gameObject1 = ((IEntityView)self).GameObject;
		if (gameObject1 == null)
			return;
		var origin = gameObject1.transform.position;
		var direction1 = gameObject1.transform.forward;
		gameObject1.GetComponent<Pivot>();
		var component1 = gameObject1.GetComponent<NPCWeaponService>();
		if (component1 != null) {
			origin = component1.GetWeaponStartPoint();
			direction1 = component1.GetWeaponAimDirection();
		}

		var component2 = gameObject1.GetComponent<NPCEnemy>();
		if (component2 != null)
			direction1 = ScatterShotDirection(direction1, component2);
		var triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
		var ragdollLayer = ScriptableObjectInstance<GameSettingsData>.Instance.RagdollLayer;
		for (var index1 = 0; index1 < bulletsCount; ++index1) {
			var direction2 = ScatterSingleBulletDirection(direction1);
			PhysicsUtility.Raycast(hits, origin, direction2, hitDistance, -1 ^ triggerInteractLayer ^ ragdollLayer,
				QueryTriggerInteraction.Ignore);
			for (var index2 = 0; index2 < hits.Count; ++index2) {
				var gameObject2 = hits[index2].collider.gameObject;
				if (!(gameObject2 == gameObject1)) {
					var entity = EntityUtility.GetEntity(gameObject2);
					if (entity != null) {
						var component3 = entity.GetComponent<EffectsComponent>();
						if (component3 != null)
							targets.Targets.Add(component3);
					}

					break;
				}
			}
		}
	}

	private Vector3 ScatterShotDirection(Vector3 direction, NPCEnemy enemy) {
		var num = Mathf.Lerp(minimumDirectionScatter, maximumDirectionScatter,
			Mathf.Min(enemy.GetAimingRotationDelta() / maximumAimingDeltaAngle, 1f));
		return Quaternion.Euler(0.0f, (float)(num / 2.0 - Random.value * (double)num), 0.0f) * direction;
	}

	private Vector3 ScatterSingleBulletDirection(Vector3 direction) {
		var bulletScatter = this.bulletScatter;
		return Quaternion.Euler(0.0f, (float)(bulletScatter / 2.0 - Random.value * (double)bulletScatter), 0.0f) *
		       direction;
	}
}