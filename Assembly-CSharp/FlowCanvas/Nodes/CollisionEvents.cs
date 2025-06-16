using ParadoxNotion.Design;
using UnityEngine;

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
