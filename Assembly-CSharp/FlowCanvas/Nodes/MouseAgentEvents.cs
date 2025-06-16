using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Mouse")]
  [Category("Events/Object")]
  [Description("Called when mouse based operations happen on target collider")]
  public class MouseAgentEvents : EventNode<Collider>
  {
    private FlowOutput enter;
    private FlowOutput over;
    private FlowOutput exit;
    private FlowOutput down;
    private FlowOutput up;
    private FlowOutput drag;
    private RaycastHit hit;

    protected override string[] GetTargetMessageEvents()
    {
      return new string[6]
      {
        "OnMouseEnter",
        "OnMouseOver",
        "OnMouseExit",
        "OnMouseDown",
        "OnMouseUp",
        "OnMouseDrag"
      };
    }

    protected override void RegisterPorts()
    {
      down = AddFlowOutput("Down");
      up = AddFlowOutput("Up");
      enter = AddFlowOutput("Enter");
      over = AddFlowOutput("Over");
      exit = AddFlowOutput("Exit");
      drag = AddFlowOutput("Drag");
      AddValueOutput("Info", () => hit);
    }

    private void OnMouseEnter()
    {
      StoreHit();
      enter.Call();
    }

    private void OnMouseOver()
    {
      StoreHit();
      over.Call();
    }

    private void OnMouseExit()
    {
      StoreHit();
      exit.Call();
    }

    private void OnMouseDown()
    {
      StoreHit();
      down.Call();
    }

    private void OnMouseUp()
    {
      StoreHit();
      up.Call();
    }

    private void OnMouseDrag()
    {
      StoreHit();
      drag.Call();
    }

    private void StoreHit()
    {
      Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity);
    }
  }
}
