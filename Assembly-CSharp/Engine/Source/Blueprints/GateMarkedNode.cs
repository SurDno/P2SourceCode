using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GateMarkedNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> gateInput;
    private ValueInput<bool> marketInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IDoorComponent doorComponent = gateInput.value;
        if (doorComponent != null)
          doorComponent.Marked.Value = marketInput.value;
        output.Call();
      });
      gateInput = AddValueInput<IDoorComponent>("Gate");
      marketInput = AddValueInput<bool>("Marked");
    }
  }
}
