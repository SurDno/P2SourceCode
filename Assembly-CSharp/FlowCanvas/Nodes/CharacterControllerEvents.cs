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
      hit = AddFlowOutput("Collider Hit");
      AddValueOutput("Other", () => hitInfo.gameObject);
      AddValueOutput("Collision Point", () => hitInfo.point);
      AddValueOutput("Collision Info", () => hitInfo);
    }

    private void OnControllerColliderHit(ControllerColliderHit hitInfo)
    {
      this.hitInfo = hitInfo;
      hit.Call();
    }
  }
}
