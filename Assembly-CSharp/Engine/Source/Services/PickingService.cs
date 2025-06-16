using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services;

[GameService(typeof(PickingService))]
public class PickingService : IUpdatable, IInitialisable {
	private const float maxDistance = 10f;
	private const float sphereCastRadius = 0.1f;
	private Ray ray;
	private IEntity targetEntity;
	private float targetEntityDistance;
	private GameObject targetGameObject;
	private float targetGameObjectDistance;
	private static List<RaycastHit> hits = new();

	[Inspected] public Ray Ray => ray;

	[Inspected] public IEntity TargetEntity => targetEntity;

	[Inspected] public float TargetEntityDistance => targetEntityDistance;

	[Inspected] public GameObject TargetGameObject => targetGameObject;

	[Inspected] public float TargetGameObjectDistance => targetGameObjectDistance;

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
	}

	public void Terminate() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
	}

	public void ComputeUpdate() {
		Clear();
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
			return;
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			return;
		var entityView = (IEntityView)player;
		if (entityView.GameObject == null)
			return;
		var cameraTransform = GameCamera.Instance.CameraTransform;
		if (cameraTransform == null)
			return;
		ray = new Ray(cameraTransform.position, cameraTransform.forward);
		targetEntityDistance = float.PositiveInfinity;
		PhysicsUtility.Raycast(hits, Ray, 10f, -1, QueryTriggerInteraction.Ignore);
		for (var index = 0; index < hits.Count; ++index) {
			var hit = hits[index];
			if (!(hit.transform == null)) {
				var gameObject = hit.transform.gameObject;
				if (!(gameObject == entityView.GameObject)) {
					targetGameObject = gameObject;
					targetGameObjectDistance = hit.distance;
					var entity = EntityUtility.GetEntity(gameObject);
					if (entity == null)
						break;
					targetEntity = entity;
					targetEntityDistance = hit.distance;
					break;
				}
			}
		}
	}

	private void Clear() {
		ray = new Ray();
		targetEntity = null;
		targetEntityDistance = float.PositiveInfinity;
		targetGameObject = null;
		targetGameObjectDistance = float.PositiveInfinity;
	}
}