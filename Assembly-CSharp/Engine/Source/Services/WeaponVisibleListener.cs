using System;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Components;

namespace Engine.Source.Services;

[GameService(typeof(WeaponVisibleListener))]
public class WeaponVisibleListener : IInitialisable {
	private ControllerComponent controller;

	public event Action<WeaponKind, bool> OnWeaponVisibleChanged;

	public void Initialise() {
		ServiceLocator.GetService<Simulation>().OnPlayerChanged += OnPlayerChanged;
		OnPlayerChanged(ServiceLocator.GetService<ISimulation>().Player);
	}

	public void Terminate() {
		ServiceLocator.GetService<Simulation>().OnPlayerChanged -= OnPlayerChanged;
	}

	private void OnPlayerChanged(IEntity player) {
		if (controller != null) {
			controller.OnWeaponEnableChanged -= OnWeaponEnableChanged;
			controller = null;
		}

		if (player != null)
			controller = player.GetComponent<ControllerComponent>();
		if (controller == null)
			return;
		controller.OnWeaponEnableChanged += OnWeaponEnableChanged;
	}

	private void OnWeaponEnableChanged(WeaponKind kind, bool value) {
		var weaponVisibleChanged = OnWeaponVisibleChanged;
		if (weaponVisibleChanged == null)
			return;
		weaponVisibleChanged(kind, value);
	}
}