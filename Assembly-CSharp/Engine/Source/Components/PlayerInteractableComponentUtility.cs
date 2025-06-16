using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Components.Interactable;

namespace Engine.Source.Components;

public static class PlayerInteractableComponentUtility {
	public static bool GetInteractCriminal(InteractableComponent interactable, InteractItem item) {
		if (interactable == null || interactable.Owner == null)
			return false;
		if (item.Type == InteractType.BreakPicklock)
			return !IsFree(interactable.Owner);
		if (item.Type == InteractType.Autopsy) {
			var player = ServiceLocator.GetService<ISimulation>().Player;
			if (player == null)
				return false;
			var component = player.GetComponent<PlayerControllerComponent>();
			return component != null && component.IsCrime(ActionEnum.Autopsy, interactable.Owner);
		}

		if (item.Type != InteractType.Loot)
			return false;
		var player1 = ServiceLocator.GetService<ISimulation>().Player;
		if (player1 == null)
			return false;
		var component1 = player1.GetComponent<PlayerControllerComponent>();
		if (component1 == null)
			return false;
		var flag = interactable?.Owner?.GetComponent<INpcControllerComponent>() != null;
		if (!flag) {
			var component2 = interactable?.Owner?.GetComponent<ParametersComponent>();
			if (component2 != null) {
				var byName = component2.GetByName<bool>(ParameterNameEnum.LootAsNPC);
				if (byName != null)
					flag = byName.Value;
			}
		}

		if (!flag)
			return component1.IsCrime(ActionEnum.Theft, interactable.Owner);
		return IsDead(interactable.Owner)
			? component1.IsCrime(ActionEnum.LootDeadCharacter, interactable.Owner)
			: component1.IsCrime(ActionEnum.TakeItemsFromSurrender, interactable.Owner);
	}

	private static bool IsFree(IEntity entity) {
		if (entity == null)
			return false;
		var component = entity.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<bool>(ParameterNameEnum.IsFree);
			if (byName != null)
				return byName.Value;
		}

		return false;
	}

	private static bool IsDead(IEntity entity) {
		if (entity == null)
			return false;
		var component = entity.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<bool>(ParameterNameEnum.Dead);
			if (byName != null)
				return byName.Value;
		}

		return false;
	}
}