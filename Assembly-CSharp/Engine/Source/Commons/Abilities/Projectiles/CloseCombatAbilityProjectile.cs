using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Projectiles;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class CloseCombatAbilityProjectile : IAbilityProjectile {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected BlockTypeEnum blocked = BlockTypeEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected HitOrientationTypeEnum orientation = HitOrientationTypeEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float radius = 1.8f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float angle = 90f;

	public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets) {
		targets.Targets = new List<EffectsComponent>();
		var gameObject = ((IEntityView)self).GameObject;
		if (gameObject == null)
			return;
		var component1 = gameObject.GetComponent<EnemyBase>();
		if (component1 == null)
			return;
		var enemy = component1.Enemy;
		if (enemy == null)
			return;
		var component2 = enemy.gameObject.GetComponent<EngineGameObject>();
		if (component2 == null)
			return;
		var owner = component2.Owner;
		if (owner == null || !CheckBlocked(owner) || !CheckHitOrientation(gameObject, enemy.gameObject))
			return;
		var component3 = owner.GetComponent<EffectsComponent>();
		if (component3 == null || (enemy.transform.position - component1.transform.position).magnitude > (double)radius)
			return;
		var direction = enemy.transform.position - component1.transform.position;
		var vector3 = component1.transform.InverseTransformDirection(direction);
		if (vector3.z > (double)radius || vector3.z < 0.0 || Mathf.Abs(vector3.x) > 0.40000000596046448)
			return;
		targets.Targets.Add(component3);
	}

	private bool CheckBlocked(IEntity target) {
		if (blocked == BlockTypeEnum.None)
			return true;
		var component = target.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<BlockTypeEnum>(ParameterNameEnum.BlockType);
			if (byName != null)
				return blocked == byName.Value;
		}

		return true;
	}

	private bool CheckHitOrientation(GameObject gameObject, GameObject target) {
		if (orientation == HitOrientationTypeEnum.None)
			return true;
		if (gameObject == null || target == null)
			return false;
		var flag = Vector3.Dot((gameObject.transform.position - target.transform.position).normalized,
			-target.transform.forward) > 0.0;
		if (orientation == HitOrientationTypeEnum.Back)
			return flag;
		return orientation != HitOrientationTypeEnum.Front || !flag;
	}
}