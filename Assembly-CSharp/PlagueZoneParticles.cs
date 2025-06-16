using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using UnityEngine;

public class PlagueZoneParticles : MonoBehaviour, IUpdatable {
	[SerializeField] private float maxRate;
	[SerializeField] private LayerMask collisionLayers;
	[SerializeField] private ParticleSystem particleSystem2;
	private float prevLevel = -1f;

	private void Awake() {
		InstanceByRequest<UpdateService>.Instance.PlagueZoneUpdater.AddUpdatable(this);
	}

	private void OnDestroy() {
		InstanceByRequest<UpdateService>.Instance.PlagueZoneUpdater.RemoveUpdatable(this);
	}

	public void ComputeUpdate() {
		var num = 0.0f;
		var position = transform.position;
		position.y += 100f;
		if (!Physics.Raycast(position, Vector3.down, 100f, collisionLayers, QueryTriggerInteraction.Ignore))
			num = PlagueZone.GetLevel(new Vector2(transform.position.x, transform.position.z));
		if (prevLevel == (double)num)
			return;
		prevLevel = num;
		var emission = particleSystem2.emission;
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(maxRate * num);
		ServiceCache.OptimizationService.FrameHasSpike = true;
	}
}