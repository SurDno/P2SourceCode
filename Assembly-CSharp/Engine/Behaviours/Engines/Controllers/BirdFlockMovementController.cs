using System;
using Engine.Behaviours.Components;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Engine.Behaviours.Engines.Controllers;

public class BirdFlockMovementController : IMovementController {
	private NavMeshAgent agent;
	private GameObject gameObject;
	private const float flockSpeed = 5f;

	public bool IsPaused { get; set; }

	public bool GeometryVisible {
		get => throw new NotImplementedException();
		set => throw new NotImplementedException();
	}

	public void Initialize(GameObject gameObject) {
		this.gameObject = gameObject;
		agent = gameObject.GetComponent<NavMeshAgent>();
		if (!(bool)(Object)agent)
			return;
		agent.enabled = false;
	}

	public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait) { }

	public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait) {
		var num = gait == EngineBehavior.GaitType.Walk ? 5f : 10f;
		gameObject.transform.position += direction.normalized * num * Time.deltaTime;
		return remainingDistance < 1.0;
	}

	public bool Rotate(Vector3 direction) {
		return true;
	}

	public void OnAnimatorMove() { }

	public void Update() { }

	public void FixedUpdate() { }
}