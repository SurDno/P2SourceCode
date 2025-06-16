// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ParticleCollisionEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
      this.collisionOut = this.AddFlowOutput("On Particle Collision");
      this.AddValueOutput<ParticleSystem>("Particle System", (ValueHandler<ParticleSystem>) (() => this.particle));
      this.AddValueOutput<Vector3>("Collision Point", (ValueHandler<Vector3>) (() => this.collisionEvents[0].intersection));
      this.AddValueOutput<Vector3>("Collision Normal", (ValueHandler<Vector3>) (() => this.collisionEvents[0].normal));
      this.AddValueOutput<Vector3>("Collision Velocity", (ValueHandler<Vector3>) (() => this.collisionEvents[0].velocity));
    }

    private void OnParticleCollision(GameObject other)
    {
      this.particle = other.GetComponent<ParticleSystem>();
      this.collisionEvents = new List<ParticleCollisionEvent>();
      if ((Object) this.particle != (Object) null)
        ParticlePhysicsExtensions.GetCollisionEvents(this.particle, this.target.value.gameObject, this.collisionEvents);
      this.collisionOut.Call();
    }
  }
}
