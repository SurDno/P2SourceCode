using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Projectiles;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class PlayerCloseCombatAbilityProjectile : IAbilityProjectile {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float radius;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float angle;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float maximumXOffset = 0.4f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected int aims = 1;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected BlockTypeEnum blocked = BlockTypeEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected HitOrientationTypeEnum orientation = HitOrientationTypeEnum.None;

	private IEntity self;

	public float Radius => radius;

	public float Angle => angle;

	public float MaximumXOffset => maximumXOffset;

	public int Aims => aims;

	public void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets) {
		this.self = self;
		var candidatsEffects = new List<EffectsComponent>();
		targets.Targets = new List<EffectsComponent>();
		var gameObject = ((IEntityView)self).GameObject;
		DetectorUtility.GetCandidats(ServiceLocator.GetService<DetectorService>().Detectablies,
			self.GetComponent<DetectorComponent>(), self.GetComponent<ILocationItemComponent>(), radius, target => {
				var vector3 =
					gameObject.transform.InverseTransformDirection(target.GameObject.transform.position -
					                                               gameObject.transform.position);
				if (vector3.z > (double)radius || vector3.z < 0.0 || Mathf.Abs(vector3.x) > (double)maximumXOffset)
					return;
				var owner = target.Detectable.Owner;
				if (owner == null)
					return;
				var component = owner.GetComponent<EffectsComponent>();
				if (component == null)
					return;
				candidatsEffects.Add(component);
			});
		if (candidatsEffects.Count <= Aims)
			targets.Targets.AddRange(candidatsEffects.FindAll(x =>
				CheckBlocked(x.Owner) && CheckHitOrientation(x.Owner) && CheckCombatIgnored(x.Owner) &&
				CheckDead(x.Owner)));
		else {
			candidatsEffects.Sort(SortByRange);
			targets.Targets.AddRange(candidatsEffects.GetRange(0, Aims).FindAll(x =>
				CheckBlocked(x.Owner) && CheckHitOrientation(x.Owner) && CheckCombatIgnored(x.Owner) &&
				CheckDead(x.Owner)));
		}
	}

	private bool CheckCombatIgnored(IEntity target) {
		var component = target.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
			if (byName != null)
				return !byName.Value;
		}

		return true;
	}

	private bool CheckDead(IEntity target) {
		var component = target.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<bool>(ParameterNameEnum.Dead);
			if (byName != null)
				return !byName.Value;
		}

		return true;
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

	private bool CheckHitOrientation(IEntity target) {
		if (orientation == HitOrientationTypeEnum.None)
			return true;
		var gameObject1 = ((IEntityView)self)?.GameObject;
		var gameObject2 = ((IEntityView)target)?.GameObject;
		if (gameObject1 == null || gameObject2 == null)
			return false;
		var flag = Vector3.Dot((gameObject1.transform.position - gameObject2.transform.position).normalized,
			-gameObject2.transform.forward) > 0.0;
		if (orientation == HitOrientationTypeEnum.Back)
			return flag;
		return orientation != HitOrientationTypeEnum.Front || !flag;
	}

	private int SortByRange(EffectsComponent p1, EffectsComponent p2) {
		var position1 = ((IEntityView)p1?.Owner)?.Position;
		var position2 = ((IEntityView)p2?.Owner)?.Position;
		if (!position1.HasValue || !position2.HasValue)
			return 0;
		var position3 = ((IEntityView)self).Position;
		var vector3_1 = position3;
		var nullable1 = position1;
		var vector3_2 = (nullable1.HasValue ? vector3_1 - nullable1.GetValueOrDefault() : new Vector3?()).Value;
		double magnitude1 = vector3_2.magnitude;
		vector3_2 = position3;
		var nullable2 = position2;
		double magnitude2 = (nullable2.HasValue ? vector3_2 - nullable2.GetValueOrDefault() : new Vector3?()).Value
			.magnitude;
		return magnitude1 < magnitude2 ? -1 : 1;
	}
}