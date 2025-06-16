using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

public class DisableInIsolatedIndoor : EngineDependent {
	[Inspected] private bool insideIndoor;
	[Inspected] private bool isolatedIndoor;
	private bool connected;

	protected override void OnConnectToEngine() {
		if (connected)
			return;
		insideIndoor = ServiceLocator.GetService<InsideIndoorListener>().InsideIndoor;
		isolatedIndoor = ServiceLocator.GetService<InsideIndoorListener>().IsolatedIndoor;
		Apply();
		ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged += OnInsideIndoorChanged;
		ServiceLocator.GetService<InsideIndoorListener>().OnPlayerBeginsExit += OnPlayerBeginsExit;
	}

	protected override void OnDisconnectFromEngine() {
		ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged -= OnInsideIndoorChanged;
		ServiceLocator.GetService<InsideIndoorListener>().OnPlayerBeginsExit -= OnPlayerBeginsExit;
		connected = false;
	}

	private void OnPlayerBeginsExit() {
		if (insideIndoor) {
			insideIndoor = false;
			isolatedIndoor = false;
		}

		Apply();
	}

	private void OnInsideIndoorChanged(bool inside) {
		insideIndoor = false;
		isolatedIndoor = false;
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player != null) {
			var component = player.GetComponent<LocationItemComponent>();
			if (component != null) {
				insideIndoor = component.IsIndoor;
				var building = player.GetComponent<NavigationComponent>().Building;
				if (building != null)
					isolatedIndoor =
						ScriptableObjectInstance<IndoorSettingsData>.Instance.IsIndoorIsolated(building.Building);
			} else
				Debug.LogError("LocationItemComponent not found, owner : " + player.GetInfo());
		}

		Apply();
	}

	private void Apply() {
		gameObject.SetActive(!isolatedIndoor);
	}

	protected override void OnDisable() { }

	private void OnDestroy() {
		base.OnDisable();
	}
}