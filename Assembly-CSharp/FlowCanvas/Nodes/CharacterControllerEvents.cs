using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Character Controller")]
  [Category("Events/Object")]
  [Description("Called when the Character Controller hits a collider while performing a Move")]
  public class CharacterControllerEvents : EventNode<CharacterController>
  {
    private ControllerColliderHit hitInfo;
    private FlowOutput hit;

    protected override string[] GetTargetMessageEvents()
    {
      return new string[1]{ "OnControllerColliderHit" };
    }

    protected override void RegisterPorts()
    {
      this.hit = this.AddFlowOutput("Collider Hit");
      this.AddValueOutput<GameObject>("Other", (ValueHandler<GameObject>) (() => this.hitInfo.gameObject));
      this.AddValueOutput<Vector3>("Collision Point", (ValueHandler<Vector3>) (() => this.hitInfo.point));
      this.AddValueOutput<ControllerColliderHit>("Collision Info", (ValueHandler<ControllerColliderHit>) (() => this.hitInfo));
    }

    private void OnControllerColliderHit(ControllerColliderHit hitInfo)
    {
      this.hitInfo = hitInfo;
      this.hit.Call();
    }
  }
}
