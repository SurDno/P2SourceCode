// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.MouseAgentEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
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
      this.down = this.AddFlowOutput("Down");
      this.up = this.AddFlowOutput("Up");
      this.enter = this.AddFlowOutput("Enter");
      this.over = this.AddFlowOutput("Over");
      this.exit = this.AddFlowOutput("Exit");
      this.drag = this.AddFlowOutput("Drag");
      this.AddValueOutput<RaycastHit>("Info", (ValueHandler<RaycastHit>) (() => this.hit));
    }

    private void OnMouseEnter()
    {
      this.StoreHit();
      this.enter.Call();
    }

    private void OnMouseOver()
    {
      this.StoreHit();
      this.over.Call();
    }

    private void OnMouseExit()
    {
      this.StoreHit();
      this.exit.Call();
    }

    private void OnMouseDown()
    {
      this.StoreHit();
      this.down.Call();
    }

    private void OnMouseUp()
    {
      this.StoreHit();
      this.up.Call();
    }

    private void OnMouseDrag()
    {
      this.StoreHit();
      this.drag.Call();
    }

    private void StoreHit()
    {
      Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out this.hit, float.PositiveInfinity);
    }
  }
}
