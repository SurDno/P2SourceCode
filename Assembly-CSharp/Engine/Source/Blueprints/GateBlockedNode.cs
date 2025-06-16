using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GateBlockedNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> gateInput;
    private ValueInput<bool> blockedInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IDoorComponent doorComponent = this.gateInput.value;
        if (doorComponent != null)
          doorComponent.Bolted.Value = this.blockedInput.value;
        output.Call();
      }));
      this.gateInput = this.AddValueInput<IDoorComponent>("Gate");
      this.blockedInput = this.AddValueInput<bool>("Blocked");
    }
  }
}
