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
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IDoorComponent doorComponent = this.gateInput.value;
        if (doorComponent != null)
          doorComponent.Marked.Value = this.marketInput.value;
        output.Call();
      }));
      this.gateInput = this.AddValueInput<IDoorComponent>("Gate");
      this.marketInput = this.AddValueInput<bool>("Marked");
    }
  }
}
