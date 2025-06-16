using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GateNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> gateInput;
    private ValueInput<bool> openInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IDoorComponent doorComponent = this.gateInput.value;
        if (doorComponent != null)
        {
          if (this.openInput.value)
            doorComponent.Opened.Value = true;
          else
            doorComponent.Opened.Value = false;
        }
        output.Call();
      }));
      this.gateInput = this.AddValueInput<IDoorComponent>("Gate");
      this.openInput = this.AddValueInput<bool>("Open");
    }
  }
}
