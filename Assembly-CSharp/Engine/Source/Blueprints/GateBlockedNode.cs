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
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IDoorComponent doorComponent = gateInput.value;
        if (doorComponent != null)
          doorComponent.Bolted.Value = blockedInput.value;
        output.Call();
      });
      gateInput = AddValueInput<IDoorComponent>("Gate");
      blockedInput = AddValueInput<bool>("Blocked");
    }
  }
}
