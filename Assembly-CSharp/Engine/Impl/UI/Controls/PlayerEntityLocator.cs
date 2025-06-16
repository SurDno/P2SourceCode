using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class PlayerEntityLocator : EngineDependent {
	[SerializeField] private EntityView view;
	[FromLocator] private Simulation simulation;

	protected override void OnConnectToEngine() {
		SetPlayer(simulation.Player);
		simulation.OnPlayerChanged += SetPlayer;
	}

	protected override void OnDisconnectFromEngine() {
		simulation.OnPlayerChanged -= SetPlayer;
		SetPlayer(null);
	}

	private void SetPlayer(IEntity entity) {
		if (!(view != null))
			return;
		view.Value = entity;
	}
}