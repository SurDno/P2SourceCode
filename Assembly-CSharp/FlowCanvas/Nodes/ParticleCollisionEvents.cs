using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("Particle Collision")]
  [Category("Events/Object")]
  [Description("Called when any Particle System collided with the target collider object")]
  public class ParticleCollisionEvents : EventNode<Collider>
  {
    private FlowOutput collisionOut;
    private ParticleSystem particle;
    private List<ParticleCollisionEvent> collisionEvents;

    protected override string[] GetTargetMessageEvents()
    {
      return new string[1]{ "OnParticleCollision" };
    }

    protected override void RegisterPorts()
    {
      collisionOut = AddFlowOutput("On Particle Collision");
      AddValueOutput("Particle System", (ValueHandler<ParticleSystem>) (() => particle));
      AddValueOutput("Collision Point", (ValueHandler<Vector3>) (() => collisionEvents[0].intersection));
      AddValueOutput("Collision Normal", (ValueHandler<Vector3>) (() => collisionEvents[0].normal));
      AddValueOutput("Collision Velocity", (ValueHandler<Vector3>) (() => collisionEvents[0].velocity));
    }

    private void OnParticleCollision(GameObject other)
    {
      particle = other.GetComponent<ParticleSystem>();
      collisionEvents = new List<ParticleCollisionEvent>();
      if ((Object) particle != (Object) null)
        ParticlePhysicsExtensions.GetCollisionEvents(particle, target.value.gameObject, collisionEvents);
      collisionOut.Call();
    }
  }
}
