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
      enter = AddFlowOutput("Enter");
      stay = AddFlowOutput("Stay");
      exit = AddFlowOutput("Exit");
      AddValueOutput("Other", () => collision.gameObject);
      AddValueOutput("Contact Point", () => collision.contacts[0]);
      AddValueOutput("Collision Info", () => collision);
    }

    private void OnCollisionEnter(Collision collision)
    {
      this.collision = collision;
      enter.Call();
    }

    private void OnCollisionStay(Collision collision)
    {
      this.collision = collision;
      stay.Call();
    }

    private void OnCollisionExit(Collision collision)
    {
      this.collision = collision;
      exit.Call();
    }
  }
}
