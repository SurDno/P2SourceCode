using System;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Rain;
using UnityEngine;

namespace Engine.Services.Engine;

[Depend(typeof(TimeService))]
[RuntimeService(typeof(EnvironmentService))]
public class EnvironmentService : IUpdatable, IInitialisable {
	private LeafManager fallingLeaves;
	private TOD_Sky tod;
	private Transform cameraTransform;
	private RainManager rain;
	[FromLocator] private ITimeService timeService;

	public event Action OnInvalidate;

	public TOD_Sky Tod => tod;

	public LeafManager FallingLeaves => fallingLeaves;

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.EnvironmentUpdater.AddUpdatable(this);
	}

	public void Terminate() {
		fallingLeaves = null;
		tod = null;
		cameraTransform = null;
		rain = null;
		InstanceByRequest<UpdateService>.Instance.EnvironmentUpdater.RemoveUpdatable(this);
	}

	public void ComputeUpdate() {
		var flag = false;
		if (cameraTransform == null) {
			cameraTransform = GameCamera.Instance.CameraTransform;
			if (cameraTransform != null)
				flag = true;
		}

		if (rain == null) {
			rain = RainManager.Instance;
			if (rain != null)
				flag = true;
		}

		if (fallingLeaves == null) {
			fallingLeaves = MonoBehaviourInstance<LeafManager>.Instance;
			if (fallingLeaves != null)
				flag = true;
		}

		if (tod == null) {
			tod = TOD_Sky.Instance;
			if (tod != null)
				flag = true;
		}

		if (rain != null && cameraTransform != null)
			rain.playerPosition = cameraTransform.position;
		if (fallingLeaves != null && cameraTransform != null)
			fallingLeaves.playerPosition = cameraTransform.position;
		if (tod != null)
			tod.Cycle.DateTime = ScriptableObjectInstance<GameSettingsData>.Instance.OffsetTime.Value +
			                     timeService.SolarTime;
		if (!flag)
			return;
		var onInvalidate = OnInvalidate;
		if (onInvalidate == null)
			return;
		onInvalidate();
	}
}