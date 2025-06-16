using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Source.Services;

[GameService(typeof(LightService))]
public class LightService : IInitialisable, IUpdatable {
	private List<LightServiceObject> lights = new();
	private float checkLightsInterval = 0.5f;
	private float timeToNextCheck;
	private bool playerIsLighted;

	public bool PlayerIsLighted => playerIsLighted;

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
	}

	public void Terminate() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
	}

	public void RegisterLight(LightServiceObject light, bool enable) {
		if (enable && !lights.Contains(light))
			lights.Add(light);
		if (enable || !lights.Contains(light))
			return;
		lights.Remove(light);
	}

	public void ComputeUpdate() {
		timeToNextCheck -= Time.deltaTime;
		if (timeToNextCheck >= 0.0)
			return;
		timeToNextCheck = checkLightsInterval;
		UpdateLights();
	}

	private void UpdateLights() {
		playerIsLighted = false;
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			return;
		var component = player.GetComponent<LocationItemComponent>();
		if (component == null || component.IsIndoor)
			return;
		var gameObject = ((IEntityView)player)?.GameObject;
		if (gameObject == null)
			return;
		var position1 = gameObject.transform.position with {
			y = 0.0f
		};
		foreach (var light in lights) {
			var position2 = light.transform.position with {
				y = 0.0f
			};
			var visibilityRadius = light.VisibilityRadius;
			if (Mathf.Abs(position2.x - position1.x) <= (double)visibilityRadius &&
			    Mathf.Abs(position2.z - position1.z) <= (double)visibilityRadius &&
			    (position2 - position1).magnitude <= (double)visibilityRadius) {
				playerIsLighted = true;
				break;
			}
		}
	}
}