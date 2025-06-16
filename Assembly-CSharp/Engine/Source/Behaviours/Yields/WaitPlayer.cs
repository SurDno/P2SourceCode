using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Source.Behaviours.Yields;

public class WaitPlayer : CustomYieldInstruction {
	public override bool keepWaiting {
		get {
			var service = ServiceLocator.GetService<ISimulation>();
			if (service == null)
				return true;
			var player = service.Player;
			if (player == null || !((IEntityView)player).IsAttached)
				return true;
			var component = player.GetComponent<LocationItemComponent>();
			return component != null && component.IsHibernation;
		}
	}
}