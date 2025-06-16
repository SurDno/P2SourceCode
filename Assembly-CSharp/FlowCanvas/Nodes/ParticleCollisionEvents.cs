using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Name("Particle Collision")]
[Category("Events/Object")]
[Description("Called when any Particle System collided with the target collider object")]
public class ParticleCollisionEvents : EventNode<Collider> {
	private FlowOutput collisionOut;
	private ParticleSystem particle;
	private List<ParticleCollisionEvent> collisionEvents;

	protected override string[] GetTargetMessageEvents() {
		return new string[1] { "OnParticleCollision" };
	}

	protected override void RegisterPorts() {
		collisionOut = AddFlowOutput("On Particle Collision");
		AddValueOutput("Particle System", () => particle);
		AddValueOutput("Collision Point", () => collisionEvents[0].intersection);
		AddValueOutput("Collision Normal", () => collisionEvents[0].normal);
		AddValueOutput("Collision Velocity", () => collisionEvents[0].velocity);
	}

	private void OnParticleCollision(GameObject other) {
		particle = other.GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
		if (particle != null)
			particle.GetCollisionEvents(target.value.gameObject, collisionEvents);
		collisionOut.Call();
	}
}