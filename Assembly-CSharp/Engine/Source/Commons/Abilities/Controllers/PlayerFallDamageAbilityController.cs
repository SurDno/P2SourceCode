using Engine.Behaviours.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class PlayerFallDamageAbilityController : IAbilityController {
	[DataReadProxy(Name = "minFall")]
	[DataWriteProxy(Name = "minFall")]
	[CopyableProxy]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float minFall;

	[DataReadProxy(Name = "maxFall")]
	[DataWriteProxy(Name = "maxFall")]
	[CopyableProxy()]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float maxFall;

	private AbilityItem abilityItem;
	private PlayerMoveController playerMoveController;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		var owner = (IEntityView)this.abilityItem.Ability.Owner;
		if (owner.GameObject == null)
			owner.OnGameObjectChangedEvent += OnViewGameObjectChanged;
		else
			OnViewGameObjectChanged();
	}

	private void OnViewGameObjectChanged() {
		var owner = (IEntityView)abilityItem.Ability.Owner;
		if (owner.GameObject == null)
			return;
		playerMoveController = owner.GameObject.GetComponent<PlayerMoveController>();
		if (!(bool)(Object)playerMoveController)
			return;
		owner.OnGameObjectChangedEvent -= OnViewGameObjectChanged;
		playerMoveController.FallDamageEvent += OnFallDamageEvent;
	}

	public void Shutdown() {
		((IEntityView)abilityItem.Ability.Owner).OnGameObjectChangedEvent -= OnViewGameObjectChanged;
		if (!(bool)(Object)playerMoveController)
			return;
		playerMoveController.FallDamageEvent -= OnFallDamageEvent;
	}

	private void OnFallDamageEvent(float fallDistance) {
		if (fallDistance < (double)minFall || fallDistance > (double)maxFall)
			return;
		Debug.Log(ObjectInfoUtility.GetStream().Append("<color=red>Fall damage : ").Append(fallDistance)
			.Append("</color> , min : ").Append(minFall).Append(" , max : ").Append(maxFall));
		abilityItem.Active = true;
		abilityItem.Active = false;
	}
}