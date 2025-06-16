// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.CollisionEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Collision")]
  [Category("Events/Object")]
  [Description("Called when Collision based events happen on target and expose collision information")]
  public class CollisionEvents : EventNode<Collider>
  {
    private Collision collision;
    private FlowOutput enter;
    private FlowOutput stay;
    private FlowOutput exit;

    protected override string[] GetTargetMessageEvents()
    {
      return new string[3]
      {
        "OnCollisionEnter",
        "OnCollisionStay",
        "OnCollisionExit"
      };
    }

    protected override void RegisterPorts()
    {
      this.enter = this.AddFlowOutput("Enter");
      this.stay = this.AddFlowOutput("Stay");
      this.exit = this.AddFlowOutput("Exit");
      this.AddValueOutput<GameObject>("Other", (ValueHandler<GameObject>) (() => this.collision.gameObject));
      this.AddValueOutput<ContactPoint>("Contact Point", (ValueHandler<ContactPoint>) (() => this.collision.contacts[0]));
      this.AddValueOutput<Collision>("Collision Info", (ValueHandler<Collision>) (() => this.collision));
    }

    private void OnCollisionEnter(Collision collision)
    {
      this.collision = collision;
      this.enter.Call();
    }

    private void OnCollisionStay(Collision collision)
    {
      this.collision = collision;
      this.stay.Call();
    }

    private void OnCollisionExit(Collision collision)
    {
      this.collision = collision;
      this.exit.Call();
    }
  }
}
