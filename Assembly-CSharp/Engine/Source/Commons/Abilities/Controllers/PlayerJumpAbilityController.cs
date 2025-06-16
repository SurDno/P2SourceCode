using Engine.Behaviours.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class PlayerJumpAbilityController : IAbilityController {
	[DataReadProxy(Name = "jump")]
	[DataWriteProxy(Name = "jump")]
	[CopyableProxy()]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool jump;

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
		playerMoveController.JumpEvent += OnJumpEvent;
	}

	private void OnJumpEvent(bool jump) {
		if (this.jump != jump)
			return;
		abilityItem.Active = true;
		abilityItem.Active = false;
	}

	public void Shutdown() {
		((IEntityView)abilityItem.Ability.Owner).OnGameObjectChangedEvent -= OnViewGameObjectChanged;
		if (!(bool)(Object)playerMoveController)
			return;
		playerMoveController.JumpEvent -= OnJumpEvent;
	}
}